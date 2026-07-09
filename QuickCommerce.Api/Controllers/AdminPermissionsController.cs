using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.AdminPermissions;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize]
    public class AdminPermissionsController : ControllerBase
    {
        private readonly IAdminPermissionService _service;

        public AdminPermissionsController(IAdminPermissionService service)
        {
            _service = service;
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var result = await _service.GetPermissionsAsync();
            return Ok(result);
        }

        [HttpGet("roles/{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var result = await _service.GetRolePermissionsAsync(roleId);
            return Ok(result);
        }

        [HttpPost("roles/{roleId}/permissions")]
        public async Task<IActionResult> AssignPermission(int roleId, AssignPermissionDto dto)
        {
            var success = await _service.AssignPermissionAsync(roleId, dto.PermissionId);

            if (!success)
                return BadRequest("Permission already assigned");

            return Ok();
        }

        [HttpDelete("roles/{roleId}/permissions/{permissionId}")]
        public async Task<IActionResult> RemovePermission(int roleId, int permissionId)
        {
            var success = await _service.RemovePermissionAsync(roleId, permissionId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}