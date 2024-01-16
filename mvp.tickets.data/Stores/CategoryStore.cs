using Dapper;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;
using Npgsql;
using System.Data;

namespace mvp.tickets.data.Stores
{
    public class CategoryStore : ICategoryStore
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConnectionStrings _connectionStrings;

        public CategoryStore(ApplicationDbContext dbContext, IConnectionStrings connectionStrings)
        {
            _dbContext = dbContext ?? ThrowHelper.ArgumentNull<ApplicationDbContext>();
            _connectionStrings = connectionStrings ?? ThrowHelper.ArgumentNull<IConnectionStrings>();
        }

        public async Task<IBaseQueryResponse<IEnumerable<ICategoryModel>>> Query(ICategoryQueryRequest request)
        {
            using (var connection = new NpgsqlConnection(_connectionStrings.DefaultConnection))
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@companyId", request.CompanyId, DbType.Int32);
                var query =
$@"SELECT
    t1.""{nameof(TicketCategory.Id)}"" AS ""{nameof(CategoryModel.Id)}""
    ,t1.""{nameof(TicketCategory.Name)}"" AS ""{nameof(CategoryModel.Name)}""
    ,t1.""{nameof(TicketCategory.IsDefault)}"" AS ""{nameof(CategoryModel.IsDefault)}""
    ,t1.""{nameof(TicketCategory.IsActive)}"" AS ""{nameof(CategoryModel.IsActive)}""
    ,t1.""{nameof(TicketCategory.IsRoot)}"" AS ""{nameof(CategoryModel.IsRoot)}""
    ,t1.""{nameof(TicketCategory.DateCreated)}"" AS ""{nameof(CategoryModel.DateCreated)}""
    ,t1.""{nameof(TicketCategory.DateModified)}"" AS ""{nameof(CategoryModel.DateModified)}""
    ,t1.""{nameof(TicketCategory.ParentCategoryId)}"" AS ""{nameof(CategoryModel.ParentCategoryId)}""
    ,t2.""{nameof(TicketCategory.Name)}"" AS ""{nameof(CategoryModel.ParentCategory)}""
FROM dbo.""{TicketCategoryExtension.TableName}"" t1
LEFT JOIN dbo.""{TicketCategoryExtension.TableName}"" t2 ON t1.""{nameof(TicketCategory.ParentCategoryId)}"" = t2.""{nameof(TicketCategory.Id)}""
WHERE t1.""{nameof(TicketCategory.CompanyId)}"" = @companyId";
                
                if (request.Id > 0)
                {
                    parameter.Add("@id", request.Id.Value, DbType.Int32);
                    query += $@" AND t1.""{nameof(TicketCategory.Id)}"" = @id";
                }
                if (request.OnlyDefault)
                {
                    query += $@" AND t1.""{nameof(TicketCategory.IsDefault)}"" = true";
                }
                if (request.OnlyActive)
                {
                    query += $@" AND t1.""{nameof(TicketCategory.IsActive)}"" = true";
                }
                if (request.OnlyRoot)
                {
                    query += $@" AND t1.""{nameof(TicketCategory.IsRoot)}"" = true";
                }

                var categories = await connection.QueryAsync<CategoryModel>(query, param: parameter).ConfigureAwait(false);

                return new BaseQueryResponse<IEnumerable<ICategoryModel>>
                {
                    Data = categories.ToList(),
                    IsSuccess = true,
                    Code = ResponseCodes.Success
                };
            }
        }

        public async Task<IBaseCommandResponse<int>> Create(ICategoryCreateCommandRequest request)
        {
            if (await _dbContext.TicketCategories.AnyAsync(s => s.Name == request.Name && s.CompanyId == request.CompanyId).ConfigureAwait(false))
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория с названием {request.Name} уже существует."
                };
            }
            if (request.IsDefault && await _dbContext.TicketCategories.AnyAsync(s => s.IsDefault && s.CompanyId == request.CompanyId).ConfigureAwait(false))
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория по умолчанию уже существует."
                };
            }

            var category = new TicketCategory
            {
                Name = request.Name,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
                IsRoot = request.ParentCategoryId == null,
                ParentCategoryId = request.ParentCategoryId,
                DateCreated = DateTimeOffset.UtcNow,
                DateModified = DateTimeOffset.UtcNow,
                CompanyId = request.CompanyId,
            };
            await _dbContext.TicketCategories.AddAsync(category).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return new BaseCommandResponse<int>
            {
                IsSuccess = true,
                Code = ResponseCodes.Success,
                Data = category.Id
            };
        }

        public async Task<IBaseCommandResponse<bool>> Update(ICategoryUpdateCommandRequest request)
        {
            if (await _dbContext.TicketCategories.AnyAsync(s => s.Name == request.Name && s.Id != request.Id && s.CompanyId == request.CompanyId).ConfigureAwait(false))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория с названием {request.Name} уже существует.",
                    Data = false
                };
            }

            if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault && s.Id != request.Id & s.CompanyId == request.CompanyId).ConfigureAwait(false))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория по умолчанию уже существует.",
                    Data = false
                };
            }

            var queryable = _dbContext.TicketCategories.AsQueryable();
            if (request.ParentCategoryId != null)
            {
                queryable = queryable.Include(s => s.SubCategories);
            }

            var category = await queryable.FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == request.CompanyId).ConfigureAwait(false);
            if (category == null)
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.NotFound,
                    Data = false
                };
            }

            if (request.ParentCategoryId != null)
            {
                category.SubCategories.ForEach(s =>
                {
                    s.ParentCategoryId = request.ParentCategoryId;
                });
            }

            request.ParentCategoryId = request.ParentCategoryId != request.Id ? request.ParentCategoryId : null;

            category.Name = request.Name;
            category.IsDefault = request.IsDefault;
            category.IsActive = request.IsActive;
            category.IsRoot = request.ParentCategoryId == null;
            category.ParentCategoryId = request.ParentCategoryId;
            category.DateModified = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return new BaseCommandResponse<bool>
            {
                IsSuccess = true,
                Code = ResponseCodes.Success,
                Data = true
            };
        }
    }
}
