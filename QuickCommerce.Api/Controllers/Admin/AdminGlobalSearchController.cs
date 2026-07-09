using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/search")]
    public class AdminGlobalSearchController : ControllerBase
    {
        private readonly IGlobalSearchService _searchService;

        public AdminGlobalSearchController(IGlobalSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var result = await _searchService.SearchAsync(q);
            return Ok(result);
        }
    }
}