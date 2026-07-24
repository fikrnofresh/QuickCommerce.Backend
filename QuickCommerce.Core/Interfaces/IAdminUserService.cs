using QuickCommerce.Core.Common;
using QuickCommerce.Core.DTOs.AdminUsers;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace QuickCommerce.Core.Interfaces
{
    public interface IAdminUserService
    {
        // ===========================
        // User CRUD
        // ===========================

        Task<AdminUserResponseDto> CreateAdminUserAsync(CreateAdminUserDto dto);

        Task<PagedResponse<AdminUserResponseDto>> GetAllAdminUsersAsync(AdminUserFilterDto filter);

        Task<AdminUserResponseDto?> GetAdminUserByIdAsync(int id);

        Task<bool> UpdateAdminUserAsync(int id, UpdateAdminUserDto dto);

        /// <summary>
        /// Soft delete user.
        /// User is never physically removed.
        /// </summary>
        Task<bool> DeleteAdminUserAsync(int id);

        // ===========================
        // User Administration
        // ===========================

        Task<object> UpdateUserStatusAsync(int userId, bool isActive);

        Task<object> UpdateUserRoleAsync(int userId, int roleId);

        Task<object> UpdateUserPermissionsAsync(int roleId, string permissionsJson);
    }
}