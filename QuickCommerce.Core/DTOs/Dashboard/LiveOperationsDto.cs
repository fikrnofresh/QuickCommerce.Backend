namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class LiveOperationsDto
    {
        public int PendingOrders { get; set; }

        public int PreparingOrders { get; set; }

        public int ReadyForPickupOrders { get; set; }

        public int OutForDeliveryOrders { get; set; }

        public int DelayedOrders { get; set; }

        public int ActiveDeliveryPartners { get; set; }

        public int BusyDeliveryPartners { get; set; }
    }
}