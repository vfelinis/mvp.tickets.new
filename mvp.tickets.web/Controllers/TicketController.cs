using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;

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

        public TicketController(ApplicationDbContext dbContext, IConnectionStrings connectionStrings, ILogger<TicketController> logger, IWebHostEnvironment environment,
            ISettings settings)
        {
            _dbContext = dbContext;
            _connectionStrings = connectionStrings;
            _logger = logger;
            _environment = environment;
            _settings = settings;
        }

        [Authorize]
        [HttpPost("report")]
        public async Task<IBaseReportQueryResponse<IEnumerable<ITicketModel>>> Report(BaseReportQueryRequest request)
        {
            IBaseReportQueryResponse<IEnumerable<ITicketModel>> response = default;
            try
            {
                if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !User.Claims.Any(s => s.Type == AuthConstants.UserClaim))
                {
                    return new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                using (var connection = new NpgsqlConnection(_connectionStrings.DefaultConnection))
                {
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add("@companyId", companyId, DbType.Int32);
                    var query =
$@"
SELECT
    COUNT(*) OVER() AS ""{nameof(TicketReportModel.Total)}""
    ,t.""{nameof(Ticket.Id)}"" AS ""{nameof(TicketReportModel.Id)}""
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
FROM dbo.""{TicketExtension.TableName}"" t
INNER JOIN dbo.""{UserExtension.TableName}"" ru ON t.""{nameof(Ticket.ReporterId)}"" = ru.""{nameof(data.Models.User.Id)}""
INNER JOIN dbo.""{TicketQueueExtension.TableName}"" tq ON t.""{nameof(Ticket.TicketQueueId)}"" = tq.""{nameof(TicketQueue.Id)}""
INNER JOIN dbo.""{TicketStatusExtension.TableName}"" ts ON t.""{nameof(Ticket.TicketStatusId)}"" = ts.""{nameof(TicketStatus.Id)}""
INNER JOIN dbo.""{TicketCategoryExtension.TableName}"" tc ON t.""{nameof(Ticket.TicketCategoryId)}"" = tc.""{nameof(TicketCategory.Id)}""
LEFT JOIN dbo.""{UserExtension.TableName}"" au ON t.""{nameof(Ticket.AssigneeId)}"" = au.""{nameof(data.Models.User.Id)}""
LEFT JOIN dbo.""{TicketPriorityExtension.TableName}"" tp ON t.""{nameof(Ticket.TicketPriorityId)}"" = tp.""{nameof(TicketPriority.Id)}""
LEFT JOIN dbo.""{TicketResolutionExtension.TableName}"" tr ON t.""{nameof(Ticket.TicketResolutionId)}"" = tr.""{nameof(TicketResolution.Id)}""
WHERE t.""{nameof(Ticket.CompanyId)}"" = @companyId
";
                    if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim))
                    {
                        var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
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
                            if (User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && string.Equals(search.Key, nameof(Ticket.ReporterId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add("@searchByReporterId", Convert.ToInt32(search.Value), DbType.Int32);
                                query += $@" AND t.""{nameof(Ticket.ReporterId)}"" = @searchByReporterId";
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
                                query += $@" AND t.""{nameof(Ticket.Source)}"" = @searchByTicketCategoryId";
                            }
                        }
                    }

                    parameter.Add("@offset", request.Offset, DbType.Int32);
                    parameter.Add("@limit", ReportConstants.DEFAULT_LIMIT, DbType.Int32);
                    query +=
$@"
ORDER BY ""{typeof(Ticket).GetProperties().FirstOrDefault(s => s.Name == request.SortBy)?.Name ?? nameof(Ticket.Id)}"" {request.SortDirection} OFFSET @offset LIMIT @limit";

                    var result = await connection.QueryAsync<TicketReportModel>(query, param: parameter);

                    var entries = result.ToList();
                    return new BaseReportQueryResponse<IEnumerable<ITicketModel>>
                    {
                        Data = entries,
                        Total = entries.FirstOrDefault()?.Total ?? 0,
                        IsSuccess = true,
                        Code = ResponseCodes.Success
                    };
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
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromForm] TicketCreateCommandRequest request)
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
                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var defaultQueue = await _dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault && s.CompanyId == companyId);
                if (defaultQueue == null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "¬ системе отсутствует первична€ очередь за€вок."
                    };
                }

                var defaultStatus = await _dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault && s.CompanyId == companyId);
                if (defaultStatus == null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "¬ системе отсутствует первичный статус за€вок."
                    };
                }

                var userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                var entry = new Ticket
                {
                    CompanyId = companyId,
                    Name = HttpUtility.HtmlAttributeEncode(request.Name),
                    IsClosed = false,
                    DateCreated = DateTimeOffset.UtcNow,
                    DateModified = DateTimeOffset.UtcNow,
                    ReporterId = userId,
                    TicketQueueId = defaultQueue.Id,
                    TicketStatusId = defaultStatus.Id,
                    TicketCategoryId = request.TicketCategoryId,
                    TicketObservations = new List<TicketObservation>
                    {
                        new TicketObservation
                        {
                            DateCreated = DateTimeOffset.Now,
                            UserId = userId
                        }
                    }
                };
                if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
                {
                    var ticketComment = new TicketComment
                    {
                        Ticket = entry,
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
                                TicketComment = ticketComment,
                                DateCreated = DateTimeOffset.UtcNow,
                                DateModified = DateTimeOffset.UtcNow,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var path = Path.Join(_settings.FilesPath, $"/{AppConstants.TicketFilesFolder}/{companyId}/{entry.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            using (var stream = System.IO.File.Create(path))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                }

                await _dbContext.Tickets.AddAsync(entry).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                response = new BaseCommandResponse<int>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entry.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseCommandResponse<int>();
                response.HandleException(ex);
            }
            return response;
        }

        //[HttpPost("telegram")]
        //public async Task<IActionResult> CreateByTelegram([FromBody] TicketCreateByTelegramCommandRequest request)
        //{
        //    if (request == null || request.ApiKey != _settings.ApiKey)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        var defaultQueue = await _dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
        //        if (defaultQueue == null)
        //        {
        //            return BadRequest();
        //        }

        //        var defaultStatus = await _dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
        //        if (defaultStatus == null)
        //        {
        //            return BadRequest();
        //        }

        //        var defaultCategory = await _dbContext.TicketCategories.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
        //        if (defaultCategory == null)
        //        {
        //            return BadRequest();
        //        }

        //        var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Phone == request.Phone);
        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                Phone = request.Phone,
        //                Email = null,
        //                FirstName = request.FirstName ?? request.Phone,
        //                LastName = request.LastName ?? "",
        //                Permissions = domain.Enums.Permissions.User,
        //                IsLocked = false,
        //                DateCreated = DateTimeOffset.Now,
        //                DateModified = DateTimeOffset.Now
        //            };
        //            await _dbContext.Users.AddAsync(user);
        //            await _dbContext.SaveChangesAsync();

        //            try
        //            {
        //                var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
        //                await firebaseAuth.CreateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
        //                {
        //                    PhoneNumber = request.Phone,
        //                    DisplayName = request.Phone,
        //                    Password = Guid.NewGuid().ToString()
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, ex.Message);
        //            }
        //        }

        //        if (user.IsLocked)
        //        {
        //            return BadRequest();
        //        }

        //        var entry = new Ticket
        //        {
        //            Name = HttpUtility.HtmlAttributeEncode(request.Name),
        //            Token = Guid.NewGuid().ToString(),
        //            Source = TicketSource.Telegram,
        //            IsClosed = false,
        //            DateCreated = DateTimeOffset.Now,
        //            DateModified = DateTimeOffset.Now,
        //            ReporterId = user.Id,
        //            TicketQueueId = defaultQueue.Id,
        //            TicketStatusId = defaultStatus.Id,
        //            TicketCategoryId = defaultCategory.Id,
        //            TicketObservations = new List<TicketObservation>
        //            {
        //                new TicketObservation
        //                {
        //                    DateCreated = DateTimeOffset.Now,
        //                    UserId = user.Id
        //                }
        //            }
        //        };
        //        if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
        //        {
        //            var ticketComment = new TicketComment
        //            {
        //                Ticket = entry,
        //                Text = HttpUtility.HtmlAttributeEncode(request.Text),
        //                IsInternal = false,
        //                IsActive = true,
        //                DateCreated = DateTimeOffset.Now,
        //                DateModified = DateTimeOffset.Now,
        //                CreatorId = user.Id,
        //            };
        //            entry.TicketComments.Add(ticketComment);

        //            if (request.Files?.Any() == true)
        //            {
        //                var botClient = new TelegramBotClient(_settings.TelegramToken);
        //                foreach (var file in request.Files)
        //                {
        //                    using var fileStream = new MemoryStream();
        //                    var fileInfo = await botClient.GetInfoAndDownloadFileAsync(
        //                        fileId: file,
        //                        destination: fileStream,
        //                        cancellationToken: CancellationToken.None);

        //                    var fileName = Path.GetFileName(fileInfo.FilePath);
        //                    var ext = Path.GetExtension(fileName).Trim('.').ToLower();
        //                    var ticketCommentAttachment = new TicketCommentAttachment
        //                    {
        //                        TicketComment = ticketComment,
        //                        DateCreated = DateTimeOffset.Now,
        //                        DateModified = DateTimeOffset.Now,
        //                        IsActive = true,
        //                        OriginalFileName = fileName,
        //                        Extension = ext,
        //                        FileName = Guid.NewGuid().ToString()
        //                    };
        //                    ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

        //                    var path = Path.Join(_environment.WebRootPath, $"/{TicketConstants.AttachmentFolder}/{entry.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
        //                    Directory.CreateDirectory(Path.GetDirectoryName(path));
        //                    using (var stream = System.IO.File.Create(path))
        //                    {
        //                        fileStream.Seek(0, SeekOrigin.Begin);
        //                        await fileStream.CopyToAsync(stream);
        //                    }
        //                }
        //            }
        //        }

        //        await _dbContext.Tickets.AddAsync(entry).ConfigureAwait(false);
        //        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        //        var link = $"https://{_settings.Host}/tickets/{entry.Id}/alt/?token={entry.Token}";
        //        return Ok(new { link });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}

        //[HttpPost("telegram/list")]
        //public async Task<IActionResult> ListByTelegram([FromBody] TicketTelegramQueryRequest request)
        //{
        //    if (request == null || request.ApiKey != _settings.ApiKey)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Phone == request.Phone);
        //        if (user == null || user.IsLocked)
        //        {
        //            return BadRequest();
        //        }

        //        var tickets = await _dbContext.Tickets.AsNoTracking().Where(s => s.ReporterId == user.Id && s.Source == TicketSource.Telegram)
        //            .OrderByDescending(s => s.DateCreated).Take(10).ToListAsync();

        //        return Ok(new { data = tickets.Select(s => new { name = s.Name, dateCreated = s.DateCreated, link = $"https://{_settings.Host}/tickets/{s.Id}/alt/?token={s.Token}" }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}

        [HttpPost("{id}/comments")]
        public async Task<IBaseCommandResponse<int>> CreateComment(int id, [FromQuery] string token, [FromForm] TicketCommentCreateCommandRequest request)
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
                if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !User.Claims.Any(s => s.Type == AuthConstants.UserClaim) && string.IsNullOrWhiteSpace(token))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.Unauthorized
                    };
                }

                var companyId = int.Parse(User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
                var ticket = await _dbContext.Tickets.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == companyId);
                if (ticket == null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound
                    };
                }

                int userId = ticket.ReporterId;
                if (User.Identity.IsAuthenticated)
                {
                    userId = int.Parse(User.Claims.First(s => s.Type == System.Security.Claims.ClaimTypes.Sid).Value);
                    if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) || ticket.ReporterId != userId)
                    {
                        return new BaseCommandResponse<int>
                        {
                            IsSuccess = false,
                            Code = ResponseCodes.BadRequest
                        };
                    }
                }
                else if (string.IsNullOrWhiteSpace(token) || ticket.Token != token)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest
                    };
                }
                
                if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
                {
                    var ticketComment = new TicketComment
                    {
                        TicketId = ticket.Id,
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
                                TicketComment = ticketComment,
                                DateCreated = DateTimeOffset.UtcNow,
                                DateModified = DateTimeOffset.UtcNow,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var path = Path.Join(_settings.FilesPath, $"/{AppConstants.TicketFilesFolder}/{companyId}/{ticket.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            using (var stream = System.IO.File.Create(path))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                    await _dbContext.TicketComments.AddAsync(ticketComment).ConfigureAwait(false);
                    await _dbContext.SaveChangesAsync().ConfigureAwait(false);
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

        //[Authorize(Policy = AuthConstants.AdminPolicy)]
        //[HttpPut]
        //public async Task<IBaseCommandResponse<bool>> Update([FromBody] QueueUpdateCommandRequest request)
        //{
        //    if (request == null)
        //    {
        //        return new BaseCommandResponse<bool>
        //        {
        //            IsSuccess = false,
        //            Code = ResponseCodes.BadRequest
        //        };
        //    }

        //    IBaseCommandResponse<bool> response = default;

        //    try
        //    {
        //        if (await _dbContext.TicketQueues.AnyAsync(s => s.Name == request.Name && s.Id != request.Id).ConfigureAwait(false))
        //        {
        //            return new BaseCommandResponse<bool>
        //            {
        //                IsSuccess = false,
        //                Code = ResponseCodes.BadRequest,
        //                ErrorMessage = $"«апись с названием {request.Name} уже существует.",
        //                Data = false
        //            };
        //        }

        //        if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault && s.Id != request.Id).ConfigureAwait(false))
        //        {
        //            return new BaseCommandResponse<bool>
        //            {
        //                IsSuccess = false,
        //                Code = ResponseCodes.BadRequest,
        //                ErrorMessage = $"ѕервична€ очередь уже существует.",
        //                Data = false
        //            };
        //        }

        //        var entry = await _dbContext.TicketQueues.FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
        //        if (entry == null)
        //        {
        //            return new BaseCommandResponse<bool>
        //            {
        //                IsSuccess = false,
        //                Code = ResponseCodes.NotFound,
        //                Data = false
        //            };
        //        }

        //        entry.Name = request.Name;
        //        entry.IsDefault = request.IsDefault;
        //        entry.IsActive = request.IsActive;
        //        entry.DateModified = DateTimeOffset.Now;

        //        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        //        response = new BaseCommandResponse<bool>
        //        {
        //            IsSuccess = true,
        //            Code = ResponseCodes.Success,
        //            Data = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        response = new BaseCommandResponse<bool>();
        //        response.HandleException(ex);
        //    }
        //    return response;
        //}
    }
}