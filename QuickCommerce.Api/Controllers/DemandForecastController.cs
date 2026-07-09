using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/forecast")]
    [Authorize(Policy = "ANALYTICS.VIEW")]
    public class DemandForecastController : ControllerBase
    {
        private readonly IDemandForecastService _forecastService;

        public DemandForecastController(IDemandForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [HttpGet("{storeId}")]
        public async Task<IActionResult> GetDemandForecast(int storeId)
        {
            var result = await _forecastService.GetDemandForecastAsync(storeId);
            return Ok(result);
        }
    }
}