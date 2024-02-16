using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using mvp.tickets.data.Models;

namespace mvp.tickets.data
{
    public class ApplicationDbContext : DbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<TicketCommentAttachment> TicketCommentAttachments { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketPriorityHistory> TicketPriorityHistories { get; set; }
        public DbSet<TicketQueue> TicketQueues { get; set; }
        public DbSet<TicketQueueHistory> TicketQueueHistories { get; set; }
        public DbSet<TicketResolution> TicketResolutions { get; set; }
        public DbSet<TicketResponseTemplate> TicketResponseTemplates { get; set; }
        public DbSet<TicketResponseTemplateType> TicketResponseTemplateTypes { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }
        public DbSet<TicketStatusRule> TicketStatusRules { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketCategoryHistory> TicketCategoryHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);

            modelBuilder.DescribeTicket();
            modelBuilder.DescribeTicketHistory();
            modelBuilder.DescribeTicketCommentAttachment();
            modelBuilder.DescribeTicketComment();
            modelBuilder.DescribeTicketPriority();
            modelBuilder.DescribeTicketPriorityHistory();
            modelBuilder.DescribeTicketQueue();
            modelBuilder.DescribeTicketQueueHistory();
            modelBuilder.DescribeTicketResolution();
            modelBuilder.DescribeTicketResponseTemplate();
            modelBuilder.DescribeTicketResponseTemplateType();
            modelBuilder.DescribeTicketStatus();
            modelBuilder.DescribeTicketStatusHistory();
            modelBuilder.DescribeTicketStatusRule();
            modelBuilder.DescribeTicketCategory();
            modelBuilder.DescribeTicketCategoryHistory();
            modelBuilder.DescribeUser();
            modelBuilder.DescribeInvite();
            modelBuilder.DescribeCompany();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetConverter>();
        }
    }

    public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public DateTimeOffsetConverter()
            : base(
                d => d.ToUniversalTime(),
                d => d.ToUniversalTime())
        {
        }
    }
}
