using Microsoft.EntityFrameworkCore;
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

        public async Task<ICompanyModel> Get(ICompanyQueryRequest request)
        {
            return await _dbContext.Companies.Where(x => x.Host == request.Host).Select(s => new CompanyModel
            {
                Id = s.Id,
                Host = s.Host,
                Name = s.Name,
                IsRoot = s.IsRoot,
                IsActive = s.IsActive,
                DateCreated = s.DateCreated,
                DateModified = s.DateModified
            }).FirstOrDefaultAsync();
        }
    }
}
