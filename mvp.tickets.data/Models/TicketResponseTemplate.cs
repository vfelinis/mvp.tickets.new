using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketResponseTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int TicketResponseTemplateTypeId { get; set; }
        public TicketResponseTemplateType TicketResponseTemplateType { get; set; }
    }

    public static class TicketResponseTemplateExtension
    {
        public static string TableName => "TicketResponseTemplates";

        public static void DescribeTicketResponseTemplate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketResponseTemplate>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.Text).IsRequired(true).HasMaxLength(2000);
            });

            modelBuilder.Entity<TicketResponseTemplate>()
                .HasOne(c => c.TicketResponseTemplateType)
                .WithMany(p => p.TicketResponseTemplates)
                .HasForeignKey(c => c.TicketResponseTemplateTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketResponseTemplate>()
                .HasIndex(p => new { p.TicketResponseTemplateTypeId, p.Name })
                .IsUnique(true);

            modelBuilder.Entity<TicketResponseTemplate>().ToTable(TableName);
        }
    }
}
