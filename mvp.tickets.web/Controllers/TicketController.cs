using Confluent.Kafka;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;
using mvp.tickets.web.Helpers;
using mvp.tickets.web.Kafka;
using Npgsql;
using System.Data;
using System.Web;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ILogger<TicketController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ISettings _settings;
        private readonly IDistributedCache _cache;
        private readonly IS3Service _s3Service;

        public TicketController(ApplicationDbContext dbContext, IConnectionStrings connectionStrings, ILogger<TicketController> logger, IWebHostEnvironment environment,
            ISettings settings, IDistributedCache cache, IS3Service s3Service)
        {
            _dbContext = dbContext;
            _connectionStrings = connectionStrings;
            _logger = logger;
            _environment = environment;
            _settings = settings;
            _cache = cache;
            _s3Service = s3Service;
        }

        [Authorize]
        [HttpPost("report")]
        public async Task<IBaseReportQueryResponse<IEnumerable<ITicketModel>>> Report(BaseReportQueryRequest request)
        {
            if (request == null)
            {
                return new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }
            IBaseReportQueryResponse<IEnumerable<ITicketModel>> response = default;
            try
            {
                if ((!request.IsUserView && !User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim))
                    || (request.IsUserView && !User.Claims.Any(s => s.Type == AuthConstants.UserClaim)))
                {
                    return new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                var options = System.Text.Json.JsonSerializer.Serialize(request);
                var cacheData = await _cache.GetTicketsReport(_logger, companyId, request.IsUserView ? userId : null, options);
                if (cacheData?.Data != null)
                {
                    return new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                    {
                        IsSuccess = cacheData.Data.IsSuccess,
                        Code = cacheData.Data.Code,
                        ErrorMessage = cacheData.Data.ErrorMessage,
                        Data = cacheData.Data.Data,
                        Total = cacheData.Data.Total
                    };
                }

                using (var connection = new NpgsqlConnection(_connectionStrings.DefaultConnection))
                {
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add("@companyId", companyId, DbType.Int32);
                    var query =
$@"
DROP TABLE IF EXISTS tickets_tmp;

CREATE TEMPORARY TABLE tickets_tmp AS 
SELECT
    COUNT(*) OVER() AS ""{nameof(TicketReportModel.Total)}""
    ,t.""{nameof(Ticket.Id)}"" AS ""{nameof(TicketReportModel.Id)}""
FROM dbo.""{TicketExtension.TableName}"" t
WHERE t.""{nameof(Ticket.CompanyId)}"" = @companyId
";
                    if (request.IsUserView)
                    {
                        parameter.Add("@searchByReporterId", userId, DbType.Int32);
                        query += $@" AND t.""{nameof(Ticket.ReporterId)}"" = @searchByReporterId";
                    }
                    if (request.SearchBy?.Any() == true)
                    {
                        foreach (var search in request.SearchBy.Where(s => !string.IsNullOrWhiteSpace($"{s.Value}")))
                        {
                            if (string.Equals(search.Key, nameof(Ticket.Id), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchById", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.Id)}"" = @searchById";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.IsClosed), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByIsClosed", Convert.ToBoolean(search.Value), DbType.Boolean);
                                query += $@" AND t.""{nameof(Ticket.IsClosed)}"" = @searchByIsClosed";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.Name), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByName", search.Value, DbType.String);
                                query += $@" AND t.""{nameof(Ticket.Name)}"" LIKE CONCAT('%', @searchByName, '%')";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.ReporterEmail), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByReporter", search.Value, DbType.String);
                                query += $@" AND t.""{nameof(Ticket.ReporterEmail)}"" LIKE CONCAT('%', @searchByReporter, '%')";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.AssigneeEmail), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByAssignee", search.Value, DbType.String);
                                query += $@" AND t.""{nameof(Ticket.AssigneeEmail)}"" LIKE CONCAT('%', @searchByAssignee, '%')";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketPriorityId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByTicketPriorityId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.TicketPriorityId)}"" = @searchByTicketPriorityId";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketQueueId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByTicketQueueId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.TicketQueueId)}"" = @searchByTicketQueueId";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketResolutionId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByTicketResolutionId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.TicketResolutionId)}"" = @searchByTicketResolutionId";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketStatusId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByTicketStatusId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.TicketStatusId)}"" = @searchByTicketStatusId";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketCategoryId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByTicketCategoryId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.TicketCategoryId)}"" = @searchByTicketCategoryId";
                            }
                            if (string.Equals(search.Key, nameof(Ticket.Source), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchBySource", (int)Enum.Parse<TicketSource>(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.Source)}"" = @searchBySource";
                            }
                        }
                    }

                    parameter.Add("@offset", request.Offset, DbType.Int32);
                    parameter.Add("@limit", ReportConstants.DEFAULT_LIMIT, DbType.Int32);
                    query +=
