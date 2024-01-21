using mvp.tickets.domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IBaseRequest
    {
        int CompanyId { get; set; }
    }
    public record BaseRequest : IBaseRequest
    {
        public int CompanyId { get; set; }
    }

    public interface IBaseCommandRequest : IBaseRequest
    {
    }
    public record BaseCommandRequest : BaseRequest, IBaseCommandRequest
    {
    }

    public interface IBaseQueryRequest : IBaseRequest { }
    public record BaseQueryRequest : BaseRequest, IBaseQueryRequest { }

    public interface IBaseReportQueryRequest : IBaseQueryRequest
    {
        bool IsUserView { get; set; }
        Dictionary<string, string> SearchBy { get; set; }
        string SortBy { get; set; }
        SortDirection SortDirection { get; set; }
        int Offset { get; set; }
    }
    public record BaseReportQueryRequest : BaseQueryRequest, IBaseReportQueryRequest
    {
        public bool IsUserView { get; set; }
        public Dictionary<string, string> SearchBy { get; set; } = new Dictionary<string, string>();
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
        [Range(0, Int32.MaxValue)]
        public int Offset { get; set; }
    }
}
