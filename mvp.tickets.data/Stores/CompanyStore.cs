using Microsoft.EntityFrameworkCore;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Stores;
using System.Data;

namespace mvp.tickets.data.Stores
{
    public class CompanyStore : ICompanyStore
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConnectionStrings _connectionStrings;

        public CompanyStore(ApplicationDbContext dbContext, IConnectionStrings connectionStrings)
        {
            _dbContext = dbContext ?? ThrowHelper.ArgumentNull<ApplicationDbContext>();
            _connectionStrings = connectionStrings ?? ThrowHelper.ArgumentNull<IConnectionStrings>();
        }

        public async Task<ICompanyModel> GetByHost(string host)
        {
            return await _dbContext.Companies.Where(x => x.Host == host && x.IsActive)
                .Select(s => new CompanyModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Host = s.Host,
                    IsActive = s.IsActive,
                    IsRoot = s.IsRoot,
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified,
                    Logo = s.Logo,
                    Color = s.Color
                }).FirstOrDefaultAsync();
        }
    }
}
