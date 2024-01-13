using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace mvp.tickets.data
{
    class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(@"Host=192.168.1.134;Port=5432;Database=tickets;Username=postgres;Password=111111");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
