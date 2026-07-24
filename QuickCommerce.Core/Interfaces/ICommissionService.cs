using QuickCommerce.Core.DTOs.Commission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface ICommissionService
    {
        Task<CategoryCommissionDto> CreateRuleAsync(CreateCategoryCommissionDto dto);

        Task<IEnumerable<CategoryCommissionDto>> GetAllRulesAsync();

        Task<CategoryCommissionDto?> UpdateRuleAsync(int id, CreateCategoryCommissionDto dto);
    }
}