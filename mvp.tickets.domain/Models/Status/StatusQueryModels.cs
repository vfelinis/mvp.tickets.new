namespace mvp.tickets.domain.Models
{
    public interface IStatusQueryRequest : IBaseQueryRequest
    {
        int? Id { get; set; }
        bool OnlyActive { get; set; }
    }

    public record StatusQueryRequest : BaseQueryRequest, IStatusQueryRequest
    {
        public int? Id { get; set; }
        public bool OnlyActive { get; set; }
    }
    public interface IStatusModel
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsDefault { get; set; }
        bool IsCompletion { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
    }

    public record StatusModel : IStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCompletion { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
