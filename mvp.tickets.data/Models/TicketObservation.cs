using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketObservation
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public static class TicketObservationExtension
    {
        public static string TableName => "TicketObservations";

        public static void DescribeTicketTicketObservation(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketObservation>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketObservations)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketObservation>()
                .HasOne(c => c.User)
                .WithMany(p => p.TicketObservations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketObservation>().ToTable(TableName);
        }
    }
}
