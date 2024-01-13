using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Code { get; set; }
        public DateTimeOffset DateSent { get; set; }
    }

    public static class InviteExtension
    {
        public static string TableName => "Invites";

        public static void DescribeInvite(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invite>(s =>
            {
                s.Property(p => p.Email).IsRequired(true).HasMaxLength(250);
                s.Property(p => p.Company).IsRequired(true).HasMaxLength(100);
                s.Property(p => p.Code).IsRequired(true).HasMaxLength(100);
            });

            modelBuilder.Entity<Invite>()
                .HasIndex(p => new { p.Email, p.Company })
                .IsUnique(true);

            modelBuilder.Entity<Invite>().ToTable(TableName);
        }
    }
}
