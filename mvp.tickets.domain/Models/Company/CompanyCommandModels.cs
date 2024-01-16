using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface ICompanyCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        string Host { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Code { get; set; }
    }
    public record CompanyCreateCommandRequest : BaseCommandRequest, ICompanyCreateCommandRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Host { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        [Required]
        [StringLength(250)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; }
    }

    public interface ICompanySetActiveCommandRequest : IBaseCommandRequest
    {
        int Id { get; set; }
        bool IsActive { get; set; }
    }

    public record CompanySetActiveCommandRequest : BaseCommandRequest, ICompanySetActiveCommandRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
