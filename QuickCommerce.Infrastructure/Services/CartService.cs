using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new CartDto();

            var items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.PriceSnapshot
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                StoreId = cart.StoreId,
                Items = items,
                TotalAmount = items.Sum(i => i.Price * i.Quantity)
            };
        }

        public async Task<CartDto> AddItem(int userId, AddCartItemDto dto)
        {
            // ===============================
            // ENTERPRISE SAFETY — STORE VALIDATION
            // ===============================

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == dto.StoreId && s.IsActive);

            if (store == null)
                throw new Exception($"Store {dto.StoreId} not found or inactive");

            // ===============================
            // PRODUCT VALIDATION
            // ===============================

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            // ===============================
            // STORE INVENTORY VALIDATION
            // ===============================

            var storeProduct = await _context.StoreProducts
                .FirstOrDefaultAsync(sp =>
                    sp.StoreId == dto.StoreId &&
                    sp.ProductId == dto.ProductId);

            if (storeProduct == null)
                throw new Exception("Product not available in this store");

            if (storeProduct.StockQuantity <= 0)
                throw new Exception("Product is out of stock");

            // ===============================
            // FIND EXISTING CART
            // ===============================

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // ===============================
            // SINGLE STORE CART RULE
            // ===============================

            if (cart != null && cart.StoreId != dto.StoreId)
            {
                throw new Exception(
                    "Cart already contains items from another store. Please clear cart first."
                );
            }

            // ===============================
            // CREATE CART IF NOT EXISTS
            // ===============================

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    StoreId = dto.StoreId
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // ===============================
            // ADD / UPDATE CART ITEM
            // ===============================

            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;

                if (newQuantity > storeProduct.StockQuantity)
                    throw new Exception(
                        $"Only {storeProduct.StockQuantity} items available in stock"
                    );

                existingItem.Quantity = newQuantity;
            }
            else
            {
                if (dto.Quantity > storeProduct.StockQuantity)
                    throw new Exception(
                        $"Only {storeProduct.StockQuantity} items available in stock"
                    );

                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    PriceSnapshot = product.Price
                });
            }

            await _context.SaveChangesAsync();

            return await GetCart(userId);
        }

        public async Task<CartDto> UpdateItem(int userId, int itemId, int quantity)
        {
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.UserId == userId);

            if (item == null)
                throw new Exception("Cart item not found");

            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            // ===============================
            // INVENTORY VALIDATION ON UPDATE
            // ===============================

            var storeProduct = await _context.StoreProducts
                .FirstOrDefaultAsync(sp =>
                    sp.StoreId == item.Cart.StoreId &&
                    sp.ProductId == item.ProductId);

            if (storeProduct == null)
                throw new Exception("Product not available in this store");

            if (quantity > storeProduct.StockQuantity)
                throw new Exception(
                    $"Only {storeProduct.StockQuantity} items available in stock"
                );

            item.Quantity = quantity;

            await _context.SaveChangesAsync();

            return await GetCart(userId);
        }

        public async Task RemoveItem(int userId, int itemId)
        {
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.UserId == userId);

            if (item == null)
                throw new Exception("Cart item not found");

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}