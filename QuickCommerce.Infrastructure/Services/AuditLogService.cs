using Microsoft.AspNetCore.Http;
using QuickCommerce.Core.Entities;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public AuditLogService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task LogAsync(
            int? userId,
            string module,
            string action,
            string? entityType = null,
            int? entityId = null,
            string? description = null,
            string? metadata = null)
        {
            var httpContext = _httpContext.HttpContext;

            // ✅ AUTO USER ID (if not passed)
            if (userId == null && httpContext?.User != null)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out var parsedId))
                    userId = parsedId;
            }

            // ✅ IP ADDRESS
            var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();

            // ✅ DEVICE (basic)
            var device = httpContext?.Request?.Headers["User-Agent"].ToString();

            // ✅ SOURCE (simple logic)
            var source = httpContext?.Request?.Path.ToString().Contains("/admin") == true
                ? "ADMIN"
                : "API";

            var log = new ActivityLog
            {
                UserId = userId,
                Module = module,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                Metadata = metadata,
                IpAddress = ip,
                Device = device,
                Source = source,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}