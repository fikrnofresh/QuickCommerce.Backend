using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuickCommerce.Core.DTOs.Admin;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuditLogService _auditLogService;

        public AuthService(ApplicationDbContext context,IConfiguration configuration,AuditLogService auditLogService)
        {
            _context = context;
            _configuration = configuration;
            _auditLogService = auditLogService;
        }

        // =========================
        // USER LOOKUP
        // =========================
        public async Task<User?> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        // =========================
        // JWT GENERATION (FIXED)
        // =========================
        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            var role = userRoles.FirstOrDefault()?.Role;

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString())
            };

            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
                claims.Add(new Claim("roleScope", role.Scope ?? "PLATFORM"));

                var userStore = await _context.UserStores
                    .FirstOrDefaultAsync(us => us.UserId == user.Id && us.IsPrimary);

                if (role.Scope == "STORE" && userStore != null)
                {
                    claims.Add(new Claim("storeId", userStore.StoreId.ToString()));
                }

                foreach (var rp in role.RolePermissions)
                {
                    claims.Add(new Claim("permission", rp.Permission.Name));
                }
            }
            else
            {
                // CUSTOMER FLOW
                claims.Add(new Claim(ClaimTypes.Role, "CUSTOMER"));
            }

            var jwtSection = _configuration.GetSection("JWT");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // =========================
        // REFRESH TOKEN
        // =========================
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private async Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        // =========================
        // SEND OTP
        // =========================
        public Task<string> SendOtpAsync(string phoneNumber)
        {
            return Task.FromResult("123456");
        }

        // =========================
        // VERIFY OTP
        // =========================
        public async Task<(string AccessToken, string RefreshToken, string Role)?>
            VerifyOtpAsync(string phoneNumber, string otp)
        {
            var user = await GetUserByPhoneAsync(phoneNumber);
            if (user == null)
                return null;

            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = await CreateRefreshTokenAsync(user);

            return (accessToken, refreshToken.Token, "CUSTOMER");
        }

        // =========================
        // ADMIN LOGIN
        // =========================
        public async Task<AdminLoginResponseDto?> AdminLoginAsync(AdminLoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserStores)
                .FirstOrDefaultAsync(u =>
                    u.Email == request.Email &&
                    !u.IsDeleted);

            if (user == null)
                return null;

            // Account inactive
            if (!user.IsActive)
                return null;

            // Account locked
            if (user.IsLocked)
            {
                if (user.LockoutEnd.HasValue &&
                    user.LockoutEnd.Value > DateTime.UtcNow)
                {
                    return null;
                }

                // Auto unlock after lockout expires
                user.IsLocked = false;
                user.LockoutEnd = null;
                user.FailedLoginAttempts = 0;
            }

            // Password verification
          
            if (!VerifyPassword(request.Password, user.PasswordHash ?? string.Empty))
            {
                user.FailedLoginAttempts++;

                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                    await _auditLogService.LogAsync(
                        user.Id,
                        "Authentication",
                        "ACCOUNT_LOCKED",
                        "User",
                        user.Id,
                        $"User '{user.Email}' locked after 5 failed login attempts."
                        );

                    // Account lock audit
                    await _auditLogService.LogAsync(
                        user.Id,
                        "Authentication",
                        "ACCOUNT_LOCKED",
                        "User",
                        user.Id,
                        $"User '{user.Email}' locked after 5 failed login attempts."
                    );
                }

                user.UpdatedAt = DateTime.UtcNow;

                // Failed login audit
                await _auditLogService.LogAsync(
                    user.Id,
                    "Authentication",
                    "LOGIN_FAILED",
                    "User",
                    user.Id,
                    $"Failed login attempt for '{user.Email}'. Failed attempts: {user.FailedLoginAttempts}"
                );

                await _context.SaveChangesAsync();

                return null;
            }

            // Successful login
            user.FailedLoginAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            var role = user.UserRoles.FirstOrDefault()?.Role;

            if (role == null)
                return null;

            var permissions = role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var accessToken = await GenerateAccessTokenAsync(user);

            var refreshToken = await CreateRefreshTokenAsync(user);

            // Mark previous sessions as non-current
            var currentSessions = await _context.AdminSessions
                .Where(s =>
                    s.UserId == user.Id &&
                    !s.IsRevoked &&
                    s.IsCurrent)
                .ToListAsync();

            foreach (var session in currentSessions)
            {
                session.IsCurrent = false;
                session.LastActivity = DateTime.UtcNow;
            }

            // Create new admin session
            _context.AdminSessions.Add(new AdminSession
            {
                UserId = user.Id,
                RefreshToken = refreshToken.Token,
                LoginTime = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                IsCurrent = true,
                IsRevoked = false
            });
            await _auditLogService.LogAsync(
                user.Id,
                "Authentication",
                "LOGIN",
                "User",
                user.Id,
                $"Admin '{user.Email}' logged in successfully."
                );

            await _context.SaveChangesAsync();

            return new AdminLoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                Role = role.Name,
                RoleScope = role.Scope ?? "PLATFORM",
                StoreId = user.UserStores
                    .FirstOrDefault(x => x.IsPrimary)?.StoreId,
                Permissions = permissions
            };
        }

        // =========================
        // REFRESH TOKEN
        // =========================
        public async Task<string?> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken == null)
                return null;

            if (existingToken.IsRevoked)
                return null;

            if (existingToken.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = existingToken.User;

            if (user == null)
                return null;

            if (!user.IsActive || user.IsDeleted || user.IsLocked)
                return null;

            var adminSession = await _context.AdminSessions
                .FirstOrDefaultAsync(s =>
                    s.RefreshToken == refreshToken &&
                    !s.IsRevoked);

            if (adminSession == null)
            {
                await _auditLogService.LogAsync(
                    user.Id,
                    "Authentication",
                    "TOKEN_REFRESH_FAILED",
                    "AdminSession",
                    0,
                    "Refresh attempted with invalid session."
                );

                return null;
            }

            // Revoke current refresh token
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            // Generate new refresh token
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            existingToken.ReplacedByToken = newRefreshToken.Token;

            _context.RefreshTokens.Add(newRefreshToken);

            // Update session
            adminSession.RefreshToken = newRefreshToken.Token;
            adminSession.LastActivity = DateTime.UtcNow;

            var newAccessToken = await GenerateAccessTokenAsync(user);

            await _auditLogService.LogAsync(
                user.Id,
                "Authentication",
                "TOKEN_REFRESH_SUCCESS",
                "AdminSession",
                adminSession.Id,
                "Access token refreshed successfully."
            );

            await _context.SaveChangesAsync();

            return newAccessToken;
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken == null)
                return false;

            if (existingToken.IsRevoked)
                return false;

            // Find corresponding admin session
            var session = await _context.AdminSessions
                .FirstOrDefaultAsync(s =>
                    s.RefreshToken == refreshToken &&
                    !s.IsRevoked);

            // Revoke refresh token
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            // Revoke admin session
            if (session != null)
            {
                session.IsRevoked = true;
                session.IsCurrent = false;
                session.LogoutTime = DateTime.UtcNow;
                session.LastActivity = DateTime.UtcNow;
                session.RevokeReason = "User Logout";

                await _auditLogService.LogAsync(
                    existingToken.UserId,
                    "Authentication",
                    "LOGOUT_SUCCESS",
                    "AdminSession",
                    session.Id,
                    $"User '{existingToken.User?.Email}' logged out successfully."
                );
            }
            else
            {
                await _auditLogService.LogAsync(
                    existingToken.UserId,
                    "Authentication",
                    "LOGOUT_WITHOUT_SESSION",
                    "RefreshToken",
                    existingToken.Id,
                    "Refresh token revoked without matching admin session."
                );
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================
        // GOOGLE LOGIN (FINAL FIXED)
        // =========================
        public async Task<(string AccessToken, string RefreshToken)> GoogleLoginAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["GoogleAuth:ClientId"] }
            });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    PhoneNumber = "",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ ASSIGN CUSTOMER ROLE
                var customerRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "CUSTOMER");

                if (customerRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = customerRole.Id
                    });
                }

                // ✅ CREATE WALLET
                _context.Wallets.Add(new Wallet
                {
                    UserId = user.Id,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = await CreateRefreshTokenAsync(user);

            return (accessToken, refreshToken.Token);
        }
        public async Task<CustomerProfileDto?> GetCustomerProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return null;

            return new CustomerProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                PreferredLanguage = user.PreferredLanguage
            };
        }

        public async Task<bool> UpdateCustomerProfileAsync(int userId, UpdateCustomerProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            user.FullName = dto.FullName ?? user.FullName;
            user.ProfileImageUrl = dto.ProfileImageUrl ?? user.ProfileImageUrl;
            user.Gender = dto.Gender ?? user.Gender;
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.PreferredLanguage = dto.PreferredLanguage ?? user.PreferredLanguage;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task TrackCustomerActivityAsync(int userId, TrackActivityDto dto)
        {
            var activity = new CustomerActivity
            {
                UserId = userId,
                Action = dto.Action,
                EntityId = dto.EntityId,
                Metadata = dto.Metadata
            };

            _context.CustomerActivities.Add(activity);
            await _context.SaveChangesAsync();

            await UpdateCustomerSegment(userId);
        }

        private async Task UpdateCustomerSegment(int userId)
        {
            var orderCount = await _context.Orders.CountAsync(o => o.CustomerId == userId);

            string segment = "NEW";

            if (orderCount >= 10)
                segment = "HIGH_VALUE";
            else if (orderCount >= 3)
                segment = "ACTIVE";

            var existing = await _context.CustomerSegments
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
            {
                _context.CustomerSegments.Add(new CustomerSegment
                {
                    UserId = userId,
                    Segment = segment
                });
            }
            else
            {
                existing.Segment = segment;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string> GetCustomerSegmentAsync(int userId)
        {
            var segment = await _context.CustomerSegments
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return segment?.Segment ?? "NEW";
        }
        // =====================================================
        // ENTERPRISE PASSWORD MANAGEMENT
        // =====================================================

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string GenerateTemporaryPassword(int length = 10)
        {
            const string chars =
                "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789@#$%";

            var password = new char[length];

            using var rng = RandomNumberGenerator.Create();

            var bytes = new byte[length];

            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                password[i] = chars[bytes[i] % chars.Length];
            }

            return new string(password);
        }

        // =====================================================
        // ENTERPRISE ADMIN METHODS
        // =====================================================

        public async Task<AdminProfileDto?> GetCurrentAdminAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var role = user.UserRoles.FirstOrDefault()?.Role;
            
            return new AdminProfileDto
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = role?.Name ?? string.Empty,
                RoleScope = role?.Scope ?? "PLATFORM",
                IsActive = user.IsActive,
                LastLogin = user.LastLoginAt,
                Permissions = role?.RolePermissions
                    .Select(x => x.Permission.Name)
                    .Distinct()
                    .ToList() ?? new List<string>()
            };
        }

        public async Task<bool> ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            if (!user.IsActive || user.IsDeleted)
                return false;

            if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash ?? string.Empty))
                return false;

            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            // Prevent using the same password
            if (VerifyPassword(dto.NewPassword, user.PasswordHash ?? string.Empty))
                return false;

            // Update password
            user.PasswordHash = HashPassword(dto.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Revoke all active refresh tokens
            var refreshTokens = await _context.RefreshTokens
                .Where(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            // Revoke all active admin sessions
            var sessions = await _context.AdminSessions
                .Where(s =>
                    s.UserId == userId &&
                    !s.IsRevoked)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsRevoked = true;
                session.IsCurrent = false;
                session.LogoutTime = DateTime.UtcNow;
                session.LastActivity = DateTime.UtcNow;
                session.RevokeReason = "Password Changed";
            }

            await _auditLogService.LogAsync(
                user.Id,
                "Authentication",
                "PASSWORD_CHANGED",
                "User",
                user.Id,
                $"Password changed successfully for '{user.Email}'. All active sessions were revoked."
            );

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email != null &&
                    x.Email.ToLower() == email);

            // Don't disclose whether the email exists
            if (user == null)
                return true;

            // Ignore inactive/deleted users
            if (!user.IsActive || user.IsDeleted)
                return true;

            // Invalidate all previous unused reset tokens
            var oldTokens = await _context.PasswordResetTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            // Generate cryptographically secure reset token
            var bytes = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var resetToken = Convert.ToBase64String(bytes);

            var entity = new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                IsUsed = false,
                Purpose = "PASSWORD_RESET",
                CreatedBy = "SYSTEM"
            };

            _context.PasswordResetTokens.Add(entity);

            await _auditLogService.LogAsync(
                user.Id,
                "Authentication",
                "PASSWORD_RESET_REQUESTED",
                "User",
                user.Id,
                $"Password reset requested for '{user.Email}'."
            );

            await _context.SaveChangesAsync();

            // ====================================================
            // FUTURE INTEGRATIONS
            // ====================================================
            //
            // await _emailService.SendPasswordResetAsync(user.Email, resetToken);
            // await _smsService.SendResetOtpAsync(user.PhoneNumber, otp);
            // await _whatsAppService.SendResetLinkAsync(user.PhoneNumber, resetToken);
            //
            // ====================================================

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            var token = await _context.PasswordResetTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == dto.Token);

            if (token == null)
                return false;

            if (token.IsUsed)
                return false;

            if (token.ExpiresAt < DateTime.UtcNow)
                return false;

            var user = token.User;

            if (user == null)
                return false;

            if (!user.IsActive || user.IsDeleted)
                return false;

            // Prevent resetting to the current password
            if (VerifyPassword(dto.NewPassword, user.PasswordHash ?? string.Empty))
                return false;

            // Update password
            user.PasswordHash = HashPassword(dto.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Mark reset token as used
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;

            // Revoke all refresh tokens
            var refreshTokens = await _context.RefreshTokens
                .Where(rt =>
                    rt.UserId == user.Id &&
                    !rt.IsRevoked)
                .ToListAsync();

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
            }

            // Revoke all admin sessions
            var sessions = await _context.AdminSessions
                .Where(s =>
                    s.UserId == user.Id &&
                    !s.IsRevoked)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsRevoked = true;
                session.IsCurrent = false;
                session.LogoutTime = DateTime.UtcNow;
                session.LastActivity = DateTime.UtcNow;
                session.RevokeReason = "Password Reset";
            }

            await _auditLogService.LogAsync(
                user.Id,
                "Authentication",
                "PASSWORD_RESET_COMPLETED",
                "User",
                user.Id,
                $"Password reset completed for '{user.Email}'. All active sessions revoked."
            );

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SessionDto>> GetSessionsAsync(int userId)
        {
            return await _context.AdminSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LastActivity)
                .Select(s => new SessionDto
                {
                    SessionId = s.Id.ToString(),
                    DeviceName = s.DeviceInfo ?? string.Empty,
                    Browser = s.Browser ?? string.Empty,
                    OperatingSystem = s.OperatingSystem ?? string.Empty,
                    IpAddress = s.IpAddress ?? string.Empty,
                    LoginTime = s.LoginTime,
                    LastActivity = s.LastActivity,
                    IsCurrentSession = s.IsCurrent
                })
                .ToListAsync();
        }

        public async Task<bool> LogoutSessionAsync(string sessionId)
        {
            if (!int.TryParse(sessionId, out var sessionIdValue))
                return false;

            var session = await _context.AdminSessions
                .FirstOrDefaultAsync(x => x.Id == sessionIdValue);

            if (session == null)
                return false;

            if (session.IsRevoked)
                return true;

            session.IsRevoked = true;
            session.LogoutTime = DateTime.UtcNow;
            session.LastActivity = DateTime.UtcNow;
            session.RevokeReason = "User Logout";

            // Revoke the corresponding refresh token if present
            if (!string.IsNullOrWhiteSpace(session.RefreshToken))
            {
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == session.RefreshToken);

                if (refreshToken != null && !refreshToken.IsRevoked)
                {
                    refreshToken.IsRevoked = true;
                    refreshToken.RevokedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
