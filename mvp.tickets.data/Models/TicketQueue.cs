using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketQueue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<TicketQueueHistory> FromTicketQueueHistories { get; set; } = new List<TicketQueueHistory>();
        public List<TicketQueueHistory> ToTicketQueueHistories { get; set; } = new List<TicketQueueHistory>();
    }

    public static class TicketQueueExtension
    {
        public static string TableName => "TicketQueues";

        public static void DescribeTicketQueue(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketQueue>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<TicketQueue>()
                .HasIndex(p => p.Name)
                .IsUnique(true);

            modelBuilder.Entity<TicketQueue>()
                .HasIndex(p => p.IsDefault)
                .IsUnique(true)
                .HasFilter($"[{nameof(TicketQueue.IsDefault)}] = 1 AND [{nameof(TicketQueue.IsActive)}] = 1");

            modelBuilder.Entity<TicketQueue>().ToTable(TableName);
        }
    }
}
