using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [Route("api/v1/admin/analytics")]
    [ApiController]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AdminAnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesAnalytics(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? compare,
            [FromQuery] string? groupBy)
        {
            var result = await _analyticsService.GetSalesAnalyticsAsync(
                from,
                to,
                compare,
                groupBy);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProductAnalytics(
           [FromQuery] DateTime? from,
           [FromQuery] DateTime? to,
           [FromQuery] int? top,
           [FromQuery] string? type,
           [FromQuery] string? sortBy)
        {
            var result = await _analyticsService.GetProductAnalyticsAsync(
                from,
                to,
                top,
                type,
                sortBy);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryAnalytics(
           [FromQuery] DateTime? from,
           [FromQuery] DateTime? to,
           [FromQuery] int? lowStockThreshold)
        {
            var result = await _analyticsService.GetInventoryAnalyticsAsync(
                from,
                to,
                lowStockThreshold);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            var result = await _analyticsService.GetAlertAnalyticsAsync();

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomerAnalytics()
        {
            var result = await _analyticsService.GetCustomerAnalyticsAsync();

            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}