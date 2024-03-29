using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;
using mvp.tickets.domain.Stores;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CompanyController> _logger;
        private readonly ISettings _settings;
        private readonly IS3Service _s3Service;

        public CompanyController(ApplicationDbContext dbContext, ILogger<CompanyController> logger, ISettings settings, IS3Service s3Service)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings;
            _s3Service = s3Service;
        }

        [HttpGet("current")]
        public async Task<IBaseQueryResponse<ICompanyModel>> Current([FromServices] ICompanyStore companyStore, [FromServices] IWebHostEnvironment env)
        {
            IBaseQueryResponse<ICompanyModel> response = default;
            try
            {
                var host = Request.Host.Value.ToLower();
                if (!env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var company = await companyStore.GetByHost(host);
                if (company == null)
                {
                    return new BaseQueryResponse<ICompanyModel>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = null,
                        ErrorMessage = "����������� �� �������."
                    };
                }

                if (!string.IsNullOrWhiteSpace(company.Logo))
                {
                    company.Logo = $"/{AppConstants.LogoFilesFolder}/{company.Logo}?v={company.DateModified.Ticks}";
                }
                response = new BaseQueryResponse<ICompanyModel>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = company,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<ICompanyModel>();
                response.HandleException(ex);
            }
            return response;
        }

        [Authorize(Policy = AuthConstants.RootSpacePolicy)]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<ICompanyModel>>> Query()
        {
            IBaseQueryResponse<IEnumerable<ICompanyModel>> response = default;
            try
            {
                var entries = await _dbContext.Companies.Where(s => !s.IsRoot).Select(s => new CompanyModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Host = s.Host,
                    IsActive = s.IsActive,
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified,
                    Color = s.Color,
                    IsRoot = s.IsRoot,
                    AuthType = s.AuthType
                }).ToListAsync();

                response = new BaseQueryResponse<IEnumerable<ICompanyModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<ICompanyModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IBaseCommandResponse<string>> Create([FromForm] CompanyCreateCommandRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
            {
                return new BaseCommandResponse<string>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<string> response = default;

            try
            {
                var invite = await _dbContext.Invites.FirstOrDefaultAsync(s => s.Email == request.Email.ToLower() && s.Code == request.Code);
                if (invite == null)
                {
                    return new BaseCommandResponse<string>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"����������� �� �������������."
                    };
                }

                var entry = new Company
                {
                    Name = request.Name,
                    Host = $"{request.Host.ToLower()}.{AppConstants.DefaultHost}",
                    IsRoot = false,
                    IsActive = true,
                    Color = request.Color,
                    AuthType = request.AuthType,
                    DateCreated = DateTimeOffset.UtcNow,
                    DateModified = DateTimeOffset.UtcNow,
                    Users = new List<User>
                    {
                        new User
                        {
                            FirstName = "Admin",
                            LastName = "Admin",
                            Phone = null,
                            Email = request.Email.ToLower(),
                            Password = HashHelper.GetSHA256Hash(request.Password),
                            IsLocked = false,
                            Permissions = Permissions.Admin | Permissions.Employee | Permissions.User,
                            DateCreated = DateTimeOffset.UtcNow,
                            DateModified = DateTimeOffset.UtcNow,
                        }
                    }
                };

                if (await _dbContext.Companies.AnyAsync(s => s.Host == entry.Host))
                {
                    return new BaseCommandResponse<string>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"����� ������� ������ ����� ��� ������������."
                    };
                }

                _dbContext.Companies.Add(entry);
                _dbContext.Invites.Remove(invite);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                if (request.Logo != null)
                {
                    var ext = Path.GetExtension(request.Logo.FileName).Trim('.').ToLower();
                    entry.Logo = $"{entry.Id}.{ext}";
                    var fileResult = await _s3Service.PutObjectAsync($"{AppConstants.LogoFilesFolder}/{entry.Logo}", request.Logo.OpenReadStream());
                    if (fileResult)
                    {
                        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                response = new BaseCommandResponse<string>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = $"https://{entry.Host}/"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<string>();
                response.HandleException(ex);
            }
            return response;
        }

        [Authorize(Policy = AuthConstants.RootSpacePolicy)]
        [HttpPut("activation")]
        public async Task<IBaseCommandResponse<bool>> SetActive([FromBody] CompanySetActiveCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<bool> response = default;

            try
            {
                var entry = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entry == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                entry.IsActive = request.IsActive;
                entry.DateModified = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<bool>();
                response.HandleException(ex);
            }
            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<IBaseQueryResponse<ICompanyModel>> Get([FromRoute] int id)
        {
            IBaseQueryResponse<ICompanyModel> response = default;
            try
            {
                if (id != int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value))
                {
                    return new BaseQueryResponse<ICompanyModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest
                    };
                }
                var entry = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
                if (entry == null)
                {
                    return new BaseQueryResponse<ICompanyModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                response = new BaseQueryResponse<ICompanyModel>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = new CompanyModel
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        IsActive = entry.IsActive,
                        Host = entry.Host.Replace($".{AppConstants.DefaultHost}", ""),
                        DateCreated = entry.DateCreated,
                        Logo = entry.Logo != null ? $"/{AppConstants.LogoFilesFolder}/{entry.Logo}?v={entry.DateModified.Ticks}" : null,
                        Color = entry.Color,
                        AuthType = entry.AuthType,
                        DateModified = entry.DateModified,
                        IsRoot = entry.IsRoot
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<ICompanyModel>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<IBaseCommandResponse<ICompanyModel>> Update([FromRoute] int id, [FromForm] CompanyUpdateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<ICompanyModel>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<ICompanyModel> response = default;
            try
            {
                if (request.Id != int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value))
                {
                    return new BaseCommandResponse<ICompanyModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest
                    };
                }
                var entry = await _dbContext.Companies.FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive);
                if (entry == null)
                {
                    return new BaseCommandResponse<ICompanyModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                entry.DateModified = DateTimeOffset.UtcNow;
                entry.Name = request.Name;
                entry.Host = $"{request.Host.ToLower()}.{AppConstants.DefaultHost}";
                entry.Color = request.Color;
                entry.AuthType = request.AuthType;

                if (request.NewLogo != null)
                {
                    var ext = Path.GetExtension(request.NewLogo.FileName).Trim('.').ToLower();
                    entry.Logo = $"{entry.Id}.{ext}";
                    var fileResult = await _s3Service.PutObjectAsync($"{AppConstants.LogoFilesFolder}/{entry.Logo}", request.NewLogo.OpenReadStream());
                    if (!fileResult)
                    {
                        return new BaseCommandResponse<ICompanyModel>
                        {
                            IsSuccess = false,
                            Code = ResponseCodes.Error,
                            ErrorMessage = "������ ��� ���������� ��������."
                        };
                    }
                }
                else if (request.RemoveLogo)
                {
                    entry.Logo = null;
                }

                await _dbContext.SaveChangesAsync();

                response = new BaseCommandResponse<ICompanyModel>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = new CompanyModel
                    {
                        Id = entry.Id,
                        Host = entry.Host,
                        Name = entry.Name,
                        Color = entry.Color,
                        DateCreated = entry.DateCreated,
                        DateModified = entry.DateModified,
                        IsActive = entry.IsActive,
                        IsRoot = entry.IsRoot,
                        Logo = !string.IsNullOrWhiteSpace(entry.Logo) ? $"/{AppConstants.LogoFilesFolder}/{entry.Logo}?v={entry.DateModified.Ticks}" : null,
                        AuthType = entry.AuthType,
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<ICompanyModel>();
                response.HandleException(ex);
            }

            return response;
        }
    }
}