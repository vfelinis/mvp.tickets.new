using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public bool IsInternal { get; set; }
        public string Type { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }

    public static class TicketHistoryExtension
    {
        public static string TableName => "TicketHistories";

        public static void DescribeTicketHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketHistory>(s =>
            {
                s.Property(p => p.Type).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.UserId).IsRequired(false);
            });

            modelBuilder.Entity<TicketHistory>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketHistories)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketHistory>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketHistories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketHistory>().ToTable(TableName);
        }
    }
}
