using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketCommentAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string OriginalFileName { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int TicketCommentId { get; set; }
        public TicketComment TicketComment { get; set; }
    }

    public static class TicketCommentAttachmentExtension
    {
        public static string TableName => "TicketCommentAttachments";

        public static void DescribeTicketCommentAttachment(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketCommentAttachment>(s =>
            {
                s.Property(p => p.FileName).IsRequired(true).HasMaxLength(50);
                s.Property(p => p.Extension).IsRequired(true).HasMaxLength(5);
                s.Property(p => p.OriginalFileName).IsRequired(true);
            });

            modelBuilder.Entity<TicketCommentAttachment>()
                .HasOne(c => c.TicketComment)
                .WithMany(p => p.TicketCommentAttachments)
                .HasForeignKey(c => c.TicketCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCommentAttachment>()
                .HasIndex(p => p.FileName)
                .IsUnique(true);

            modelBuilder.Entity<TicketCommentAttachment>().ToTable(TableName);
        }
    }
}
