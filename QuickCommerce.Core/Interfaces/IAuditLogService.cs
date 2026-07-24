using System.Threading.Tasks;

public interface IAuditLogService
{
    Task LogAsync(
        int? userId,
        string module,
        string action,
        string? entityName = null,
        int? entityId = null,
        string? description = null,
        string? metadata = null);
}