namespace mvp.tickets.domain.Models
{
    public interface IInviteModel
    {
        int Id { get; set; }
        string Email { get; set; }
        string Company { get; set; }
        DateTimeOffset DateSent { get; set; }
    }

    public record InviteModel : IInviteModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public DateTimeOffset DateSent { get; set; }
    }
}
