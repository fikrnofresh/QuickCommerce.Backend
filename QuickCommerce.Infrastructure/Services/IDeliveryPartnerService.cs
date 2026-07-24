using QuickCommerce.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface IDeliveryPartnerService
    {
        Task<DeliveryPartner> CreateAsync(DeliveryPartner partner);
        Task<List<DeliveryPartner>> GetAllAsync();
        Task UpdateAvailabilityAsync(int partnerId, bool isAvailable);
        Task UpdateLocationAsync(int partnerId, decimal lat, decimal lng);
    }
}