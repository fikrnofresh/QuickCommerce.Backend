using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.AdminRoles;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services; // ✅ ADDED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AdminRoleService : IAdminRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService; // ✅ ADDED

        public AdminRoleService(
            ApplicationDbContext context,
            AuditLogService auditLogService) // ✅ UPDATED
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto)
        {
            // ==========================
            // Duplicate Validation
            // ==========================
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r =>
                    !r.IsDeleted &&
                    r.Name.ToLower() == dto.Name.ToLower());

            if (existingRole != null)
                throw new Exception("Role name already exists.");

            // ==========================
            // Create Role
            // ==========================
            var role = new Role
            {
                Name = dto.Name.Trim(),
                Description = dto.Description,
                Scope = "STORE",
                IsSystemRole = false,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);

            await _context.SaveChangesAsync();

            // ==========================
            // Audit
            // ==========================
            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "CREATE_ROLE",
                entityType: "Role",
                entityId: role.Id,
                description: $"Role '{role.Name}' created."
            );

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        public async Task<IEnumerable<RoleResponseDto>> GetRolesAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Id == id && !r.IsDeleted)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRoleAsync(int id, UpdateRoleDto dto)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
                return false;

            // Prevent duplicate role names
            var duplicate = await _context.Roles.AnyAsync(r =>
                r.Id != id &&
                !r.IsDeleted &&
                r.Name.ToLower() == dto.Name.ToLower());

            if (duplicate)
                throw new Exception("Role name already exists.");

            // Prevent modification of system roles
            if (role.IsSystemRole)
                throw new Exception("System roles cannot be modified.");

            role.Name = dto.Name.Trim();
            role.Description = dto.Description;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "UPDATE_ROLE",
                entityType: "Role",
                entityId: role.Id,
                description: $"Role '{role.Name}' updated."
            );

            return true;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
                return false;

            // Prevent deleting system roles
            if (role.IsSystemRole)
                throw new Exception("System roles cannot be deleted.");

            // Prevent deleting roles assigned to users
            if (role.UserRoles.Any())
                throw new Exception("Role is assigned to one or more users and cannot be deleted.");

            // Enterprise Soft Delete
            role.IsDeleted = true;
            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "SOFT_DELETE_ROLE",
                entityType: "Role",
                entityId: role.Id,
                description: $"Role '{role.Name}' soft deleted."
            );

            return true;
        }
    }
}