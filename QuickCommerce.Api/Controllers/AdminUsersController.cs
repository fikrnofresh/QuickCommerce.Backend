using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.AdminUsers;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _service;

        public AdminUsersController(IAdminUserService service)
        {
            _service = service;
        }

        // =========================
        // CREATE ADMIN USER
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateAdminUserDto dto)
        {
            var result = await _service.CreateAdminUserAsync(dto);
            return Ok(result);
        }

        // =========================
        // GET ALL USERS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserFilterDto filter)
        {
            var result = await _service.GetAllAdminUsersAsync(filter);
            return Ok(result);
        }

        // =========================
        // GET USER BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _service.GetAdminUserByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================
        // UPDATE USER
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateAdminUserDto dto)
        {
            var success = await _service.UpdateAdminUserAsync(id, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        // =========================
        // DELETE USER
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _service.DeleteAdminUserAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        // =========================
        // ACTIVATE / SUSPEND USER
        // =========================
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool isActive)
        {
            var result = await _service.UpdateUserStatusAsync(id, isActive);
            return Ok(result);
        }

        // =========================
        // UPDATE ROLE
        // =========================
        [HttpPost("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleDto dto)
        {
            var result = await _service.UpdateUserRoleAsync(id, dto.RoleId);
            return Ok(result);
        }

        // =========================
        // UPDATE PERMISSIONS (TICK SYSTEM)
        // =========================
        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, UpdateUserPermissionsDto dto)
        {
            var result = await _service.UpdateUserPermissionsAsync(dto.RoleId, dto.PermissionsJson);
            return Ok(result);
        }
    }
}