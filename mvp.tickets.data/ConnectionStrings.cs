namespace mvp.tickets.data
{
    public interface IConnectionStrings
    {
        string DefaultConnection { get; set; }
    }

    public record ConnectionStrings : IConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
