using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class DeliveryPartnerService : IDeliveryPartnerService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryPartnerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryPartner> CreateAsync(DeliveryPartner partner)
        {
            partner.CreatedAt = DateTime.UtcNow;
            partner.UpdatedAt = DateTime.UtcNow;
            partner.IsActive = true;
            partner.IsAvailable = false;
            partner.IsVerified = true;

            await _context.DeliveryPartners.AddAsync(partner);
            await _context.SaveChangesAsync();

            return partner;
        }

        public async Task<List<DeliveryPartner>> GetAllAsync()
        {
            return await _context.DeliveryPartners.ToListAsync();
        }

        public async Task UpdateAvailabilityAsync(int partnerId, bool isAvailable)
        {
            var partner = await _context.DeliveryPartners.FindAsync(partnerId);
            if (partner == null)
                throw new Exception("Partner not found");

            partner.IsAvailable = isAvailable;
            partner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateLocationAsync(int partnerId, decimal lat, decimal lng)
        {
            var partner = await _context.DeliveryPartners.FindAsync(partnerId);
            if (partner == null)
                throw new Exception("Partner not found");

            partner.CurrentLatitude = lat;
            partner.CurrentLongitude = lng;
            partner.LastLocationUpdate = DateTime.UtcNow;
            partner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}