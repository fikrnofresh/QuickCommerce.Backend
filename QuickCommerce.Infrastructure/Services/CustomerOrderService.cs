using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext _context;

        public CustomerOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CUSTOMER ORDER LIST
        // =========================
        public async Task<List<CustomerOrderDto>> GetOrders(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new CustomerOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return orders;
        }

        // =========================
        // CUSTOMER ORDER DETAIL
        // =========================
        public async Task<CustomerOrderDetailDto> GetOrderDetail(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId);

            if (order == null)
                throw new Exception("Order not found");

            return new CustomerOrderDetailDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(i => new CustomerOrderItemDto
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    PricePerUnit = i.PricePerUnit,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }

        // =========================
        // CUSTOMER ORDER TRACKING
        // =========================
        public async Task<IEnumerable<CustomerOrderTrackingDto>> GetOrderTracking(int customerId, int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);

            if (order == null)
                throw new Exception("Order not found");

            var tracking = await _context.OrderStatusHistories
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.ChangedAt)
                .Select(h => new CustomerOrderTrackingDto
                {
                    Status = h.NewStatus,
                    Remarks = h.Remarks,
                    ChangedAt = h.ChangedAt
                })
                .ToListAsync();

            return tracking;
        }
    }
}