using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using mvp.tickets.data;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Models;
using mvp.tickets.web.Helpers;

namespace mvp.tickets.web.Kafka
{
    public class RequestTimeConsumer : BackgroundService
    {
        private readonly string _topic;
        private readonly IConsumer<Ignore, string> _kafkaConsumer;
        private readonly ILogger<RequestTimeConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDistributedCache _cache;
        private readonly ISettings _settings;

        public RequestTimeConsumer(IConnectionStrings connectionStrings, ILogger<RequestTimeConsumer> logger, IServiceScopeFactory serviceScopeFactory,
            IDistributedCache cache, ISettings settings)
        {
            _topic = KafkaModels._ticketsTopic;
            var consumerConfig = new ConsumerConfig
            {
                GroupId = KafkaModels._ticketsTopic,
                BootstrapServers = connectionStrings.KafkaConnection,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 5000,
                EnableAutoOffsetStore = false,
            };
            _kafkaConsumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
            _settings = settings;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _kafkaConsumer.Consume(cancellationToken);

                    if (cr != null)
                    {
                        Process(cr);
                        _kafkaConsumer.StoreOffset(cr);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, $"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
        }

        public override void Dispose()
        {
            _kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            _kafkaConsumer.Dispose();

            base.Dispose();
        }

        private void Process(ConsumeResult<Ignore, string> cr)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var data = System.Text.Json.JsonSerializer.Deserialize<KafkaModels.Message>(cr.Message.Value);
                if (data?.Ticket != null && data.Type == KafkaModels.MessageType.NewTicket)
                {
                    if (dbContext.Tickets.Any(s => s.UniqueId == data.Ticket.UniqueId))
                    {
                        return;
                    }

                    var defaultQueue = dbContext.TicketQueues.AsNoTracking().FirstOrDefault(s => s.IsDefault && s.CompanyId == data.Ticket.CompanyId);
                    if (defaultQueue == null)
                    {
                        _logger.LogError("В системе отсутствует первичная очередь заявок.");
                        return;
                    }

                    var defaultStatus = dbContext.TicketStatuses.AsNoTracking().FirstOrDefault(s => s.IsDefault && s.CompanyId == data.Ticket.CompanyId);
                    if (defaultStatus == null)
                    {
                        _logger.LogError("В системе отсутствует первичный статус заявок.");
                        return;
                    }

                    data.Ticket.TicketQueueId = defaultQueue.Id;
                    data.Ticket.TicketStatusId = defaultStatus.Id;
                    dbContext.Tickets.Add(data.Ticket);
                    dbContext.SaveChanges();
                    _cache.ClearTicketsReport(_logger, data.Ticket.CompanyId, data.Ticket.ReporterId);
                    _cache.ClearTicketsReport(_logger, data.Ticket.CompanyId, null);
                }
                else if (data?.Comment != null && (data.Type == KafkaModels.MessageType.NewCommentFromUser || data.Type == KafkaModels.MessageType.NewCommentFromEmployee))
                {
                    if (dbContext.TicketComments.Any(s => s.UniqueId == data.Comment.UniqueId))
                    {
                        return;
                    }

                    var ticket = dbContext.Tickets.FirstOrDefault(s => s.Id == data.Comment.TicketId && s.CompanyId == data.CompanyId);
                    if (ticket == null)
                    {
                        return;
                    }

                    if (data.Type == KafkaModels.MessageType.NewCommentFromUser && ticket.ReporterId != data.UserId)
                    {
                        return;
                    }

                    foreach (var item in data.Comment.TicketCommentAttachments)
                    {
                        var fromPath = Path.Join(_settings.FilesPath, $"/{AppConstants.TicketFilesTempFolder}/{data.CompanyId}/{item.FileName}.{item.Extension}");
                        var toPath = Path.Join(_settings.FilesPath, $"/{AppConstants.TicketFilesFolder}/{data.CompanyId}/{ticket.ReporterId}/{item.FileName}.{item.Extension}");
                        System.IO.File.Move(fromPath, toPath);
                    }

                    dbContext.TicketComments.Add(data.Comment);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
