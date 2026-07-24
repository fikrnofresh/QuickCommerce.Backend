using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface IDeliveryAppService
    {
        Task<object> GetMyOrdersAsync(int userId);

        Task AcceptOrderAsync(int userId, int orderId);

        Task RejectOrderAsync(int userId, int orderId);

        Task<object> GetEarningsAsync(int userId);
    }
}