using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IResponseTemplateTypeCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        bool IsActive { get; set; }
    }
    public record ResponseTemplateTypeCreateCommandRequest : BaseCommandRequest, IResponseTemplateTypeCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public interface IResponseTemplateTypeUpdateCommandRequest : IResponseTemplateTypeCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record ResponseTemplateTypeUpdateCommandRequest : ResponseTemplateTypeCreateCommandRequest, IResponseTemplateTypeUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
