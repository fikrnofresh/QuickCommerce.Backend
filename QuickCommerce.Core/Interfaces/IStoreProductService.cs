using QuickCommerce.Core.DTOs.Common;
using QuickCommerce.Core.DTOs.Store;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IStoreProductService
    {
        Task<PagedResultDto<StoreProductListDto>> GetStoreProductsAsync(
            int storeId,
            int pageNumber,
            int pageSize,
            int? categoryId,
            string? search);
    }
}