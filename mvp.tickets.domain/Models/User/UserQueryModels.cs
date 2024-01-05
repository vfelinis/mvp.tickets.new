using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface IUserQueryRequest: IBaseQueryRequest
    {
        int? Id { get; set; }
        string Email { get; set; }
    }

    public record UserQueryRequest: BaseQueryRequest, IUserQueryRequest
    {
        public int? Id { get; set; }
        public string Email { get; set; }
    }

    public interface IUserModel
    {
        int Id { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        Permissions Permissions { get; set; }
        bool IsLocked { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
    }

    public record UserModel: IUserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Permissions Permissions { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
