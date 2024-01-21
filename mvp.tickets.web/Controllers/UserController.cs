using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Security.Claims;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ISettings _settings;
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _env;

        public UserController(IUserService userService, ApplicationDbContext dbContext, ISettings settings, ILogger<UserController> logger, IWebHostEnvironment env)
        {
            _userService = userService;
            _dbContext = dbContext;
            _settings = settings;
            _logger = logger;
            _env = env;
        }

        [HttpGet("current")]
        public IBaseQueryResponse<IUserModel> Current()
        {
            if (User.Identity.IsAuthenticated)
            {
                BaseQueryResponse<IUserModel> response = default;
                try
                {
                    var user = System.Text.Json.JsonSerializer.Deserialize<UserModel>(User.Claims.First(s => s.Type == AuthConstants.UserDataClaim).Value);
                    response = new BaseQueryResponse<IUserModel>
                    {
                        IsSuccess = true,
                        Code = domain.Enums.ResponseCodes.Success,
                        Data = user
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    response = new BaseQueryResponse<IUserModel>();
                    response.HandleException(ex);
                }
                return response;
            }
            else
            {
                return new BaseQueryResponse<IUserModel>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = null
                };
            }
        }

        [HttpPost("registerRequest")]
        public async Task<IBaseCommandResponse<bool>> RegisterRequest([FromBody] UserRegisterRequestCommandRequest request,
            [FromServices] ICompanyStore companyStore, [FromServices] IEmailService emailService)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest,
                    Data = false
                };
            }
            IBaseCommandResponse<bool> response = default;
            try
            {
                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var company = await companyStore.GetByHost(host);
                if (company == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = false,
                        ErrorMessage = "����������� �� �������."
                    };
                }

                var email = request.Email.ToLower();
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == company.Id);
                if (existingUser != null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "����� ������������ ��� ����������."
                    };
                }

                var userData = new UserJWTData(email, company.Id, JWTType.Register);
                var code = TokenHelper.GenerateToken(userData, 30);
                await emailService.Send(email, $"{company.Name} - �����������.",
                    $"��� ����������� ����������� ��������� �� ��������� ������ <a href='https://{host}/register/?email={email}&code={code}'>������� �����</a>", true);

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = true,
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

        [HttpPost("register")]
        public async Task<IBaseCommandResponse<bool>> Register([FromBody] UserRegisterCommandRequest request,
            [FromServices] ICompanyStore companyStore)
        {
            if (string.IsNullOrWhiteSpace(request?.Code) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest,
                    Data = false
                };
            }
            IBaseCommandResponse<bool> response = default;
            try
            {
                var userData = TokenHelper.ValidateToken(request.Code);
                if (userData == null || userData.Type != JWTType.Register)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "��� ������� �� ������������."
                    };
                }

                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var company = await companyStore.GetByHost(host);
                if (company == null || company.Id != userData.CompanyId)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = false,
                        ErrorMessage = "����������� �� �������."
                    };
                }

                var email = userData.Email.ToLower();
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == company.Id);
                if (existingUser != null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "����� ������������ ��� ����������."
                    };
                }

                var user = new User
                {
                    CompanyId = company.Id,
                    Email = email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    IsLocked = false,
                    Password = HashHelper.GetSHA256Hash(request.Password),
                    Permissions = domain.Enums.Permissions.User,
                    Phone = null,
                    DateCreated = DateTimeOffset.UtcNow,
                    DateModified = DateTimeOffset.UtcNow,
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                
                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = true,
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

        [HttpPost("login")]
        public async Task<IBaseCommandResponse<IUserModel>> Login(UserLoginCommandRequest request)
        {
            IBaseCommandResponse<IUserModel> response = default;
            try
            {
                if (request != null)
                {
                    var host = Request.Host.Value.ToLower();
                    if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                    {
                        host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                    }
                    request.Host = host;
                }

                var loginResponse = await _userService.Login(request);

                if (loginResponse.IsSuccess)
                {
                    var claimsIdentity = new ClaimsIdentity(loginResponse.Data.claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                }

                response = new BaseCommandResponse<IUserModel>
                {
                    IsSuccess = loginResponse.IsSuccess,
                    Code = loginResponse.Code,
                    ErrorMessage = loginResponse.ErrorMessage,
                    Data = loginResponse.Data.user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<IUserModel>();
                response.HandleException(ex);
            }
            return response;
        }

        [HttpPost("loginByCode")]
        public async Task<IBaseCommandResponse<IUserModel>> LoginByCode(UserLoginByCodeCommandRequest request, [FromServices] ICompanyStore companyStore)
        {
            if (string.IsNullOrWhiteSpace(request?.Code))
            {
                return new BaseCommandResponse<IUserModel>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }
            IBaseCommandResponse<IUserModel> response = default;
            try
            {
                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var userData = TokenHelper.ValidateToken(request.Code);
                if (userData == null || userData.Type != JWTType.Support)
                {
                    return new BaseCommandResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "��� ������� �� ������������."
                    };
                }
                var rootCompany = await companyStore.GetByHost(host);
                if (!rootCompany.IsRoot)
                {
                    return new BaseCommandResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "������� ���� �� �������� ��������."
                    };
                }
                var userCompany = await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userData.CompanyId && s.IsActive);
                if (userCompany == null)
                {
                    return new BaseCommandResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "������������ �����������."
                    };
                }
                var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == userData.Email && !s.IsLocked);
                if (user == null || !user.Permissions.HasFlag(Permissions.Admin))
                {
                    return new BaseCommandResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "������������ ������������."
                    };
                }

                var email = $"{userCompany.Id}@company";
                var rootUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == rootCompany.Id);
                if (rootUser == null)
                {
                    rootUser = new User
                    {
                        CompanyId = rootCompany.Id,
                        Email = email,
                        IsLocked = false,
                        Password = HashHelper.GetSHA256Hash(Guid.NewGuid().ToString()),
                        DateCreated = DateTimeOffset.UtcNow,
                        DateModified = DateTimeOffset.UtcNow,
                        Phone = null,
                        Permissions = Permissions.User,
                        FirstName = "�����������",
                        LastName = userCompany.Name,
                    };
                    _dbContext.Users.Add(rootUser);
                    await _dbContext.SaveChangesAsync();
                }
                else if (rootUser.IsLocked)
                {
                    return new BaseCommandResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "������������ ������������."
                    };
                }

                var userModel = new UserModel
                {
                    Id = rootUser.Id,
                    CompanyId = rootUser.CompanyId,
                    Email = rootUser.Email,
                    IsLocked = rootUser.IsLocked,
                    Permissions = rootUser.Permissions,
                    FirstName = rootUser.FirstName,
                    LastName = rootUser.LastName,
                    DateCreated = rootUser.DateCreated,
                    DateModified = rootUser.DateModified,
                };

                var claims = UserHelper.GetClaims(userModel, rootCompany.IsRoot);

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                response = new BaseCommandResponse<IUserModel>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = userModel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<IUserModel>();
                response.HandleException(ex);
            }
            return response;
        }

        [HttpPost("logout")]
        public async Task<IBaseCommandResponse<object>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new BaseCommandResponse<object>
            {
                IsSuccess = true,
                Code = domain.Enums.ResponseCodes.Success
            };
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost("report")]
        public async Task<IBaseReportQueryResponse<IEnumerable<IUserModel>>> GetReport(BaseReportQueryRequest request)
        {
            if (request != null)
            {
                request.CompanyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
            }
            return await _userService.GetReport(request);
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<IBaseQueryResponse<IUserModel>> Get(int id)
        {
            var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
            IBaseQueryResponse<IUserModel> response = default;
            try
            {
                var user = await _dbContext.Users
                    .Where(s => s.Id == id && s.CompanyId == companyId)
                    .Select(s => new UserModel
                    {
                        Id = id,
                        Email = s.Email,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Permissions = s.Permissions,
                        IsLocked = s.IsLocked,
                        DateCreated = s.DateCreated,
                        DateModified = s.DateModified,
                        CompanyId = companyId
                    }).FirstOrDefaultAsync();

                response = user != null
                    ? new BaseQueryResponse<IUserModel>
                    {
                        IsSuccess = true,
                        Code = domain.Enums.ResponseCodes.Success,
                        Data = user
                    }
                    : new BaseQueryResponse<IUserModel>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        ErrorMessage = "������������ �� ������."
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IUserModel>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create(UserCreateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<int> response = default;
            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var email = request.Email.ToLower();
                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == companyId);
                if (user != null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        ErrorMessage = $"������������ � ����������� ������� {email} ��� ����������."
                    };
                }

                user = new data.Models.User
                {
                    Email = email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Permissions = request.Permissions,
                    IsLocked = request.IsLocked,
                    DateCreated = DateTimeOffset.UtcNow,
                    DateModified = DateTimeOffset.UtcNow,
                    CompanyId = companyId,
                    Password = HashHelper.GetSHA256Hash(request.Password),
                };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                response = new BaseCommandResponse<int>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = user.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<int>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<IBaseCommandResponse<bool>> Update(UserUpdateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<bool> response = default;
            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == companyId);
                if (user == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        ErrorMessage = "������������ �� ������."
                    };
                }

                var email = request.Email.ToLower();
                var oldEmail = user.Email;
                if (oldEmail != email && await _dbContext.Users.AnyAsync(s => s.Email == email && s.Id != request.Id && s.CompanyId == companyId))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        ErrorMessage = $"������������ � ������ {email} ��� ����������."
                    };
                }
                
                user.Email = email;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Permissions = request.Permissions;
                user.IsLocked = request.IsLocked;
                user.DateModified = DateTimeOffset.UtcNow;
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    user.Password = HashHelper.GetSHA256Hash(request.Password);
                }

                await _dbContext.SaveChangesAsync();

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
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

        [HttpPost("forgotPassword")]
        public async Task<IBaseCommandResponse<bool>> ForgotPassword([FromBody] UserFortogPasswordCommandRequest request,
            [FromServices] ICompanyStore companyStore, [FromServices] IEmailService emailService)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest,
                    Data = false
                };
            }
            IBaseCommandResponse<bool> response = default;
            try
            {
                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var company = await companyStore.GetByHost(host);
                if (company == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = false,
                        ErrorMessage = "����������� �� �������."
                    };
                }

                var email = request.Email.ToLower();
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == company.Id);
                if (existingUser == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = false,
                        ErrorMessage = "������������ �� ������."
                    };
                }

                var userData = new UserJWTData(email, company.Id, JWTType.ResetPassword);
                var code = TokenHelper.GenerateToken(userData, 30);
                await emailService.Send(email, $"{company.Name} - ����� ������.",
                    $"��� ������ ������ ��������� �� ��������� ������ <a href='https://{host}/resetPassword/?code={code}'>������� �����</a>", true);

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = true,
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

        [HttpPost("resetPassword")]
        public async Task<IBaseCommandResponse<bool>> ResetPassword([FromBody] UserResetPasswordCommandRequest request,
            [FromServices] ICompanyStore companyStore)
        {
            if (string.IsNullOrWhiteSpace(request?.Code) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = domain.Enums.ResponseCodes.BadRequest,
                    Data = false
                };
            }
            IBaseCommandResponse<bool> response = default;
            try
            {
                var userData = TokenHelper.ValidateToken(request.Code);
                if (userData == null || userData.Type != JWTType.ResetPassword)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "��� ������� �� ������������."
                    };
                }

                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }

                var company = await companyStore.GetByHost(host);
                if (company == null || company.Id != userData.CompanyId)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.NotFound,
                        Data = false,
                        ErrorMessage = "����������� �� �������."
                    };
                }

                var email = userData.Email.ToLower();
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email && s.CompanyId == company.Id);
                if (existingUser == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "������������ �� ������."
                    };
                }

                existingUser.DateModified = DateTimeOffset.UtcNow;
                existingUser.Password = HashHelper.GetSHA256Hash(request.Password);
                await _dbContext.SaveChangesAsync();

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = true,
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

        [Authorize(Policy = AuthConstants.EmployeePolicy)]
        [HttpGet("assignees")]
        public async Task<IBaseQueryResponse<IEnumerable<IUserAssigneeModel>>> GetAssignees()
        {
            IBaseQueryResponse<IEnumerable<IUserAssigneeModel>> response = default;
            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var query = _dbContext.Users.AsNoTracking().Where(s => s.CompanyId == companyId && !s.IsLocked
                        && s.Permissions.HasFlag(Permissions.Employee))
                    .Select(s => new UserAssigneeModel
                    {
                        Id = s.Id,
                        Name = s.FirstName + " " + s.LastName + " (" + s.Email + ")",
                    });

                var str = query.ToQueryString();

                var entries = await query.ToListAsync();
                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                response = new BaseQueryResponse<IEnumerable<IUserAssigneeModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries.OrderByDescending(s => s.Id == userId).ThenBy(s => s.Name).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<IUserAssigneeModel>>();
                response.HandleException(ex);
            }

            return response;
        }
    }
}