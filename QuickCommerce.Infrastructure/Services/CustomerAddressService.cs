using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces.Services;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ApplicationDbContext _context;

        public CustomerAddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerAddressDto>> GetAddresses(int userId)
        {
            return await _context.CustomerAddresses
                .Where(x => x.UserId == userId)
                .Select(x => new CustomerAddressDto
                {
                    Id = x.Id,
                    Label = x.Label,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    City = x.City,
                    State = x.State,
                    Pincode = x.Pincode,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    IsDefault = x.IsDefault
                })
                .ToListAsync();
        }

        public async Task<CustomerAddressDto> CreateAddress(int userId, CustomerAddressDto dto)
        {
            var entity = new CustomerAddress
            {
                UserId = userId,
                Label = dto.Label,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsDefault = dto.IsDefault
            };

            _context.CustomerAddresses.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAddress(int id, int userId)
        {
            var address = await _context.CustomerAddresses
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}