using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.Interfaces;
using System.Security.Claims;

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

        // =====================================================
        // ADMIN LOGIN
        // =====================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AdminLoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password."
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // =====================================================
        // CURRENT ADMIN PROFILE
        // =====================================================
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentAdmin()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid authentication token."
                });
            }

            var profile = await _authService.GetCurrentAdminAsync(userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Admin profile not found."
                });
            }

            return Ok(new
            {
                success = true,
                data = profile
            });
        }

        // =====================================================
        // CHANGE PASSWORD
        // =====================================================
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId, dto);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password could not be changed."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Password changed successfully."
            });
        }

        // =====================================================
        // FORGOT PASSWORD
        // =====================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(request.Email);

            return Ok(new
            {
                success = result,
                message = result
                    ? "Password reset request accepted."
                    : "Unable to process request."
            });
        }

        // =====================================================
        // RESET PASSWORD
        // =====================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password reset failed."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Password reset successfully."
            });
        }

        // =====================================================
        // ACTIVE SESSIONS
        // =====================================================
        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var sessions = await _authService.GetSessionsAsync(userId);

            return Ok(new
            {
                success = true,
                data = sessions
            });
        }

        // =====================================================
        // LOGOUT SINGLE SESSION
        // =====================================================
        [Authorize]
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> LogoutSession(string sessionId)
        {
            var result = await _authService.LogoutSessionAsync(sessionId);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Unable to logout session."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Session logged out successfully."
            });
        }
    }
}