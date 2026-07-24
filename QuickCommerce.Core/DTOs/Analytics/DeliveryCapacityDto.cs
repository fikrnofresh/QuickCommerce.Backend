namespace QuickCommerce.Core.DTOs.Analytics
{
    public class DeliveryCapacityDto
    {
        public int ActivePartners { get; set; }
        public int BusyPartners { get; set; }
        public int OrdersWaiting { get; set; }
        public bool CapacityShortage { get; set; }
    }
}