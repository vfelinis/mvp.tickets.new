using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IInviteCreateCommandRequest : IBaseCommandRequest
    {
        string Email { get; set; }
        string Company { get; set; }
    }
    public record InviteCreateCommandRequest : BaseCommandRequest, IInviteCreateCommandRequest
    {
        [Required]
        [StringLength(250)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Company { get; set; }
    }

    public interface IInviteValidateCommandRequest : IBaseCommandRequest
    {
        string Email { get; set; }
        string Code { get; set; }
    }
    public record InviteValidateCommandRequest : BaseCommandRequest, IInviteValidateCommandRequest
    {
        [Required]
        [StringLength(250)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; }
    }
}
