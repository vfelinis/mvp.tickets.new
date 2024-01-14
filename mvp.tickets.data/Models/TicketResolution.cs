using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketResolution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

    public static class TicketResolutionExtension
    {
        public static string TableName => "TicketResolutions";

        public static void DescribeTicketResolution(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketResolution>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<TicketResolution>()
                .HasOne(c => c.Company)
                .WithMany(p => p.TicketResolutions)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketResolution>()
                .HasIndex(p => new { p.CompanyId, p.Name })
                .IsUnique(true);

            modelBuilder.Entity<TicketResolution>().ToTable(TableName);
        }
    }
}