$@"
ORDER BY t.""{typeof(Ticket).GetProperties().FirstOrDefault(s => s.Name == request.SortBy)?.Name ?? nameof(Ticket.Id)}"" {request.SortDirection} OFFSET @offset LIMIT @limit;

SELECT
    tmp.""{nameof(TicketReportModel.Total)}""
    ,tmp.""{nameof(TicketReportModel.Id)}""
    ,t.""{nameof(Ticket.Name)}"" AS ""{nameof(TicketReportModel.Name)}""
    ,t.""{nameof(Ticket.Token)}"" AS ""{nameof(TicketReportModel.Token)}""
    ,CASE t.""{nameof(Ticket.Source)}""
        WHEN {(int)TicketSource.Email} THEN 'Email'
        WHEN {(int)TicketSource.Telegram} THEN 'Telegram'
        ELSE 'Web'
    END AS ""{nameof(TicketReportModel.Source)}""
    ,t.""{nameof(Ticket.IsClosed)}"" AS ""{nameof(TicketReportModel.IsClosed)}""
    ,t.""{nameof(Ticket.DateCreated)}"" AS ""{nameof(TicketReportModel.DateCreated)}""
    ,t.""{nameof(Ticket.DateModified)}"" AS ""{nameof(TicketReportModel.DateModified)}""

    ,ru.""{nameof(data.Models.User.Id)}"" AS ""{nameof(TicketReportModel.ReporterId)}""
    ,ru.""{nameof(data.Models.User.Email)}"" AS ""{nameof(TicketReportModel.ReporterEmail)}""
    ,ru.""{nameof(data.Models.User.FirstName)}"" AS ""{nameof(TicketReportModel.ReporterFirstName)}""
    ,ru.""{nameof(data.Models.User.LastName)}"" AS ""{nameof(TicketReportModel.ReporterLastName)}""

    ,au.""{nameof(data.Models.User.Id)}"" AS ""{nameof(TicketReportModel.AssigneeId)}""
    ,au.""{nameof(data.Models.User.Email)}"" AS ""{nameof(TicketReportModel.AssigneeEmail)}""
    ,au.""{nameof(data.Models.User.FirstName)}"" AS ""{nameof(TicketReportModel.AssigneeFirstName)}""
    ,au.""{nameof(data.Models.User.LastName)}"" AS ""{nameof(TicketReportModel.AssigneeLastName)}""
            
    ,tp.""{nameof(TicketPriority.Id)}"" AS ""{nameof(TicketReportModel.TicketPriorityId)}""
    ,tp.""{nameof(TicketPriority.Name)}"" AS ""{nameof(TicketReportModel.TicketPriority)}""
            
    ,tq.""{nameof(TicketQueue.Id)}"" AS ""{nameof(TicketReportModel.TicketQueueId)}""
    ,tq.""{nameof(TicketQueue.Name)}"" AS ""{nameof(TicketReportModel.TicketQueue)}""

    ,tr.""{nameof(TicketResolution.Id)}"" AS ""{nameof(TicketReportModel.TicketResolutionId)}""
    ,tr.""{nameof(TicketResolution.Name)}"" AS ""{nameof(TicketReportModel.TicketResolution)}""

    ,ts.""{nameof(TicketStatus.Id)}"" AS ""{nameof(TicketReportModel.TicketStatusId)}""
    ,ts.""{nameof(TicketStatus.Name)}"" AS ""{nameof(TicketReportModel.TicketStatus)}""

    ,tc.""{nameof(TicketCategory.Id)}"" AS ""{nameof(TicketReportModel.TicketCategoryId)}""
    ,tc.""{nameof(TicketCategory.Name)}"" AS ""{nameof(TicketReportModel.TicketCategory)}""
