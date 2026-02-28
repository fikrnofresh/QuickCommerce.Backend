using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        
        public AuthController(
            IAuthService authService,
            IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // =========================
        // STEP 1: SEND OTP
        // =========================
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest("Phone number is required");

            try
            {
                var result = await _authService.SendOtpAsync(request.PhoneNumber);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =========================
        // STEP 2: VERIFY OTP
        // =========================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            var result = await _authService.VerifyOtpAsync(request.PhoneNumber, request.Otp);

            if (result == null)
                return BadRequest("Invalid OTP");

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                refreshToken = result.Value.RefreshToken,
                expiresIn = 3600
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var newAccessToken = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (newAccessToken == null)
                return Unauthorized("Invalid or expired refresh token");

            return Ok(new
            {
                accessToken = newAccessToken,
                expiresIn = 3600
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.LogoutAsync(request.RefreshToken);

            if (!result)
                return BadRequest("Invalid refresh token");

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

        // =========================
        
    }
}
