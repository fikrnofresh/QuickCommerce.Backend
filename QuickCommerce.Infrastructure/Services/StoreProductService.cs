using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Common;
using QuickCommerce.Core.DTOs.Store;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class StoreProductService : IStoreProductService
    {
        private readonly ApplicationDbContext _context;

        public StoreProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<StoreProductListDto>> GetStoreProductsAsync(
            int storeId,
            int pageNumber,
            int pageSize,
            int? categoryId,
            string? search)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            // ✅ NEW STRUCTURE
            var baseQuery = _context.StoreProducts
                .AsNoTracking()
                .Where(sp => sp.StoreId == storeId)
                .Include(sp => sp.Product)
                .ThenInclude(p => p.Category)
                .Where(sp => sp.Product.IsAvailable);

            if (categoryId.HasValue)
            {
                baseQuery = baseQuery
                    .Where(sp => sp.Product.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                baseQuery = baseQuery.Where(sp =>
                    sp.Product.Name.ToLower().Contains(search) ||
                    (sp.Product.SearchKeywords != null &&
                     sp.Product.SearchKeywords.ToLower().Contains(search)));
            }

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .OrderBy(sp => sp.Product.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => new StoreProductListDto
                {
                    ProductId = sp.Product.Id,
                    Name = sp.Product.Name,
                    Description = sp.Product.Description,
                    Price = sp.Product.Price,
                    MRP = sp.Product.MRP,
                    Unit = sp.Product.Unit,
                    AvailableStock = sp.StockQuantity,
                    IsLowStock = sp.StockQuantity <= sp.LowStockThreshold,
                    IsAvailable = sp.StockQuantity > 0,
                    CategoryName = sp.Product.Category.Name,
                    ImageUrls = sp.Product.ImageUrls
                })
                .ToListAsync();

            return new PagedResultDto<StoreProductListDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            };
        }
    }
}