﻿using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCompletion { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<TicketStatusRule> TicketFromStatusRules { get; set; } = new List<TicketStatusRule>();
        public List<TicketStatusRule> TicketToStatusRules { get; set; } = new List<TicketStatusRule>();
        public List<TicketStatusHistory> FromTicketStatusHistories { get; set; } = new List<TicketStatusHistory>();
        public List<TicketStatusHistory> ToTicketStatusHistories { get; set; } = new List<TicketStatusHistory>();
    }

    public static class TicketStatusExtension
    {
        public static string TableName => "TicketStatuses";

        public static void DescribeTicketStatus(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketStatus>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<TicketStatus>()
                .HasOne(c => c.Company)
                .WithMany(p => p.TicketStatuses)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketStatus>()
                .HasIndex(p => new { p.CompanyId, p.Name })
                .IsUnique(true);

            modelBuilder.Entity<TicketStatus>()
                .HasIndex(p => new { p.CompanyId, p.IsDefault })
                .IsUnique(true)
                .HasFilter($"\"{nameof(TicketStatus.IsDefault)}\" = true AND \"{nameof(TicketStatus.IsActive)}\" = true");

            modelBuilder.Entity<TicketStatus>().ToTable(TableName);
        }
    }
}
