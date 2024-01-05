namespace mvp.tickets.domain.Models
{
    public interface IPriorityQueryRequest : IBaseQueryRequest
    {
        int? Id { get; set; }
        bool OnlyActive { get; set; }
    }

    public record PriorityQueryRequest : BaseQueryRequest, IPriorityQueryRequest
    {
        public int? Id { get; set; }
        public bool OnlyActive { get; set; }
    }
    public interface IPriorityModel
    {
        int Id { get; set; }
        string Name { get; set; }
        int Level { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
    }

    public record PriorityModel : IPriorityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
