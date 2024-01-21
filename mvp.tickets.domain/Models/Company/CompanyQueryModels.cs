using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface ICompanyModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Host { get; set; }
        bool IsActive { get; set; }
        bool IsRoot { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        string Logo { get; set; }
        string Color { get; set; }
    }

    public record CompanyModel : ICompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public bool IsActive { get; set; }
        public bool IsRoot { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public string Logo { get; set; }
        public string Color { get; set; }
    }
}
