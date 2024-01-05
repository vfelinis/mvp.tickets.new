using mvp.tickets.data.Models;
using mvp.tickets.domain.Models;


namespace mvp.tickets.data.Procedures
{
    [Procedure]
    public static class GetCategoriesProcedure
    {
        public static string Name => "procGetCategories";
        public static int Version => 3;
        public static class Params
        {
            public static string Id => "@id";
            public static string OnlyDefault => "@onlyDefault";
            public static string OnlyActive => "@onlyActive";
            public static string OnlyRoot => "@onlyRoot";
        }

        public static string Text => $@"
/* version={Version} */
CREATE PROCEDURE [{Name}]
    {Params.Id} INT = NULL,
    {Params.OnlyDefault} BIT,
    {Params.OnlyActive} BIT,
    {Params.OnlyRoot} BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT
        t1.[{nameof(TicketCategory.Id)}] AS [{nameof(CategoryModel.Id)}]
        ,t1.[{nameof(TicketCategory.Name)}] AS [{nameof(CategoryModel.Name)}]
        ,t1.[{nameof(TicketCategory.IsDefault)}] AS [{nameof(CategoryModel.IsDefault)}]
        ,t1.[{nameof(TicketCategory.IsActive)}] AS [{nameof(CategoryModel.IsActive)}]
        ,t1.[{nameof(TicketCategory.IsRoot)}] AS [{nameof(CategoryModel.IsRoot)}]
        ,t1.[{nameof(TicketCategory.DateCreated)}] AS [{nameof(CategoryModel.DateCreated)}]
        ,t1.[{nameof(TicketCategory.DateModified)}] AS [{nameof(CategoryModel.DateModified)}]
        ,t1.[{nameof(TicketCategory.ParentCategoryId)}] AS [{nameof(CategoryModel.ParentCategoryId)}]
        ,t2.[{nameof(TicketCategory.Name)}] AS [{nameof(CategoryModel.ParentCategory)}]
    FROM [{TicketCategoryExtension.TableName}] t1
    LEFT JOIN [{TicketCategoryExtension.TableName}] t2 ON t1.[{nameof(TicketCategory.ParentCategoryId)}] = t2.[{nameof(TicketCategory.Id)}]
    WHERE ({Params.Id} IS NULL OR t1.[{nameof(TicketCategory.Id)}] = {Params.Id})
    AND ({Params.OnlyDefault} = 0 OR t1.[{nameof(TicketCategory.IsDefault)}] = 1)
    AND ({Params.OnlyActive} = 0 OR t1.[{nameof(TicketCategory.IsActive)}] = 1)
    AND ({Params.OnlyRoot} = 0 OR t1.[{nameof(TicketCategory.IsRoot)}] = 1)
END";
    }
}