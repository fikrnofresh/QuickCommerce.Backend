using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.AdminRoles;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/roles")]
    [Authorize]
    public class AdminRolesController : ControllerBase
    {
        private readonly IAdminRoleService _service;

        public AdminRolesController(IAdminRoleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDto dto)
        {
            var result = await _service.CreateRoleAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _service.GetRolesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var result = await _service.GetRoleByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, UpdateRoleDto dto)
        {
            var success = await _service.UpdateRoleAsync(id, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var success = await _service.DeleteRoleAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}