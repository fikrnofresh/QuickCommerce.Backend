using QuickCommerce.Core.DTOs.Customer;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> Checkout(int userId, CheckoutRequestDto dto);
    }
}