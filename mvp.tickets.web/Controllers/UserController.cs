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
using Org.BouncyCastle.Asn1.Ocsp;
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
                        ErrorMessage = "Предприятие не найдено."
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
                        ErrorMessage = "Такой пользователь уже существует."
                    };
                }

                var userData = new UserRegisterRequestData(email, company.Id);
                var code = TokenHelper.GenerateToken(userData, 30);
                await emailService.Send(email, $"{company.Name} - регистрация.",
                    $"Для продолжения регистрации перейдите по следующей ссылке <a href='https://{host}/register/?email={email}&code={code}'>нажмите здесь</a>", true);

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
                if (userData == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "Код доступа не действителен."
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
                        ErrorMessage = "Предприятие не найдено."
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
                        ErrorMessage = "Такой пользователь уже существует."
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
            if (request != null)
            {
                var host = Request.Host.Value.ToLower();
                if (!_env.IsProduction() && Request.Cookies.ContainsKey(AppConstants.DebugHostCookie))
                {
                    host = Request.Cookies[AppConstants.DebugHostCookie].ToLower();
                }
                request.Host = host;
            }
            var response = await _userService.Login(request);
            if (response.IsSuccess)
            {
                var claimsIdentity = new ClaimsIdentity(response.Data.claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }
            return new BaseCommandResponse<IUserModel>
            {
                IsSuccess = response.IsSuccess,
                Code = response.Code,
                ErrorMessage = response.ErrorMessage,
                Data = response.Data.user
            };
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
                        ErrorMessage = "Пользователь не найден."
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
                        ErrorMessage = $"Пользователь с электронным адресом {email} уже существует."
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
                        ErrorMessage = "Пользователь не найден."
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
                        ErrorMessage = $"Пользователь с почтой {email} уже существует."
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
                        ErrorMessage = "Предприятие не найдено."
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
                        ErrorMessage = "Пользователь не найден."
                    };
                }

                var userData = new UserRegisterRequestData(email, company.Id);
                var code = TokenHelper.GenerateToken(userData, 30);
                await emailService.Send(email, $"{company.Name} - сброс пароля.",
                    $"Для сброса пароля перейдите по следующей ссылке <a href='https://{host}/resetPassword/?code={code}'>нажмите здесь</a>", true);

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
                if (userData == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = domain.Enums.ResponseCodes.BadRequest,
                        Data = false,
                        ErrorMessage = "Код доступа не действителен."
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
                        ErrorMessage = "Предприятие не найдено."
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
                        ErrorMessage = "Пользователь не найден."
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