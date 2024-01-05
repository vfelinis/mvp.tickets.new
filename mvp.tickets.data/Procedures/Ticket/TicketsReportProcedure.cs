using mvp.tickets.data.Models;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Models;


namespace mvp.tickets.data.Procedures
{
    [Procedure]
    public static class TicketsReportProcedure
    {
        public static string Name => "procTicketsReport";
        public static int Version => 5;
        public static class Params
        {
            public static string SearchById => "@searchById";
            public static string SearchByIsClosed => "@searchByIsClosed";
            public static string SearchByReporterId => "@searchByReporterId";
            public static string SearchByAssigneeId => "@searchByAssigneeId";
            public static string SearchByTicketPriorityId => "@searchByTicketPriorityId";
            public static string SearchByTicketQueueId => "@searchByTicketQueueId";
            public static string SearchByTicketResolutionId => "@searchByTicketResolutionId";
            public static string SearchByTicketStatusId => "@searchByTicketStatusId";
            public static string SearchByTicketCategoryId => "@searchByTicketCategoryId";
            public static string SearchBySource => "@searchBySource";
            public static string SortBy => "@sortBy";
            public static string SortDirection => "@sortDirection";
            public static string Offset => "@offset";
            public static string Limit => "@limit";
        }

        public static string Text => $@"
/* version={Version} */
CREATE PROCEDURE [{Name}]
    {Params.SearchById} INT = NULL,
    {Params.SearchByIsClosed} BIT = NULL,
    {Params.SearchByReporterId} INT = NULL,
    {Params.SearchByAssigneeId} INT = NULL,
    {Params.SearchByTicketPriorityId} INT = NULL,
    {Params.SearchByTicketQueueId} INT = NULL,
    {Params.SearchByTicketResolutionId} INT = NULL,
    {Params.SearchByTicketStatusId} INT = NULL,
    {Params.SearchByTicketCategoryId} INT = NULL,
    {Params.SearchBySource} INT = NULL,
    {Params.SortBy} NVARCHAR(MAX) = '{nameof(User.Id)}',
    {Params.SortDirection} NVARCHAR(4) = '{SortDirection.ASC.ToString()}',
    {Params.Offset} INT,
    {Params.Limit} INT
AS
BEGIN
    DECLARE @Sql NVARCHAR(MAX) =
    N'
        SET NOCOUNT ON;
        SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

        SELECT
            COUNT(*) OVER() AS [{nameof(TicketReportModel.Total)}]
            ,t.[{nameof(Ticket.Id)}] AS [{nameof(TicketReportModel.Id)}]
            ,t.[{nameof(Ticket.Name)}] AS [{nameof(TicketReportModel.Name)}]
            ,t.[{nameof(Ticket.Token)}] AS [{nameof(TicketReportModel.Token)}]
            ,CASE t.[{nameof(Ticket.Source)}]
                WHEN {(int)TicketSource.Email} THEN ''Email''
                WHEN {(int)TicketSource.Telegram} THEN ''Telegram''
                ELSE ''Web''
            END AS [{nameof(TicketReportModel.Source)}]
            ,t.[{nameof(Ticket.IsClosed)}] AS [{nameof(TicketReportModel.IsClosed)}]
            ,t.[{nameof(Ticket.DateCreated)}] AS [{nameof(TicketReportModel.DateCreated)}]
            ,t.[{nameof(Ticket.DateModified)}] AS [{nameof(TicketReportModel.DateModified)}]

            ,ru.[{nameof(User.Id)}] AS [{nameof(TicketReportModel.ReporterId)}]
            ,ru.[{nameof(User.Email)}] AS [{nameof(TicketReportModel.ReporterEmail)}]
            ,ru.[{nameof(User.FirstName)}] AS [{nameof(TicketReportModel.ReporterFirstName)}]
            ,ru.[{nameof(User.LastName)}] AS [{nameof(TicketReportModel.ReporterLastName)}]

            ,au.[{nameof(User.Id)}] AS [{nameof(TicketReportModel.AssigneeId)}]
            ,au.[{nameof(User.Email)}] AS [{nameof(TicketReportModel.AssigneeEmail)}]
            ,au.[{nameof(User.FirstName)}] AS [{nameof(TicketReportModel.AssigneeFirstName)}]
            ,au.[{nameof(User.LastName)}] AS [{nameof(TicketReportModel.AssigneeLastName)}]
            
            ,tp.[{nameof(TicketPriority.Id)}] AS [{nameof(TicketReportModel.TicketPriorityId)}]
            ,tp.[{nameof(TicketPriority.Name)}] AS [{nameof(TicketReportModel.TicketPriority)}]
            
            ,tq.[{nameof(TicketQueue.Id)}] AS [{nameof(TicketReportModel.TicketQueueId)}]
            ,tq.[{nameof(TicketQueue.Name)}] AS [{nameof(TicketReportModel.TicketQueue)}]

            ,tr.[{nameof(TicketResolution.Id)}] AS [{nameof(TicketReportModel.TicketResolutionId)}]
            ,tr.[{nameof(TicketResolution.Name)}] AS [{nameof(TicketReportModel.TicketResolution)}]

