using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customer/orders")]
    [Authorize]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly ICustomerOrderService _service;

        public CustomerOrdersController(ICustomerOrderService service)
        {
            _service = service;
        }

        // =========================
        // GET CUSTOMER ORDERS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var orders = await _service.GetOrders(userId);

            return Ok(orders);
        }

        // =========================
        // GET ORDER DETAILS
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var order = await _service.GetOrderDetail(userId, id);

            return Ok(order);
        }

        // =========================
        // ORDER TRACKING
        // =========================
        [Authorize(Roles = "CUSTOMER,SUPER_ADMIN")]
        [HttpGet("{id}/tracking")]
        public async Task<IActionResult> GetOrderTracking(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var tracking = await _service.GetOrderTracking(userId, id);

            return Ok(tracking);
        }
    }
}