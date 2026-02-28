using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IOtpSender
    {
        Task SendAsync(string phoneNumber, string otp);
    }
}
