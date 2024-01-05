using Microsoft.Extensions.Logging;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;

namespace mvp.tickets.domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryStore _store;
        private readonly ILogger<CategoryService> _logger;
        private readonly ISettings _settings;

        public CategoryService(ICategoryStore store, ILogger<CategoryService> logger, ISettings settings)
        {
            _store = store;
            _logger = logger;
            _settings = settings;
        }

        public async Task<IBaseQueryResponse<IEnumerable<ICategoryModel>>> Query(ICategoryQueryRequest request)
        {
            if (request == null)
            {
                return new BaseQueryResponse<IEnumerable<ICategoryModel>>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseQueryResponse<IEnumerable<ICategoryModel>> response = default;

            try
            {
                response = await _store.Query(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<ICategoryModel>>();
                response.HandleException(ex);
            }
            return response;
        }

        public async Task<IBaseCommandResponse<int>> Create(ICategoryCreateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<int> response = default;

            try
            {
                response = await _store.Create(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<int>();
                response.HandleException(ex);
            }
            return response;
        }

        public async Task<IBaseCommandResponse<bool>> Update(ICategoryUpdateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<bool> response = default;

            try
            {
                response = await _store.Update(request).ConfigureAwait(false);
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
