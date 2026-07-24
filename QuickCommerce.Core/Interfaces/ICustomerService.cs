using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICustomerService
{
    Task<List<ReorderRecommendationDto>> GetReorderRecommendations(int userId);
    Task<List<BundleRecommendationDto>> GetFrequentlyBoughtTogether(int productId);
    Task<List<SearchSuggestionDto>> GetSearchSuggestions(string query, int userId);
    Task<List<FinalRecommendationDto>> GetSmartRecommendations(int userId);
}