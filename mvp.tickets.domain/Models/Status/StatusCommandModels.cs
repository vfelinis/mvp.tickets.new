using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IStatusCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        bool IsDefault { get; set; }
        bool IsCompletion { get; set; }
        bool IsActive { get; set; }
    }
    public record StatusCreateCommandRequest : BaseCommandRequest, IStatusCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCompletion { get; set; }
        public bool IsActive { get; set; }
    }

    public interface IStatusUpdateCommandRequest : IStatusCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record StatusUpdateCommandRequest : StatusCreateCommandRequest, IStatusUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
