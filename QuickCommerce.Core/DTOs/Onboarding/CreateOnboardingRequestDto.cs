namespace QuickCommerce.Core.DTOs.Onboarding
{
    public class CreateOnboardingRequestDto
    {
        public string Type { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public int? StoreId { get; set; }

        public string? RequestedRole { get; set; }

        public string? RequestedPermissions { get; set; }
    }
}