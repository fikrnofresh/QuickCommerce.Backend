using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAuthService
    {
        // =====================================================
        // USER LOOKUP
        // =====================================================

        Task<User?> GetUserByPhoneAsync(string phoneNumber);

        // =====================================================
        // JWT TOKEN
        // =====================================================

        Task<string> GenerateAccessTokenAsync(User user);

        // =====================================================
        // CUSTOMER OTP LOGIN
        // =====================================================

        Task<string> SendOtpAsync(string phoneNumber);

        Task<(string AccessToken, string RefreshToken, string Role)?>
            VerifyOtpAsync(string phoneNumber, string otp);

        // =====================================================
        // ADMIN LOGIN
        // =====================================================

        Task<AdminLoginResponseDto?> AdminLoginAsync(AdminLoginRequestDto request);

        // =====================================================
        // ENTERPRISE ADMIN AUTHENTICATION
        // =====================================================

        /// <summary>
        /// Returns currently logged-in admin profile.
        /// </summary>
        Task<AdminProfileDto?> GetCurrentAdminAsync(int userId);

        /// <summary>
        /// Change current admin password.
        /// </summary>
        Task<bool> ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto);

        /// <summary>
        /// Send forgot password link / OTP.
        /// </summary>
        Task<bool> ForgotPasswordAsync(string email);

        /// <summary>
        /// Reset password using token.
        /// </summary>
        Task<bool> ResetPasswordAsync(
            ResetPasswordDto dto);

        /// <summary>
        /// Get active login sessions.
        /// </summary>
        Task<IEnumerable<SessionDto>> GetSessionsAsync(
            int userId);

        /// <summary>
        /// Logout one active session.
        /// </summary>
        Task<bool> LogoutSessionAsync(
            string sessionId);

        // =====================================================
        // TOKEN MANAGEMENT
        // =====================================================

        Task<string?> RefreshTokenAsync(string refreshToken);

        Task<bool> LogoutAsync(string refreshToken);

        // =====================================================
        // GOOGLE LOGIN
        // =====================================================

        Task<(string AccessToken, string RefreshToken)>
            GoogleLoginAsync(string idToken);

        // =====================================================
        // CUSTOMER PROFILE
        // =====================================================

        Task<CustomerProfileDto?> GetCustomerProfileAsync(int userId);

        Task<bool> UpdateCustomerProfileAsync(
            int userId,
            UpdateCustomerProfileDto dto);

        Task TrackCustomerActivityAsync(
            int userId,
            TrackActivityDto dto);

        Task<string> GetCustomerSegmentAsync(int userId);

        // =====================================================
        // ENTERPRISE PASSWORD MANAGEMENT
        // =====================================================

        /// <summary>
        /// Hash a plain text password using BCrypt.
        /// Used by:
        /// - Store Owner Onboarding
        /// - Admin Creation
        /// - Vendor Creation
        /// - Delivery Partner Creation
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verify plain password against hash.
        /// </summary>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Generate a secure temporary password.
        /// Used during onboarding workflows.
        /// </summary>
        string GenerateTemporaryPassword(int length = 10);
    }
}