using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInventoryRepository
{
    Task<IEnumerable<StoreProduct>> GetStoreInventoryAsync(int storeId);

    Task<IEnumerable<StoreProduct>> GetLowStockAsync();

    Task<IEnumerable<StoreProduct>> GetOutOfStockAsync();

    Task<IEnumerable<InventoryMovement>> GetHistoryAsync(int storeProductId);

    Task AdjustStockAsync(int storeProductId, int quantityChange, string reason, int userId);
}