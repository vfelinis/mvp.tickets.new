using mvp.tickets.domain.Models;
using System.Security.Claims;

namespace mvp.tickets.domain.Services
{
    public interface IUserService
    {
        Task<IBaseQueryResponse<IUserModel>> Query(IUserQueryRequest request);
        Task<IBaseCommandResponse<(IUserModel user, List<Claim> claims)>> Login(IUserLoginCommandRequest request);
        Task<IBaseReportQueryResponse<IEnumerable<IUserModel>>> GetReport(IBaseReportQueryRequest request);
    }
}