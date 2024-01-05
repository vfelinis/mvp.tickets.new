using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketQueueHistory
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int FromTicketQueueId { get; set; }
        public TicketQueue FromTicketQueue { get; set; }

        public int ToTicketQueueId { get; set; }
        public TicketQueue ToTicketQueue { get; set; }
    }

    public static class TicketQueueHistoryExtension
    {
        public static string TableName => "TicketQueueHistories";

        public static void DescribeTicketQueueHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketQueueHistory>(s =>
            {
                s.Property(p => p.UserId).IsRequired(false);
            });

            modelBuilder.Entity<TicketQueueHistory>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketQueueHistories)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketQueueHistory>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketQueueHistories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketQueueHistory>()
                .HasOne(c => c.FromTicketQueue)
                .WithMany(p => p.FromTicketQueueHistories)
                .HasForeignKey(c => c.FromTicketQueueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketQueueHistory>()
                .HasOne(c => c.ToTicketQueue)
                .WithMany(p => p.ToTicketQueueHistories)
                .HasForeignKey(c => c.ToTicketQueueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketQueueHistory>().ToTable(TableName);
        }
    }
}
