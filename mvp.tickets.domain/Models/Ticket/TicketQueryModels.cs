using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface ITicketTelegramQueryRequest : IBaseQueryRequest
    {
        string ApiKey { get; set; }
        string Phone { get; set; }
    }

    public record TicketTelegramQueryRequest : BaseQueryRequest, ITicketTelegramQueryRequest
    {
        public string ApiKey { get; set; }
        public string Phone { get; set; }
    }

    public interface ITicketQueryRequest : IBaseQueryRequest
    {
        bool IsUserView { get; set; }
        string Token { get; set; }
    }

    public record TicketQueryRequest : BaseQueryRequest, ITicketQueryRequest
    {
        public bool IsUserView { get; set; }
        public string Token { get; set; }
    }

    public interface ITicketModel
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsClosed { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        string Source { get; set; }

        int ReporterId { get; set; }
        string ReporterEmail { get; set; }
        string ReporterFirstName { get; set; }
        string ReporterLastName { get; set; }

        int? AssigneeId { get; set; }
        string AssigneeEmail { get; set; }
        string AssigneeFirstName { get; set; }
        string AssigneeLastName { get; set; }

        int? TicketPriorityId { get; set; }
        string TicketPriority { get; set; }

        int TicketQueueId { get; set; }
        string TicketQueue { get; set; }

        int? TicketResolutionId { get; set; }
        string TicketResolution { get; set; }

        int TicketStatusId { get; set; }
        string TicketStatus { get; set; }

        int TicketCategoryId { get; set; }
        string TicketCategory { get; set; }
        IEnumerable<ITicketCommentModel> TicketComments { get; set; }
    }

    public record TicketModel : ITicketModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Source { get; set; }
        public bool IsClosed { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }

        public int ReporterId { get; set; }
        public string ReporterEmail { get; set; }
        public string ReporterFirstName { get; set; }
        public string ReporterLastName { get; set; }

        public int? AssigneeId { get; set; }
        public string AssigneeEmail { get; set; }
        public string AssigneeFirstName { get; set; }
        public string AssigneeLastName { get; set; }

        public int? TicketPriorityId { get; set; }
        public string TicketPriority { get; set; }

        public int TicketQueueId { get; set; }
        public string TicketQueue { get; set; }

        public int? TicketResolutionId { get; set; }
        public string TicketResolution { get; set; }

        public int TicketStatusId { get; set; }
        public string TicketStatus { get; set; }

        public int TicketCategoryId { get; set; }
        public string TicketCategory { get; set; }
        public IEnumerable<ITicketCommentModel> TicketComments { get; set; } = new List<TicketCommentModel>();
    }

    public interface ITicketCommentModel
    {
        int Id { get; set; }
        string Text { get; set; }
        bool IsInternal { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        int CreatorId { get; set; }
        string CreatorEmail { get; set; }
        string CreatorFirstName { get; set; }
        string CreatorLastName { get; set; }
        IEnumerable<ITicketCommentAttachmentModel> TicketCommentAttachmentModels { get; set; }
    }

    public record TicketCommentModel: ITicketCommentModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsInternal { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public int CreatorId { get; set; }
        public string CreatorEmail { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public IEnumerable<ITicketCommentAttachmentModel> TicketCommentAttachmentModels { get; set; } = new List<TicketCommentAttachmentModel>();
    }

    public interface ITicketCommentAttachmentModel
    {
        int Id { get; set; }
        string Path { get; set; }
        string OriginalFileName { get; set; }
        DateTimeOffset DateCreated { get; set; }
    }

    public record TicketCommentAttachmentModel: ITicketCommentAttachmentModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string OriginalFileName { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
