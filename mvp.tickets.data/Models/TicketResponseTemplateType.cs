﻿using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class TicketResponseTemplateType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public List<TicketResponseTemplate> TicketResponseTemplates { get; set; } = new List<TicketResponseTemplate>();
    }

    public static class TicketResponseTemplateTypeExtension
    {
        public static string TableName => "TicketResponseTemplateTypes";

        public static void DescribeTicketResponseTemplateType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketResponseTemplateType>(s =>
            {
                s.Property(p => p.Name).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<TicketResponseTemplateType>()
                .HasOne(c => c.Company)
                .WithMany(p => p.TicketResponseTemplateTypes)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketResponseTemplateType>()
                .HasIndex(p => new { p.CompanyId, p.Name })
                .IsUnique(true);

            modelBuilder.Entity<TicketResponseTemplateType>().ToTable(TableName);
        }
    }
}
