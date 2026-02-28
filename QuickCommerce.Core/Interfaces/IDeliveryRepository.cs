using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<Delivery?> GetByIdAsync(int id);
        Task<IEnumerable<Delivery>> GetAllAsync();
        Task AddAsync(Delivery delivery);
        Task UpdateAsync(Delivery delivery);
    }
}
