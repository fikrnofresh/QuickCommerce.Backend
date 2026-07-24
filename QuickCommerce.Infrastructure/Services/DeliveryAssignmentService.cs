using Microsoft.EntityFrameworkCore;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Enums;
using QuickCommerce.Infrastructure.Services; // ✅ ADDED
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class DeliveryAssignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService; // ✅ ADDED

        private const int MAX_ACTIVE_ORDERS = 3;

        public DeliveryAssignmentService(
            ApplicationDbContext context,
            AuditLogService auditLogService) // ✅ UPDATED
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // =========================
        // GET BEST INTERNAL DELIVERY BOY (LOAD BALANCED)
        // =========================
        private async Task<User?> GetAvailableStoreDeliveryBoy(int storeId)
        {
            var deliveryBoy = await _context.Users
                .Where(u =>
                    u.IsActive &&
                    u.UserStores.Any(us => us.StoreId == storeId) &&
                    u.UserRoles.Any(ur => ur.Role.Name == "DELIVERY_BOY"))
                .Select(u => new
                {
                    User = u,
                    ActiveOrders = _context.Deliveries.Count(d =>
                        d.AssignedToUserId == u.Id &&
                        (d.Status == OrderStatus.Assigned ||
                         d.Status == OrderStatus.OutForDelivery ||
                         d.Status == OrderStatus.Confirmed))
                })
                .Where(x => x.ActiveOrders < MAX_ACTIVE_ORDERS)
                .OrderBy(x => x.ActiveOrders)
                .ThenBy(x => x.User.CreatedAt)
                .FirstOrDefaultAsync();

            return deliveryBoy?.User;
        }

        // =========================
        // GET BEST DELIVERY PARTNER
        // =========================
        private async Task<DeliveryPartner?> GetBestDeliveryPartner(Order order)
        {
            return await _context.DeliveryPartners
                .Where(p =>
                    p.IsActive &&
                    p.IsVerified &&
                    p.IsAvailable &&
                    p.CurrentLatitude.HasValue &&
                    p.CurrentLongitude.HasValue &&
                    p.CurrentWorkload < MAX_ACTIVE_ORDERS)
                .OrderBy(p =>
                    Math.Pow((double)(p.CurrentLatitude.Value - order.DeliveryAddress.Latitude.Value), 2) +
                    Math.Pow((double)(p.CurrentLongitude.Value - order.DeliveryAddress.Longitude.Value), 2)
                )
                .ThenBy(p => p.CurrentWorkload)
                .FirstOrDefaultAsync();
        }

        // =========================
        // ASSIGN DELIVERY (SMART ENGINE)
        // =========================
        public async Task AssignDeliveryAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.DeliveryAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var existingDelivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.OrderId == orderId);

            if (existingDelivery != null)
                return;

            Delivery delivery;

            var deliveryBoy = await GetAvailableStoreDeliveryBoy(order.StoreId);

            if (deliveryBoy != null)
            {
                delivery = new Delivery
                {
                    OrderId = order.Id,
                    AssignedToUserId = deliveryBoy.Id,
                    AssignedToPartnerId = null,
                    AgentType = DeliveryAgentType.Internal,
                    Status = OrderStatus.Assigned,
                    AssignedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            else
            {
                var partner = await GetBestDeliveryPartner(order);

                if (partner == null)
                {
                    Console.WriteLine("⚠️ No delivery partner available");
                    return;
                }

                delivery = new Delivery
                {
                    OrderId = order.Id,
                    AssignedToPartnerId = partner.Id,
                    AssignedToUserId = null,
                    AgentType = DeliveryAgentType.External,
                    Status = OrderStatus.Assigned,
                    AssignedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                partner.CurrentWorkload += 1;
            }

            await _context.Deliveries.AddAsync(delivery);

            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 🔥 AUDIT LOG (ASSIGN)
            await _auditLogService.LogAsync(
                null,
                "DELIVERY",
                "ASSIGN",
                "Order",
                order.Id,
                "Delivery assigned"
            );
        }

        // =========================
        // UPDATE DELIVERY STATUS
        // =========================
        public async Task UpdateDeliveryStatusAsync(int orderId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Invalid delivery status");

            status = status.ToUpper();

            var delivery = await _context.Deliveries
                .Include(d => d.ExternalAgent)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.OrderId == orderId);

            if (delivery == null)
                throw new Exception("Delivery record not found");

            if (!Enum.TryParse<DeliveryStatus>(status, true, out var parsedStatus))
                throw new Exception("Invalid delivery status");

            delivery.Status = parsedStatus.ToString();
            delivery.UpdatedAt = DateTime.UtcNow;

            switch (parsedStatus)
            {
                case DeliveryStatus.ACCEPTED:
                    delivery.AcceptedAt = DateTime.UtcNow;
                    break;

                case DeliveryStatus.PICKED_UP:
                    delivery.PickedUpAt = DateTime.UtcNow;

                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = OrderStatus.OutForDelivery;
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case DeliveryStatus.IN_TRANSIT:
                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = OrderStatus.OutForDelivery;
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case DeliveryStatus.DELIVERED:

                    if (delivery.Order.PaymentMode == "COD" && delivery.CodCollectedAmount == null)
                        throw new Exception("COD not collected");

                    delivery.DeliveredAt = DateTime.UtcNow;

                    if (delivery.AgentType == DeliveryAgentType.External &&
                        delivery.ExternalAgent != null &&
                        delivery.ExternalAgent.CurrentWorkload > 0)
                    {
                        delivery.ExternalAgent.CurrentWorkload -= 1;
                    }

                    if (delivery.Order != null)
                    {
                        delivery.Order.Status = OrderStatus.Delivered;
                        delivery.Order.UpdatedAt = DateTime.UtcNow;
                    }
                    break;

                case DeliveryStatus.FAILED:

                    if (delivery.AgentType == DeliveryAgentType.External &&
                        delivery.ExternalAgent != null &&
                        delivery.ExternalAgent.CurrentWorkload > 0)
                    {
                        delivery.ExternalAgent.CurrentWorkload -= 1;
                    }

                    break;

                default:
                    throw new Exception("Unsupported delivery status");
            }

            await _context.SaveChangesAsync();

            // 🔥 AUDIT LOG (STATUS UPDATE)
            await _auditLogService.LogAsync(
                null,
                "DELIVERY",
                "STATUS_UPDATE",
                "Order",
                orderId,
                $"Delivery status updated: {status}"
            );
        }
    }
}