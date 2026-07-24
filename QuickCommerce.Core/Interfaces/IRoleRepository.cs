using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
    }
}