using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Delivery;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System.Threading.Tasks;
using System.Linq;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/delivery-partners")]
    [Authorize]
    public class DeliveryPartnersController : ControllerBase
    {
        private readonly IDeliveryPartnerService _service;
        private readonly ApplicationDbContext _context;

        // ✅ FIXED CONSTRUCTOR
        public DeliveryPartnersController(
            IDeliveryPartnerService service,
            ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDeliveryPartnerDto dto)
        {
            var partner = new DeliveryPartner
            {
                UserId = dto.UserId,
                VehicleType = dto.VehicleType,
                VehicleNumber = dto.VehicleNumber,
                DrivingLicenseNumber = dto.DrivingLicenseNumber,
                AadharNumber = dto.AadharNumber
            };

            var result = await _service.CreateAsync(partner);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, UpdateAvailabilityDto dto)
        {
            await _service.UpdateAvailabilityAsync(id, dto.IsAvailable);
            return Ok(new { message = "Availability updated" });
        }

        [HttpPatch("{id}/location")]
        public async Task<IActionResult> UpdateLocation(int id, UpdateLocationDto dto)
        {
            await _service.UpdateLocationAsync(id, dto.Latitude, dto.Longitude);
            return Ok(new { message = "Location updated" });
        }

        // 🔥 PERFORMANCE API (FINAL)
        [HttpGet("{id}/performance")]
        public async Task<IActionResult> GetPerformance(int id)
        {
            var partner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partner == null)
                return NotFound();

            var deliveries = await _context.Deliveries
                .Where(d => d.AssignedToPartnerId == id)
                .ToListAsync();

            var completed = deliveries
                .Where(d => d.Status == "DELIVERED")
                .ToList();

            var failed = deliveries
                .Count(d => d.Status == "FAILED");

            var avgTime = completed.Any()
                ? completed.Average(d =>
                    (d.DeliveredAt.Value - d.AssignedAt.Value).TotalMinutes)
                : 0;

            var earnings = completed.Sum(d => d.PartnerEarnings ?? 0);

            return Ok(new
            {
                totalDeliveries = deliveries.Count,
                completedDeliveries = completed.Count,
                failedDeliveries = failed,
                avgDeliveryTimeMinutes = Math.Round(avgTime, 2),
                totalEarnings = earnings,
                rating = partner.Rating ?? 0
            });
        }
    }
}