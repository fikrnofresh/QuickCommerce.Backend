using QuickCommerce.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAdminAuditService
    {
        Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsAsync();

        Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByUserAsync(int userId);

        Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByModuleAsync(string module);

        Task<IEnumerable<AuditLogResponseDto>> GetAuditLogsByEntityAsync(string entityType, int entityId);

        // 🔥 ADD THIS (MISSING METHOD)
        Task<object> GetFilteredLogsAsync(AuditLogFilterDto filter);
    }
}