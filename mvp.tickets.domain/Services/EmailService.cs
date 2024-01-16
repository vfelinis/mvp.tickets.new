using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using mvp.tickets.domain.Models;


namespace mvp.tickets.domain.Services
{
    public class EmailService: IEmailService
    {
        private readonly ISettings _settings;
        public EmailService(ISettings settings)
        {
            _settings = settings;
        }

        public async Task Send(string to, string subject, string body, bool isBodyHtml = false)
        {
            ClientSecrets secrets = new ClientSecrets
            {
                ClientId = _settings.Gmail.ClientID,
                ClientSecret = _settings.Gmail.ClientSecret
            };
            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, new string[] { GmailService.Scope.MailGoogleCom }, "user", CancellationToken.None);
            using var gmailService = new GmailService(new BaseClientService.Initializer { HttpClientInitializer = credentials });

            var mimeMessage = new MimeMessage();
            mimeMessage.To.Add(MailboxAddress.Parse(to));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(isBodyHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
            {
                Text = body
            };

            Message message = new Message();

            using (var memory = new MemoryStream())
            {
                mimeMessage.WriteTo(memory);

                var buffer = memory.GetBuffer();
                int length = (int)memory.Length;

                message.Raw = Convert.ToBase64String(buffer, 0, length);
            }

            await gmailService.Users.Messages.Send(message, "me").ExecuteAsync();
        }
    }
}
