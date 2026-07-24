namespace QuickCommerce.Core.DTOs.Onboarding
{
    public class ApproveOnboardingDto
    {
        public int RequestId { get; set; }

        public int ApprovedBy { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? PermissionsJson { get; set; }
        // ["ORDERS_VIEW","ORDERS_UPDATE"]
    }
}