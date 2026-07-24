using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.AdminPermissions;
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
    public class AdminPermissionService : IAdminPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService; // ✅ ADDED

        public AdminPermissionService(
            ApplicationDbContext context,
            AuditLogService auditLogService) // ✅ UPDATED
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<PermissionResponseDto>> GetPermissionsAsync()
        {
            return await _context.Permissions
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => new PermissionResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PermissionResponseDto>> GetRolePermissionsAsync(int roleId)
        {
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == roleId && !r.IsDeleted && r.IsActive);

            if (!roleExists)
                throw new Exception("Role not found.");

            return await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Join(
                    _context.Permissions.Where(p => p.IsActive),
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => new PermissionResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    })
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> AssignPermissionAsync(int roleId, int permissionId)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted && r.IsActive);

            if (role == null)
                throw new Exception("Role not found.");

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == permissionId && p.IsActive);

            if (permission == null)
                throw new Exception("Permission not found.");

            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (exists)
                throw new Exception("Permission is already assigned to this role.");

            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "ASSIGN_PERMISSION",
                entityType: "Role",
                entityId: roleId,
                description: $"Permission '{permission.Name}' assigned to role '{role.Name}'."
            );

            return true;
        }

        public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted && r.IsActive);

            if (role == null)
                throw new Exception("Role not found.");

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == permissionId);

            if (permission == null)
                throw new Exception("Permission not found.");

            var mapping = await _context.RolePermissions
                .FirstOrDefaultAsync(rp =>
                    rp.RoleId == roleId &&
                    rp.PermissionId == permissionId);

            if (mapping == null)
                throw new Exception("Permission is not assigned to this role.");

            _context.RolePermissions.Remove(mapping);

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "REMOVE_PERMISSION",
                entityType: "Role",
                entityId: roleId,
                description: $"Permission '{permission.Name}' removed from role '{role.Name}'."
            );

            return true;
        }
    }
}