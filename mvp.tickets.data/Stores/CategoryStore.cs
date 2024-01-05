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
            using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
            {
                DynamicParameters parameter = new DynamicParameters();
                if (request.Id > 0)
                {
                    parameter.Add(GetCategoriesProcedure.Params.Id, request.Id.Value, DbType.Int32);
                }
                parameter.Add(GetCategoriesProcedure.Params.OnlyDefault, request.OnlyDefault, DbType.Boolean);
                parameter.Add(GetCategoriesProcedure.Params.OnlyActive, request.OnlyActive, DbType.Boolean);
                parameter.Add(GetCategoriesProcedure.Params.OnlyRoot, request.OnlyRoot, DbType.Boolean);

                var categories = await connection.QueryAsync<CategoryModel>(GetCategoriesProcedure.Name, param: parameter,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

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
            if (await _dbContext.TicketCategories.AnyAsync(s => s.Name == request.Name).ConfigureAwait(false))
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория с названием {request.Name} уже существует."
                };
            }
            if (request.IsDefault && await _dbContext.TicketCategories.AnyAsync(s => s.IsDefault).ConfigureAwait(false))
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
                DateCreated = DateTimeOffset.Now,
                DateModified = DateTimeOffset.Now,
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
            if (await _dbContext.TicketCategories.AnyAsync(s => s.Name == request.Name && s.Id != request.Id).ConfigureAwait(false))
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest,
                    ErrorMessage = $"Категория с названием {request.Name} уже существует.",
                    Data = false
                };
            }

            if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault && s.Id != request.Id).ConfigureAwait(false))
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

            var category = await queryable.FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
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
            category.DateModified = DateTimeOffset.Now;

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
