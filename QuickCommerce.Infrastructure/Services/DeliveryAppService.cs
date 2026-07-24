using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Enums;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class DeliveryAppService : IDeliveryAppService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryAppService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET MY ORDERS
        // =========================
        public async Task<object> GetMyOrdersAsync(int userId)
        {
            var partner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (partner == null)
                throw new Exception("Delivery partner not found");

            var deliveries = await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                .Include(d => d.Order)
                    .ThenInclude(o => o.DeliveryAddress)
                .Where(d =>
                    d.AssignedToPartnerId == partner.Id &&
                    d.AgentType == DeliveryAgentType.External)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var result = deliveries.Select(d => new
            {
                deliveryId = d.Id,
                orderId = d.OrderId,
                status = d.Status,
                assignedAt = d.AssignedAt,

                order = new
                {
                    orderNumber = d.Order.OrderNumber,
                    totalAmount = d.Order.TotalAmount,
                    status = d.Order.Status,

                    address = d.Order.DeliveryAddress != null
                        ? $"{d.Order.DeliveryAddress.HouseNumber}, {d.Order.DeliveryAddress.Street}, {d.Order.DeliveryAddress.Area}, {d.Order.DeliveryAddress.City} - {d.Order.DeliveryAddress.Pincode}" +
                          (string.IsNullOrEmpty(d.Order.DeliveryAddress.Landmark) ? "" : $" (Near {d.Order.DeliveryAddress.Landmark})")
                        : "",

                    instructions = d.Order.DeliveryInstructions,

                    items = d.Order.OrderItems.Select(i => new
                    {
                        productId = i.ProductId,
                        productName = i.ProductName,
                        quantity = i.Quantity,
                        unit = i.Unit,
                        pricePerUnit = i.PricePerUnit,
                        totalPrice = i.TotalPrice
                    })
                }
            });

            return result;
        }

        // =========================
        // ACCEPT ORDER
        // =========================
        public async Task AcceptOrderAsync(int userId, int orderId)
        {
            var partner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (partner == null)
                throw new Exception("Delivery partner not found");

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d =>
                    d.OrderId == orderId &&
                    d.AssignedToPartnerId == partner.Id &&
                    d.AgentType == DeliveryAgentType.External);

            if (delivery == null)
                throw new Exception("Order not assigned to you");

            if (delivery.Status != "ASSIGNED")
                throw new Exception("Order already processed");

            delivery.Status = "ACCEPTED";
            delivery.AcceptedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // REJECT ORDER
        // =========================
        public async Task RejectOrderAsync(int userId, int orderId)
        {
            var partner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (partner == null)
                throw new Exception("Delivery partner not found");

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d =>
                    d.OrderId == orderId &&
                    d.AssignedToPartnerId == partner.Id &&
                    d.AgentType == DeliveryAgentType.External);

            if (delivery == null)
                throw new Exception("Order not assigned to you");

            if (delivery.Status != "ASSIGNED")
                throw new Exception("Cannot reject after acceptance");

            delivery.Status = DeliveryStatus.FAILED.ToString();
            delivery.UpdatedAt = DateTime.UtcNow;

            if (partner.CurrentWorkload > 0)
                partner.CurrentWorkload--;

            await _context.SaveChangesAsync();
        }

        // =========================
        // EARNINGS
        // =========================
        public async Task<object> GetEarningsAsync(int userId)
        {
            var partner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (partner == null)
                throw new Exception("Delivery partner not found");

            var deliveries = await _context.Deliveries
                .Where(d =>
                    d.AssignedToPartnerId == partner.Id &&
                    d.AgentType == DeliveryAgentType.External &&
                    d.Status == "DELIVERED")
                .ToListAsync();

            var totalEarnings = deliveries.Sum(d => d.PartnerEarnings ?? 0);

            return new
            {
                totalEarnings,
                totalDeliveries = deliveries.Count
            };
        }
    }
}