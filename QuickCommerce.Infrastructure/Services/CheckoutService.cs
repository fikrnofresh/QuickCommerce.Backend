using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Enums;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;

        public CheckoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckoutResponseDto> Checkout(int userId, CheckoutRequestDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // LOAD CART
                // =========================
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                    throw new Exception("Cart is empty");

                // =========================
                // STORE SAFETY CHECK
                // =========================
                var store = await _context.Stores
                    .FirstOrDefaultAsync(s => s.Id == cart.StoreId && s.IsActive);

                if (store == null)
                    throw new Exception($"Store {cart.StoreId} not found");

                // =========================
                // ADDRESS VALIDATION
                // =========================
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == dto.AddressId && a.UserId == userId);

                if (address == null)
                    throw new Exception("Address not found");

                decimal subtotal = 0;

                // =========================
                // CREATE ORDER
                // =========================
                var order = new Order
                {
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                    CustomerId = userId,
                    StoreId = cart.StoreId,
                    DeliveryAddressId = dto.AddressId,

                    // ✅ CTO FIX
                    Status = OrderStatus.Pending,

                    SubtotalAmount = 0,
                    DeliveryFee = 0,
                    DiscountAmount = 0,
                    TaxAmount = 0,
                    TotalAmount = 0,

                    PaymentMode = dto.PaymentMethod,
                    PaymentStatus = "PENDING",

                    PlatformCommission = 0,
                    StorePayout = 0,
                    CommissionPercentApplied = 0,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // =========================
                // STATUS HISTORY (FIXED)
                // =========================
                await _context.OrderStatusHistories.AddAsync(
                    new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        OldStatus = "SYSTEM",
                        NewStatus = order.Status,
                        ChangedByUserId = userId,
                        Remarks = "Order created via checkout",
                        ChangedAt = DateTime.UtcNow
                    });

                await _context.SaveChangesAsync();

                // =========================
                // CREATE ORDER ITEMS
                // =========================
                foreach (var item in cart.Items)
                {
                    var storeProduct = await _context.StoreProducts
                        .FirstOrDefaultAsync(sp =>
                            sp.ProductId == item.ProductId &&
                            sp.StoreId == cart.StoreId);

                    if (storeProduct == null)
                        throw new Exception($"Product {item.ProductId} not available in store");

                    if (storeProduct.StockQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for product {item.ProductId}");

                    // Deduct inventory
                    storeProduct.StockQuantity -= item.Quantity;

                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Name ?? "Product",
                        Quantity = item.Quantity,
                        Unit = product.Unit ?? "unit",
                        PricePerUnit = item.PriceSnapshot,
                        TotalPrice = item.PriceSnapshot * item.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.OrderItems.Add(orderItem);

                    subtotal += orderItem.TotalPrice;
                }

                // =========================
                // UPDATE ORDER TOTAL
                // =========================
                order.SubtotalAmount = subtotal;

                order.TotalAmount =
                    order.SubtotalAmount +
                    order.DeliveryFee +
                    order.TaxAmount -
                    order.DiscountAmount;

                order.StorePayout = order.TotalAmount;

                order.UpdatedAt = DateTime.UtcNow;

                // =========================
                // CLEAR CART
                // =========================
                _context.CartItems.RemoveRange(cart.Items);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new CheckoutResponseDto
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}