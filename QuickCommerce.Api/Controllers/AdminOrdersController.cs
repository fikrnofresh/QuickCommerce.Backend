using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/orders")]
    [Authorize]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderManagementService _service;

        public AdminOrdersController(IOrderManagementService service)
        {
            _service = service;
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            await _service.UpdateOrderStatusAsync(id, dto.Status, null, dto.Remarks);

            return Ok(new { message = "Order status updated successfully" });
        }
    }
}