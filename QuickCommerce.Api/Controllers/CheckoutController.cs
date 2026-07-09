using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customer/checkout")]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _service;

        public CheckoutController(ICheckoutService service)
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

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutRequestDto dto)
        {
            var userId = GetUserId();

            var result = await _service.Checkout(userId, dto);

            return Ok(result);
        }
    }
}