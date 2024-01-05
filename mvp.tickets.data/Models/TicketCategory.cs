using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int? ParentCategoryId { get; set; }
        public TicketCategory ParentCategory { get; set; }

        public List<TicketCategory> SubCategories { get; set; } = new List<TicketCategory>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<TicketCategoryHistory> FromTicketCategoryHistories { get; set; } = new List<TicketCategoryHistory>();
        public List<TicketCategoryHistory> ToTicketCategoryHistories { get; set; } = new List<TicketCategoryHistory>();
    }

    public static class TicketCategoryExtension
    {
        public static string TableName => "TicketCategories";

        public static void DescribeTicketCategory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketCategory>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.ParentCategoryId).IsRequired(false);
            });

            modelBuilder.Entity<TicketCategory>()
                .HasOne(c => c.ParentCategory)
                .WithMany(p => p.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketCategory>()
                .HasIndex(p => p.Name)
                .IsUnique(true);

            modelBuilder.Entity<TicketCategory>()
                .HasIndex(p => p.IsDefault)
                .IsUnique(true)
                .HasFilter($"[{nameof(TicketCategory.IsDefault)}] = 1 AND [{nameof(TicketCategory.IsActive)}] = 1");

            modelBuilder.Entity<TicketCategory>().ToTable(TableName);
        }
    }
}
