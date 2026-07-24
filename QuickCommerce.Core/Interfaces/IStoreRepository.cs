using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store?> GetByIdAsync(int id);

        Task<List<Store>> GetAllAsync();

        Task AddAsync(Store store);

        Task UpdateAsync(Store store);

        Task<bool> ExistsByNameAsync(string name);

        Task<bool> ExistsByCodeAsync(string code);

        Task<string> GenerateStoreCodeAsync();

        Task SaveChangesAsync();
    }
}