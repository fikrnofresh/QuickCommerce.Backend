using System;

namespace QuickCommerce.Core.DTOs.AdminUsers
{
    /// <summary>
    /// Standard filter model for Admin User listing.
    /// Enterprise ready.
    /// </summary>
    public class AdminUserFilterDto
    {
        // Pagination
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        // Search
        public string? Search { get; set; }

        // Filters
        public int? RoleId { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        // Sorting
        public string SortBy { get; set; } = "FullName";

        public bool Descending { get; set; } = false;
    }
}