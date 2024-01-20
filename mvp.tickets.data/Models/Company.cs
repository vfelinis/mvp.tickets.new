using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public string Host { get; set; }
        public string Logo { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public List<User> Users { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<TicketCategory> TicketCategories { get; set; } = new();
        public List<TicketPriority> TicketPriorities { get; set; } = new();
        public List<TicketQueue> TicketQueues { get; set; } = new();
        public List<TicketResolution> TicketResolutions { get; set; } = new();
        public List<TicketResponseTemplateType> TicketResponseTemplateTypes { get; set; } = new();
        public List<TicketStatus> TicketStatuses { get; set; } = new();
    }

    public static class CompanyExtension
    {
        public static string TableName => "Companies";

        public static void DescribeCompany(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.Host).IsRequired(true).HasMaxLength(250);
                s.Property(p => p.Logo).IsRequired(false).HasMaxLength(50);
                s.Property(p => p.Color).IsRequired(true).HasMaxLength(50).HasDefaultValue("#1976d2");
            });

            modelBuilder.Entity<Company>()
                .HasIndex(p => p.IsRoot)
                .IsUnique(true)
                .HasFilter($"\"{nameof(Company.IsRoot)}\" = true");

            modelBuilder.Entity<Company>()
                .HasIndex(p => p.Host)
                .IsUnique(true);

            modelBuilder.Entity<Company>().ToTable(TableName);
        }
    }
}
