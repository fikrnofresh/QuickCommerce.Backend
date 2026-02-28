using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderRequestDto request);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
