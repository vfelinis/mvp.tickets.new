using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketPriorityHistory
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? FromTicketPriorityId { get; set; }
        public TicketPriority FromTicketPriority { get; set; }

        public int ToTicketPriorityId { get; set; }
        public TicketPriority ToTicketPriority { get; set; }
    }

    public static class TicketPriorityHistoryExtension
    {
        public static string TableName => "TicketPriorityHistories";

        public static void DescribeTicketPriorityHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketPriorityHistory>(s =>
            {
                s.Property(p => p.FromTicketPriorityId).IsRequired(false);
                s.Property(p => p.UserId).IsRequired(false);
            });

            modelBuilder.Entity<TicketPriorityHistory>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketPriorityHistories)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketPriorityHistory>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketPriorityHistories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketPriorityHistory>()
                .HasOne(c => c.FromTicketPriority)
                .WithMany(p => p.FromTicketPriorityHistories)
                .HasForeignKey(c => c.FromTicketPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketPriorityHistory>()
                .HasOne(c => c.ToTicketPriority)
                .WithMany(p => p.ToTicketPriorityHistories)
                .HasForeignKey(c => c.ToTicketPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketPriorityHistory>().ToTable(TableName);
        }
    }
}
