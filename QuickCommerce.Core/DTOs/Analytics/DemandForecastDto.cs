namespace QuickCommerce.Core.DTOs.Analytics
{
    public class DemandForecastDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        
        public int SoldLast7Days { get; set; }

        public decimal DailyAverage { get; set; }

        public int CurrentStock { get; set; }

        public int EstimatedDemandTomorrow { get; set; }

        public int DaysUntilStockout { get; set; }

        // Used by AdminDashboardService
        public int EstimatedDaysRemaining { get; set; }
    }
}