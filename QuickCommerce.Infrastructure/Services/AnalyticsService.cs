using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // DASHBOARD SUMMARY
        // =========================================================
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var ordersTodayQuery = _context.Orders.AsNoTracking()
                .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow);

            var ordersToday = await ordersTodayQuery.CountAsync();

            var revenueToday = await ordersTodayQuery
                .Where(o =>
                    o.PaymentStatus == "PAID" ||
                    (o.PaymentMode == "COD" && o.Status == "DELIVERED"))
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var averageOrderValue = ordersToday > 0
                ? revenueToday / ordersToday
                : 0;

            var cancelledCount = await ordersTodayQuery
                .Where(o => o.Status == "CANCELLED")
                .CountAsync();

            var cancelRate = ordersToday > 0
                ? (double)cancelledCount / ordersToday * 100
                : 0;

            var pendingOrders = await _context.Orders.AsNoTracking()
                .Where(o =>
                    o.Status == "PENDING" ||
                    o.Status == "CONFIRMED" ||
                    o.Status == "PREPARING" ||
                    o.Status == "ASSIGNED" ||
                    o.Status == "OUT_FOR_DELIVERY")
                .CountAsync();

            var lowStockProducts = await _context.StoreProductInventories
                .AsNoTracking()
                .Where(i => i.Stock <= i.LowStockThreshold)
                .CountAsync();

            var activeCustomersToday = await ordersTodayQuery
                .Select(o => o.CustomerId)
                .Distinct()
                .CountAsync();

            return new DashboardSummaryDto
            {
                OrdersToday = ordersToday,
                RevenueToday = revenueToday,
                AverageOrderValueToday = averageOrderValue,
                CancelRateToday = cancelRate,
                PendingOrders = pendingOrders,
                LowStockProductsCount = lowStockProducts,
                ActiveCustomersToday = activeCustomersToday
            };
        }

        // =========================================================
        // SALES ANALYTICS
        // =========================================================
        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            string? compare,
            string? groupBy)
        {
            var today = DateTime.UtcNow.Date;

            var startDate = from ?? new DateTime(today.Year, today.Month, 1);
            var endDate = (to ?? today).AddDays(1);

            var baseQuery = _context.Orders.AsNoTracking()
                .Where(o =>
                    o.CreatedAt >= startDate &&
                    o.CreatedAt < endDate &&
                    (
                        o.PaymentStatus == "PAID" ||
                        (o.PaymentMode == "COD" && o.Status == "DELIVERED")
                    )
                );

            var totalRevenue = await baseQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
            var totalOrders = await baseQuery.CountAsync();
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var trend = await baseQuery
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesTrendDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var peakHours = await baseQuery
                .GroupBy(o => o.CreatedAt.Hour)
                .Select(g => new PeakHourDto
                {
                    Hour = g.Key,
                    Orders = g.Count()
                })
                .OrderByDescending(x => x.Orders)
                .ToListAsync();

            return new SalesAnalyticsDto
            {
                Summary = new SalesSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalOrders = totalOrders,
                    AverageOrderValue = avgOrderValue,
                    GrowthPercentage = 0
                },
                Trend = trend,
                PeakHours = peakHours
            };
        }

        // =========================================================
        // CUSTOMER ANALYTICS
        // =========================================================
        public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync()
        {
            var realizedOrders = _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.PaymentStatus == "PAID" ||
                    (o.PaymentMode == "COD" && o.Status == "DELIVERED"));

            var customerData = await realizedOrders
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            int totalCustomers = customerData.Count;
            int repeatCustomers = customerData.Count(c => c.OrderCount >= 2);

            return new CustomerAnalyticsDto
            {
                TotalCustomers = totalCustomers,
                RepeatCustomers = repeatCustomers,
                RepeatRatePercentage = totalCustomers > 0
                    ? (double)repeatCustomers / totalCustomers * 100
                    : 0,
                VipCustomers = new List<CustomerSegmentDto>(),
                ActiveCustomers = new List<CustomerSegmentDto>(),
                AtRiskCustomers = new List<CustomerSegmentDto>(),
                DeadCustomers = new List<CustomerSegmentDto>(),
                TopCustomers = new List<TopCustomerDto>()
            };
        }

        // =========================================================
        // PRODUCT ANALYTICS
        // =========================================================
        public async Task<ProductAnalyticsDto> GetProductAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? top,
            string? type,
            string? sortBy)
        {
            var today = DateTime.UtcNow.Date;

            var startDate = from ?? new DateTime(today.Year, today.Month, 1);
            var endDate = (to ?? today).AddDays(1);
            int topN = top ?? 5;

            var realizedOrders = _context.Orders.AsNoTracking()
                .Where(o =>
                    o.CreatedAt >= startDate &&
                    o.CreatedAt < endDate &&
                    (
                        o.PaymentStatus == "PAID" ||
                        (o.PaymentMode == "COD" && o.Status == "DELIVERED")
                    )
                );

            var productQuery =
                from oi in _context.OrderItems.AsNoTracking()
                join o in realizedOrders on oi.OrderId equals o.Id
                group oi by oi.ProductId into g
                select new ProductAnalyticsItemDto
                {
                    ProductId = g.Key,
                    ProductName = _context.Products
                        .Where(p => p.Id == g.Key)
                        .Select(p => p.Name)
                        .FirstOrDefault() ?? "",
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.TotalPrice)
                };

            var products = await productQuery
                .OrderByDescending(x => x.Revenue)
                .Take(topN)
                .ToListAsync();

            return new ProductAnalyticsDto
            {
                Products = products
            };
        }

        // =========================================================
        // INVENTORY ANALYTICS (Multi-Store Safe)
        // =========================================================
        public async Task<InventoryAnalyticsDto> GetInventoryAnalyticsAsync(
            DateTime? from,
            DateTime? to,
            int? lowStockThreshold)
        {
            int threshold = lowStockThreshold ?? 10;

            var lowStockProducts = await _context.StoreProductInventories
                .Include(i => i.Product)
                .AsNoTracking()
                .Where(i => i.Stock <= threshold)
                .Select(i => new LowStockDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    CurrentStock = i.Stock
                })
                .ToListAsync();

            var totalInventoryValue = await _context.StoreProductInventories
                .Include(i => i.Product)
                .AsNoTracking()
                .SumAsync(i => (decimal?)(i.Stock * i.Product.Price)) ?? 0;

            return new InventoryAnalyticsDto
            {
                LowStockProducts = lowStockProducts,
                FastMovingProducts = new List<FastMovingDto>(),
                TotalInventoryValue = totalInventoryValue,
                StockTurnoverRatio = 0
            };
        }

        // =========================================================
        // ALERT ENGINE
        // =========================================================
        public async Task<AlertAnalyticsDto> GetAlertAnalyticsAsync()
        {
            var lowStockCritical = await _context.StoreProductInventories
                .AsNoTracking()
                .AnyAsync(i => i.Stock <= 5);

            return new AlertAnalyticsDto
            {
                LowStockAlert = lowStockCritical,
                CancelSpikeAlert = false,
                RevenueDropAlert = false,
                StuckOrdersAlert = false,
                DeadProductAlert = false,
                Message = "Alert evaluation completed."
            };
        }
    }
}