using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.StoreUsers;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stores/{storeId}/users")]
    [Authorize]
    public class StoreUsersController : ControllerBase
    {
        private readonly IStoreUserService _service;

        public StoreUsersController(IStoreUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStoreUser(int storeId, CreateStoreUserDto dto)
        {
            var result = await _service.CreateStoreUserAsync(storeId, dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreUsers(int storeId)
        {
            var result = await _service.GetStoreUsersAsync(storeId);
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateStoreUser(int storeId, int userId, UpdateStoreUserDto dto)
        {
            var success = await _service.UpdateStoreUserAsync(storeId, userId, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveStoreUser(int storeId, int userId)
        {
            var success = await _service.RemoveStoreUserAsync(storeId, userId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}