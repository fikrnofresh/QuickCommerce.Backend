using QuickCommerce.Core.DTOs.Search;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IGlobalSearchService
    {
        Task<GlobalSearchResultDto> SearchAsync(string query);
    }
}