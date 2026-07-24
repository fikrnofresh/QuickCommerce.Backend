using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Analytics;
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAnalyticsService
    {
        // =============================
        // DASHBOARD SUMMARY
        // =============================
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        // =============================
        // SALES ANALYTICS
        // =============================
        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            string? compare,
            string? groupBy);

        // =============================
        // PRODUCT ANALYTICS
        // =============================
        Task<ProductAnalyticsDto> GetProductAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? top,
            string? type,
            string? sortBy);

        // =============================
        // INVENTORY ANALYTICS
        // =============================
        Task<InventoryAnalyticsDto> GetInventoryAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? lowStockThreshold);

        // =============================
        // ALERT ANALYTICS
        // =============================
        Task<AlertAnalyticsDto> GetAlertAnalyticsAsync();

        // =============================
        // CUSTOMER INTELLIGENCE
        // =============================
        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync();
    }
}