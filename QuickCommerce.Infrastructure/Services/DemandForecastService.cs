using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Analytics;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class DemandForecastService : IDemandForecastService
    {
        private readonly ApplicationDbContext _context;

        public DemandForecastService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DemandForecastDto>> GetDemandForecastAsync(int storeId)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var sales = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .Where(o =>
                    o.Order.StoreId == storeId &&
                    o.Order.CreatedAt >= sevenDaysAgo)
                .GroupBy(o => new { o.ProductId, o.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    SoldLast7Days = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            var stockData = await _context.StoreProducts
                .Where(sp => sp.StoreId == storeId)
                .ToDictionaryAsync(sp => sp.ProductId, sp => sp.StockQuantity);

            var results = new List<DemandForecastDto>();

            foreach (var s in sales)
            {
                var stock = stockData.ContainsKey(s.ProductId)
                    ? stockData[s.ProductId]
                    : 0;

                decimal dailyAverage = s.SoldLast7Days / 7m;

                int forecastTomorrow = (int)Math.Ceiling(dailyAverage);

                int daysUntilStockout = 0;

                if (dailyAverage > 0)
                    daysUntilStockout = (int)(stock / dailyAverage);

                results.Add(new DemandForecastDto
                {
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    SoldLast7Days = s.SoldLast7Days,
                    DailyAverage = dailyAverage,
                    CurrentStock = stock,
                    EstimatedDemandTomorrow = forecastTomorrow,
                    DaysUntilStockout = daysUntilStockout
                });
            }

            return results
                .OrderBy(x => x.DaysUntilStockout)
                .Take(50)
                .ToList();
        }
    }
}