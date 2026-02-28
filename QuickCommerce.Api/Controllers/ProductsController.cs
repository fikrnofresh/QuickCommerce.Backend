using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // =========================
        // GET ALL PRODUCTS (Catalog Only)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetProducts(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync(categoryId);
            return Ok(products);
        }

        // =========================
        // GET PRODUCT BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // =========================
        // CREATE PRODUCT (Catalog Only)
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // =========================
        // UPDATE PRODUCT (Catalog Only)
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.MRP = updatedProduct.MRP;
            existingProduct.Unit = updatedProduct.Unit;
            existingProduct.IsAvailable = updatedProduct.IsAvailable;
            existingProduct.CategoryId = updatedProduct.CategoryId;
            existingProduct.ImageUrls = updatedProduct.ImageUrls;
            existingProduct.SearchKeywords = updatedProduct.SearchKeywords;

            await _productRepository.UpdateAsync(existingProduct);
            return NoContent();
        }

        // =========================
        // DELETE PRODUCT
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productRepository.DeleteAsync(product);
            return NoContent();
        }
    }
}