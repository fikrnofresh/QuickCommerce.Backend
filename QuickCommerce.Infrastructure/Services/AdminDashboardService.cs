using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Dashboard;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickCommerce.Core.DTOs.Analytics;

namespace QuickCommerce.Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================================
        // MAIN DASHBOARD
        // ======================================================

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;

            var ordersToday = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == today);

            var revenueToday = await _context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var commissionToday = await _context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.PlatformCommission) ?? 0;

            var activeStores = await _context.Stores
                .CountAsync(s => s.IsActive);

            var ordersInProgress = await _context.Orders
                .CountAsync(o =>
                    o.Status == "PENDING" ||
                    o.Status == "CONFIRMED" ||
                    o.Status == "PREPARING");

            var cancelledOrders = await _context.Orders
                .CountAsync(o => o.Status == "CANCELLED");

            var yesterday = today.AddDays(-1);

            var yesterdayOrders = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == yesterday);

            decimal orderGrowthPercent = 0;

            if (yesterdayOrders > 0)
                orderGrowthPercent =
                    ((ordersToday - yesterdayOrders) / (decimal)yesterdayOrders) * 100;

            var weeklyOrders = await _context.Orders
                .CountAsync(o => o.CreatedAt >= today.AddDays(-7));

            var monthlyRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= today.AddDays(-30))
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var commissionMonth = await _context.Orders
                .Where(o => o.CreatedAt >= today.AddDays(-30))
                .SumAsync(o => (decimal?)o.PlatformCommission) ?? 0;

            var storePayoutToday = await _context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.StorePayout) ?? 0;

            var averageOrderValue = await _context.Orders
                .Where(o => o.CreatedAt.Date == today)
                .AverageAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var pendingOrders = await _context.Orders
                .CountAsync(o => o.Status == "PENDING");

            var preparingOrders = await _context.Orders
                .CountAsync(o => o.Status == "PREPARING");

            var outForDelivery = await _context.Orders
                .CountAsync(o => o.Status == "OUT_FOR_DELIVERY");

            var delayedOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status != "DELIVERED" &&
                    o.Status != "COMPLETED" &&
                    o.CreatedAt < DateTime.UtcNow.AddMinutes(-60));

            var offlineStores = await _context.Stores
                .CountAsync(s => !s.IsOnline);

            var inactiveStores = await _context.Stores
                .CountAsync(s => !s.IsActive);

            var topStores = await GetStoreLeaderboardAsync();

            var topProducts = await _context.OrderItems
                .GroupBy(o => new { o.ProductId, o.ProductName })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            var lowSellingProducts = await _context.OrderItems
                .GroupBy(o => new { o.ProductId, o.ProductName })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            var lowStockProducts = await _context.StoreProducts
                .Include(sp => sp.Product)
                .Where(sp => sp.StockQuantity > 0 &&
                             sp.StockQuantity <= sp.LowStockThreshold)
                .Select(sp => new InventoryAlertDto
                {
                    StoreId = sp.StoreId,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    StockQuantity = sp.StockQuantity
                })
                .Take(10)
                .ToListAsync();

            var outOfStockProducts = await _context.StoreProducts
                .Include(sp => sp.Product)
                .Where(sp => sp.StockQuantity == 0)
                .Select(sp => new InventoryAlertDto
                {
                    StoreId = sp.StoreId,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    StockQuantity = sp.StockQuantity
                })
                .Take(10)
                .ToListAsync();

            var alerts = new List<string>();

            if (offlineStores > 0)
                alerts.Add($"{offlineStores} stores are currently offline.");

            if (delayedOrders > 0)
                alerts.Add($"{delayedOrders} orders are delayed.");

            if (outOfStockProducts.Any())
                alerts.Add($"{outOfStockProducts.Count} products are out of stock.");

            return new AdminDashboardDto
            {
                BusinessHealth = new BusinessHealthDto
                {
                    OrdersToday = ordersToday,
                    RevenueToday = revenueToday,
                    PlatformCommissionToday = commissionToday,
                    ActiveStores = activeStores,
                    OrdersInProgress = ordersInProgress,
                    CancelledOrders = cancelledOrders
                },

                GrowthMetrics = new GrowthMetricsDto
                {
                    OrdersGrowthPercent = orderGrowthPercent,
                    RevenueGrowthPercent = 0,
                    WeeklyOrders = weeklyOrders,
                    MonthlyRevenue = monthlyRevenue
                },

                FinancialInsights = new FinancialInsightsDto
                {
                    PlatformCommissionMonth = commissionMonth,
                    StorePayoutToday = storePayoutToday,
                    AverageOrderValue = averageOrderValue
                },

                OrderInsights = new OrderInsightsDto
                {
                    PendingOrders = pendingOrders,
                    PreparingOrders = preparingOrders,
                    OutForDelivery = outForDelivery,
                    DelayedOrders = delayedOrders
                },

                StoreInsights = new StoreInsightsDto
                {
                    OfflineStores = offlineStores,
                    InactiveStores = inactiveStores
                },

                TopStores = topStores,
                TopProducts = topProducts,
                LowSellingProducts = lowSellingProducts,
                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts,
                Alerts = alerts
            };
        }

        // ======================================================
        // REVENUE CHART
        // ======================================================

        public async Task<List<DashboardChartDto>> GetRevenueChartAsync()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-30);

            return await _context.Orders
                .Where(o => o.CreatedAt >= startDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DashboardChartDto
                {
                    Date = g.Key,
                    Orders = g.Count(),
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        // ======================================================
        // CATEGORY REVENUE CHART
        // ======================================================

        public async Task<List<CategoryRevenueDto>> GetCategoryRevenueChartAsync()
        {
            return await _context.OrderItems
                .Include(o => o.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(o => new { o.Product.CategoryId, o.Product.Category.Name })
                .Select(g => new CategoryRevenueDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();
        }

        // ======================================================
        // STORE LEADERBOARD
        // ======================================================

        public async Task<List<TopStoreDto>> GetStoreLeaderboardAsync()
        {
            return await _context.Orders
                .Include(o => o.Store)
                .GroupBy(o => new { o.StoreId, o.Store.Name })
                .Select(g => new TopStoreDto
                {
                    StoreId = g.Key.StoreId,
                    StoreName = g.Key.Name,
                    Orders = g.Count(),
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync();
        }
        public async Task<List<DemandForecastDto>> GetDemandForecastAsync()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);

            var data = await _context.OrderItems
                .Include(o => o.Product)
                .GroupBy(o => new { o.ProductId, o.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    SoldLast7Days = g.Sum(x => x.Quantity),
                    CurrentStock = _context.StoreProducts
                        .Where(sp => sp.ProductId == g.Key.ProductId)
                        .Sum(sp => sp.StockQuantity)
                })
                .ToListAsync();

            var result = data.Select(x =>
            {
                var dailyAverage = x.SoldLast7Days / 7.0;

                int daysLeft = dailyAverage == 0
                    ? 999
                    : (int)(x.CurrentStock / dailyAverage);

                return new DemandForecastDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    SoldLast7Days = x.SoldLast7Days,
                    CurrentStock = x.CurrentStock,
                    EstimatedDaysRemaining = daysLeft
                };
            })
            .OrderBy(x => x.EstimatedDaysRemaining)
            .Take(20)
            .ToList();

            return result;
        }
        public async Task<List<StorePerformanceDto>> GetStorePerformanceAsync()
        {
            return await _context.Orders
                .Include(o => o.Store)
                .GroupBy(o => new { o.StoreId, o.Store.Name })
                .Select(g => new StorePerformanceDto
                {
                    StoreId = g.Key.StoreId,
                    StoreName = g.Key.Name,
                    Orders = g.Count(),
                    Revenue = g.Sum(x => x.TotalAmount),
                    PlatformCommission = g.Sum(x => x.PlatformCommission)
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();
        }
        public async Task<List<CategoryCommissionAnalyticsDto>> GetCategoryCommissionAnalyticsAsync()
        {
            return await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(o => new { o.Product.CategoryId, o.Product.Category.Name })
                .Select(g => new CategoryCommissionAnalyticsDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Orders = g.Count(),
                    Revenue = g.Sum(x => x.TotalPrice),
                    PlatformCommission = g.Sum(x => x.Order.PlatformCommission)
                })
                .OrderByDescending(x => x.PlatformCommission)
                .ToListAsync();
        }
        public async Task<LiveOperationsDto> GetLiveOperationsAsync()
        {
            var pendingOrders = await _context.Orders
                .CountAsync(o => o.Status == "PENDING");

            var preparingOrders = await _context.Orders
                .CountAsync(o => o.Status == "PREPARING");

            var readyOrders = await _context.Orders
                .CountAsync(o => o.Status == "READY_FOR_PICKUP");

            var outForDelivery = await _context.Orders
                .CountAsync(o => o.Status == "OUT_FOR_DELIVERY");

            var delayedOrders = await _context.Orders
                .CountAsync(o =>
                    o.Status != "DELIVERED" &&
                    o.Status != "COMPLETED" &&
                    o.CreatedAt < DateTime.UtcNow.AddMinutes(-60));

            var activeDeliveryPartners = await _context.DeliveryPartners
                .CountAsync(d => d.IsActive);

            var busyDeliveryPartners = await _context.Deliveries
                .CountAsync(d => d.Status == "OUT_FOR_DELIVERY");

            return new LiveOperationsDto
            {
                PendingOrders = pendingOrders,
                PreparingOrders = preparingOrders,
                ReadyForPickupOrders = readyOrders,
                OutForDeliveryOrders = outForDelivery,
                DelayedOrders = delayedOrders,
                ActiveDeliveryPartners = activeDeliveryPartners,
                BusyDeliveryPartners = busyDeliveryPartners
            };
        }
        public async Task<List<OrderQueueDto>> GetOrderQueueAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .OrderBy(o => o.CreatedAt)
                .Select(o => new OrderQueueDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    StoreId = o.StoreId,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                })
                .Take(50)
                .ToListAsync();
        }
        public async Task GenerateDailySettlementAsync()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt.Date == yesterday)
                .ToListAsync();

            var grouped = orders
                .GroupBy(o => o.StoreId);

            foreach (var g in grouped)
            {
                var totalOrders = g.Count();
                var revenue = g.Sum(o => o.TotalAmount);
                var commission = g.Sum(o => o.PlatformCommission);
                var payout = g.Sum(o => o.StorePayout);

                var settlement = new StoreSettlement
                {
                    StoreId = g.Key,
                    TotalOrders = totalOrders,
                    TotalRevenue = revenue,
                    PlatformCommission = commission,
                    StorePayout = payout,
                    SettlementDate = yesterday,
                    Status = "PENDING"
                };

                _context.StoreSettlements.Add(settlement);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<StoreSettlementDto>> GetStoreSettlementsAsync()
        {
            return await _context.StoreSettlements
                .Select(s => new StoreSettlementDto
                {
                    StoreId = s.StoreId,
                    TotalOrders = s.TotalOrders,
                    TotalRevenue = s.TotalRevenue,
                    PlatformCommission = s.PlatformCommission,
                    StorePayout = s.StorePayout,
                    SettlementDate = s.SettlementDate,
                    Status = s.Status
                })
                .OrderByDescending(s => s.SettlementDate)
                .ToListAsync();
        }
        public async Task<FinancialDashboardDto> GetFinancialDashboardAsync()
        {
            var platformTotal = await _context.Orders
                .SumAsync(o => (decimal?)o.PlatformCommission) ?? 0;

            var storePayout = await _context.Orders
                .SumAsync(o => (decimal?)o.StorePayout) ?? 0;

            var pendingSettlements = await _context.StoreSettlements
                .Where(s => s.Status == "PENDING")
                .SumAsync(s => (decimal?)s.StorePayout) ?? 0;

            var completedSettlements = await _context.StoreSettlements
                .Where(s => s.Status == "COMPLETED")
                .SumAsync(s => (decimal?)s.StorePayout) ?? 0;

            var topStores = await _context.Orders
                .Include(o => o.Store)
                .GroupBy(o => new { o.StoreId, o.Store.Name })
                .Select(g => new TopStoreEarningDto
                {
                    StoreId = g.Key.StoreId,
                    StoreName = g.Key.Name,
                    Revenue = g.Sum(x => x.TotalAmount),
                    PlatformCommission = g.Sum(x => x.PlatformCommission)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync();

            var categoryCommission = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(o => new { o.Product.CategoryId, o.Product.Category.Name })
                .Select(g => new CategoryCommissionDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    PlatformCommission = g.Sum(x => x.Order.PlatformCommission)
                })
                .OrderByDescending(x => x.PlatformCommission)
                .ToListAsync();

            return new FinancialDashboardDto
            {
                PlatformTotalEarnings = platformTotal,
                TotalStorePayout = storePayout,
                PendingSettlements = pendingSettlements,
                CompletedSettlements = completedSettlements,
                TopEarningStores = topStores,
                CommissionByCategory = categoryCommission
            };
        }
    }
}