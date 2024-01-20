using Dapper;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;
using Npgsql;
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
                DateCreated = DateTimeOffset.UtcNow,
                DateModified = DateTimeOffset.UtcNow,
                CompanyId = request.CompanyId,
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
                    CompanyId = user.CompanyId,
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
                if (request.Password != null)
                {
                    user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == request.Email.ToLower()
                        && s.Password == request.Password && s.CompanyId == request.CompanyId).ConfigureAwait(false);
                }
                else
                {
                    user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == request.Email.ToLower() && s.CompanyId == request.CompanyId).ConfigureAwait(false);
                }
            }
            else if (request?.Id != null)
            {
                user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == request.CompanyId).ConfigureAwait(false);
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
                    CompanyId = user.CompanyId,
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
            using (var connection = new NpgsqlConnection(_connectionStrings.DefaultConnection))
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@companyId", request.CompanyId, DbType.Int32);
                var query =
$@"SELECT
    u.""{nameof(User.Id)}"" AS ""{nameof(UserReportModel.Id)}""
    ,u.""{nameof(User.Email)}"" AS ""{nameof(UserReportModel.Email)}""
    ,u.""{nameof(User.FirstName)}"" AS ""{nameof(UserReportModel.FirstName)}""
    ,u.""{nameof(User.LastName)}"" AS ""{nameof(UserReportModel.LastName)}""
    ,u.""{nameof(User.Permissions)}"" AS ""{nameof(UserReportModel.Permissions)}""
    ,u.""{nameof(User.IsLocked)}"" AS ""{nameof(UserReportModel.IsLocked)}""
    ,u.""{nameof(User.DateCreated)}"" AS ""{nameof(UserReportModel.DateCreated)}""
    ,u.""{nameof(User.DateModified)}"" AS ""{nameof(UserReportModel.DateModified)}""
    ,u.""{nameof(User.CompanyId)}"" AS ""{nameof(UserReportModel.CompanyId)}""
    ,COUNT(*) OVER() AS ""{nameof(UserReportModel.Total)}""
FROM dbo.""{UserExtension.TableName}"" u
WHERE u.""{nameof(User.CompanyId)}"" = @companyId";
                
                if (request.SearchBy?.Any() == true)
                {
                    foreach (var search in request.SearchBy.Where(s => !string.IsNullOrWhiteSpace($"{s.Value}")))
                    {
                        if (string.Equals(search.Key, nameof(User.Email), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchByEmal", search.Value.ToLower(), DbType.String);
                            query += $@" AND ""{nameof(User.Email)}"" = @searchByEmal";
                        }
                        else if (string.Equals(search.Key, nameof(User.FirstName), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchByFirstName", search.Value, DbType.String);
                            query += $@" AND ""{nameof(User.FirstName)}"" = @searchByFirstName";
                        }
                        else if (string.Equals(search.Key, nameof(User.LastName), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchByLastName", search.Value, DbType.String);
                            query += $@" AND ""{nameof(User.LastName)}"" = @searchByLastName";
                        }
                        else if (string.Equals(search.Key, nameof(User.IsLocked), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchByIsLocked", Convert.ToBoolean(search.Value), DbType.Boolean);
                            query += $@" AND ""{nameof(User.IsLocked)}"" = @searchByIsLocked";
                        }
                        else if (string.Equals(search.Key, nameof(User.Permissions), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchByPermissions", Convert.ToInt32(search.Value), DbType.Int32);
                            query += $@" AND (""{nameof(User.Permissions)}"" & @searchByPermissions) = @searchByPermissions";
                        }
                        else if (string.Equals(search.Key, nameof(User.Id), StringComparison.OrdinalIgnoreCase))
                        {
                            parameter.Add("@searchById", Convert.ToInt32(search.Value), DbType.Int32);
                            query += $@" AND ""{nameof(User.Id)}"" = @searchById";
                        }
                    }
                }

                parameter.Add("@offset", request.Offset, DbType.Int32);
                parameter.Add("@limit", ReportConstants.DEFAULT_LIMIT, DbType.Int32);
                query +=
$@"
ORDER BY ""{typeof(User).GetProperties().FirstOrDefault(s => s.Name == request.SortBy)?.Name ?? nameof(User.Id)}"" {request.SortDirection} OFFSET @offset LIMIT @limit";
                
                var result = await connection.QueryAsync<UserReportModel>(query, param: parameter);
                var entries = result.ToList();
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
