using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Core.Utils;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IOtpSender _otpSender;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IOtpSender otpSender)
        {
            _context = context;
            _configuration = configuration;
            _otpSender = otpSender;
        }

        // =========================
        // SEND OTP
        // =========================
        public async Task<string> SendOtpAsync(string phoneNumber)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (existingUser != null &&
                existingUser.LockoutEnd != null &&
                existingUser.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception("Account is temporarily locked. Try again later.");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var otpHash = OtpHasher.Hash(otp);

            var oldOtps = await _context.OtpVerifications
                .Where(x => x.PhoneNumber == phoneNumber && !x.IsVerified)
                .ToListAsync();

            foreach (var old in oldOtps)
            {
                old.IsVerified = true;
            }

            var otpEntity = new OtpVerification
            {
                PhoneNumber = phoneNumber,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                AttemptCount = 0,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.OtpVerifications.AddAsync(otpEntity);
            await _context.SaveChangesAsync();

            await _otpSender.SendAsync(phoneNumber, otp);

            return otp; // DEV MODE
        }

        // =========================
        // VERIFY OTP
        // =========================
        public async Task<(string AccessToken, string RefreshToken)?> VerifyOtpAsync(string phoneNumber, string otp)
        {
            var otpRecord = await _context.OtpVerifications
                .Where(o => o.PhoneNumber == phoneNumber && !o.IsVerified)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null || otpRecord.ExpiresAt < DateTime.UtcNow)
                return null;

            var inputHash = OtpHasher.Hash(otp);

            if (otpRecord.OtpHash != inputHash)
            {
                otpRecord.AttemptCount++;
                await _context.SaveChangesAsync();
                return null;
            }

            otpRecord.IsVerified = true;
            await _context.SaveChangesAsync();

            var user = await GetUserByPhoneAsync(phoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    IsPhoneVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var customerRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "CUSTOMER");

                if (customerRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = customerRole.Id,
                        GrantedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                }

                user = await GetUserByPhoneAsync(phoneNumber);
            }
            else
            {
                user.IsPhoneVerified = true;
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            var accessToken = await GenerateAccessTokenAsync(user!);
            var refreshToken = GenerateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user!.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        // =========================
        // GET USER WITH ROLES + STORES
        // =========================
        public async Task<User?> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserStores)
                    .ThenInclude(us => us.Store)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        // =========================
        // REFRESH TOKEN
        // =========================
        public async Task<string?> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken == null ||
                existingToken.IsRevoked ||
                existingToken.ExpiresAt < DateTime.UtcNow)
                return null;

            return await GenerateAccessTokenAsync(existingToken.User);
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken == null || existingToken.IsRevoked)
                return false;

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // GENERATE JWT WITH STORES
        // =========================
        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = user.UserRoles?
                .Select(ur => ur.Role.Name)
                .Distinct()
                .ToList() ?? new List<string>();

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userStores = user.UserStores?
                .Select(us => us.StoreId)
                .Distinct()
                .ToList() ?? new List<int>();

            if (userStores.Any())
            {
                foreach (var storeId in userStores)
                {
                    claims.Add(new Claim("storeIds", storeId.ToString()));
                }

                var primaryStore = user.UserStores
                    .FirstOrDefault(us => us.IsPrimary);

                if (primaryStore != null)
                {
                    claims.Add(new Claim("primaryStoreId", primaryStore.StoreId.ToString()));
                }
            }

            if (roles.Contains("SUPER_ADMIN"))
            {
                claims.Add(new Claim("isSuperAdmin", "true"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["AccessTokenExpiryMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}