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

        public UserController(IUserService userService, ApplicationDbContext dbContext, ISettings settings, ILogger<UserController> logger)
        {
            _userService = userService;
            _dbContext = dbContext;
            _settings = settings;
            _logger = logger;
        }

        [HttpPost("current")]
        public async Task<IBaseQueryResponse<IUserModel>> Current()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
                var response = await _userService.Query(new UserQueryRequest { Id = id });
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
            return await _userService.GetReport(request);
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<IBaseQueryResponse<IUserModel>> Get(int id)
        {
            IBaseQueryResponse<IUserModel> response = default;
            try
            {
                var user = await _dbContext.Users
                    .Where(s => s.Id == id)
                    .Select(s => new UserModel
                    {
                        Id = id,
                        Email = s.Email,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Permissions = s.Permissions,
                        IsLocked = s.IsLocked,
                        DateCreated = s.DateCreated,
                        DateModified = s.DateModified
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
                var email = request.Email.ToLower();
                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email);
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
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now
                };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                response = new BaseCommandResponse<int>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = user.Id
                };

                try
                {
                    var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
                    await firebaseAuth.CreateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                    {
                        Email = user.Email,
                        DisplayName = $"{user.FirstName} {user.LastName}",
                        Password = !string.IsNullOrWhiteSpace(request.Password)
                            ? request.Password
                            : Guid.NewGuid().ToString()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
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
                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == request.Id);
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
                if (oldEmail != email && await _dbContext.Users.AnyAsync(s => s.Email == email && s.Id != request.Id))
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
                user.DateModified = DateTimeOffset.Now;

                await _dbContext.SaveChangesAsync();

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = domain.Enums.ResponseCodes.Success,
                    Data = true
                };

                if (oldEmail != email || !string.IsNullOrWhiteSpace(request.Password))
                {
                    try
                    {
                        var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
                        var fbUser = await firebaseAuth.GetUserByEmailAsync(oldEmail);
                        if (fbUser != null)
                        {
                            await firebaseAuth.UpdateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                            {
                                Uid = fbUser.Uid,
                                Email = email,
                                Password = !string.IsNullOrWhiteSpace(request.Password)
                                    ? request.Password
                                    : null
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
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