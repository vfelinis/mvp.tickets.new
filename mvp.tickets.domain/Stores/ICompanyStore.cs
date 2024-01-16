using mvp.tickets.domain.Models;

namespace mvp.tickets.domain.Stores
{
    public interface ICompanyStore
    {
        Task<int?> GetIdByHost(string host);
    }
}
