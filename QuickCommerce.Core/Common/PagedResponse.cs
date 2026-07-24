using System.Collections.Generic;

namespace QuickCommerce.Core.Common
{
    /// <summary>
    /// Standard paginated API response.
    /// </summary>
    public class PagedResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public IEnumerable<T> Data { get; set; } = new List<T>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static PagedResponse<T> Create(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalRecords,
            string message = "Success")
        {
            return new PagedResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize)
            };
        }
    }
}