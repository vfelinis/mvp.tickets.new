using Microsoft.EntityFrameworkCore;
using mvp.tickets.domain.Enums;

namespace mvp.tickets.data.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public TicketSource Source { get; set; }
        public bool IsClosed { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int ReporterId { get; set; }
        public User Reporter { get; set; }

        public int? AssigneeId { get; set; }
        public User Assignee { get; set; }

        public int? TicketPriorityId { get; set; }
        public TicketPriority TicketPriority { get; set; }

        public int TicketQueueId { get; set; }
        public TicketQueue TicketQueue { get; set; }

        public int? TicketResolutionId { get; set; }
        public TicketResolution TicketResolution { get; set; }

        public int TicketStatusId { get; set; }
        public TicketStatus TicketStatus { get; set; }

        public int TicketCategoryId { get; set; }
        public TicketCategory TicketCategory { get; set; }

        public List<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
        public List<TicketObservation> TicketObservations { get; set; } = new List<TicketObservation>();
        public List<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
        public List<TicketStatusHistory> TicketStatusHistories { get; set; } = new List<TicketStatusHistory>();
        public List<TicketQueueHistory> TicketQueueHistories { get; set; } = new List<TicketQueueHistory>();
        public List<TicketPriorityHistory> TicketPriorityHistories { get; set; } = new List<TicketPriorityHistory>();
        public List<TicketCategoryHistory> TicketCategoryHistories { get; set; } = new List<TicketCategoryHistory>();
    }

    public static class TicketExtension
    {
        public static string TableName => "Tickets";

        public static void DescribeTicket(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.Token).IsRequired(false).HasMaxLength(250);
                s.Property(p => p.AssigneeId).IsRequired(false);
                s.Property(p => p.TicketPriorityId).IsRequired(false);
                s.Property(p => p.TicketResolutionId).IsRequired(false);
            });

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.Reporter)
                .WithMany(p => p.TicketReporters)
                .HasForeignKey(c => c.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.TicketPriority)
                .WithMany(p => p.Tickets)
                .HasForeignKey(c => c.TicketPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.Assignee)
                .WithMany(p => p.TicketAssignees)
                .HasForeignKey(c => c.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.TicketQueue)
                .WithMany(p => p.Tickets)
                .HasForeignKey(c => c.TicketQueueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.TicketResolution)
                .WithMany(p => p.Tickets)
                .HasForeignKey(c => c.TicketResolutionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.TicketStatus)
                .WithMany(p => p.Tickets)
                .HasForeignKey(c => c.TicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(c => c.TicketCategory)
                .WithMany(p => p.Tickets)
                .HasForeignKey(c => c.TicketCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>().ToTable(TableName);
        }
    }
}