FROM tickets_tmp tmp
INNER JOIN dbo.""{TicketExtension.TableName}"" t ON tmp.""Id"" = t.""Id""
INNER JOIN dbo.""{UserExtension.TableName}"" ru ON t.""{nameof(Ticket.ReporterId)}"" = ru.""{nameof(data.Models.User.Id)}""
INNER JOIN dbo.""{TicketQueueExtension.TableName}"" tq ON t.""{nameof(Ticket.TicketQueueId)}"" = tq.""{nameof(TicketQueue.Id)}""
INNER JOIN dbo.""{TicketStatusExtension.TableName}"" ts ON t.""{nameof(Ticket.TicketStatusId)}"" = ts.""{nameof(TicketStatus.Id)}""
INNER JOIN dbo.""{TicketCategoryExtension.TableName}"" tc ON t.""{nameof(Ticket.TicketCategoryId)}"" = tc.""{nameof(TicketCategory.Id)}""
LEFT JOIN dbo.""{UserExtension.TableName}"" au ON t.""{nameof(Ticket.AssigneeId)}"" = au.""{nameof(data.Models.User.Id)}""
LEFT JOIN dbo.""{TicketPriorityExtension.TableName}"" tp ON t.""{nameof(Ticket.TicketPriorityId)}"" = tp.""{nameof(TicketPriority.Id)}""
LEFT JOIN dbo.""{TicketResolutionExtension.TableName}"" tr ON t.""{nameof(Ticket.TicketResolutionId)}"" = tr.""{nameof(TicketResolution.Id)}""
ORDER BY t.""{typeof(Ticket).GetProperties().FirstOrDefault(s => s.Name == request.SortBy)?.Name ?? nameof(Ticket.Id)}"" {request.SortDirection} OFFSET @offset LIMIT @limit;
";

                    var result = await connection.QueryAsync<TicketReportModel>(query, param: parameter);

                    var entries = result.ToList();
                    response = new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                    {
                        Data = entries,
                        Total = entries.FirstOrDefault()?.Total ?? 0,
                        IsSuccess = true,
                        Code = ResponseCodes.Success
                    };
                    await _cache.SetTicketsReport(_logger, companyId, request.IsUserView ? userId : null, options, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseReportQueryResponse<IEnumerable<ITicketModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [HttpGet("{id}")]
        public async Task<IBaseQueryResponse<ITicketModel>> Get(int id, [FromQuery] TicketQueryRequest request)
        {
            IBaseQueryResponse<ITicketModel> response = default;
            try
            {
                if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !User.Claims.Any(s => s.Type == AuthConstants.UserClaim) && string.IsNullOrWhiteSpace(request?.Token))
                {
                    return new BaseQueryResponse<ITicketModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var includeIternal = User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !request.IsUserView;
                var entry = await _dbContext.Tickets.AsNoTracking().Where(s => s.Id == id && s.CompanyId == companyId)
                    .Select(s => new TicketModel
                    {
                        Id = s.Id,
                        AssigneeId = s.AssigneeId,
                        AssigneeEmail = s.Assignee.Email,
                        AssigneeFirstName = s.Assignee.FirstName,
                        AssigneeLastName = s.Assignee.LastName,
                        IsClosed = s.IsClosed,
                        DateCreated = s.DateCreated,
                        DateModified = s.DateModified,
                        Name = s.Name,
                        ReporterId = s.ReporterId,
                        ReporterEmail = s.Reporter.Email,
                        ReporterFirstName = s.Reporter.FirstName,
                        ReporterLastName = s.Reporter.LastName,
                        Source = s.Source.ToString(),
                        TicketCategoryId = s.TicketCategoryId,
                        TicketCategory = s.TicketCategory.Name,
                        TicketPriorityId = s.TicketPriorityId,
                        TicketPriority = s.TicketPriority.Name,
                        TicketQueueId = s.TicketQueueId,
                        TicketQueue = s.TicketQueue.Name,
                        TicketResolutionId = s.TicketResolutionId,
                        TicketResolution = s.TicketResolution.Name,
                        TicketStatusId = s.TicketStatusId,
                        TicketStatus = s.TicketStatus.Name,
                        Token = s.Token,
                        TicketComments = s.TicketComments
                            .Where(c => c.IsActive && (includeIternal || !c.IsInternal))
                            .Select(c => new TicketCommentModel
                            {
                                Id = c.Id,
                                Text = c.Text,
                                IsInternal = c.IsInternal,
                                CreatorId = c.CreatorId,
                                CreatorEmail = c.Creator.Email,
                                CreatorFirstName = c.Creator.FirstName,
                                CreatorLastName = c.Creator.LastName,
                                DateCreated = c.DateCreated,
                                DateModified = c.DateModified,
                                TicketCommentAttachmentModels = c.TicketCommentAttachments.Where(x => x.IsActive).Select(x => new TicketCommentAttachmentModel
                                {
                                    Id = x.Id,
                                    DateCreated = x.DateCreated,
                                    OriginalFileName = x.OriginalFileName,
                                    Path = $"/{AppConstants.TicketFilesFolder}/{s.CompanyId}/{c.CreatorId}/{x.FileName + "." + x.Extension}"
                                }).ToList()
                            }).ToList()
                    }).FirstOrDefaultAsync();

                if (entry == null)
                {
                    return new BaseQueryResponse<ITicketModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                if (User.Identity.IsAuthenticated)
                {
                    var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                    if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && entry.ReporterId != userId)
                    {
                        return new BaseQueryResponse<ITicketModel>
                        {
                            IsSuccess = false,
                            Code = ResponseCodes.Unauthorized
                        };
                    }
                }
                else if (entry.Token != request?.Token)
                {
                    return new BaseQueryResponse<ITicketModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                return new BaseQueryResponse<ITicketModel>
                {
                    Data = entry,
                    IsSuccess = true,
                    Code = ResponseCodes.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<ITicketModel>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.UserPolicy)]
        [HttpPut("{id}/close")]
        public async Task<IBaseCommandResponse<bool>> Close(int id)
        {
            IBaseCommandResponse<bool> response = default;
            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                var entry = await _dbContext.Tickets.FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);

                if (entry == null || entry.ReporterId != userId)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                entry.IsClosed = true;
                var closeStatus = await _dbContext.TicketStatuses.FirstOrDefaultAsync(s => s.CompanyId == companyId && s.IsActive && s.IsCompletion);
                if (closeStatus != null)
                {
                    entry.TicketStatusId = closeStatus.Id;
                }
                
                await _dbContext.SaveChangesAsync();
                _cache.ClearTicketsReport(_logger, companyId, userId);
                _cache.ClearTicketsReport(_logger, companyId, null);

                return new BaseCommandResponse<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    Code = ResponseCodes.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<bool>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.UserPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<bool>> Create([FromForm] TicketCreateCommandRequest request, [FromServices] KafkaDependentProducer<Null, string> producer)
        {
            if (request == null)
            {
                return new BaseCommandResponse<bool>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<bool> response = default;

            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                var user = System.Text.Json.JsonSerializer.Deserialize<UserModel>(User.Claims.First(s => s.Type == AuthConstants.UserDataClaim).Value);
                var entry = new Ticket
                {
                    UniqueId = Guid.NewGuid(),
                    CompanyId = companyId,
                    Name = HttpUtility.HtmlAttributeEncode(request.Name),
                    IsClosed = false,
                    DateCreated = DateTimeOffset.UtcNow,
                    DateModified = DateTimeOffset.UtcNow,
                    ReporterId = userId,
                    ReporterEmail = user.Email,
                    TicketQueueId = 0,
                    TicketStatusId = 0,
                    TicketCategoryId = request.TicketCategoryId,
                    TicketObservations = new List<TicketObservation>
                    {
                        new TicketObservation
                        {
                            DateCreated = DateTimeOffset.UtcNow,
                            UserId = userId
                        }
                    }
                };
                if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
                {
                    var ticketComment = new TicketComment
                    {
                        UniqueId = Guid.NewGuid(),
                        Text = HttpUtility.HtmlAttributeEncode(request.Text),
                        IsInternal = false,
                        IsActive = true,
                        DateCreated = DateTimeOffset.UtcNow,
                        DateModified = DateTimeOffset.UtcNow,
                        CreatorId = userId,
                    };
                    entry.TicketComments.Add(ticketComment);

                    if (request.Files?.Any() == true)
                    {
                        foreach (var file in request.Files)
                        {
                            var ext = Path.GetExtension(file.FileName).Trim('.').ToLower();
                            var ticketCommentAttachment = new TicketCommentAttachment
                            {
                                DateCreated = DateTimeOffset.UtcNow,
                                DateModified = DateTimeOffset.UtcNow,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var fileResult = await _s3Service.PutObjectAsync($"{AppConstants.TicketFilesFolder}/{companyId}/{entry.ReporterId}/{ticketCommentAttachment.FileName}.{ticketCommentAttachment.Extension}",
                                file.OpenReadStream());
                            if (!fileResult)
                            {
                                return new BaseCommandResponse<bool>
                                {
                                    IsSuccess = false,
                                    Code = ResponseCodes.Error,
                                    ErrorMessage = $"Ошибка при сохранении вложенного файла {file.FileName}."
                                };
                            }
                        }
                    }
                }

                var message = System.Text.Json.JsonSerializer.Serialize(new KafkaModels.Message
                {
                    CompanyId = companyId,
                    UserId = userId,
                    Type = KafkaModels.MessageType.NewTicket,
                    Ticket = entry
                });
                await producer.ProduceAsync(KafkaModels._ticketsTopic, new Message<Null, string> { Value = message });

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<bool>();
                response.HandleException(ex);
            }
            return response;
        }

        [Authorize(Policy = AuthConstants.EmployeePolicy)]
        [HttpPut("{id}")]
        public async Task<IBaseCommandResponse<ITicketModel>> Update([FromRoute] int id, [FromBody] TicketUpdateCommandRequest request)
        {
            if (request == null)
            {
                return new BaseCommandResponse<ITicketModel>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }
            IBaseCommandResponse<ITicketModel> response = default;
            try
            {
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var entry = await _dbContext.Tickets.FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == companyId);

                if (entry == null)
                {
                    return new BaseCommandResponse<ITicketModel>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                switch (request.UpdatedField)
                {
                    case UpdatedTicketField.Assignee:
                        var assignee = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value &&  s.CompanyId == companyId);
                        if (assignee != null)
                        {
                            entry.AssigneeId = assignee.Id;
                            entry.AssigneeEmail = assignee.Email;
                        }
                        break;
                    case UpdatedTicketField.Priority:
                        var priority = await _dbContext.TicketPriorities.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value && s.CompanyId == companyId);
                        if (priority != null)
                        {
                            entry.TicketPriorityId = priority.Id;
                        }
                        break;
                    case UpdatedTicketField.Status:
                        var status = await _dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value && s.CompanyId == companyId);
                        if (status != null)
                        {
                            entry.TicketStatusId = status.Id;
                            entry.IsClosed = status.IsCompletion;
                        }
                        break;
                    case UpdatedTicketField.Queue:
                        var queue = await _dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value && s.CompanyId == companyId);
                        if (queue != null)
                        {
                            entry.TicketQueueId = queue.Id;
                        }
                        break;
                    case UpdatedTicketField.Category:
                        var category = await _dbContext.TicketCategories.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value && s.CompanyId == companyId);
                        if (category != null)
                        {
                            entry.TicketCategoryId = category.Id;
                        }
                        break;
                    case UpdatedTicketField.Resolution:
                        var resolution = await _dbContext.TicketResolutions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.Value && s.CompanyId == companyId);
                        if (resolution != null)
                        {
                            entry.TicketResolutionId = resolution.Id;
                        }
                        break;
                }

                entry.DateModified = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();
                _cache.ClearTicketsReport(_logger, companyId, null);
                _cache.ClearTicketsReport(_logger, companyId, entry.ReporterId);

                var result = await Get(entry.Id, new TicketQueryRequest { IsUserView = false });

                return new BaseCommandResponse<ITicketModel>
                {
                    Data = result.Data,
                    IsSuccess = result.IsSuccess,
                    Code = result.Code,
                    ErrorMessage = result.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<ITicketModel>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize]
        [HttpPost("{id}/comments")]
        public async Task<IBaseCommandResponse<int>> CreateComment(int id, [FromForm] TicketCommentCreateCommandRequest request,
            [FromServices] KafkaDependentProducer<Null, string> producer)
        {
            if (request == null)
            {
                return new BaseCommandResponse<int>
                {
                    IsSuccess = false,
                    Code = ResponseCodes.BadRequest
                };
            }

            IBaseCommandResponse<int> response = default;

            try
            {
                if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !User.Claims.Any(s => s.Type == AuthConstants.UserClaim))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
                {
                    var ticketComment = new TicketComment
                    {
                        UniqueId = Guid.NewGuid(),
                        TicketId = id,
                        Text = !string.IsNullOrWhiteSpace(request.Text) ? HttpUtility.HtmlAttributeEncode(request.Text) : null,
                        IsInternal = request.IsInternal,
                        IsActive = true,
                        DateCreated = DateTimeOffset.UtcNow,
                        DateModified = DateTimeOffset.UtcNow,
                        CreatorId = userId,
                    };

                    if (request.Files?.Any() == true)
                    {
                        foreach (var file in request.Files)
                        {
                            var ext = Path.GetExtension(file.FileName).Trim('.').ToLower();
                            var ticketCommentAttachment = new TicketCommentAttachment
                            {
                                DateCreated = DateTimeOffset.UtcNow,
                                DateModified = DateTimeOffset.UtcNow,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var fileResult = await _s3Service.PutObjectAsync($"{AppConstants.TicketFilesTempFolder}/{companyId}/{ticketCommentAttachment.FileName}.{ticketCommentAttachment.Extension}",
                                file.OpenReadStream());
                            
                            if (!fileResult)
                            {
                                return new BaseCommandResponse<int>
                                {
                                    IsSuccess = false,
                                    Code = ResponseCodes.Error,
                                    ErrorMessage = $"Ошибка при сохранении вложенного файла {file.FileName}."
                                };
                            }
                        }
                    }

                    var message = System.Text.Json.JsonSerializer.Serialize(new KafkaModels.Message
                    {
                        CompanyId = companyId,
                        UserId = userId,
                        Type = User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) ? KafkaModels.MessageType.NewCommentFromEmployee : KafkaModels.MessageType.NewCommentFromUser,
                        Comment = ticketComment
                    });
                    await producer.ProduceAsync(KafkaModels._ticketsTopic, new Message<Null, string> { Value = message });

                    response = new BaseCommandResponse<int>
                    {
                        IsSuccess = true,
                        Code = ResponseCodes.Success,
                        Data = ticketComment.Id
                    };
                }
                else
                {
                    response = new BaseCommandResponse<int>
                    {
                        IsSuccess = true,
                        Code = ResponseCodes.Success,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<int>();
                response.HandleException(ex);
            }
            return response;
        }
    }
}