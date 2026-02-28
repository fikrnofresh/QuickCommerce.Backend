using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/deliveries")]
    [Authorize]
    public class DeliveriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DeliveryAssignmentService _deliveryService;

        public DeliveriesController(
            ApplicationDbContext context,
            DeliveryAssignmentService deliveryService)
        {
            _context = context;
            _deliveryService = deliveryService;
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN,DELIVERY_PARTNER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .Include(d => d.DeliveryPartner)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
                return NotFound();

            return Ok(delivery);
        }

        [Authorize(Roles = "DELIVERY_PARTNER,ADMIN,SUPER_ADMIN")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateDeliveryStatusDto request)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
                return NotFound();

            await _deliveryService.UpdateDeliveryStatusAsync(
                delivery.OrderId,
                request.Status.ToString()
            );

            return NoContent();
        }
    }
}