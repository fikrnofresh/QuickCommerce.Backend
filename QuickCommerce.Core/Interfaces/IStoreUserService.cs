using QuickCommerce.Core.DTOs.StoreUsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IStoreUserService
    {
        Task<StoreUserResponseDto> CreateStoreUserAsync(int storeId, CreateStoreUserDto dto);

        Task<IEnumerable<StoreUserResponseDto>> GetStoreUsersAsync(int storeId);

        Task<bool> UpdateStoreUserAsync(int storeId, int userId, UpdateStoreUserDto dto);

        Task<bool> RemoveStoreUserAsync(int storeId, int userId);
    }
}