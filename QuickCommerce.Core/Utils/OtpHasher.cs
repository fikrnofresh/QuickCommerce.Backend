using System;
using System.Security.Cryptography;
using System.Text;

namespace QuickCommerce.Core.Utils
{
    public static class OtpHasher
    {
        public static string Hash(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(bytes);
        }
    }
}
