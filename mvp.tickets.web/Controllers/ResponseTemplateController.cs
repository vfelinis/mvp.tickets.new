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
    [Route("api/responseTemplates")]
    public class ResponseTemplateController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ResponseTemplateController> _logger;

        public ResponseTemplateController(ApplicationDbContext dbContext, ILogger<ResponseTemplateController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<IResponseTemplateModel>>> Query([FromQuery] ResponseTemplateQueryRequest request)
        {
            IBaseQueryResponse<IEnumerable<IResponseTemplateModel>> response = default;
            try
            {
                var queryable = _dbContext.TicketResponseTemplates.AsNoTracking().AsQueryable();
                if (request?.Id > 0)
                {
                    queryable = queryable.Where(x => x.Id == request.Id);
                }
                if (request?.OnlyActive == true)
                {
                    queryable = queryable.Where(x => x.IsActive == true);
                }
                var entries = await queryable.Select(s => new ResponseTemplateModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Text = s.Text,
                    IsActive = s.IsActive,
                    DateCreated = s.DateCreated,
                    DateModified = s.DateModified,
                    TicketResponseTemplateTypeId = s.TicketResponseTemplateTypeId,
                    TicketResponseTemplateType = s.TicketResponseTemplateType.Name
                }).ToListAsync();

                response = new BaseQueryResponse<IEnumerable<IResponseTemplateModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<IResponseTemplateModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromBody] ResponseTemplateCreateCommandRequest request)
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
                if (await _dbContext.TicketResponseTemplates.AnyAsync(s => s.TicketResponseTemplateTypeId == request.TicketResponseTemplateTypeId && s.Name == request.Name)
                    .ConfigureAwait(false))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Запись с названием {request.Name} уже существует для этого типа шаблона."
                    };
                }

                var entry = new TicketResponseTemplate
                {
                    Name = request.Name,
                    Text = request.Text,
                    IsActive = request.IsActive,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
                    TicketResponseTemplateTypeId = request.TicketResponseTemplateTypeId
                };
                await _dbContext.TicketResponseTemplates.AddAsync(entry).ConfigureAwait(false);
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
        public async Task<IBaseCommandResponse<bool>> Update([FromBody] ResponseTemplateUpdateCommandRequest request)
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
                if (await _dbContext.TicketResponseTemplates.AnyAsync(s => s.TicketResponseTemplateTypeId == request.TicketResponseTemplateTypeId && s.Name == request.Name
                    && s.Id != request.Id).ConfigureAwait(false))
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Запись с названием {request.Name} уже существует для этого типа шаблона.",
                        Data = false
                    };
                }

                var entry = await _dbContext.TicketResponseTemplates.FirstOrDefaultAsync(s => s.Id == request.Id).ConfigureAwait(false);
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
                entry.Text = request.Text;
                entry.IsActive = request.IsActive;
                entry.DateModified = DateTimeOffset.Now;
                entry.TicketResponseTemplateTypeId = request.TicketResponseTemplateTypeId;

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