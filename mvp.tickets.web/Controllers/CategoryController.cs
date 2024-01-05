using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Models;
using mvp.tickets.domain.Services;

namespace mvp.tickets.web.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<IBaseQueryResponse<IEnumerable<ICategoryModel>>> Query([FromQuery] CategoryQueryRequest request)
        {
            return await _service.Query(request);
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPost]
        public async Task<IBaseCommandResponse<int>> Create([FromBody] CategoryCreateCommandRequest request)
        {
            return await _service.Create(request);
        }

        [Authorize(Policy = AuthConstants.AdminPolicy)]
        [HttpPut]
        public async Task<IBaseCommandResponse<bool>> Update([FromBody] CategoryUpdateCommandRequest request)
        {
            return await _service.Update(request);
        }
    }
}