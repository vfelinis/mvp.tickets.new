using Microsoft.AspNetCore.Mvc;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Services;

namespace mvp.tickets.web.Controllers
{
    public class FileController : Controller
    {
        private readonly IS3Service _service;

        public FileController(IS3Service service)
        {
            _service = service;
        }

        [HttpGet($"{AppConstants.TicketFilesFolder}/{{companyId}}/{{creatorId}}/{{fileName}}")]
        [ResponseCache(Duration = 604800)]
        public async Task<IActionResult> Ticket([FromRoute] string fileName)
        {
            var file = await _service.GetObjectStreamAsync(Request.Path.Value.TrimStart('/'));
            if (file == null)
            {
                return NotFound();
            }
            return File(file, "application/octet-stream", fileName);
        }

        [HttpGet($"{AppConstants.LogoFilesFolder}/{{fileName}}")]
        [ResponseCache(Duration = 604800)]
        public async Task<IActionResult> Logo([FromRoute] string fileName)
        {
            var file = await _service.GetObjectStreamAsync(Request.Path.Value.TrimStart('/'));
            if (file == null)
            {
                return NotFound();
            }
            return File(file, "application/octet-stream", fileName);
        }
    }
}
