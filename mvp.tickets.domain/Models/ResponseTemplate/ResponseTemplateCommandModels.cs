using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IResponseTemplateCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        string Text { get; set; }
        bool IsActive { get; set; }
        int TicketResponseTemplateTypeId { get; set; }
    }
    public record ResponseTemplateCreateCommandRequest : BaseCommandRequest, IResponseTemplateCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2000)]
        public string Text { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int TicketResponseTemplateTypeId { get; set; }
    }

    public interface IResponseTemplateUpdateCommandRequest : IResponseTemplateCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record ResponseTemplateUpdateCommandRequest : ResponseTemplateCreateCommandRequest, IResponseTemplateUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
