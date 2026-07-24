using QuickCommerce.Core.DTOs.AdminPermissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAdminPermissionService
    {
        Task<IEnumerable<PermissionResponseDto>> GetPermissionsAsync();

        Task<IEnumerable<PermissionResponseDto>> GetRolePermissionsAsync(int roleId);

        Task<bool> AssignPermissionAsync(int roleId, int permissionId);

        Task<bool> RemovePermissionAsync(int roleId, int permissionId);
    }
}