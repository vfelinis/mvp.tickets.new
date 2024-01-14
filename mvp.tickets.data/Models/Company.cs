using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        //public string ContactName { get; set; }
        //public string ContactEmail { get; set; }
        //public string ContactPhone { get; set; }
        public string Host { get; set; }
        //public bool IsDefaultHost { get; set; }
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
                //s.Property(p => p.ContactName).IsRequired(false).HasMaxLength(100);
                //s.Property(p => p.ContactEmail).IsRequired(false).HasMaxLength(250);
                //s.Property(p => p.ContactPhone).IsRequired(false).HasMaxLength(20);
                s.Property(p => p.Host).IsRequired(true).HasMaxLength(250);
            });

            modelBuilder.Entity<Company>()
                .HasIndex(p => p.IsRoot)
                .IsUnique(true)
                .HasFilter($"\"{nameof(Company.IsRoot)}\" = true");

            modelBuilder.Entity<Company>()
                .HasIndex(p => p.Name)
                .IsUnique(true);

            modelBuilder.Entity<Company>()
                .HasIndex(p => p.Host)
                .IsUnique(true);

            modelBuilder.Entity<Company>().ToTable(TableName);
        }
    }
}
