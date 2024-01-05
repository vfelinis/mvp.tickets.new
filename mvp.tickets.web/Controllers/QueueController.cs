using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Models;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/queues")]
    public class QueueController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<QueueController> _logger;

        public QueueController(ApplicationDbContext dbContext, ILogger<QueueController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<IQueueModel>>> Query([FromQuery] QueueQueryRequest request)
        {
            IBaseQueryResponse<IEnumerable<IQueueModel>> response = default;
            try
            {
                var queryable = _dbContext.TicketQueues.AsNoTracking().AsQueryable();
                if (request?.Id > 0)
                {
                    queryable = queryable.Where(x => x.Id == request.Id);
                }
                if (request?.OnlyActive == true)
                {
                    queryable = queryable.Where(x => x.IsActive == true);
                }
                var entries = await queryable.Select(s => new QueueModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsDefault = s.IsDefault,
                    IsActive = s.IsActive,
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified
                }).ToListAsync();

                response = new BaseQueryResponse<IEnumerable<IQueueModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<IQueueModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromBody] QueueCreateCommandRequest request)
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
                if (await _dbContext.TicketQueues.AnyAsync(s => s.Name == request.Name).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Запись с названием {request.Name} уже существует."
                    };
                }

                if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Первичная очередь уже существует."
                    };
                }

                var entry = new TicketQueue
                {
                    Name = request.Name,
                    IsActive = request.IsActive,
                    IsDefault = request.IsDefault,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
                };
                await _dbContext.TicketQueues.AddAsync(entry).ConfigureAwait(false);
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
                        ErrorMessage = $"Запись с названием {request.Name} уже существует.",
                        Data = false
                    };
                }

                if (request.IsDefault && await _dbContext.TicketQueues.AnyAsync(s => s.IsDefault && s.Id != request.Id).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Первичная очередь уже существует.",
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