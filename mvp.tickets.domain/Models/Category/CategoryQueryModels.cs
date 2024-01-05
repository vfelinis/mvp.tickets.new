namespace mvp.tickets.domain.Models
{
    public interface ICategoryQueryRequest : IBaseQueryRequest
    {
        int? Id { get; set; }
        bool OnlyDefault { get; set; }
        bool OnlyActive { get; set; }
        bool OnlyRoot { get; set; }
    }

    public record CategoryQueryRequest : BaseQueryRequest, ICategoryQueryRequest
    {
        public int? Id { get; set; }
        public bool OnlyDefault { get; set; }
        public bool OnlyActive { get; set; }
        public bool OnlyRoot { get; set; }
    }
    public interface ICategoryModel
    {
        int Id { get; set; }
        string Name { get; set; }
        bool IsRoot { get; set; }
        bool IsDefault { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateModified { get; set; }
        int? ParentCategoryId { get; set; }
        string ParentCategory { get; set; }
    }

    public record CategoryModel: ICategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategory { get; set; }
    }
}
