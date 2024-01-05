using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsInternal { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public List<TicketCommentAttachment> TicketCommentAttachments { get; set; } = new List<TicketCommentAttachment>();
    }

    public static class TicketCommentExtension
    {
        public static string TableName => "TicketComments";

        public static void DescribeTicketComment(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketComment>(s =>
            {
                s.Property(p => p.Text).IsRequired(false).HasMaxLength(2000);
            });

            modelBuilder.Entity<TicketComment>()
                .HasOne(c => c.Ticket)
                .WithMany(p => p.TicketComments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketComment>()
                .HasOne(c => c.Creator)
                .WithMany(p => p.TicketComments)
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketComment>().ToTable(TableName);
        }
    }
}
