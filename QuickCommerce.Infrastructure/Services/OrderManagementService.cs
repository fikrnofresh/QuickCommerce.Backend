using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services; // ✅ ADDED
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService; // ✅ ADDED

        public OrderManagementService(
            ApplicationDbContext context,
            AuditLogService auditLogService) // ✅ UPDATED
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus, int? changedByUserId, string? remarks)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var oldStatus = order.Status;

            if (oldStatus == newStatus)
                return;

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            var history = new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedByUserId = changedByUserId,
                Remarks = remarks,
                ChangedAt = DateTime.UtcNow
            };

            _context.OrderStatusHistories.Add(history);

            await _context.SaveChangesAsync();

            // 🔥 AUDIT LOG (ADMIN ACTION)
            await _auditLogService.LogAsync(
                changedByUserId,
                "ORDERS",
                "ADMIN_STATUS_UPDATE",
                "Order",
                order.Id,
                $"Admin changed order status: {oldStatus} → {newStatus}"
            );
        }
    }
}