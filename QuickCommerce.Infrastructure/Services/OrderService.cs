using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly DeliveryAssignmentService _deliveryService;
        private readonly ApplicationDbContext _context;

        private static readonly Dictionary<string, List<string>> AllowedTransitions =
            new()
            {
                { "PENDING", new List<string> { "CONFIRMED", "CANCELLED", "FAILED" } },
                { "CONFIRMED", new List<string> { "PREPARING", "CANCELLED" } },
                { "PREPARING", new List<string> { "READY_FOR_PICKUP" } },
                { "READY_FOR_PICKUP", new List<string> { "ASSIGNED" } },
                { "ASSIGNED", new List<string> { "OUT_FOR_DELIVERY" } },
                { "OUT_FOR_DELIVERY", new List<string> { "DELIVERED" } },
                { "DELIVERED", new List<string> { "COMPLETED" } },
                { "COMPLETED", new List<string>() },
                { "CANCELLED", new List<string>() },
                { "FAILED", new List<string>() }
            };

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            DeliveryAssignmentService deliveryService,
            ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _deliveryService = deliveryService;
            _context = context;
        }

        // =========================
        // CREATE ORDER (Multi-Store Inventory Safe)
        // =========================
        public async Task<Order> CreateOrderAsync(OrderRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Order must contain at least one item.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    StoreId = request.StoreId, // 🏬 Store Scope
                    DeliveryAddressId = request.DeliveryAddressId,
                    PaymentMode = request.PaymentMode,
                    DeliveryInstructions = request.DeliveryInstructions,
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                    Status = "PENDING",
                    PaymentStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                decimal subtotal = 0;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null)
                        throw new Exception($"Product {item.ProductId} not found.");

                    // 🔒 Fetch Store-Specific Inventory
                    var inventory = await _context.StoreProductInventories
                        .FirstOrDefaultAsync(i =>
                            i.ProductId == item.ProductId &&
                            i.StoreId == request.StoreId);

                    if (inventory == null)
                        throw new Exception($"Product not available in this store.");

                    if (inventory.Stock < item.Quantity)
                        throw new Exception($"Insufficient stock for {product.Name}");

                    // ➖ Deduct store stock
                    inventory.Stock -= item.Quantity;
                    inventory.UpdatedAt = DateTime.UtcNow;

                    // 📦 Inventory Log (Store Scoped)
                    await _context.InventoryMovements.AddAsync(
                        new InventoryMovement
                        {
                            ProductId = product.Id,
                            StoreId = request.StoreId,
                            QuantityChanged = -item.Quantity,
                            Reason = "Order Created",
                            CreatedAt = DateTime.UtcNow
                        });

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Unit = product.Unit,
                        Quantity = item.Quantity,
                        PricePerUnit = product.Price,
                        TotalPrice = product.Price * item.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };

                    subtotal += orderItem.TotalPrice;
                    order.OrderItems.Add(orderItem);
                }

                order.SubtotalAmount = subtotal;
                order.TotalAmount = subtotal;

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // =========================
        // GET ORDER
        // =========================
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        // =========================
        // UPDATE ORDER STATUS
        // =========================
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            newStatus = newStatus.ToUpper();

            if (!IsValidTransition(order.Status, newStatus))
                throw new Exception($"Invalid status transition from {order.Status} to {newStatus}");

            // Payment validation
            if (newStatus == "CONFIRMED")
            {
                if (order.PaymentMode != "COD" && order.PaymentStatus != "PAID")
                    throw new Exception("Payment not completed.");
            }

            // 🔄 Restore Store Inventory if Cancelled
            if (newStatus == "CANCELLED")
            {
                foreach (var item in order.OrderItems)
                {
                    var inventory = await _context.StoreProductInventories
                        .FirstOrDefaultAsync(i =>
                            i.ProductId == item.ProductId &&
                            i.StoreId == order.StoreId);

                    if (inventory != null)
                    {
                        inventory.Stock += item.Quantity;
                        inventory.UpdatedAt = DateTime.UtcNow;

                        await _context.InventoryMovements.AddAsync(
                            new InventoryMovement
                            {
                                ProductId = item.ProductId,
                                StoreId = order.StoreId,
                                QuantityChanged = item.Quantity,
                                Reason = "Order Cancelled",
                                CreatedAt = DateTime.UtcNow
                            });
                    }
                }
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            if (newStatus == "CONFIRMED")
                await _deliveryService.AssignDeliveryAsync(order.Id);

            return true;
        }

        private bool IsValidTransition(string currentStatus, string newStatus)
        {
            if (!AllowedTransitions.ContainsKey(currentStatus))
                return false;

            return AllowedTransitions[currentStatus].Contains(newStatus);
        }
    }
}