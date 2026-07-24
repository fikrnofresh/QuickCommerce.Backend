using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Store;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    /// <summary>
    /// Enterprise Store Onboarding Service
    ///
    /// Responsibilities:
    /// • Validate onboarding request
    /// • Generate Store Code
    /// • Generate Temporary Password
    /// • Create Store
    /// • Create Store Owner
    /// • Assign STORE_OWNER role
    /// • Link User ↔ Store
    /// • Return onboarding result
    /// </summary>
    public class StoreOnboardingService : IStoreOnboardingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public StoreOnboardingService(
            ApplicationDbContext context,
            IStoreRepository storeRepository,
            IUserRepository userRepository,
            IAuthService authService)
        {
            _context = context;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _authService = authService;
        }

        // =====================================================
        // VALIDATION
        // =====================================================

        private async Task ValidateRequest(StoreOnboardingDto dto)
        {
            if (await _storeRepository.ExistsByNameAsync(dto.Name))
                throw new Exception("Store name already exists.");

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                if (await _storeRepository.ExistsByCodeAsync(dto.Code))
                    throw new Exception("Store code already exists.");
            }

            if (await _userRepository.ExistsByPhoneAsync(dto.OwnerMobile))
                throw new Exception("Owner mobile already exists.");

            if (await _userRepository.ExistsByEmailAsync(dto.OwnerEmail))
                throw new Exception("Owner email already exists.");
        }

        // =====================================================
        // STORE CODE
        // =====================================================

        private async Task<string> GenerateStoreCode(StoreOnboardingDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Code))
                return dto.Code;

            return await _storeRepository.GenerateStoreCodeAsync();
        }

        // =====================================================
        // PASSWORD
        // =====================================================

        private string GeneratePassword(StoreOnboardingDto dto)
        {
            if (!dto.AutoGeneratePassword &&
                !string.IsNullOrWhiteSpace(dto.TemporaryPassword))
            {
                return dto.TemporaryPassword;
            }

            return _authService.GenerateTemporaryPassword();
        }

        // =====================================================
        // ONBOARD STORE
        // =====================================================

        public async Task<StoreOnboardingResultDto> OnboardStoreAsync(StoreOnboardingDto dto)
        {
            await ValidateRequest(dto);

            var storeCode = await GenerateStoreCode(dto);

            var password = GeneratePassword(dto);

            return new StoreOnboardingResultDto
            {
                Success = true,
                Message = "Validation successful.",
                StoreCode = storeCode,
                TemporaryPassword = password
            };
        }
    }
}