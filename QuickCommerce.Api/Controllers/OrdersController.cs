using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Core.Interfaces.Services;

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

        // =========================
        // CREATE ORDER (CUSTOMER)
        // =========================
        [Authorize(Roles = "CUSTOMER,SUPER_ADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            request.CustomerId = userId;

            var order = await _orderService.CreateOrderAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        // =========================
        // GET ORDER BY ID
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // =========================
        // UPDATE ORDER STATUS
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);

            await _orderService.UpdateOrderStatusAsync(
                id,
                request.Status.ToString(),
                userId,
                "Updated by admin"
            );

            return Ok("Order status updated successfully");
        }

        // =========================
        // ADMIN ORDER LIST
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("admin/list")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // =========================
        // STORE ORDERS
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("admin/store/{storeId}")]
        public async Task<IActionResult> GetOrdersByStore(int storeId)
        {
            var orders = await _orderService.GetOrdersByStoreAsync(storeId);

            return Ok(orders);
        }

        // =========================
        // CANCEL ORDER
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            await _orderService.UpdateOrderStatusAsync(id, "CANCELLED", userId, "Order cancelled by admin");

            return Ok(new { message = "Order cancelled" });
        }

        // =========================
        // REFUND ORDER
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPatch("{id}/refund")]
        public async Task<IActionResult> RefundOrder(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            await _orderService.UpdateOrderStatusAsync(id, "REFUNDED", userId, "Order refunded");

            return Ok(new { message = "Order refunded" });
        }

        // =========================
        // CUSTOMER ORDERS
        // =========================
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);

            return Ok(orders);
        }
    }
}