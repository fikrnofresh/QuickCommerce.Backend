using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/delivery")]
    [Authorize]
    public class DeliveryPartnerController : ControllerBase
    {
        private readonly IDeliveryAppService _service;

        public DeliveryPartnerController(IDeliveryAppService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new System.Exception("Invalid token");

            return int.Parse(userIdClaim);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var result = await _service.GetMyOrdersAsync(userId);

            return Ok(result);
        }

        [HttpPost("{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var userId = GetUserId();

            await _service.AcceptOrderAsync(userId, orderId);

            return Ok(new { message = "Order accepted" });
        }

        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            var userId = GetUserId();

            await _service.RejectOrderAsync(userId, orderId);

            return Ok(new { message = "Order rejected" });
        }

        [HttpGet("earnings")]
        public async Task<IActionResult> GetEarnings()
        {
            var userId = GetUserId();
            var result = await _service.GetEarningsAsync(userId);

            return Ok(result);
        }
    }
}