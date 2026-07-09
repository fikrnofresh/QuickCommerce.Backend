using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/alerts")]
    [Authorize(Policy = "DASHBOARD.VIEW")]
    public class AdminAlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AdminAlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlerts()
        {
            var alerts = await _alertService.GetPlatformAlertsAsync();
            
            return Ok(alerts);
        }
    }
}