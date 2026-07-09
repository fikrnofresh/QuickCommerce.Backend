using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.DTOs.Auth;
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
                return BadRequest(new { message = "Phone number is required" });

            try
            {
                var result = await _authService.SendOtpAsync(request.PhoneNumber);

                return Ok(new
                {
                    message = "OTP sent successfully",
                    otp = result // DEV ONLY
                });
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
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new { message = "Phone number and OTP are required" });

            var result = await _authService.VerifyOtpAsync(request.PhoneNumber, request.Otp);

            if (result == null)
                return Unauthorized(new { message = "Invalid or expired OTP" });

            return Ok(new LoginResponseDto
            {
                AccessToken = result.Value.AccessToken,
                RefreshToken = result.Value.RefreshToken,
                Role = result.Value.Role
            });
        }

        // =========================
        // REFRESH TOKEN
        // =========================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var newAccessToken = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (newAccessToken == null)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            return Ok(new
            {
                accessToken = newAccessToken,
                expiresIn = 900
            });
        }

        // =========================
        // LOGOUT
        // =========================
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var result = await _authService.LogoutAsync(request.RefreshToken);

            if (!result)
                return BadRequest(new { message = "Invalid refresh token" });

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

        // =========================
        // GOOGLE LOGIN (CUSTOMER)
        // =========================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { message = "IdToken is required" });

            try
            {
                var result = await _authService.GoogleLoginAsync(request.IdToken);

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken
                });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }
        }
    }
}