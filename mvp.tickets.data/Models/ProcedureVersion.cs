using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class ProcedureVersion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }

    public static class ProcedureVersionExtension
    {
        public const string TABLE_NAME = "ProcedureVersions";
        public static void DescribeProcedureVersion(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcedureVersion>(c =>
            {
                c.Property(p => p.Name).IsRequired(true);
            });
            modelBuilder.Entity<ProcedureVersion>().ToTable(TABLE_NAME);
        }
    }
}
