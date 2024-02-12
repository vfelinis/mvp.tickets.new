using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvp.tickets.data;
using mvp.tickets.data.Models;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Enums;
using mvp.tickets.domain.Extensions;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/invites")]
    public class InviteController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<InviteController> _logger;

        public InviteController(ApplicationDbContext dbContext, ILogger<InviteController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize(Policy = AuthConstants.RootSpacePolicy)]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<IInviteModel>>> Query()
        {
            IBaseQueryResponse<IEnumerable<IInviteModel>> response = default;
            try
            {
                var entries = await _dbContext.Invites.Select(s => new InviteModel
                {
                    Id = s.Id,
                    Email = s.Email,
                    Company = s.Company,
                    DateSent = s.DateSent,
                }).ToListAsync();

                response = new BaseQueryResponse<IEnumerable<IInviteModel>>
                {
                    IsSuccess = true,
                    Code = ResponseCodes.Success,
                    Data = entries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new BaseQueryResponse<IEnumerable<IInviteModel>>();
                response.HandleException(ex);
            }

            return response;
        }

        [Authorize(Policy = AuthConstants.RootSpacePolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromBody] InviteCreateCommandRequest request, [FromServices] IEmailService emailService)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Company))
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
                if (await _dbContext.Invites.AnyAsync(s => s.Email == request.Email.ToLower() && s.Company == request.Company))
                {
                    return new BaseCommandResponse<int>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.BadRequest,
                        ErrorMessage = $"Приглашение уже существует."
                    };
                }

                var entry = new Invite
                {
                    Email = request.Email.ToLower(),
                    Company = request.Company,
                    Code = Guid.NewGuid().ToString().Replace("-", ""),
                    DateSent = DateTimeOffset.UtcNow,
                };
                await _dbContext.Invites.AddAsync(entry).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                var rootCompany = await _dbContext.Companies.FirstOrDefaultAsync(s => s.IsRoot);

                await emailService.Send(entry.Email,
                    $"Приглашение в MVP Tickets",
                    $"Для регистрации предприятия перейдите по следующей ссылке <a href='https://{rootCompany?.Host}/companies/create/?email={entry.Email}&code={entry.Code}'>нажать здесь</a>",
                    isBodyHtml: true);

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

        [Authorize(Policy = AuthConstants.RootSpacePolicy)]
        [HttpDelete("{id}")]
        public async Task<IBaseCommandResponse<bool>> Delete([FromRoute] int id)
        {
            IBaseCommandResponse<bool> response = default;

            try
            {
                var entry = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == id);
                if (entry != null)
                {
                    _dbContext.Invites.Remove(entry);
                    await _dbContext.SaveChangesAsync();
                }

                response = new BaseCommandResponse<bool>
                {
                    IsSuccess = true,
                    Code= ResponseCodes.Success,
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

        [AllowAnonymous]
        [HttpPost("validation")]
        public async Task<IBaseCommandResponse<bool>> Validate([FromBody] InviteValidateCommandRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
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
                var entry = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Email == request.Email.ToLower() && x.Code == request.Code);
                if (entry == null)
                {
                    return new BaseCommandResponse<bool>
                    {
                        IsSuccess = false,
                        Code = ResponseCodes.NotFound,
                        ErrorMessage = "Приглашение не действительно."
                    };
                }
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