namespace QuickCommerce.Core.Constants
{
    public static class StoreConstants
    {
        // ===========================
        // Roles
        // ===========================

        public const string StoreOwnerRole = "STORE_OWNER";
        public const string StoreManagerRole = "STORE_MANAGER";
        public const string StoreStaffRole = "STORE_STAFF";

        // ===========================
        // Code Prefix
        // ===========================

        public const string StoreCodePrefix = "STR";

        // ===========================
        // Default Values
        // ===========================

        public const bool DefaultIsActive = true;
        public const bool DefaultIsOnline = true;
        public const bool DefaultIsVerified = false;

        // ===========================
        // Inventory
        // ===========================

        public const int DefaultLowStockThreshold = 5;

        // ===========================
        // Password
        // ===========================

        public const int TemporaryPasswordLength = 10;
    }
}     