using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IPriorityCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        int Level { get; set; }
        bool IsActive { get; set; }
    }
    public record PriorityCreateCommandRequest : BaseCommandRequest, IPriorityCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
    }

    public interface IPriorityUpdateCommandRequest : IPriorityCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record PriorityUpdateCommandRequest : PriorityCreateCommandRequest, IPriorityUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
