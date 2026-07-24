using QuickCommerce.Core.DTOs.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces.Services
{
    public interface ICustomerAddressService
    {
        Task<List<CustomerAddressDto>> GetAddresses(int userId);

        Task<CustomerAddressDto> CreateAddress(int userId, CustomerAddressDto dto);

        Task DeleteAddress(int id, int userId);
    }
}