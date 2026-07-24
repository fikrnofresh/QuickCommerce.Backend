using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.StoreUsers;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class StoreUserService : IStoreUserService
    {
        private readonly ApplicationDbContext _context;

        public StoreUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StoreUserResponseDto> CreateStoreUserAsync(int storeId, CreateStoreUserDto dto)
        {
            var user = new User
            {
                PhoneNumber = dto.PhoneNumber,
                IsPhoneVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = dto.RoleId,
                GrantedAt = DateTime.UtcNow
            };

            await _context.UserRoles.AddAsync(userRole);

            var userStore = new UserStore
            {
                UserId = user.Id,
                StoreId = storeId,
                AssignedAt = DateTime.UtcNow
            };

            await _context.UserStores.AddAsync(userStore);

            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstAsync(r => r.Id == dto.RoleId);

            return new StoreUserResponseDto
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                Role = role.Name
            };
        }

        public async Task<IEnumerable<StoreUserResponseDto>> GetStoreUsersAsync(int storeId)
        {
            return await _context.UserStores
                .Where(us => us.StoreId == storeId)
                .Join(_context.Users,
                    us => us.UserId,
                    u => u.Id,
                    (us, u) => new { us, u })
                .Join(_context.UserRoles,
                    temp => temp.u.Id,
                    ur => ur.UserId,
                    (temp, ur) => new { temp, ur })
                .Join(_context.Roles,
                    temp => temp.ur.RoleId,
                    r => r.Id,
                    (temp, r) => new StoreUserResponseDto
                    {
                        UserId = temp.temp.u.Id,
                        PhoneNumber = temp.temp.u.PhoneNumber,
                        Role = r.Name
                    })
                .ToListAsync();
        }

        public async Task<bool> UpdateStoreUserAsync(int storeId, int userId, UpdateStoreUserDto dto)
        {
            var role = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (role == null)
                return false;

            role.RoleId = dto.RoleId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveStoreUserAsync(int storeId, int userId)
        {
            var mapping = await _context.UserStores
                .FirstOrDefaultAsync(x => x.StoreId == storeId && x.UserId == userId);

            if (mapping == null)
                return false;

            _context.UserStores.Remove(mapping);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}