using QuickCommerce.Core.DTOs.Analytics;
using QuickCommerce.Core.DTOs.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IDemandForecastService
    {
        Task<List<DemandForecastDto>> GetDemandForecastAsync(int storeId);
    }
}