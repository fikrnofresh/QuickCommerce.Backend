using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/analytics")]
    [Authorize(Policy = "ANALYTICS.VIEW")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // =====================================================
        // DASHBOARD SUMMARY
        // =====================================================

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _analyticsService.GetDashboardSummaryAsync();
            return Ok(result);
        }

        // =====================================================
        // SALES ANALYTICS
        // =====================================================

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

            return Ok(result);
        }

        // =====================================================
        // PRODUCT ANALYTICS
        // =====================================================

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

            return Ok(result);
        }

        // =====================================================
        // INVENTORY ANALYTICS
        // =====================================================

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

            return Ok(result);
        }

        // =====================================================
        // ALERT ANALYTICS
        // =====================================================

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            var result = await _analyticsService.GetAlertAnalyticsAsync();
            return Ok(result);
        }

        // =====================================================
        // CUSTOMER ANALYTICS
        // =====================================================

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomerAnalytics()
        {
            var result = await _analyticsService.GetCustomerAnalyticsAsync();
            return Ok(result);
        }
    }
}