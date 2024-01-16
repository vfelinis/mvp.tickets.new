using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface ICompanyModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Host { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
    }

    public record CompanyModel : ICompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
