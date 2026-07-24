using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface IOrderManagementService
    {
        Task UpdateOrderStatusAsync(int orderId, string newStatus, int? changedByUserId, string? remarks);
    }
}