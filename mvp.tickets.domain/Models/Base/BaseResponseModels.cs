using mvp.tickets.domain.Enums;

namespace mvp.tickets.domain.Models
{
    public interface IBaseResponse
    {
        bool IsSuccess { get; set; }
        ResponseCodes Code { get; set; }
        string ErrorMessage { get; set; }
    }
    public record BaseResponse : IBaseResponse
    {
        public bool IsSuccess { get; set; }
        public ResponseCodes Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IBaseCommandResponse<T> : IBaseResponse
    {
        T Data { get; set; }
    }
    public record BaseCommandResponse<T> : BaseResponse, IBaseCommandResponse<T>
    {
        public T Data { get; set; }
    }

    public interface IBaseQueryResponse<T> : IBaseResponse
    {
        T Data { get; set; }
    }
    public record BaseQueryResponse<T> : BaseResponse, IBaseQueryResponse<T>
    {
        public T Data { get; set; }
    }

    public interface IBaseReportQueryResponse<T> : IBaseQueryResponse<T>
    {
        int Total { get; set; }
    }
    public record BaseReportQueryResponse<T> : BaseQueryResponse<T>, IBaseReportQueryResponse<T>
    {
        public int Total { get; set; }
    }
}
