using QuickCommerce.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class ConsoleOtpSender : IOtpSender
    {
        public Task SendAsync(string phoneNumber, string otp)
        {
            Console.WriteLine($"Sending OTP to {phoneNumber}: {otp}");
            return Task.CompletedTask;
           
        }
    }
}
