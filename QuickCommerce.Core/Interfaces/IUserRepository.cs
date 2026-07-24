using QuickCommerce.Core.Entities;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByPhoneNumberAsync(string phoneNumber);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> ExistsByPhoneAsync(string phoneNumber);

        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task SaveChangesAsync();
    }
}