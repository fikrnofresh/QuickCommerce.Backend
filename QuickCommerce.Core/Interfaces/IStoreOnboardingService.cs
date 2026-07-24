using System.Threading.Tasks;
using QuickCommerce.Core.DTOs.Store;

namespace QuickCommerce.Core.Interfaces
{
    /// <summary>
    /// Enterprise Store Onboarding Service
    ///
    /// Responsible for:
    /// • Creating Store
    /// • Creating Store Owner
    /// • Assigning STORE_OWNER role
    /// • Mapping User ↔ Store
    /// • Generating Login Credentials
    /// • Returning onboarding result
    /// </summary>
    public interface IStoreOnboardingService
    {
        /// <summary>
        /// Complete enterprise onboarding workflow.
        /// </summary>
        Task<StoreOnboardingResultDto> OnboardStoreAsync(
            StoreOnboardingDto dto);
    }
}