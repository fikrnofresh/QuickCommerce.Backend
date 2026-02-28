using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [Route("api/v1/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AdminDashboardController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _analyticsService.GetDashboardSummaryAsync();
            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}