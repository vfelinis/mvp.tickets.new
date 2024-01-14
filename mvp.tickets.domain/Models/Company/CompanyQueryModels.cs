using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface ICompanyQueryRequest : IBaseQueryRequest
    {
        string Host { get; set; }
    }

    public record CompanyQueryRequest : BaseQueryRequest, ICompanyQueryRequest
    {
        public string Host { get; set; }
    }

    public interface ICompanyModel
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsRoot { get; set; }
        string Host { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
    }

    public record CompanyModel : ICompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public string Host { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
