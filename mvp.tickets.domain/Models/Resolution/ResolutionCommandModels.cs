using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IResolutionCreateCommandRequest : IBaseCommandRequest
    {
        string Name { get; set; }
        bool IsActive { get; set; }
    }
    public record ResolutionCreateCommandRequest : BaseCommandRequest, IResolutionCreateCommandRequest
    {
        [Required]
        [StringLength(maximumLength:100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public interface IResolutionUpdateCommandRequest : IResolutionCreateCommandRequest
    {
        int Id { get; set; }
    }
    public record ResolutionUpdateCommandRequest : ResolutionCreateCommandRequest, IResolutionUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
