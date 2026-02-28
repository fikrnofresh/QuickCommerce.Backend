using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
