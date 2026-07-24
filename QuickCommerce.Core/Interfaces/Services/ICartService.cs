using QuickCommerce.Core.DTOs.Customer;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCart(int userId);

        Task<CartDto> AddItem(int userId, AddCartItemDto dto);

        Task<CartDto> UpdateItem(int userId, int itemId, int quantity);

        Task RemoveItem(int userId, int itemId);
    }
}