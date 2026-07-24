using System;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class StoreSettlementDto
    {
        public int StoreId { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal PlatformCommission { get; set; }

        public decimal StorePayout { get; set; }

        public DateTime SettlementDate { get; set; }

        public string Status { get; set; }
    }
}