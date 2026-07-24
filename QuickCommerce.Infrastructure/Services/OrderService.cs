using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Enums;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services; // 🔥 ADDED
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
        private readonly AuditLogService _auditLogService; // 🔥 ADDED

        private static readonly Dictionary<string, List<string>> AllowedTransitions =
        new()
        {
            { OrderStatus.Pending, new List<string> { OrderStatus.Confirmed, OrderStatus.Cancelled, OrderStatus.Failed } },
            { OrderStatus.Confirmed, new List<string> { OrderStatus.Preparing, OrderStatus.Cancelled } },
            { OrderStatus.Preparing, new List<string> { OrderStatus.ReadyForPickup } },
            { OrderStatus.ReadyForPickup, new List<string> { OrderStatus.Assigned } },
            { OrderStatus.Assigned, new List<string> { OrderStatus.OutForDelivery } },
            { OrderStatus.OutForDelivery, new List<string> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new List<string> { OrderStatus.Completed } },
            { OrderStatus.Completed, new List<string>() },
            { OrderStatus.Cancelled, new List<string>() },
            { OrderStatus.Failed, new List<string>() }
        };

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            DeliveryAssignmentService deliveryService,
            ApplicationDbContext context,
            AuditLogService auditLogService) // 🔥 UPDATED
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _deliveryService = deliveryService;
            _context = context;
            _auditLogService = auditLogService;
        }

        // =========================
        // CREATE ORDER
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
                    StoreId = request.StoreId,
                    DeliveryAddressId = request.DeliveryAddressId,
                    PaymentMode = request.PaymentMode,
                    DeliveryInstructions = request.DeliveryInstructions,
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                    Status = OrderStatus.Pending,
                    PaymentStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                decimal subtotal = 0;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new Exception($"Product {item.ProductId} not found.");

                    var storeProduct = await _context.StoreProducts
                        .FirstOrDefaultAsync(sp => sp.ProductId == item.ProductId && sp.StoreId == request.StoreId);

                    if (storeProduct == null)
                        throw new Exception($"Product not available in this store.");

                    if (storeProduct.StockQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for {product.Name}");

                    storeProduct.StockQuantity -= item.Quantity;
                    storeProduct.UpdatedAt = DateTime.UtcNow;

                    await _context.InventoryMovements.AddAsync(new InventoryMovement
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

                var firstProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == request.Items.First().ProductId);

                var rule = await _context.CategoryCommissionRules
                    .FirstOrDefaultAsync(c => c.CategoryId == firstProduct.CategoryId && c.IsActive);

                decimal percent = subtotal * (rule.CommissionPercent / 100);
                decimal final = Math.Max(percent, rule.MinimumCommissionPerOrder);

                order.PlatformCommission = final;
                order.StorePayout = subtotal - final;
                order.CommissionPercentApplied = rule.CommissionPercent;

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
                {
                    OrderId = order.Id,
                    OldStatus = "SYSTEM",
                    NewStatus = OrderStatus.Pending,
                    ChangedByUserId = request.CustomerId,
                    Remarks = "Order created",
                    ChangedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                // 🔥 AUDIT LOG
                await _auditLogService.LogAsync(
                    request.CustomerId,
                    "ORDERS",
                    "CREATE",
                    "Order",
                    order.Id,
                    $"Order created: {order.OrderNumber}"
                );

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
        // UPDATE STATUS
        // =========================
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, int? changedByUserId, string? remarks)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            newStatus = newStatus.ToUpper();

            if (!AllowedTransitions.ContainsKey(order.Status) ||
                !AllowedTransitions[order.Status].Contains(newStatus))
                throw new Exception($"Invalid transition {order.Status} → {newStatus}");

            var oldStatus = order.Status;

            if (newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems)
                {
                    var sp = await _context.StoreProducts
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.StoreId == order.StoreId);

                    if (sp != null)
                    {
                        sp.StockQuantity += item.Quantity;

                        await _context.InventoryMovements.AddAsync(new InventoryMovement
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

            await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedByUserId = changedByUserId,
                Remarks = remarks,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // 🔥 AUDIT LOG
            await _auditLogService.LogAsync(
                changedByUserId,
                "ORDERS",
                "STATUS_UPDATE",
                "Order",
                order.Id,
                $"Order status changed: {oldStatus} → {newStatus}"
            );

            if (newStatus == OrderStatus.Confirmed)
                await _deliveryService.AssignDeliveryAsync(order.Id);

            return true;
        }
        // =========================
        // GET ORDER BY ID
        // =========================
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // =========================
        // GET ALL ORDERS
        // =========================
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET ORDERS BY STORE
        // =========================
        public async Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId)
        {
            return await _context.Orders
                .Where(o => o.StoreId == storeId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET ORDERS BY CUSTOMER
        // =========================
        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET ORDER TRACKING
        // =========================
        public async Task<IEnumerable<CustomerOrderTrackingDto>> GetOrderTrackingAsync(int orderId)
        {
            return await _context.OrderStatusHistories
                .Where(h => h.OrderId == orderId)
                .OrderBy(h => h.ChangedAt)
                .Select(h => new CustomerOrderTrackingDto
                {
                    Status = h.NewStatus,
                    Remarks = h.Remarks,
                    ChangedAt = h.ChangedAt
                })
                .ToListAsync();
        }
    }
}