namespace QuickCommerce.Core.Enums
{
    public static class OrderStatus
    {
        // ✅ ADDED (CRITICAL FIX)
        public const string Pending = "PENDING";

        public const string Placed = "PLACED"; // (can be deprecated later)
        public const string Confirmed = "CONFIRMED";
        public const string Preparing = "PREPARING";
        public const string ReadyForPickup = "READY_FOR_PICKUP";
        public const string Assigned = "ASSIGNED";
        public const string OutForDelivery = "OUT_FOR_DELIVERY";
        public const string Delivered = "DELIVERED";
        public const string Completed = "COMPLETED";
        public const string Cancelled = "CANCELLED";

        // ✅ OPTIONAL (GOOD PRACTICE)
        public const string Failed = "FAILED";
    }
}