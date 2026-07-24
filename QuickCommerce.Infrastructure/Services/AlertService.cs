using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Core.Entities;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickCommerce.Core.Enums;


namespace QuickCommerce.Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;

        public AlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlatformAlertDto>> GetPlatformAlertsAsync()
        {
            var alerts = new List<PlatformAlertDto>();

            var now = DateTime.UtcNow;

            // STORE OFFLINE ALERT
            var offlineStores = await _context.Stores
                .Where(s => !s.IsOnline)
                .CountAsync();

            if (offlineStores > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "STORE_OFFLINE",
                    Message = $"{offlineStores} stores are currently offline.",
                    Severity = "HIGH",
                    CreatedAt = now
                });
            }

            // STORE SUSPENDED
            var suspendedStores = await _context.Stores
                .Where(s => s.Status == StoreStatus.Suspended)
                .CountAsync();

            if (suspendedStores > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "STORE_SUSPENDED",
                    Message = $"{suspendedStores} stores are suspended.",
                    Severity = "MEDIUM",
                    CreatedAt = now
                });
            }

            // DELAYED ORDERS
            var delayedOrders = await _context.Orders
                .Where(o =>
                    o.Status != "DELIVERED" &&
                    o.Status != "COMPLETED" &&
                    o.CreatedAt < DateTime.UtcNow.AddMinutes(-60))
                .CountAsync();

            if (delayedOrders > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "DELAYED_ORDERS",
                    Message = $"{delayedOrders} orders are delayed.",
                    Severity = "HIGH",
                    CreatedAt = now
                });
            }

            // OUT OF STOCK PRODUCTS
            var outOfStock = await _context.StoreProducts
                .Where(sp => sp.StockQuantity == 0)
                .CountAsync();

            if (outOfStock > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "OUT_OF_STOCK",
                    Message = $"{outOfStock} products are out of stock.",
                    Severity = "HIGH",
                    CreatedAt = now
                });
            }

            // LOW STOCK PRODUCTS
            var lowStock = await _context.StoreProducts
                .Where(sp =>
                    sp.StockQuantity > 0 &&
                    sp.StockQuantity <= sp.LowStockThreshold)
                .CountAsync();

            if (lowStock > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "LOW_STOCK",
                    Message = $"{lowStock} products are running low on stock.",
                    Severity = "MEDIUM",
                    CreatedAt = now
                });
            }

            // CATEGORY WITHOUT COMMISSION
            var missingCommission = await _context.Categories
                .Where(c =>
                    !_context.CategoryCommissionRules
                        .Any(r => r.CategoryId == c.Id))
                .CountAsync();

            if (missingCommission > 0)
            {
                alerts.Add(new PlatformAlertDto
                {
                    AlertType = "COMMISSION_MISSING",
                    Message = $"{missingCommission} categories have no commission rule.",
                    Severity = "HIGH",
                    CreatedAt = now
                });
            }

            return alerts;
        }
    }
}