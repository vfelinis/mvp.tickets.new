using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;
using System.Security.Claims;

namespace mvp.tickets.domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;
        private readonly ILogger<UserService> _logger;
        private readonly ISettings _settings;

        public UserService(IUserStore userStore, ILogger<UserService> logger, ISettings settings)
        {
            _userStore = userStore;
            _logger = logger;
            _settings = settings;
        }

        public async Task<IBaseQueryResponse<IUserModel>> Query(IUserQueryRequest request)
        {
            if (request == null)
            {
                return new BaseQueryResponse<IUserModel>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseQueryResponse<IUserModel> response = default;

            try
            {
                response = await _userStore.Query(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IUserModel>();
                response.HandleException(ex);
            }
            return response;
        }

        public async Task<IBaseCommandResponse<(IUserModel user, List<Claim> claims)>> Login(IUserLoginCommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.IdToken))
            {
                return new BaseCommandResponse<(IUserModel user, List<Claim> claims)>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<(IUserModel user, List<Claim> claims)> response = default;

            try
            {
                var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
                FirebaseToken decoded = await firebaseAuth.VerifyIdTokenAsync(request.IdToken);
                var email = decoded.Claims["email"].ToString().ToLower();
                var userResponse = await _userStore.Query(new UserQueryRequest { Email = email }).ConfigureAwait(false);
                IUserModel userModel = userResponse.Data;
                if (userResponse.Code == ResponseCodes.NotFound)
                {
                    var name = decoded.Claims.TryGetValue("name", out object val)
                        ? (string)val
                        : email.Split('@').First();

                    var createResponse = await _userStore.Create(new UserCreateCommandRequest
                    {
                        Email = email,
                        FirstName = name?.Split(' ').First(),
                        LastName = name?.Split(' ').Last(),
                        Permissions = Permissions.User,
                        IsLocked = false
                    }).ConfigureAwait(false);

                    userModel = createResponse.Data;
                }
                else if (userModel.IsLocked)
                {
                    response = new BaseCommandResponse<(IUserModel user, List<Claim> claims)>
                    {
                        Data = (userModel, new List<Claim>()),
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "Учетная запись пользователя заблокирована."
                    };
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, userModel.Id.ToString()),
                    new Claim(AuthConstants.FirebaseIdClaim, decoded.Uid),
                };

                if (userModel.Permissions.HasFlag(Permissions.Admin))
                {
                    claims.Add(new Claim(AuthConstants.AdminClaim, "true"));
                }
                if (userModel.Permissions.HasFlag(Permissions.Employee))
                {
                    claims.Add(new Claim(AuthConstants.EmployeeClaim, "true"));
                }
                if (userModel.Permissions.HasFlag(Permissions.User))
                {
                    claims.Add(new Claim(AuthConstants.UserClaim, "true"));
                }

                response = new BaseCommandResponse<(IUserModel user, List<Claim> claims)>
                {
                    Data = (userModel, claims),
                    IsSuccess = true,
                    Code = ResponseCodes.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<(IUserModel user, List<Claim> claims)>();
                response.HandleException(ex);
            }
            return response;
        }

        public async Task<IBaseReportQueryResponse<IEnumerable<IUserModel>>> GetReport(IBaseReportQueryRequest request)
        {
            if (request == null)
            {
                return new BaseReportQueryResponse<IEnumerable<IUserModel>>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseReportQueryResponse<IEnumerable<IUserModel>> response = default;

            try
            {
                response = await _userStore.GetReport(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseReportQueryResponse<IEnumerable<IUserModel>>();
                response.HandleException(ex);
            }
            return response;
        }
    }
}
