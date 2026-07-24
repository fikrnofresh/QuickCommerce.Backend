using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Common;
using QuickCommerce.Core.DTOs.AdminUsers;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly IAuthService _authService;

        public AdminUserService(
    ApplicationDbContext context,
    AuditLogService auditLogService,
    IAuthService authService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _authService = authService;
        }
        private async Task<bool> EmailExistsAsync(string? email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _context.Users.AnyAsync(x =>
                x.Email == email &&
                (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
        }

        private async Task<bool> PhoneExistsAsync(string? phone, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return await _context.Users.AnyAsync(x =>
                x.PhoneNumber == phone &&
                (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
        }

        private async Task<bool> UsernameExistsAsync(string? username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _context.Users.AnyAsync(x =>
                x.Username == username &&
                (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
        }
        
        private async Task<bool> EmployeeCodeExistsAsync(string? employeeCode, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                return false;

            return await _context.Users.AnyAsync(x =>
                x.EmployeeCode == employeeCode &&
                (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
        }

        // =========================
        // CREATE USER
        // =========================
        public async Task<AdminUserResponseDto> CreateAdminUserAsync(CreateAdminUserDto dto)
        {
            // ==========================
            // VALIDATIONS
            // ==========================

            if (await EmailExistsAsync(dto.Email))
                throw new Exception("Email already exists.");

            if (await PhoneExistsAsync(dto.PhoneNumber))
                throw new Exception("Phone number already exists.");

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                if (await UsernameExistsAsync(dto.UserName))
                    throw new Exception("Username already exists.");
            }

            if (!string.IsNullOrWhiteSpace(dto.EmployeeCode))
            {
                if (await EmployeeCodeExistsAsync(dto.EmployeeCode))
                    throw new Exception("Employee code already exists.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null)
                throw new Exception("Invalid role selected.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ==========================
                // PASSWORD
                // ==========================

                var temporaryPassword = _authService.GenerateTemporaryPassword();

                // ==========================
                // USER
                // ==========================

                var user = new User
                {
                    FullName = dto.FullName,
                    Username = dto.UserName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    EmployeeCode = dto.EmployeeCode,

                    PasswordHash = _authService.HashPassword(temporaryPassword),

                    IsActive = dto.IsActive,
                    IsPhoneVerified = true,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = null,
                    UpdatedBy = null
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // ==========================
                // ROLE
                // ==========================

                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = dto.RoleId,
                    GrantedAt = DateTime.UtcNow
                });

                // ==========================
                // STORE MAPPING
                // ==========================

                if (dto.StoreId.HasValue)
                {
                    var storeExists = await _context.Stores
                        .AnyAsync(s => s.Id == dto.StoreId.Value);

                    if (!storeExists)
                        throw new Exception("Store not found.");

                    await _context.UserStores.AddAsync(new UserStore
                    {
                        UserId = user.Id,
                        StoreId = dto.StoreId.Value
                    });
                }

                await _context.SaveChangesAsync();

                // ==========================
                // AUDIT
                // ==========================

                await _auditLogService.LogAsync(
                    userId: null,
                    module: "USERS",
                    action: "CREATE_USER",
                    entityType: "User",
                    entityId: user.Id,
                    description: $"Admin user '{user.FullName}' created with role '{role.Name}'."
                );

                await transaction.CommitAsync();

                // ==========================
                // RESPONSE
                // ==========================

                return new AdminUserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmployeeCode = user.EmployeeCode,
                    StoreId = dto.StoreId,
                    RoleId = role.Id,
                    Role = role.Name,
                    IsActive = user.IsActive,
                    TemporaryPassword = temporaryPassword
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResponse<AdminUserResponseDto>> GetAllAdminUsersAsync(AdminUserFilterDto filter)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .AsQueryable();

            // ==========================
            // SEARCH
            // ==========================

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();

                query = query.Where(x =>
                    (x.FullName != null && x.FullName.ToLower().Contains(search)) ||
                    (x.Email != null && x.Email.ToLower().Contains(search)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(search)) ||
                    (x.EmployeeCode != null && x.EmployeeCode.Contains(search)));
            }

            // ==========================
            // STATUS
            // ==========================

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == filter.IsActive.Value);
            }

            // ==========================
            // ROLE
            // ==========================

            if (filter.RoleId.HasValue)
            {
                query = query.Where(x =>
                    x.UserRoles.Any(r => r.RoleId == filter.RoleId.Value));
            }

            // ==========================
            // STORE
            // ==========================

            if (filter.StoreId.HasValue)
            {
                query = query.Where(x =>
                    x.UserStores.Any(s => s.StoreId == filter.StoreId.Value));
            }

            // ==========================
            // SORTING
            // ==========================

            switch (filter.SortBy?.ToLower())
            {
                case "email":
                    query = filter.Descending
                        ? query.OrderByDescending(x => x.Email)
                        : query.OrderBy(x => x.Email);
                    break;

                case "createdat":
                    query = filter.Descending
                        ? query.OrderByDescending(x => x.CreatedAt)
                        : query.OrderBy(x => x.CreatedAt);
                    break;

                default:
                    query = filter.Descending
                        ? query.OrderByDescending(x => x.FullName)
                        : query.OrderBy(x => x.FullName);
                    break;
            }

            // ==========================
            // TOTAL
            // ==========================

            var totalRecords = await query.CountAsync();

            // ==========================
            // PAGINATION
            // ==========================

            var users = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName ?? "",
                    Email = u.Email ?? "",
                    PhoneNumber = u.PhoneNumber ?? "",
                    EmployeeCode = u.EmployeeCode,
                    IsActive = u.IsActive,
                    CreatedOn = u.CreatedAt,
                    UpdatedOn = u.UpdatedAt,
                    LastLoginOn = u.LastLoginAt,

                    RoleId = u.UserRoles
                        .Select(r => r.RoleId)
                        .FirstOrDefault(),

                    Role = u.UserRoles
                        .Select(r => r.Role.Name)
                        .FirstOrDefault() ?? ""
                })
                .ToListAsync();

            return PagedResponse<AdminUserResponseDto>.Create(
                users,
                filter.PageNumber,
                filter.PageSize,
                totalRecords,
                "Users fetched successfully.");
        }

        public async Task<AdminUserResponseDto?> GetAdminUserByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id && !u.IsDeleted)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Select(u => new AdminUserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    EmployeeCode = u.EmployeeCode,
                    IsActive = u.IsActive,

                    RoleId = u.UserRoles
                        .Select(x => x.RoleId)
                        .FirstOrDefault(),

                    Role = u.UserRoles
                        .Select(x => x.Role.Name)
                        .FirstOrDefault() ?? ""
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAdminUserAsync(int id, UpdateAdminUserDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return false;

            // Duplicate validation
            if (await EmailExistsAsync(dto.Email, id))
                throw new Exception("Email already exists.");

            if (await PhoneExistsAsync(dto.PhoneNumber, id))
                throw new Exception("Phone number already exists.");

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                if (await UsernameExistsAsync(dto.UserName, id))
                    throw new Exception("Username already exists.");
            }

            // Update profile
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Username = dto.UserName;
            user.EmployeeCode = dto.EmployeeCode;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update Role
            var userRole = user.UserRoles.FirstOrDefault();

            if (userRole != null)
            {
                userRole.RoleId = dto.RoleId;
            }
            else
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = dto.RoleId,
                    GrantedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: user.Id,
                module: "USERS",
                action: "UPDATE_ADMIN_USER",
                entityType: "User",
                entityId: user.Id,
                description: $"Admin user '{user.FullName}' updated."
            );

            return true;
        }

        public async Task<bool> DeleteAdminUserAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return false;

            // Enterprise Soft Delete
            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: id,
                module: "USERS",
                action: "SOFT_DELETE_USER",
                entityType: "User",
                entityId: id,
                description: $"User '{user.FullName}' soft deleted."
            );

            return true;
        }

        public async Task<object> UpdateUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted);

            if (user == null)
                throw new Exception("User not found.");

            if (user.IsActive == isActive)
            {
                return new
                {
                    message = isActive
                        ? "User is already active."
                        : "User is already inactive."
                };
            }

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: userId,
                module: "USERS",
                action: isActive ? "ACTIVATE_USER" : "DEACTIVATE_USER",
                entityType: "User",
                entityId: userId,
                description: isActive
                    ? $"User '{user.FullName}' activated."
                    : $"User '{user.FullName}' deactivated."
            );

            return new
            {
                message = isActive
                    ? "User activated successfully."
                    : "User deactivated successfully."
            };
        }

        public async Task<object> UpdateUserRoleAsync(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                throw new Exception("User not found.");

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                throw new Exception("Role not found.");

            var existingRole = user.UserRoles.FirstOrDefault();

            var oldRole = existingRole?.RoleId;

            if (existingRole != null)
            {
                existingRole.RoleId = roleId;
            }
            else
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    GrantedAt = DateTime.UtcNow
                });
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: userId,
                module: "RBAC",
                action: "UPDATE_ROLE",
                entityType: "User",
                entityId: userId,
                description: $"Role changed from [{oldRole}] to [{role.Name}]"
            );

            return new
            {
                message = "User role updated successfully."
            };
        }

        public async Task<object> UpdateUserPermissionsAsync(int roleId, string permissionsJson)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                throw new Exception("Role not found.");

            var permissions = System.Text.Json.JsonSerializer
                .Deserialize<List<string>>(permissionsJson);

            _context.RolePermissions.RemoveRange(role.RolePermissions);

            if (permissions != null && permissions.Any())
            {
                var dbPermissions = await _context.Permissions
                    .Where(p => permissions.Contains(p.Name))
                    .ToListAsync();

                foreach (var permission in dbPermissions)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: null,
                module: "RBAC",
                action: "UPDATE_PERMISSIONS",
                entityType: "Role",
                entityId: role.Id,
                description: $"Permissions updated for role '{role.Name}'.",
                metadata: permissionsJson
            );

            return new
            {
                message = "Permissions updated successfully."
            };
        }
    }
}