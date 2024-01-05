using mvp.tickets.data.Models;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Models;


namespace mvp.tickets.data.Procedures
{
    [Procedure]
    public static class UsersReportProcedure
    {
        public static string Name => "procUsersReport";
        public static int Version => 7;
        public static class Params
        {
            public static string SearchByEmal => "@searchByEmal";
            public static string SearchByFirstName => "@searchByFirstName";
            public static string SearchByLastName => "@searchByLastName";
            public static string SearchByIsLocked => "@searchByIsLocked";
            public static string SearchByPermissions => "@searchByPermissions";
            public static string SearchById => "@searchById";
            public static string SortBy => "@sortBy";
            public static string SortDirection => "@sortDirection";
            public static string Offset => "@offset";
            public static string Limit => "@limit";
        }

        public static string Text => $@"
/* version={Version} */
CREATE PROCEDURE [{Name}]
    {Params.SearchByEmal} NVARCHAR(MAX) = NULL,
    {Params.SearchByFirstName} NVARCHAR(MAX) = NULL,
    {Params.SearchByLastName} NVARCHAR(MAX) = NULL,
    {Params.SearchByIsLocked} BIT = NULL,
    {Params.SearchByPermissions} INT = NULL,
    {Params.SearchById} INT = NULL,
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
            [{nameof(User.Id)}] AS [{nameof(UserReportModel.Id)}]
            ,[{nameof(User.Email)}] AS [{nameof(UserReportModel.Email)}]
            ,[{nameof(User.FirstName)}] AS [{nameof(UserReportModel.FirstName)}]
            ,[{nameof(User.LastName)}] AS [{nameof(UserReportModel.LastName)}]
            ,[{nameof(User.Permissions)}] AS [{nameof(UserReportModel.Permissions)}]
            ,[{nameof(User.IsLocked)}] AS [{nameof(UserReportModel.IsLocked)}]
            ,[{nameof(User.DateCreated)}] AS [{nameof(UserReportModel.DateCreated)}]
            ,[{nameof(User.DateModified)}] AS [{nameof(UserReportModel.DateModified)}]
            ,COUNT(*) OVER() AS [{nameof(UserReportModel.Total)}]
        FROM [{UserExtension.TableName}]
        WHERE 1 = 1
    ';

    IF LEN({Params.SearchByEmal}) > 0
    BEGIN
        SET @Sql = @Sql + ' AND [{nameof(User.Email)}] = {Params.SearchByEmal}';
    END
    IF LEN({Params.SearchByFirstName}) > 0
    BEGIN
        SET @Sql = @Sql + ' AND [{nameof(User.FirstName)}] = {Params.SearchByFirstName}';
    END
    IF LEN({Params.SearchByLastName}) > 0
    BEGIN
        SET @Sql = @Sql + ' AND [{nameof(User.LastName)}] = {Params.SearchByLastName}';
    END
    IF {Params.SearchByIsLocked} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND [{nameof(User.IsLocked)}] = {Params.SearchByIsLocked}';
    END
    IF {Params.SearchByPermissions} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND ([{nameof(User.Permissions)}] & {Params.SearchByPermissions}) = {Params.SearchByPermissions}';
    END
    IF {Params.SearchById} IS NOT NULL
    BEGIN
        SET @Sql = @Sql + ' AND [{nameof(User.Id)}] = {Params.SearchById}';
    END

    SET @Sql = @Sql + ' ORDER BY [' + {Params.SortBy} + '] ' + {Params.SortDirection} + ' OFFSET {Params.Offset} ROWS FETCH NEXT {Params.Limit} ROWS ONLY';

    DECLARE @params NVARCHAR(500) =
    N'
        {Params.SearchByEmal} NVARCHAR(MAX),
        {Params.SearchByFirstName} NVARCHAR(MAX),
        {Params.SearchByLastName} NVARCHAR(MAX),
        {Params.SearchByIsLocked} BIT,
        {Params.SearchByPermissions} INT,
        {Params.SearchById} INT,
        {Params.Offset} INT,
        {Params.Limit} INT
    ';
    EXECUTE sp_executesql @Sql,
        @params,
        {Params.SearchByEmal} = {Params.SearchByEmal},
        {Params.SearchByFirstName} = {Params.SearchByFirstName},
        {Params.SearchByLastName} = {Params.SearchByLastName},
        {Params.SearchByIsLocked} = {Params.SearchByIsLocked},
        {Params.SearchByPermissions} = {Params.SearchByPermissions},
        {Params.SearchById} = {Params.SearchById},
        {Params.Offset} = {Params.Offset},
        {Params.Limit} = {Params.Limit};
END";
    }

    public record UserReportModel : UserModel
    {
        public int Total { get; set; }
    }
}