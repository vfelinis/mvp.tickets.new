using MailKit.Net.Smtp;
using MailKit.Security;
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
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("", _settings.Gmail.Alias));
            mimeMessage.To.Add(new MailboxAddress("", to));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(isBodyHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Gmail.Email, _settings.Gmail.AppPassword);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
