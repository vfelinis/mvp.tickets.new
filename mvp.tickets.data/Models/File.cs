using Microsoft.EntityFrameworkCore;

namespace mvp.tickets.data.Models
{
    public class File
    {
        public int Id { get; set; }
        public byte[] Content { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }

    public static class FileExtension
    {
        public static string TableName => "Files";

        public static void DescribeFile(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>().ToTable(TableName);
        }
    }
}
