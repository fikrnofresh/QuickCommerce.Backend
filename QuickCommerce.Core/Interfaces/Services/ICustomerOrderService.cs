using QuickCommerce.Core.DTOs.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface ICustomerOrderService
    {
        // =========================
        // CUSTOMER ORDER LIST
        // =========================
        Task<List<CustomerOrderDto>> GetOrders(int userId);

        // =========================
        // CUSTOMER ORDER DETAIL
        // =========================
        Task<CustomerOrderDetailDto> GetOrderDetail(int userId, int orderId);

        // =========================
        // CUSTOMER ORDER TRACKING
        // =========================
        Task<IEnumerable<CustomerOrderTrackingDto>> GetOrderTracking(int customerId, int orderId);
    }
}