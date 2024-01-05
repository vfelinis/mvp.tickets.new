namespace mvp.tickets.domain.Models
{
    public interface IResponseTemplateQueryRequest : IBaseQueryRequest
    {
        int? Id { get; set; }
        bool OnlyActive { get; set; }
    }

    public record ResponseTemplateQueryRequest : BaseQueryRequest, IResponseTemplateQueryRequest
    {
        public int? Id { get; set; }
        public bool OnlyActive { get; set; }
    }
    public interface IResponseTemplateModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        int TicketResponseTemplateTypeId { get; set; }
        string TicketResponseTemplateType { get; set; }
    }

    public record ResponseTemplateModel : IResponseTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public int TicketResponseTemplateTypeId { get; set; }
        public string TicketResponseTemplateType { get; set; }
    }
}
