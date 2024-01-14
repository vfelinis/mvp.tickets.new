﻿using mvp.tickets.domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace mvp.tickets.domain.Models
{
    public interface IBaseRequest
    {
        int CompantId { get; set; }
    }
    public record BaseRequest : IBaseRequest
    {
        public int CompantId { get; set; }
    }

    public interface IBaseCommandRequest : IBaseRequest
    {
        int CompanyId { get; set; }
    }
    public record BaseCommandRequest : BaseRequest, IBaseCommandRequest
    {
        public int CompanyId { get; set; }
    }

    public interface IBaseQueryRequest : IBaseRequest { }
    public record BaseQueryRequest : BaseRequest, IBaseQueryRequest { }

    public interface IBaseReportQueryRequest : IBaseQueryRequest
    {
        Dictionary<string, string> SearchBy { get; set; }
        string SortBy { get; set; }
        SortDirection SortDirection { get; set; }
        int Offset { get; set; }
    }
    public record BaseReportQueryRequest : BaseQueryRequest, IBaseReportQueryRequest
    {
        public Dictionary<string, string> SearchBy { get; set; } = new Dictionary<string, string>();
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
        [Range(0, Int32.MaxValue)]
        public int Offset { get; set; }
    }
}
