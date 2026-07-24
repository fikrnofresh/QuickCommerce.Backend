using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Search;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class GlobalSearchService : IGlobalSearchService
    {
        private readonly ApplicationDbContext _context;

        public GlobalSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalSearchResultDto> SearchAsync(string query)
        {
            var result = new GlobalSearchResultDto();

            if (string.IsNullOrWhiteSpace(query))
                return result;

            query = query.ToLower();

            // USERS
            var users = await _context.Users
                .Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(query)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(query)))
                .Take(5)
                .ToListAsync();

            result.Results.AddRange(users.Select(u => new GlobalSearchItemDto
            {
                Type = "User",
                Id = u.Id,
                Title = u.Email ?? u.PhoneNumber ?? "User",
                Description = "Platform User",
                Route = $"/users/{u.Id}"
            }));


            // STORES
            var stores = await _context.Stores
                .Where(s => s.Name.ToLower().Contains(query))
                .Take(5)
                .ToListAsync();

            result.Results.AddRange(stores.Select(s => new GlobalSearchItemDto
            {
                Type = "Store",
                Id = s.Id,
                Title = s.Name,
                Description = "Store",
                Route = $"/stores/{s.Id}"
            }));


            // PRODUCTS
            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(query))
                .Take(5)
                .ToListAsync();

            result.Results.AddRange(products.Select(p => new GlobalSearchItemDto
            {
                Type = "Product",
                Id = p.Id,
                Title = p.Name,
                Description = "Product",
                Route = $"/products/{p.Id}"
            }));


            // ORDERS
            var orders = await _context.Orders
                .Where(o => o.OrderNumber.ToLower().Contains(query))
                .Take(5)
                .ToListAsync();

            result.Results.AddRange(orders.Select(o => new GlobalSearchItemDto
            {
                Type = "Order",
                Id = o.Id,
                Title = o.OrderNumber,
                Description = "Order",
                Route = $"/orders/{o.Id}"
            }));


            // DELIVERY PARTNERS
            //if (_context.DeliveryPartners != null)
            //{
            //var delivery = await _context.DeliveryPartners
            //        .Where(d =>
            //          d.Name.ToLower().Contains(query) ||
            //        d.PhoneNumber.Contains(query))
            //  .Take(5)
            //.ToListAsync();

            //result.Results.AddRange(delivery.Select(d => new GlobalSearchItemDto
            //{
            //  Type = "DeliveryPartner",
            //Id = d.Id,
            // Title = d.Name,
            //Description = "Delivery Partner",
            //Route = $"/delivery/{d.Id}"
            //}));
            //}


            // CATEGORIES
            if (_context.Categories != null)
            {
                var categories = await _context.Categories
                    .Where(c => c.Name.ToLower().Contains(query))
                    .Take(5)
                    .ToListAsync();

                result.Results.AddRange(categories.Select(c => new GlobalSearchItemDto
                {
                    Type = "Category",
                    Id = c.Id,
                    Title = c.Name,
                    Description = "Product Category",
                    Route = $"/categories/{c.Id}"
                }));
            }

            return result;
        }
    }
}