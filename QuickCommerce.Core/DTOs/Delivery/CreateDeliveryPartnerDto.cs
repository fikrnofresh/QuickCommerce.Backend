namespace QuickCommerce.Core.DTOs.Delivery
{
    public class CreateDeliveryPartnerDto
    {
        public int UserId { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public string? DrivingLicenseNumber { get; set; }
        public string? AadharNumber { get; set; }
    }
}