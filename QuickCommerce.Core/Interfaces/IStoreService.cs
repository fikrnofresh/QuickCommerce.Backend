using QuickCommerce.Core.DTOs.Store;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IStoreService
    {
        // =====================================================
        // CRUD
        // =====================================================

        Task<StoreDto> CreateStoreAsync(CreateStoreDto dto);

        Task<StoreDto?> GetStoreByIdAsync(int id);

        Task<IEnumerable<StoreDto>> GetAllStoresAsync();

        Task<StoreDto> UpdateStoreAsync(
            int id,
            UpdateStoreDto dto);

        // =====================================================
        // STATUS MANAGEMENT
        // =====================================================

        Task ActivateStoreAsync(int id);

        Task DeactivateStoreAsync(int id);
        
        Task SuspendStoreAsync(int id);

        Task BlockStoreAsync(int id);

        Task CloseStoreAsync(int id);

        Task VerifyStoreAsync(int id);

        Task UnverifyStoreAsync(int id);

        Task SetStoreOnlineAsync(int id);

        Task SetStoreOfflineAsync(int id);

        // =====================================================
        // DASHBOARD
        // =====================================================

        Task<StoreDashboardDto> GetStoreDashboardAsync(int id);

        // =====================================================
        // INVENTORY
        // =====================================================

        Task<List<StoreInventoryMonitorDto>> GetStoreInventoryMonitorAsync(int id);
    }
}