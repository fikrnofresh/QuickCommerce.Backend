using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/settlements")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class AdminSettlementController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminSettlementController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettlements()
        {
            var result = await _dashboardService.GetStoreSettlementsAsync();
            return Ok(result);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSettlement()
        {
            await _dashboardService.GenerateDailySettlementAsync();
            return Ok("Settlement generated successfully");
        }
    }
}