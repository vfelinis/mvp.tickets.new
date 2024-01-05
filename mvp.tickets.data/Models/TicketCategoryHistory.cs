using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketCategoryHistory
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int FromTicketCategoryId { get; set; }
        public TicketCategory FromTicketCategory { get; set; }

        public int ToTicketCategoryId { get; set; }
        public TicketCategory ToTicketCategory { get; set; }
    }

    public static class TicketCategoryHistoryExtension
    {
        public static string TableName => "TicketCategoryHistories";

        public static void DescribeTicketCategoryHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketCategoryHistory>(s =>
            {
                s.Property(p => p.UserId).IsRequired(false);
            });

            modelBuilder.Entity<TicketCategoryHistory>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketCategoryHistories)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCategoryHistory>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketCategoryHistories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCategoryHistory>()
                .HasOne(c => c.FromTicketCategory)
                .WithMany(p => p.FromTicketCategoryHistories)
                .HasForeignKey(c => c.FromTicketCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCategoryHistory>()
                .HasOne(c => c.ToTicketCategory)
                .WithMany(p => p.ToTicketCategoryHistories)
                .HasForeignKey(c => c.ToTicketCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCategoryHistory>().ToTable(TableName);
        }
    }
}
