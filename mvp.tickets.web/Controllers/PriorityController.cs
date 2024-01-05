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
    [Route("api/priorities")]
    public class PriorityController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PriorityController> _logger;

        public PriorityController(ApplicationDbContext dbContext, ILogger<PriorityController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<IPriorityModel>>> Query([FromQuery] PriorityQueryRequest request)
        {
            IBaseQueryResponse<IEnumerable<IPriorityModel>> response = default;
            try
            {
                var queryable = _dbContext.TicketPriorities.AsNoTracking().AsQueryable();
                if (request?.Id > 0)
                {
                    queryable = queryable.Where(x => x.Id == request.Id);
                }
                if (request?.OnlyActive == true)
                {
                    queryable = queryable.Where(x => x.IsActive == true);
                }
                var entries = await queryable.Select(s => new PriorityModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    Level = s.Level,
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified
                }).ToListAsync();

                response = new BaseQueryResponse<IEnumerable<IPriorityModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<IPriorityModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromBody] PriorityCreateCommandRequest request)
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
                if (await _dbContext.TicketPriorities.AnyAsync(s => s.Name == request.Name).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Запись с названием {request.Name} уже существует."
                    };
                }

                var entry = new TicketPriority
                {
                    Name = request.Name,
                    IsActive = request.IsActive,
                    Level = request.Level,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
                };
                await _dbContext.TicketPriorities.AddAsync(entry).ConfigureAwait(false);
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
        public async Task<IBaseCommandResponse<bool>> Update([FromBody] PriorityUpdateCommandRequest request)
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
                if (await _dbContext.TicketPriorities.AnyAsync(s => s.Name == request.Name && s.Id != request.Id).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Запись с названием {request.Name} уже существует.",
                        Data = false
                    };
                }

                var entry = await _dbContext.TicketPriorities.FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
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
                entry.IsActive = request.IsActive;
                entry.Level = request.Level;
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