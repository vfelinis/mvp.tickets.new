using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface ICategoryCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        bool IsDefault { get; set; }
        bool IsActive { get; set; }
        int? ParentCategoryId { get; set; }
    }
    public record CategoryCreateCommandRequest : BaseCommandRequest, ICategoryCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int? ParentCategoryId { get; set; }
    }

    public interface ICategoryUpdateCommandRequest : ICategoryCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record CategoryUpdateCommandRequest : CategoryCreateCommandRequest, ICategoryUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
