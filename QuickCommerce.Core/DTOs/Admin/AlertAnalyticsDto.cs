namespace QuickCommerce.Core.DTOs.Admin
{
    public class AlertAnalyticsDto
    {
        public bool LowStockAlert { get; set; }
        public bool CancelSpikeAlert { get; set; }
        public bool RevenueDropAlert { get; set; }
        public bool StuckOrdersAlert { get; set; }
        public bool DeadProductAlert { get; set; }

        public string? Message { get; set; }
    }
}