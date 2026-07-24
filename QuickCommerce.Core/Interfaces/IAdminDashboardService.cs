using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCommerce.Core.DTOs.Analytics;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAdminDashboardService
    {
        // =========================
        // MAIN DASHBOARD
        // =========================
        Task<AdminDashboardDto> GetDashboardAsync();

        // =========================
        // CHART INTELLIGENCE
        // =========================

        // Revenue + Orders trend (30 days)
        Task<List<DashboardChartDto>> GetRevenueChartAsync();

        // Category revenue distribution (pie chart)
        Task<List<CategoryRevenueDto>> GetCategoryRevenueChartAsync();

        // Store performance leaderboard
        Task<List<TopStoreDto>> GetStoreLeaderboardAsync();
        Task<List<DemandForecastDto>> GetDemandForecastAsync();

        Task<List<StorePerformanceDto>> GetStorePerformanceAsync();

        Task<List<CategoryCommissionAnalyticsDto>> GetCategoryCommissionAnalyticsAsync();
        Task<LiveOperationsDto> GetLiveOperationsAsync();
        Task<List<OrderQueueDto>> GetOrderQueueAsync(string status);
        Task<List<StoreSettlementDto>> GetStoreSettlementsAsync();
        Task GenerateDailySettlementAsync();
        Task<FinancialDashboardDto> GetFinancialDashboardAsync();
    }
}