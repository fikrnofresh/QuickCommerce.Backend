using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Admin; // ✅ ADDED
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/audit-logs")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class AdminAuditController : ControllerBase
    {
        private readonly IAdminAuditService _service;

        public AdminAuditController(IAdminAuditService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL LOGS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _service.GetAuditLogsAsync());
        }

        // =========================
        // GET BY USER
        // =========================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLogsByUser(int userId)
        {
            return Ok(await _service.GetAuditLogsByUserAsync(userId));
        }

        // =========================
        // GET BY MODULE
        // =========================
        [HttpGet("module/{module}")]
        public async Task<IActionResult> GetLogsByModule(string module)
        {
            return Ok(await _service.GetAuditLogsByModuleAsync(module));
        }

        // =========================
        // GET BY ENTITY
        // =========================
        // =========================
        // GET BY ENTITY TYPE
        // =========================
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<IActionResult> GetLogsByEntity(string entityType, int entityId)
        {
            return Ok(await _service.GetAuditLogsByEntityAsync(entityType, entityId));
        }

        // =========================
        // 🔥 NEW — FILTER + PAGINATION API
        // =========================
        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredLogs([FromBody] AuditLogFilterDto filter)
        {
            var result = await _service.GetFilteredLogsAsync(filter);
            return Ok(result);
        }
    }
}