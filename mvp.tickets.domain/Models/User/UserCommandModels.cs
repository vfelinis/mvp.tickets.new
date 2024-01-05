using mvp.tickets.domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvp.tickets.domain.Models
{
    public interface IUserLoginCommandRequest: IBaseCommandRequest
    {
        string IdToken { get; set; }
    }
    public record UserLoginCommandRequest: BaseCommandRequest, IUserLoginCommandRequest
    {
        [Required]
        public string IdToken { get; set; }
    }

    public interface IUserCreateCommandRequest : IBaseCommandRequest
    {
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        Permissions Permissions { get; set; }
        bool IsLocked { get; set; }
        string Password { get; set; }
    }
    public record UserCreateCommandRequest : BaseCommandRequest, IUserCreateCommandRequest
    {
        [EmailAddress]
        [Required]
        [StringLength(maximumLength: 250)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string LastName { get; set; }
        public Permissions Permissions { get; set; }
        public bool IsLocked { get; set; }
        public string Password { get; set; }
    }

    public interface IUserUpdateCommandRequest: IUserCreateCommandRequest
    {
        int Id { get; set; }
    }

    public record UserUpdateCommandRequest: UserCreateCommandRequest, IUserUpdateCommandRequest
    {
        public int Id { get; set; }
    }
}
