using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class FinancialDashboardDto
    {
        public decimal PlatformTotalEarnings { get; set; }

        public decimal TotalStorePayout { get; set; }

        public decimal PendingSettlements { get; set; }

        public decimal CompletedSettlements { get; set; }

        public List<TopStoreEarningDto> TopEarningStores { get; set; } = new();

        public List<CategoryCommissionDto> CommissionByCategory { get; set; } = new();
    }

    public class TopStoreEarningDto
    {
        public int StoreId { get; set; }

        public string StoreName { get; set; }

        public decimal Revenue { get; set; }

        public decimal PlatformCommission { get; set; }
    }

    public class CategoryCommissionDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal PlatformCommission { get; set; }
    }
}