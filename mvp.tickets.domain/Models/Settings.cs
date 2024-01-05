namespace mvp.tickets.domain.Models
{
    public interface ISettings
    {
        string FirebaseAdminConfig { get; set; }
        GMailSettings Gmail { get; set; }
        string Host { get; set; }
        string ApiKey { get; set; }
        string TelegramToken { get; set; }
    }
    public record Settings: ISettings
    {
        public string FirebaseAdminConfig { get; set; }
        public GMailSettings Gmail { get; set; }
        public string Host { get; set; }
        public string ApiKey { get; set; }
        public string TelegramToken { get; set; }
    }
    public class GMailSettings
    {
        public string Alias { get; set; }
        public string Email { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }
}
