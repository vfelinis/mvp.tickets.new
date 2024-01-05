using mvp.tickets.domain.Models;

namespace mvp.tickets.domain.Services
{
    public interface ICategoryService
    {
        Task<IBaseQueryResponse<IEnumerable<ICategoryModel>>> Query(ICategoryQueryRequest request);
        Task<IBaseCommandResponse<int>> Create(ICategoryCreateCommandRequest request);
        Task<IBaseCommandResponse<bool>> Update(ICategoryUpdateCommandRequest request);
    }
}