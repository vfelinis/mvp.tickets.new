using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketStatusHistory
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int FromTicketStatusId { get; set; }
        public TicketStatus FromTicketStatus { get; set; }

        public int ToTicketStatusId { get; set; }
        public TicketStatus ToTicketStatus { get; set; }
    }

    public static class TicketStatusHistoryExtension
    {
        public static string TableName => "TicketStatusHistories";

        public static void DescribeTicketStatusHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketStatusHistory>(s =>
            {
                s.Property(p => p.UserId).IsRequired(false);
            });

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketStatusHistories)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketStatusHistories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(c => c.FromTicketStatus)
                .WithMany(p => p.FromTicketStatusHistories)
                .HasForeignKey(c => c.FromTicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(c => c.ToTicketStatus)
                .WithMany(p => p.ToTicketStatusHistories)
                .HasForeignKey(c => c.ToTicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusHistory>().ToTable(TableName);
        }
    }
}
