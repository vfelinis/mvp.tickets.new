namespace mvp.tickets.domain.Models
{
    public interface IResponseTemplateTypeQueryRequest : IBaseQueryRequest
    {
        int? Id { get; set; }
        bool OnlyActive { get; set; }
    }

    public record ResponseTemplateTypeQueryRequest : BaseQueryRequest, IResponseTemplateTypeQueryRequest
    {
        public int? Id { get; set; }
        public bool OnlyActive { get; set; }
    }
    public interface IResponseTemplateTypeModel
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
    }

    public record ResponseTemplateTypeModel : IResponseTemplateTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
    }
}
