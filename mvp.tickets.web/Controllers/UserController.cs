using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;
using mvp.tickets.web.Extensions;
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

        [HttpPost("current")]
        public async Task<IBaseQueryResponse<IUserModel>> Current()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
                var compantId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var response = await _userService.Query(new UserQueryRequest { Id = id, CompanyId = compantId });
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
    }
}