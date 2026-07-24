using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
    }
}