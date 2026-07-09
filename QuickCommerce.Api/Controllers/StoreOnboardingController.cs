using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Common;
using QuickCommerce.Core.DTOs.Store;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stores/onboarding")]
    [Authorize(Policy = "STORE.CREATE")]
    public class StoreOnboardingController : ControllerBase
    {
        private readonly IStoreOnboardingService _storeOnboardingService;

        public StoreOnboardingController(
            IStoreOnboardingService storeOnboardingService)
        {
            _storeOnboardingService = storeOnboardingService;
        }

        /// <summary>
        /// Enterprise Store Onboarding
        /// Creates:
        /// • Store
        /// • Store Admin
        /// • Store Mapping
        /// • Login Credentials
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StoreOnboardingResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] StoreOnboardingDto dto)
        {
            var result = await _storeOnboardingService.OnboardStoreAsync(dto);

            return Ok(
                ApiResponse<StoreOnboardingResultDto>.SuccessResponse(
                    result,
                    result.Message));
        }
    }
}