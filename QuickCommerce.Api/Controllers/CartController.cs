using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customer/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst("userId");

            if (claim == null)
                throw new UnauthorizedAccessException();

            return int.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var cart = await _service.GetCart(userId);

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(AddCartItemDto dto)
        {
            var userId = GetUserId();

            var cart = await _service.AddItem(userId, dto);

            return Ok(cart);
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, int quantity)
        {
            var userId = GetUserId();

            var cart = await _service.UpdateItem(userId, id, quantity);

            return Ok(cart);
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var userId = GetUserId();

            await _service.RemoveItem(userId, id);

            return Ok();
        }
    }
}