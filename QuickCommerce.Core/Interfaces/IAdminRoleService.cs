using QuickCommerce.Core.DTOs.AdminRoles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAdminRoleService
    {
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto);

        Task<IEnumerable<RoleResponseDto>> GetRolesAsync();

        Task<RoleResponseDto?> GetRoleByIdAsync(int id);

        Task<bool> UpdateRoleAsync(int id, UpdateRoleDto dto);

        Task<bool> DeleteRoleAsync(int id);
    }
}