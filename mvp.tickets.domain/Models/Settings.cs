namespace mvp.tickets.domain.Models
{
    public interface ISettings
    {
        string FirebaseAdminConfig { get; set; }
        GMailSettings Gmail { get; set; }
        string Host { get; set; }
        string ApiKey { get; set; }
        string TelegramToken { get; set; }
        S3Settings S3 { get; set; }
    }
    public record Settings: ISettings
    {
        public string FirebaseAdminConfig { get; set; }
        public GMailSettings Gmail { get; set; }
        public string Host { get; set; }
        public string ApiKey { get; set; }
        public string TelegramToken { get; set; }
        public S3Settings S3 { get; set; }
    }
    public class GMailSettings
    {
        public string Alias { get; set; }
        public string Email { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string AppPassword { get; set; }
    }

    public class S3Settings
    {
        public string Url { get; set; }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
    }
}
