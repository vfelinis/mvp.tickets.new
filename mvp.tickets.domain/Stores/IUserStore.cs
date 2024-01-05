using mvp.tickets.domain.Models;

namespace mvp.tickets.domain.Stores
{
    public interface IUserStore
    {
        Task<IBaseCommandResponse<IUserModel>> Create(IUserCreateCommandRequest request);
        Task<IBaseQueryResponse<IUserModel>> Query(IUserQueryRequest request);
        Task<IBaseReportQueryResponse<IEnumerable<IUserModel>>> GetReport(IBaseReportQueryRequest request);
    }
}
