﻿using Microsoft.Extensions.Logging;
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
        private readonly ICompanyStore _companyStore;
        private readonly ILogger<UserService> _logger;
        private readonly ISettings _settings;

        public UserService(IUserStore userStore, ICompanyStore companyStore, ILogger<UserService> logger, ISettings settings)
        {
            _userStore = userStore;
            _companyStore = companyStore;
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
            if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
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
                
                var companyModel = await _companyStore.GetByHost(request.Host);
                if (companyModel == null)
                {
                    return new BaseCommandResponse<(IUserModel user, List<Claim> claims)>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "Host is unknown."
                    };
                }

                var email = request.Email;
                var password = HashHelper.GetSHA256Hash(request.Password);
                var userResponse = await _userStore.Query(new UserQueryRequest { Email = email, Password = password, CompanyId = companyModel.Id }).ConfigureAwait(false);
                IUserModel userModel = userResponse.Data;
                if (userResponse.Code == ResponseCodes.NotFound)
                {
                    return new BaseCommandResponse<(IUserModel user, List<Claim> claims)>
                    {
                        Data = (userModel, new List<Claim>()),
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "Неверный электронный адрес или пароль."
                    };
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

                var claims = UserHelper.GetClaims(userModel, companyModel.IsRoot);

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
