using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> GetUserByPhoneAsync(string phoneNumber);
        Task<string> GenerateAccessTokenAsync(User user);
        Task<string> SendOtpAsync(string phoneNumber);

        Task<(string AccessToken, string RefreshToken)?> VerifyOtpAsync(string phoneNumber, string otp);
        Task<string?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);

    }
}
