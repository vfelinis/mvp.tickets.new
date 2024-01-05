using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using Telegram.Bot;

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

                using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
                {
                    DynamicParameters parameter = new DynamicParameters();
                    if (!User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim))
                    {
                        var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
                        parameter.Add(TicketsReportProcedure.Params.SearchByReporterId, userId, DbType.Int32);
                    }
                    if (request.SearchBy?.Any() == true)
                    {
                        foreach (var search in request.SearchBy.Where(s => !string.IsNullOrWhiteSpace($"{s.Value}")))
                        {
                            if (string.Equals(search.Key, nameof(Ticket.Id), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchById, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.IsClosed), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByIsClosed, Convert.ToBoolean(search.Value), DbType.Boolean);
                            }
                            if (User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && string.Equals(search.Key, nameof(Ticket.ReporterId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByReporterId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketPriorityId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByTicketPriorityId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketQueueId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByTicketQueueId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketResolutionId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByTicketResolutionId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketStatusId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByTicketStatusId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.TicketCategoryId), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchByTicketCategoryId, Convert.ToInt32(search.Value), DbType.Int32);
                            }
                            if (string.Equals(search.Key, nameof(Ticket.Source), StringComparison.OrdinalIgnoreCase))
                            {
                                parameter.Add(TicketsReportProcedure.Params.SearchBySource, (int)Enum.Parse<TicketSource>(search.Value), DbType.Int32);
                            }
                        }
                    }

                    parameter.Add(TicketsReportProcedure.Params.SortBy, request.SortBy, DbType.String);
                    parameter.Add(TicketsReportProcedure.Params.SortDirection, request.SortDirection.ToString(), DbType.String);
                    parameter.Add(TicketsReportProcedure.Params.Offset, request.Offset, DbType.Int32);
                    parameter.Add(TicketsReportProcedure.Params.Limit, ReportConstants.DEFAULT_LIMIT, DbType.Int32);

                    var query = await connection.QueryAsync<TicketReportModel>(TicketsReportProcedure.Name, param: parameter,
                        commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                    var entries = query.ToList();
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

                using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
                {
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(TicketsReportProcedure.Params.SearchById, id, DbType.Int32);
                    parameter.Add(TicketsReportProcedure.Params.SortBy, nameof(Ticket.Id), DbType.String);
                    parameter.Add(TicketsReportProcedure.Params.SortDirection, SortDirection.ASC.ToString(), DbType.String);
                    parameter.Add(TicketsReportProcedure.Params.Offset, 0, DbType.Int32);
                    parameter.Add(TicketsReportProcedure.Params.Limit, 1, DbType.Int32);

                    var entry = await connection.QueryFirstOrDefaultAsync<TicketReportModel>(TicketsReportProcedure.Name, param: parameter,
                        commandType: CommandType.StoredProcedure).ConfigureAwait(false);

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
                        var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
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

                    var includeIternal = User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim) && !request.IsUserView;
                    entry.TicketComments = await _dbContext.TicketComments
                        .Where(s => s.TicketId == entry.Id && s.IsActive && (includeIternal || !s.IsInternal))
                        .Select(s => new TicketCommentModel
                        {
                            Id = s.Id,
                            Text = s.Text,
                            IsInternal = s.IsInternal,
                            CreatorId = s.CreatorId,
                            CreatorEmail = s.Creator.Email,
                            CreatorFirstName = s.Creator.FirstName,
                            CreatorLastName = s.Creator.LastName,
                            DateCreated = s.DateCreated,
                            DateModified = s.DateModified,
                            TicketCommentAttachmentModels = s.TicketCommentAttachments.Where(x => x.IsActive).Select(x => new TicketCommentAttachmentModel
                            {
                                Id = x.Id,
                                DateCreated = x.DateCreated,
                                OriginalFileName = x.OriginalFileName,
                                Path = $"/{TicketConstants.AttachmentFolder}/{s.CreatorId}/{x.FileName + "." + x.Extension}"
                            }).ToList()
                        })
                        .ToListAsync();

                    return new BaseQueryResponse<ITicketModel>
                    {
                        Data = entry,
                        IsSuccess = true,
                        Code = ResponseCodes.Success
                    };
                }
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
                var defaultQueue = await _dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                if (defaultQueue == null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "¬ системе отсутствует первична€ очередь за€вок."
                    };
                }

                var defaultStatus = await _dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                if (defaultStatus == null)
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = "¬ системе отсутствует первичный статус за€вок."
                    };
                }

                var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
                var entry = new Ticket
                {
                    Name = HttpUtility.HtmlAttributeEncode(request.Name),
                    IsClosed = false,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
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
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now,
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
                                DateCreated = DateTimeOffset.Now,
                                DateModified = DateTimeOffset.Now,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var path = Path.Join(_environment.WebRootPath, $"/{TicketConstants.AttachmentFolder}/{entry.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
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

        [HttpPost("telegram")]
        public async Task<IActionResult> CreateByTelegram([FromBody] TicketCreateByTelegramCommandRequest request)
        {
            if (request == null || request.ApiKey != _settings.ApiKey)
            {
                return BadRequest();
            }

            try
            {
                var defaultQueue = await _dbContext.TicketQueues.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                if (defaultQueue == null)
                {
                    return BadRequest();
                }

                var defaultStatus = await _dbContext.TicketStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                if (defaultStatus == null)
                {
                    return BadRequest();
                }

                var defaultCategory = await _dbContext.TicketCategories.AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault);
                if (defaultCategory == null)
                {
                    return BadRequest();
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Phone == request.Phone);
                if (user == null)
                {
                    user = new User
                    {
                        Phone = request.Phone,
                        Email = null,
                        FirstName = request.FirstName ?? request.Phone,
                        LastName = request.LastName ?? "",
                        Permissions = domain.Enums.Permissions.User,
                        IsLocked = false,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now
                    };
                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();

                    try
                    {
                        var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_settings.FirebaseAdminConfig);
                        await firebaseAuth.CreateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs
                        {
                            PhoneNumber = request.Phone,
                            DisplayName = request.Phone,
                            Password = Guid.NewGuid().ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                if (user.IsLocked)
                {
                    return BadRequest();
                }

                var entry = new Ticket
                {
                    Name = HttpUtility.HtmlAttributeEncode(request.Name),
                    Token = Guid.NewGuid().ToString(),
                    Source = TicketSource.Telegram,
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
                if (!string.IsNullOrWhiteSpace(request.Text) || request.Files?.Any() == true)
                {
                    var ticketComment = new TicketComment
                    {
                        Ticket = entry,
                        Text = HttpUtility.HtmlAttributeEncode(request.Text),
                        IsInternal = false,
                        IsActive = true,
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now,
                        CreatorId = user.Id,
                    };
                    entry.TicketComments.Add(ticketComment);

                    if (request.Files?.Any() == true)
                    {
                        var botClient = new TelegramBotClient(_settings.TelegramToken);
                        foreach (var file in request.Files)
                        {
                            using var fileStream = new MemoryStream();
                            var fileInfo = await botClient.GetInfoAndDownloadFileAsync(
                                fileId: file,
                                destination: fileStream,
                                cancellationToken: CancellationToken.None);

                            var fileName = Path.GetFileName(fileInfo.FilePath);
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

                            var path = Path.Join(_environment.WebRootPath, $"/{TicketConstants.AttachmentFolder}/{entry.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            using (var stream = System.IO.File.Create(path))
                            {
                                fileStream.Seek(0, SeekOrigin.Begin);
                                await fileStream.CopyToAsync(stream);
                            }
                        }
                    }
                }

                await _dbContext.Tickets.AddAsync(entry).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                var link = $"https://{_settings.Host}/tickets/{entry.Id}/alt/?token={entry.Token}";
                return Ok(new { link });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("telegram/list")]
        public async Task<IActionResult> ListByTelegram([FromBody] TicketTelegramQueryRequest request)
        {
            if (request == null || request.ApiKey != _settings.ApiKey)
            {
                return BadRequest();
            }

            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Phone == request.Phone);
                if (user == null || user.IsLocked)
                {
                    return BadRequest();
                }

                var tickets = await _dbContext.Tickets.AsNoTracking().Where(s => s.ReporterId == user.Id && s.Source == TicketSource.Telegram)
                    .OrderByDescending(s => s.DateCreated).Take(10).ToListAsync();

                return Ok(new { data = tickets.Select(s => new { name = s.Name, dateCreated = s.DateCreated, link = $"https://{_settings.Host}/tickets/{s.Id}/alt/?token={s.Token}" }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

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

                var ticket = await _dbContext.Tickets.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
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
                    userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
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
                        DateCreated = DateTimeOffset.Now,
                        DateModified = DateTimeOffset.Now,
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
                                DateCreated = DateTimeOffset.Now,
                                DateModified = DateTimeOffset.Now,
                                IsActive = true,
                                OriginalFileName = file.FileName,
                                Extension = ext,
                                FileName = Guid.NewGuid().ToString()
                            };
                            ticketComment.TicketCommentAttachments.Add(ticketCommentAttachment);

                            var path = Path.Join(_environment.WebRootPath, $"/{TicketConstants.AttachmentFolder}/{ticket.ReporterId}/{ticketCommentAttachment.FileName}.{ext}");
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

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPut]
        public async Task<IBaseCommandResponse<bool>> Update([FromBody] QueueUpdateCommandRequest request)
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
                if (await _dbContext.TicketQueues.AnyAsync(s => s.Name == request.Name && s.Id != request.Id).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"«апись с названием {request.Name} уже существует.",
                        Data = false
                    };
                }

                if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault && s.Id != request.Id).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"ѕервична€ очередь уже существует.",
                        Data = false
                    };
                }

                var entry = await _dbContext.TicketQueues.FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
                if (entry == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound,
                        Data = false
                    };
                }

                entry.Name = request.Name;
                entry.IsDefault = request.IsDefault;
                entry.IsActive = request.IsActive;
                entry.DateModified = DateTimeOffset.Now;

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
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
    }
}