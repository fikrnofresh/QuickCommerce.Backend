using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
	[ApiController]
	[Route("api/v1/deliveries")]
	public class DeliveriesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public DeliveriesController(ApplicationDbContext context)
		{
			_context = context;
		}

		// =========================
		// GET DELIVERY BY ID
		// =========================
		[HttpGet("{id}")]
		public async Task<IActionResult> GetDelivery(int id)
		{
			var delivery = await _context.Deliveries
				.Include(d => d.Order)
				.Include(d => d.ExternalAgent) // ✅ FIXED
				.FirstOrDefaultAsync(d => d.Id == id);

			if (delivery == null)
				return NotFound();

			return Ok(delivery);
		}

		// =========================
		// UPDATE DELIVERY STATUS
		// =========================
		[HttpPut("{id}/status")]
		public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
		{
			var delivery = await _context.Deliveries
				.Include(d => d.ExternalAgent) // ✅ FIXED
				.Include(d => d.Order)
				.FirstOrDefaultAsync(d => d.Id == id);

			if (delivery == null)
				return NotFound();

			if (!Enum.TryParse<DeliveryStatus>(status, true, out var parsedStatus))
				return BadRequest("Invalid status");

			delivery.Status = parsedStatus.ToString();
			delivery.UpdatedAt = DateTime.UtcNow;

			switch (parsedStatus)
			{
				case DeliveryStatus.ACCEPTED:
					delivery.AcceptedAt = DateTime.UtcNow;
					break;

				case DeliveryStatus.PICKED_UP:
					delivery.PickedUpAt = DateTime.UtcNow;

					if (delivery.Order != null)
					{
						delivery.Order.Status = "OUT_FOR_DELIVERY";
						delivery.Order.UpdatedAt = DateTime.UtcNow;
					}
					break;

				case DeliveryStatus.IN_TRANSIT:
					if (delivery.Order != null)
					{
						delivery.Order.Status = "OUT_FOR_DELIVERY";
						delivery.Order.UpdatedAt = DateTime.UtcNow;
					}
					break;

				case DeliveryStatus.DELIVERED:
					delivery.DeliveredAt = DateTime.UtcNow;

					if (delivery.ExternalAgent != null && delivery.ExternalAgent.CurrentWorkload > 0)
						delivery.ExternalAgent.CurrentWorkload--;

					if (delivery.Order != null)
					{
						delivery.Order.Status = "DELIVERED";
						delivery.Order.UpdatedAt = DateTime.UtcNow;
					}
					break;

				case DeliveryStatus.FAILED:
					if (delivery.ExternalAgent != null && delivery.ExternalAgent.CurrentWorkload > 0)
						delivery.ExternalAgent.CurrentWorkload--;
					break;
			}

			await _context.SaveChangesAsync();

			return Ok(new { message = "Delivery status updated successfully" });
		}

		// =========================
		// ASSIGN DELIVERY MANUALLY
		// =========================
		[HttpPost("assign")]
		public async Task<IActionResult> AssignDelivery([FromBody] int orderId)
		{
			var order = await _context.Orders
				.FirstOrDefaultAsync(o => o.Id == orderId);

			if (order == null)
				return NotFound("Order not found");

			var partner = await _context.DeliveryPartners
				.Where(p => p.IsActive && p.IsAvailable)
				.OrderBy(p => p.CurrentWorkload)
				.FirstOrDefaultAsync();

			if (partner == null)
				return BadRequest("No delivery partner available");

			var delivery = new QuickCommerce.Core.Entities.Delivery
			{
				OrderId = order.Id,
				AssignedToPartnerId = partner.Id, // ✅ FIXED
				AssignedToUserId = null,
				AgentType = DeliveryAgentType.External,

				Status = "ASSIGNED",
				AssignedAt = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			await _context.Deliveries.AddAsync(delivery);

			partner.CurrentWorkload++;

			await _context.SaveChangesAsync();

			return Ok(new { message = "Delivery assigned successfully" });
		}
	}
}