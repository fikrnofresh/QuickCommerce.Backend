using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderRequestDto request);
        Task<Order?> GetOrderByIdAsync(int id);

        // ✅ UPDATED SIGNATURE (IMPORTANT)
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, int? changedByUserId, string? remarks);

        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);

        // ✅ TRACKING
        Task<IEnumerable<CustomerOrderTrackingDto>> GetOrderTrackingAsync(int orderId);
    }
}