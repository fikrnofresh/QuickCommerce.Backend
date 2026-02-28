using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.Interfaces;
using System.Security.Claims;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // CUSTOMER ONLY
        [Authorize(Roles = "CUSTOMER")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            request.CustomerId = userId;

            var order = await _orderService.CreateOrderAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        // ADMIN ONLY
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            await _orderService.UpdateOrderStatusAsync(id, request.Status.ToString());
            return Ok("Order status updated successfully");
        }
    }
}