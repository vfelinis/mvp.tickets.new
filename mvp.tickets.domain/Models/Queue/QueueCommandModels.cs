using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IQueueCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        bool IsDefault { get; set; }
        bool IsActive { get; set; }
    }
    public record QueueCreateCommandRequest : BaseCommandRequest, IQueueCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public interface IQueueUpdateCommandRequest : IQueueCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record QueueUpdateCommandRequest : QueueCreateCommandRequest, IQueueUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
