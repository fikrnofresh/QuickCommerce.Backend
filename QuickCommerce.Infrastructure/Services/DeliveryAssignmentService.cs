using Microsoft.EntityFrameworkCore;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class DeliveryAssignmentService
    {
        private readonly ApplicationDbContext _context;

        // 🔥 Maximum active deliveries per partner
        private const int MAX_ACTIVE_ORDERS = 3;

        public DeliveryAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔥 ASSIGN DELIVERY (MULTI ORDER SUPPORT)
        public async Task AssignDeliveryAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.DeliveryAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");
            
            // Prevent duplicate delivery record
            var existingDelivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.OrderId == orderId);

            if (existingDelivery != null)
                return;

            // 🔥 Select partner with lowest workload
            var partner = await _context.DeliveryPartners
                .Where(p =>
                    p.IsActive &&
                    p.IsVerified &&
                    p.CurrentLatitude.HasValue &&
                    p.CurrentLongitude.HasValue &&
                    p.CurrentWorkload < MAX_ACTIVE_ORDERS)
                .OrderBy(p => p.CurrentWorkload)
                .FirstOrDefaultAsync();

            if (partner == null)
                throw new Exception("No delivery partner available");

            var delivery = new Delivery
            {
                OrderId = order.Id,
                DeliveryPartnerId = partner.Id,
                Status = "ASSIGNED",
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Deliveries.AddAsync(delivery);

            // 🔥 Increase workload
            partner.CurrentWorkload += 1;

            // 🔥 Update order status
            order.Status = "ASSIGNED";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // 🔥 UPDATE DELIVERY STATUS (Lifecycle)
        public async Task UpdateDeliveryStatusAsync(int orderId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Invalid delivery status");

            status = status.ToUpper();

            var delivery = await _context.Deliveries
                .Include(d => d.DeliveryPartner)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.OrderId == orderId);

            if (delivery == null)
                throw new Exception("Delivery record not found");

            delivery.Status = status;
            delivery.UpdatedAt = DateTime.UtcNow;

            switch (status)
            {
                case "ACCEPTED":
                    delivery.AcceptedAt = DateTime.UtcNow;

                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = "ASSIGNED";
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case "PICKED_UP":
                    delivery.PickedUpAt = DateTime.UtcNow;

                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = "OUT_FOR_DELIVERY";
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case "DELIVERED":
                    delivery.DeliveredAt = DateTime.UtcNow;

                    // Reduce workload
                    if (delivery.DeliveryPartner != null &&
                        delivery.DeliveryPartner.CurrentWorkload > 0)
                    {
                        delivery.DeliveryPartner.CurrentWorkload -= 1;
                    }

                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = "DELIVERED";
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                default:
                    throw new Exception("Unsupported delivery status");
            }

            await _context.SaveChangesAsync();
        }
    }
}

