using mvp.tickets.domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IUserRegisterRequestCommandRequest : IBaseCommandRequest
    {
        string Email { get; set; }
    }
    public record UserRegisterRequestCommandRequest : BaseCommandRequest, IUserRegisterRequestCommandRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(maximumLength: 250)]
        public string Email { get; set; }
    }

    public interface IUserRegisterCommandRequest : IBaseCommandRequest
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Password { get; set; }
        string Code { get; set; }
    }
    public record UserRegisterCommandRequest : BaseCommandRequest, IUserRegisterCommandRequest
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string Password { get; set; }
        [Required]
        public string Code { get; set; }
    }

    public interface IUserLoginCommandRequest: IBaseCommandRequest
    {
        string Email { get; set; }
        string Password { get; set; }
        string Host { get; set; }
    }
    public record UserLoginCommandRequest: BaseCommandRequest, IUserLoginCommandRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Host { get; set; }
    }

    public interface IUserLoginByCodeCommandRequest : IBaseCommandRequest
    {
        string Code { get; set; }
    }
    public record UserLoginByCodeCommandRequest : BaseCommandRequest, IUserLoginByCodeCommandRequest
    {
        [Required]
        public string Code { get; set; }
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
        [StringLength(maximumLength: 50)]
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

    public interface IUserFortogPasswordCommandRequest : IBaseCommandRequest
    {
        string Email { get; set; }
    }
    public record UserFortogPasswordCommandRequest : BaseCommandRequest, IUserFortogPasswordCommandRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(maximumLength: 250)]
        public string Email { get; set; }
    }

    public interface IUserResetPasswordCommandRequest : IBaseCommandRequest
    {
        string Password { get; set; }
        string Code { get; set; }
    }
    public record UserResetPasswordCommandRequest : BaseCommandRequest, IUserResetPasswordCommandRequest
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string Password { get; set; }
        [Required]
        public string Code { get; set; }
    }

    public enum JWTType
    {
        Register = 0,
        ResetPassword = 1,
        Support = 2,
    }
    public record UserJWTData(string Email, int CompanyId, JWTType Type);
}
