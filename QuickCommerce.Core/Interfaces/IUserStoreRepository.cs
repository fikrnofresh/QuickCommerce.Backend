using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IUserStoreRepository
    {
        Task AddAsync(UserStore userStore);
    }
}