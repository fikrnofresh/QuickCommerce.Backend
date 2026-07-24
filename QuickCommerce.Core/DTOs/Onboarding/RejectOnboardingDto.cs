namespace QuickCommerce.Core.DTOs.Onboarding
{
    public class RejectOnboardingDto
    {
        public int RequestId { get; set; }

        public string Reason { get; set; } = string.Empty;

        public int RejectedBy { get; set; }
    }
}