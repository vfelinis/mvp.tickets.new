﻿using Microsoft.EntityFrameworkCore;
using mvp.tickets.domain.Enums;

namespace mvp.tickets.data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Permissions Permissions { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public List<Ticket> TicketReporters { get; set; } = new List<Ticket>();
        public List<Ticket> TicketAssignees { get; set; } = new List<Ticket>();
        public List<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
        public List<TicketObservation> TicketObservations { get; set; } = new List<TicketObservation>();
        public List<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
        public List<TicketStatusHistory> TicketStatusHistories { get; set; } = new List<TicketStatusHistory>();
        public List<TicketQueueHistory> TicketQueueHistories { get; set; } = new List<TicketQueueHistory>();
        public List<TicketPriorityHistory> TicketPriorityHistories { get; set; } = new List<TicketPriorityHistory>();
        public List<TicketCategoryHistory> TicketCategoryHistories { get; set; } = new List<TicketCategoryHistory>();
    }

    public static class UserExtension
    {
        public static string TableName => "Users";

        public static void DescribeUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(s =>
            {
                s.Property(p => p.Email).IsRequired(false).HasMaxLength(250);
                s.Property(p => p.Phone).IsRequired(false).HasMaxLength(20);
                s.Property(p => p.FirstName).IsRequired(true).HasMaxLength(50);
                s.Property(p => p.LastName).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.Password).IsRequired(true).HasMaxLength(500);
            });

            modelBuilder.Entity<User>()
                .HasOne(c => c.Company)
                .WithMany(p => p.Users)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.CompanyId, p.Email })
                .IsUnique(true)
                .HasFilter($"\"{nameof(User.Email)}\" IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.CompanyId, p.Phone })
                .IsUnique(true)
                .HasFilter($"\"{nameof(User.Phone)}\" IS NOT NULL");

            modelBuilder.Entity<User>().ToTable(TableName);
        }
    }
}
