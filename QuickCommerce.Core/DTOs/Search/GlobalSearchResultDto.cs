using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Search
{
    public class GlobalSearchResultDto
    {
        public List<GlobalSearchItemDto> Results { get; set; } = new();
    }
}