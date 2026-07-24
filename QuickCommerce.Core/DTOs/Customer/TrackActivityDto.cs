namespace QuickCommerce.Core.DTOs.Customer
{
    public class TrackActivityDto
    {
        public string Action { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? Metadata { get; set; }
    }
}