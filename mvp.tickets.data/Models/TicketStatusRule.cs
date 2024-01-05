using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketStatusRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int? FromTicketStatusId { get; set; }
        public TicketStatus FromTicketStatus { get; set; }

        public int? ToTicketStatusId { get; set; }
        public TicketStatus ToTicketStatus { get; set; }
    }

    public static class TicketStatusRuleExtension
    {
        public static string TableName => "TicketStatusRules";

        public static void DescribeTicketStatusRule(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketStatusRule>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.FromTicketStatusId).IsRequired(false);
                s.Property(p => p.ToTicketStatusId).IsRequired(false);
            });

            modelBuilder.Entity<TicketStatusRule>()
                .HasOne(c => c.ToTicketStatus)
                .WithMany(p => p.TicketToStatusRules)
                .HasForeignKey(c => c.ToTicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusRule>()
                .HasOne(c => c.FromTicketStatus)
                .WithMany(p => p.TicketFromStatusRules)
                .HasForeignKey(c => c.FromTicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatusRule>()
                .HasIndex(p => p.Name)
                .IsUnique(true);

            modelBuilder.Entity<TicketStatusRule>()
                .HasIndex(p => new { p.FromTicketStatusId, p.ToTicketStatusId })
                .IsUnique(true);

            modelBuilder.Entity<TicketStatusRule>().ToTable(TableName);
        }
    }
}
