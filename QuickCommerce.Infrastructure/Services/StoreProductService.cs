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

            var baseQuery = _context.StoreProductInventories
                .AsNoTracking()
                .Where(inv => inv.StoreId == storeId)
                .Join(_context.Products.AsNoTracking(),
                    inv => inv.ProductId,
                    product => product.Id,
                    (inv, product) => new { inv, product })
                .Join(_context.Categories.AsNoTracking(),
                    temp => temp.product.CategoryId,
                    category => category.Id,
                    (temp, category) => new
                    {
                        inventory = temp.inv,
                        product = temp.product,
                        category
                    })
                .Where(x => x.product.IsAvailable);

            if (categoryId.HasValue)
            {
                baseQuery = baseQuery
                    .Where(x => x.product.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                baseQuery = baseQuery.Where(x =>
                    x.product.Name.ToLower().Contains(search) ||
                    (x.product.SearchKeywords != null &&
                     x.product.SearchKeywords.ToLower().Contains(search)));
            }

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .OrderBy(x => x.product.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StoreProductListDto
                {
                    ProductId = x.product.Id,
                    Name = x.product.Name,
                    Description = x.product.Description,
                    Price = x.product.Price,
                    MRP = x.product.MRP,
                    Unit = x.product.Unit,
                    AvailableStock = x.inventory.Stock,
                    IsLowStock = x.inventory.Stock <= x.inventory.LowStockThreshold,
                    IsAvailable = x.inventory.Stock > 0,
                    CategoryName = x.category.Name,
                    ImageUrls = x.product.ImageUrls
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