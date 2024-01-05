using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;
using System.Data;

namespace mvp.tickets.data.Stores
{
    public class UserStore : IUserStore
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConnectionStrings _connectionStrings;

        public UserStore(ApplicationDbContext dbContext, IConnectionStrings connectionStrings)
        {
            _dbContext = dbContext ?? ThrowHelper.ArgumentNull<ApplicationDbContext>();
            _connectionStrings = connectionStrings ?? ThrowHelper.ArgumentNull<IConnectionStrings>();
        }

        public async Task<IBaseCommandResponse<IUserModel>> Create(IUserCreateCommandRequest request)
        {
            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsLocked = request.IsLocked,
                Permissions = request.Permissions,
                DateCreated = DateTimeOffset.Now,
                DateModified = DateTimeOffset.Now
            };
            await _dbContext.Users.AddAsync(user).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return new BaseCommandResponse<IUserModel>
            {
                Data = new UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsLocked = user.IsLocked,
                    Permissions = user.Permissions,
                    DateCreated = user.DateCreated,
                    DateModified = user.DateModified,
                },
                IsSuccess = true,
                Code = ResponseCodes.Success
            };
        }

        public async Task<IBaseQueryResponse<IUserModel>> Query(IUserQueryRequest request)
        {
            var response = new BaseReportQueryResponse<IUserModel>();
            User user = default;
            if (request?.Email != null)
            {
                user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == request.Email.ToLower()).ConfigureAwait(false);
            }
            else if (request?.Id != null)
            {
                user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
            }

            if (user != null)
            {
                response.IsSuccess = true;
                response.Code = ResponseCodes.Success;
                response.Data = new UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Permissions = user.Permissions,
                    IsLocked = user.IsLocked,
                    DateCreated = user.DateCreated,
                    DateModified = user.DateModified,
                };
            }
            else
            {
                response.IsSuccess = false;
                response.Code = ResponseCodes.NotFound;
            }

            return response;
        }

        public async Task<IBaseReportQueryResponse<IEnumerable<IUserModel>>> GetReport(IBaseReportQueryRequest request)
        {
            using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
            {
                DynamicParameters parameter = new DynamicParameters();
                if (request.SearchBy?.Any() == true)
                {
                    foreach (var search in request.SearchBy.Where(s => !string.IsNullOrWhiteSpace($"{s.Value}")))
                    {
                        if (string.Equals(search.Key, nameof(User.Email), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchByEmal, search.Value, DbType.String);
                        }
                        else if (string.Equals(search.Key, nameof(User.FirstName), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchByFirstName, search.Value, DbType.String);
                        }
                        else if (string.Equals(search.Key, nameof(User.LastName), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchByLastName, search.Value, DbType.String);
                        }
                        else if (string.Equals(search.Key, nameof(User.IsLocked), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchByIsLocked, Convert.ToBoolean(search.Value), DbType.Boolean);
                        }
                        else if (string.Equals(search.Key, nameof(User.Permissions), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchByPermissions, Convert.ToInt32(search.Value), DbType.Int32);
                        }
                        else if (string.Equals(search.Key, nameof(User.Id), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add(UsersReportProcedure.Params.SearchById, Convert.ToInt32(search.Value), DbType.Int32);
                        }
                    }
                }
                
                parameter.Add(UsersReportProcedure.Params.SortBy, request.SortBy, DbType.String);
                parameter.Add(UsersReportProcedure.Params.SortDirection, request.SortDirection.ToString(), DbType.String);
                parameter.Add(UsersReportProcedure.Params.Offset, request.Offset, DbType.Int32);
                parameter.Add(UsersReportProcedure.Params.Limit, ReportConstants.DEFAULT_LIMIT, DbType.Int32);

                var query = await connection.QueryAsync<UserReportModel>(UsersReportProcedure.Name, param: parameter,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                var entries = query.ToList();
                return new BaseReportQueryResponse<IEnumerable<IUserModel>>
                {
                    Data = entries,
                    Total = entries.FirstOrDefault()?.Total ?? 0,
                    IsSuccess = true,
                    Code = ResponseCodes.Success
                };
            }
        }
    }
}
