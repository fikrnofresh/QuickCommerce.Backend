public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogService auditService)
    {
        await _next(context);

        var userId = context.User?.FindFirst("userId")?.Value;

        await auditService.LogAsync(
            userId != null ? int.Parse(userId) : null,
            "API",
            context.Request.Method,
            context.Request.Path,
            null,
            "API request executed");
    }
}