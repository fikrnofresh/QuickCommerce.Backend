using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AdminAuditService : IAdminAuditService
    {
        private readonly ApplicationDbContext _context;

        public AdminAuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL LOGS
        // =========================
        public async Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsAsync()
        {
            return await _context.ActivityLogs
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AuditLogResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Module = x.Module,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Description = x.Description,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // =========================
        // GET BY USER
        // =========================
        public async Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByUserAsync(int userId)
        {
            return await _context.ActivityLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AuditLogResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Module = x.Module,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Description = x.Description,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // =========================
        // GET BY MODULE
        // =========================
        public async Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByModuleAsync(string module)
        {
            return await _context.ActivityLogs
                .Where(x => x.Module == module)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AuditLogResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Module = x.Module,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Description = x.Description,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // =========================
        // GET BY ENTITY
        // =========================
        public async Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByEntityAsync(string entityType, int entityId)
        {
            return await _context.ActivityLogs
                .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AuditLogResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Module = x.Module,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Description = x.Description,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // =========================
        // 🔥 FILTER + PAGINATION
        // =========================
        public async Task<object> GetFilteredLogsAsync(AuditLogFilterDto filter)
        {
            var query = _context.ActivityLogs.AsQueryable();

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.Module))
                query = query.Where(x => x.Module == filter.Module);

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(x => x.Action == filter.Action);

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(x => x.EntityType == filter.EntityType);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(x => x.CreatedAt <= filter.ToDate);

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new AuditLogResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Module = x.Module,
                    Action = x.Action,
                    EntityType = x.EntityType, // ✅ FIXED
                    EntityId = x.EntityId,
                    Description = x.Description,
                    Metadata = x.Metadata,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new
            {
                total,
                page = filter.Page,
                pageSize = filter.PageSize,
                data
            };
        }
    }
}