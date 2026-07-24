using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByIdAsync(int id)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Store>> GetAllAsync()
        {
            return await _context.Stores
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
        }

        public Task UpdateAsync(Store store)
        {
            _context.Stores.Update(store);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Stores
                .AnyAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Stores
                .AnyAsync(x => x.Code == code);
        }

        public async Task<string> GenerateStoreCodeAsync()
        {
            int count = await _context.Stores.CountAsync();

            return $"STR{(count + 1):D5}";
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}