using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Repositories
{
    public class UserStoreRepository : IUserStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public UserStoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserStore userStore)
        {
            await _context.UserStores.AddAsync(userStore);
        }
    }
}