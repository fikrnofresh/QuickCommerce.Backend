using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuickCommerce.Infrastructure.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 💰 GET WALLET
        public async Task<Wallet?> GetWalletAsync(int userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // 💰 ADD WALLET TRANSACTION
        public async Task AddWalletTransactionAsync(int userId, decimal amount, string type, string reference)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0
                };

                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            if (type == "CREDIT")
                wallet.Balance += amount;
            else
                wallet.Balance -= amount;

            wallet.UpdatedAt = DateTime.UtcNow;

            var transaction = new WalletTransaction
            {
                UserId = userId,
                Amount = amount,
                Type = type,
                Reference = reference
            };

            _context.WalletTransactions.Add(transaction);

            await _context.SaveChangesAsync();
        }

        // 🎫 CREATE SUPPORT
        public async Task<SupportTicket> CreateTicketAsync(int userId, string subject, string message)
        {
            var ticket = new SupportTicket
            {
                UserId = userId,
                Subject = subject,
                Message = message
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        // 🎫 GET SUPPORT
        public async Task<List<SupportTicket>> GetTicketsAsync(int userId)
        {
            return await _context.SupportTickets
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<RecommendationDto>> GetRecommendationsAsync(int userId)
        {
            var activities = await _context.CustomerActivities
                .Where(x => x.UserId == userId && x.EntityId != null)
                .ToListAsync();

            var grouped = activities
                .GroupBy(x => x.EntityId)
                .Select(g =>
                {
                    int score = g.Sum(x =>
                    {
                        return x.Action switch
                        {
                            "VIEW" => 1,
                            "ADD_TO_CART" => 3,
                            "ORDER" => 5,
                            _ => 0
                        };
                    });

                    return new RecommendationDto
                    {
                        ProductId = g.Key!.Value,
                        Type = "AI",
                        Score = score
                    };
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .ToList();

            return grouped;
        }
        public async Task<List<ReorderRecommendationDto>> GetReorderRecommendations(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId && o.Status == "DELIVERED")
                .Include(o => o.OrderItems)
                .ToListAsync();

            if (!orders.Any()) return new List<ReorderRecommendationDto>();

            var lastStoreId = orders
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.StoreId)
                .FirstOrDefault();

            var productGroups = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.ProductId);

            var productIds = productGroups.Select(g => g.Key).ToList();

            var storeProducts = await _context.StoreProducts
                .Include(sp => sp.Product)
                .Where(sp => productIds.Contains(sp.ProductId) && sp.StoreId == lastStoreId)
                .ToListAsync();

            return productGroups.Select(g =>
            {
                var orderCount = g.Count();
                var lastOrderDate = g.Max(x => x.CreatedAt);

                var days = (DateTime.UtcNow - lastOrderDate).Days;

                int recency = days switch
                {
                    <= 2 => 5,
                    <= 7 => 3,
                    <= 15 => 2,
                    _ => 1
                };

                var score = (orderCount * 2) + recency;

                var sp = storeProducts.FirstOrDefault(x => x.ProductId == g.Key);

                return new ReorderRecommendationDto
                {
                    ProductId = g.Key,
                    ProductName = sp?.Product?.Name,
                    StoreProductId = sp?.Id ?? 0,
                    Price = sp?.Price ?? 0,
                    Unit = sp?.Product?.Unit ?? "",
                    ImageUrl = null,
                    OrderCount = orderCount,
                    LastOrderedAt = lastOrderDate,
                    Score = score
                };
            })
            .Where(x => x.StoreProductId != 0)
            .OrderByDescending(x => x.Score)
            .Take(10)
            .ToList();
        }
        public async Task<List<BundleRecommendationDto>> GetFrequentlyBoughtTogether(int productId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(i => i.ProductId == productId))
                .ToListAsync();

            var related = orders
                .SelectMany(o => o.OrderItems)
                .Where(i => i.ProductId != productId)
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var ids = related.Select(x => x.ProductId).ToList();

            var storeProducts = await _context.StoreProducts
                .Include(sp => sp.Product)
                .Where(sp => ids.Contains(sp.ProductId))
                .ToListAsync();

            return related.Select(r =>
            {
                var sp = storeProducts.FirstOrDefault(x => x.ProductId == r.ProductId);

                return new BundleRecommendationDto
                {
                    ProductId = r.ProductId,
                    ProductName = sp?.Product?.Name,
                    StoreProductId = sp?.Id ?? 0,
                    Price = sp?.Price ?? 0,
                    Unit = sp?.Product?.Unit ?? "",
                    ImageUrl = null,
                    Frequency = r.Count
                };
            })
            .Where(x => x.StoreProductId != 0)
            .ToList();
        }
        public async Task<List<SearchSuggestionDto>> GetSearchSuggestions(string query, int userId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchSuggestionDto>();

            query = query.ToLower();

            var products = await _context.StoreProducts
                .Include(p => p.Product)
                .Where(p => p.Product.Name.ToLower().Contains(query))
                .Take(10)
                .ToListAsync();

            var activities = await _context.CustomerActivities
                .Where(x => x.UserId == userId && x.Action == "SEARCH")
                .ToListAsync();

            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            return products.Select(p =>
            {
                int score = 1;

                if (activities.Any(a => a.Metadata != null && a.Metadata.Contains(p.Product.Name)))
                    score += 3;

                if (orders.Any(o => o.OrderItems.Any(i => i.ProductId == p.ProductId)))
                    score += 5;

                return new SearchSuggestionDto
                {
                    ProductId = p.ProductId,
                    Name = p.Product.Name,
                    ImageUrl = null,
                    Price = p.Price,
                    Score = score
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();
        }
        public async Task<List<FinalRecommendationDto>> GetSmartRecommendations(int userId)
        {
            var reorder = await GetReorderRecommendations(userId);

            return reorder.Select(r => new FinalRecommendationDto
            {
                ProductId = r.ProductId,
                Name = r.ProductName,
                ImageUrl = r.ImageUrl,
                Price = r.Price,
                Score = r.Score * 0.4
            })
            .OrderByDescending(x => x.Score)
            .ToList();
        }

    }

}