            ,ts.[{nameof(TicketStatus.Id)}] AS [{nameof(TicketReportModel.TicketStatusId)}]
            ,ts.[{nameof(TicketStatus.Name)}] AS [{nameof(TicketReportModel.TicketStatus)}]

            ,tc.[{nameof(TicketCategory.Id)}] AS [{nameof(TicketReportModel.TicketCategoryId)}]
            ,tc.[{nameof(TicketCategory.Name)}] AS [{nameof(TicketReportModel.TicketCategory)}]
        FROM [{TicketExtension.TableName}] t
        JOIN [{UserExtension.TableName}] ru ON t.[{nameof(Ticket.ReporterId)}] = ru.[{nameof(User.Id)}]
        JOIN [{TicketQueueExtension.TableName}] tq ON t.[{nameof(Ticket.TicketQueueId)}] = tq.[{nameof(TicketQueue.Id)}]
        JOIN [{TicketStatusExtension.TableName}] ts ON t.[{nameof(Ticket.TicketStatusId)}] = ts.[{nameof(TicketStatus.Id)}]
        JOIN [{TicketCategoryExtension.TableName}] tc ON t.[{nameof(Ticket.TicketCategoryId)}] = tc.[{nameof(TicketCategory.Id)}]
        LEFT JOIN [{UserExtension.TableName}] au ON t.[{nameof(Ticket.AssigneeId)}] = au.[{nameof(User.Id)}]
        LEFT JOIN [{TicketPriorityExtension.TableName}] tp ON t.[{nameof(Ticket.TicketPriorityId)}] = tp.[{nameof(TicketPriority.Id)}]
        LEFT JOIN [{TicketResolutionExtension.TableName}] tr ON t.[{nameof(Ticket.TicketResolutionId)}] = tr.[{nameof(TicketResolution.Id)}]
        WHERE 1 = 1
    ';

    IF {Params.SearchById} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.Id)}] = {Params.SearchById}';
    END
    IF {Params.SearchByIsClosed} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.IsClosed)}] = {Params.SearchByIsClosed}';
    END
    IF {Params.SearchByReporterId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.ReporterId)}] = {Params.SearchByReporterId}';
    END
    IF {Params.SearchByAssigneeId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.AssigneeId)}] = {Params.SearchByAssigneeId}';
    END
    IF {Params.SearchByTicketPriorityId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.TicketPriorityId)}] = {Params.SearchByTicketPriorityId}';
    END
    IF {Params.SearchByTicketQueueId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.TicketQueueId)}] = {Params.SearchByTicketQueueId}';
    END
    IF {Params.SearchByTicketResolutionId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.TicketResolutionId)}] = {Params.SearchByTicketResolutionId}';
    END
    IF {Params.SearchByTicketStatusId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.TicketStatusId)}] = {Params.SearchByTicketStatusId}';
    END
    IF {Params.SearchByTicketCategoryId} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.TicketCategoryId)}] = {Params.SearchByTicketCategoryId}';
    END
    IF {Params.SearchBySource} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND t.[{nameof(Ticket.Source)}] = {Params.SearchBySource}';
    END

    SET @Sql = @Sql + ' ORDER BY t.[' + {Params.SortBy} + '] ' + {Params.SortDirection} + ' OFFSET {Params.Offset} ROWS FETCH NEXT {Params.Limit} ROWS ONLY';

    DECLARE @params NVARCHAR(500) =
    N'
        {Params.SearchById} INT,
        {Params.SearchByIsClosed} BIT,
        {Params.SearchByReporterId} INT,
        {Params.SearchByAssigneeId} INT,
        {Params.SearchByTicketPriorityId} INT,
        {Params.SearchByTicketQueueId} INT,
        {Params.SearchByTicketResolutionId} INT,
        {Params.SearchByTicketStatusId} INT,
        {Params.SearchByTicketCategoryId} INT,
        {Params.SearchBySource} INT,
        {Params.Offset} INT,
        {Params.Limit} INT
    ';
    EXECUTE sp_executesql @Sql,
        @params,
        {Params.SearchById} = {Params.SearchById},
        {Params.SearchByIsClosed} = {Params.SearchByIsClosed},
        {Params.SearchByReporterId} = {Params.SearchByReporterId},
        {Params.SearchByAssigneeId} = {Params.SearchByAssigneeId},
        {Params.SearchByTicketPriorityId} = {Params.SearchByTicketPriorityId},
        {Params.SearchByTicketQueueId} = {Params.SearchByTicketQueueId},
        {Params.SearchByTicketResolutionId} = {Params.SearchByTicketResolutionId},
        {Params.SearchByTicketStatusId} = {Params.SearchByTicketStatusId},
        {Params.SearchByTicketCategoryId} = {Params.SearchByTicketCategoryId},
        {Params.SearchBySource} = {Params.SearchBySource},
        {Params.Offset} = {Params.Offset},
        {Params.Limit} = {Params.Limit};
END";
    }

    public record TicketReportModel : TicketModel
    {
        public int Total { get; set; }
    }
}