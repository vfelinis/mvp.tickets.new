using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<TicketPriorityHistory> FromTicketPriorityHistories { get; set; } = new List<TicketPriorityHistory>();
        public List<TicketPriorityHistory> ToTicketPriorityHistories { get; set; } = new List<TicketPriorityHistory>();
    }

    public static class TicketPriorityExtension
    {
        public static string TableName => "TicketPriorities";

        public static void DescribeTicketPriority(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketPriority>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<TicketPriority>()
                .HasOne(c => c.Company)
                .WithMany(p => p.TicketPriorities)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketPriority>()
                .HasIndex(p => new { p.CompanyId, p.Name })
                .IsUnique(true);

            modelBuilder.Entity<TicketPriority>().ToTable(TableName);
        }
    }
}
