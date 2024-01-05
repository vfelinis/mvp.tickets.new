using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.web.Helpers;
using System.Text;
using System.Web;

namespace mvp.tickets.web.Services
{
    public class EmailBackgroundSearvice : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EmailBackgroundSearvice> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ISettings _settings;
        private const int _delayInSec = 15;

        public EmailBackgroundSearvice(IServiceScopeFactory serviceScopeFactory, ILogger<EmailBackgroundSearvice> logger, ISettings settings, IWebHostEnvironment environment)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _settings = settings;
            _environment = environment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    ClientSecrets secrets = new ClientSecrets
                    {
                        ClientId = _settings.Gmail.ClientID,
                        ClientSecret = _settings.Gmail.ClientSecret
                    };
                    var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, new string[] { GmailService.Scope.MailGoogleCom }, "user", CancellationToken.None);
                    using var gmailService = new GmailService(new BaseClientService.Initializer { HttpClientInitializer = credentials });
                    var listRequest = gmailService.Users.Messages.List(_settings.Gmail.Email);
                    listRequest.Q = "is:unread in:anywhere";
                    ListMessagesResponse listResponse = null;

                    do
                    {
                        listResponse = await listRequest.ExecuteAsync();
                        if (listResponse.Messages != null)
                        {
                            foreach (var messageFromList in listResponse.Messages)
                            {
                                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                                var messageRequest = gmailService.Users.Messages.Get(_settings.Gmail.Email, messageFromList.Id);
                                messageRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
                                var messageResponse = await messageRequest.ExecuteAsync();
                                using MemoryStream rawInStream = new MemoryStream(Base64FUrlDecode(messageResponse.Raw));
                                var mailMessage = MimeKit.MimeMessage.Load(rawInStream);
                                var aliase = mailMessage.To.Mailboxes.First().Address.ToLower();
                                if (!string.Equals(aliase, _settings.Gmail.Alias))
                                {
                                    await gmailService.Users.Messages.Delete(_settings.Gmail.Email, messageFromList.Id).ExecuteAsync();
                                    continue;
                                }

                                var defaultQueue = await dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                                if (defaultQueue == null)
                                {
                                    throw new InvalidOperationException("В системе отсутствует первичная очередь заявок.");
                                }

                                var defaultStatus = await dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                                if (defaultStatus == null)
                                {
                                    throw new InvalidOperationException("В системе отсутствует первичный статус заявок.");
                                }

                                var defaultCategory = await dbContext.TicketCategories.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                                if (defaultCategory == null)
                                {
                                    throw new InvalidOperationException("В системе отсутствует категория по умолчанию.");
                                }

                                var email = mailMessage.From.Mailboxes.First().Address.ToLower();
                                var userName = mailMessage.From.Mailboxes.First().Name ?? "";
                                var user = await dbContext.Users.FirstOrDefaultAsync(s => s.Email == email);
                                if (user == null)
                                {
                                    user = new User
                                    {
                                        Email = email,
                                        FirstName = userName?.Split(' ').First(),
                                        LastName = userName?.Split(' ').Last(),
                                        Permissions = domain.Enums.Permissions.User,
                                        IsLocked = false,
                                        DateCreated = DateTimeOffset.Now,
                                        DateModified = DateTimeOffset.Now
                                    };
                                    await dbContext.Users.AddAsync(user);
                                    await dbContext.SaveChangesAsync();

                                    try
                                    {
                                        var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
                                        await firebaseAuth.CreateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                                        {
                                            Email = user.Email,
                                            DisplayName = !string.IsNullOrWhiteSpace(userName) ? userName : user.Email.Split('@').First(),
                                            Password = Guid.NewGuid().ToString()
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, ex.Message);
                                    }
                                }

                                if (!user.IsLocked)
                                {
                                    var entry = new Ticket
                                    {
                                        Name = HttpUtility.HtmlAttributeEncode(mailMessage.Subject),
                                        Token = Guid.NewGuid().ToString(),
                                        Source = domain.Enums.TicketSource.Email,
                                        IsClosed = false,
                                        DateCreated = DateTimeOffset.Now,
                                        DateModified = DateTimeOffset.Now,
                                        ReporterId = user.Id,
                                        TicketQueueId = defaultQueue.Id,
                                        TicketStatusId = defaultStatus.Id,
                                        TicketCategoryId = defaultCategory.Id,
                                        TicketObservations = new List<TicketObservation>
                                        {
                                            new TicketObservation
                                            {
                                                DateCreated = DateTimeOffset.Now,
                                                UserId = user.Id
                                            }
                                        }
                                    };

                                    var ticketComment = new TicketComment
                                    {
                                        Ticket = entry,
                                        Text = HttpUtility.HtmlAttributeEncode(mailMessage.TextBody ?? HtmlUtilities.ConvertToPlainText(mailMessage.HtmlBody)),
                                        IsInternal = false,
                                        IsActive = true,
                                        DateCreated = DateTimeOffset.Now,
                                        DateModified = DateTimeOffset.Now,
                                        CreatorId = user.Id,
                                    };
                                    entry.TicketComments.Add(ticketComment);

                                    if (mailMessage.Attachments?.Any() == true)
                                    {
                                        foreach (var attachment in mailMessage.Attachments)
                                        {
                                            using var file = new MemoryStream();
                                            var fileName = "";
                                            if (attachment is MimeKit.MessagePart)
                                            {
                                                fileName = attachment.ContentDisposition?.FileName;
                                                var rfc822 = (MimeKit.MessagePart)attachment;

                                                if (string.IsNullOrEmpty(fileName))
                                                    fileName = "attached-message.eml";

                                                rfc822.Message.WriteTo(file);
                                            }
                                            else
                                            {
                                                var part = (MimePart)attachment;
                                                fileName = part.FileName;
                                                part.Content.DecodeTo(file);
                                            }
                                            if (!string.IsNullOrEmpty(fileName))
                                            {
                                                var ext = Path.GetExtension(fileName).Trim('.').ToLower();
                                                var ticketCommentAttachment = new TicketCommentAttachment
                                                {
                                                    TicketComment = ticketComment,
                                                    DateCreated = DateTimeOffset.Now,
                                                    DateModified = DateTimeOffset.Now,
                                                    IsActive = true,
                                                    OriginalFileName = fileName,
                                                    Extension = ext,
                                                    FileName = Guid.NewGuid().ToString()
                                                };
                                                ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                                                var path = Path.Join(_environment.WebRootPath, $"/{TicketConstants.AttachmentFolder}/{user.Id}/{ticketCommentAttachment.FileName}.{ext}");
                                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                                                using (var stream = System.IO.File.Create(path))
                                                {
                                                    file.Seek(0, SeekOrigin.Begin);
                                                    await file.CopyToAsync(stream);
                                                }
                                            }
                                        }
                                    }

                                    await dbContext.Tickets.AddAsync(entry);
                                    await dbContext.SaveChangesAsync();

                                    var link = $"https://{_settings.Host}/tickets/{entry.Id}/alt/?token={entry.Token}";
                                    mailMessage.To.Clear();
                                    mailMessage.To.Add(new MimeKit.MailboxAddress("", user.Email));
                                    mailMessage.Subject = entry.Name;
                                    mailMessage.Body = new MimeKit.BodyBuilder { HtmlBody = $"<a href='{link}'>Ссылка на созданную заявку {link}</a>" }.ToMessageBody();

                                    messageResponse.Raw = Encode(mailMessage);
                                    await gmailService.Users.Messages.Send(messageResponse, _settings.Gmail.Email).ExecuteAsync();
                                }

                                await gmailService.Users.Messages.Delete(_settings.Gmail.Email, messageFromList.Id).ExecuteAsync();
                            }
                        }

                        listRequest.PageToken = listResponse.NextPageToken;
                    }
                    while (listRequest.PageToken != null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.GetBaseException().Message);
                }

                await Delay(stoppingToken).ConfigureAwait(false);
            }
        }

        private async Task<UserCredential> Login(string googleClientId, string googleClientSecret, string[] scopes)
        {
            ClientSecrets secrets = new ClientSecrets
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret
            };
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, "user", CancellationToken.None);
        }

        private async Task Delay(CancellationToken stoppingToken)
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_delayInSec), stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
            }
        }

        public static byte[] Base64FUrlDecode(string input)
        {
            int padChars = (input.Length % 4) == 0 ? 0 : (4 - (input.Length % 4));
            StringBuilder result = new StringBuilder(input, input.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }

        public static string Encode(MimeKit.MimeMessage mimeMessage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                mimeMessage.WriteTo(ms);
                return Convert.ToBase64String(ms.GetBuffer())
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }
    }
}
