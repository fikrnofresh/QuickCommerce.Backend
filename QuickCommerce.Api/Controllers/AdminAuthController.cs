using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // =========================
        // ADMIN LOGIN (EMAIL + PASSWORD)
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var result = await _authService.AdminLoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(result);
        }
    }
}