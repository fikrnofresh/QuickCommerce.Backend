using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Analytics;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/dashboard")]
    [Authorize(Policy = "DASHBOARD.VIEW")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(
            IAdminDashboardService dashboardService,
            ApplicationDbContext context)
        {
            _dashboardService = dashboardService;
            _context = context;
        }

        // =========================
        // MAIN DASHBOARD
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            return Ok(dashboard);
        }

        // =========================
        // REVENUE CHART
        // =========================
        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart()
        {
            var result = await _dashboardService.GetRevenueChartAsync();
            return Ok(result);
        }

        // =========================
        // CATEGORY REVENUE
        // =========================
        [HttpGet("category-revenue")]
        public async Task<IActionResult> GetCategoryRevenue()
        {
            var result = await _dashboardService.GetCategoryRevenueChartAsync();
            return Ok(result);
        }

        // =========================
        // STORE LEADERBOARD
        // =========================
        [HttpGet("store-leaderboard")]
        public async Task<IActionResult> GetStoreLeaderboard()
        {
            var result = await _dashboardService.GetStoreLeaderboardAsync();
            return Ok(result);
        }

        // =========================
        // DEMAND FORECAST
        // =========================
        [HttpGet("demand-forecast")]
        public async Task<IActionResult> GetDemandForecast()
        {
            var result = await _dashboardService.GetDemandForecastAsync();
            return Ok(result);
        }

        // =========================
        // STORE PERFORMANCE
        // =========================
        [HttpGet("store-performance")]
        public async Task<IActionResult> GetStorePerformance()
        {
            var result = await _dashboardService.GetStorePerformanceAsync();
            return Ok(result);
        }

        // =========================
        // CATEGORY COMMISSION
        // =========================
        [HttpGet("category-commission")]
        public async Task<IActionResult> GetCategoryCommission()
        {
            var result = await _dashboardService.GetCategoryCommissionAnalyticsAsync();
            return Ok(result);
        }

        // =========================
        // LIVE OPERATIONS
        // =========================
        [HttpGet("live-operations")]
        public async Task<IActionResult> GetLiveOperations()
        {
            var result = await _dashboardService.GetLiveOperationsAsync();
            return Ok(result);
        }

        // =========================
        // ORDER QUEUE
        // =========================
        [HttpGet("order-queue")]
        public async Task<IActionResult> GetOrderQueue([FromQuery] string status)
        {
            var result = await _dashboardService.GetOrderQueueAsync(status);
            return Ok(result);
        }

        // =========================
        // FINANCIAL DASHBOARD
        // =========================
        [HttpGet("financial")]
        public async Task<IActionResult> GetFinancialDashboard()
        {
            var result = await _dashboardService.GetFinancialDashboardAsync();
            return Ok(result);
        }

        // =========================
        // DELIVERY ANALYTICS (FIXED)
        // =========================
        [HttpGet("delivery-analytics")]
        public async Task<IActionResult> GetDeliveryAnalytics()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.ExternalAgent) // ✅ FIXED
                .ToListAsync();

            var total = deliveries.Count;

            var completed = deliveries
                .Where(d => d.Status == "DELIVERED")
                .ToList();

            var failed = deliveries
                .Count(d => d.Status == "FAILED");

            var avgTime = completed.Any()
                ? completed.Average(d =>
                    (d.DeliveredAt.Value - d.AssignedAt.Value).TotalMinutes)
                : 0;

            // ✅ FIXED GROUPING (External Partner)
            var topPartners = deliveries
                .Where(d =>
                    d.Status == "DELIVERED" &&
                    d.AgentType == DeliveryAgentType.External &&
                    d.AssignedToPartnerId.HasValue)
                .GroupBy(d => d.AssignedToPartnerId)
                .Select(g => new
                {
                    partnerId = g.Key,
                    totalDeliveries = g.Count()
                })
                .OrderByDescending(x => x.totalDeliveries)
                .Take(5)
                .ToList();

            return Ok(new
            {
                totalDeliveries = total,
                completedDeliveries = completed.Count,
                failedDeliveries = failed,
                successRate = total > 0 ? (completed.Count * 100.0 / total) : 0,
                avgDeliveryTimeMinutes = System.Math.Round(avgTime, 2),
                topPartners = topPartners
            });
        }
    }
}