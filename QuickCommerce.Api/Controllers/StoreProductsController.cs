using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stores/{storeId}/products")]
    public class StoreProductsController : ControllerBase
    {
        private readonly IStoreProductService _storeProductService;

        public StoreProductsController(IStoreProductService storeProductService)
        {
            _storeProductService = storeProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreProducts(
            int storeId,
            int pageNumber = 1,
            int pageSize = 10,
            int? categoryId = null,
            string? search = null)
        {
            var result = await _storeProductService.GetStoreProductsAsync(
                storeId,
                pageNumber,
                pageSize,
                categoryId,
                search);

            return Ok(result);
        }
    }
}