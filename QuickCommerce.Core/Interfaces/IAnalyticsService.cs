using QuickCommerce.Core.DTOs.Admin;
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAnalyticsService
    {
        // Dashboard
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        // Sales
        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            string? compare,
            string? groupBy);

        // Product
        Task<ProductAnalyticsDto> GetProductAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? top,
            string? type,
            string? sortBy);

        // Inventory
        Task<InventoryAnalyticsDto> GetInventoryAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? lowStockThreshold);

        // Alerts
        Task<AlertAnalyticsDto> GetAlertAnalyticsAsync();
         
        // Customer Intelligence
        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync();
    }
}