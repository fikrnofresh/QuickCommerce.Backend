using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Entities;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/store-products")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public class StoreProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoreProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================
        // GET STORE PRODUCTS
        // =============================
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetStoreProducts(int storeId)
        {
            var products = await _context.StoreProducts
                .Include(x => x.Product)
                .Where(x => x.StoreId == storeId)
                .ToListAsync();

            return Ok(products);
        }

        // =============================
        // ADD PRODUCT TO STORE
        // =============================
        [HttpPost]
        public async Task<IActionResult> AddProductToStore(StoreProduct model)
        {
            model.CreatedAt = DateTime.UtcNow;

            await _context.StoreProducts.AddAsync(model);

            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =============================
        // UPDATE STOCK
        // =============================
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            var sp = await _context.StoreProducts.FindAsync(id);

            if (sp == null)
                return NotFound();

            sp.StockQuantity = quantity;
            sp.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(sp);
        }

        // =============================
        // UPDATE PRICE
        // =============================
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, decimal price)
        {
            var sp = await _context.StoreProducts.FindAsync(id);

            if (sp == null)
                return NotFound();

            sp.Price = price;
            sp.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(sp);
        }

        // =============================
        // TOGGLE AVAILABILITY
        // =============================
        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, bool status)
        {
            var sp = await _context.StoreProducts.FindAsync(id);

            if (sp == null)
                return NotFound();

            sp.IsAvailable = status;
            sp.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(sp);
        }
    }
}