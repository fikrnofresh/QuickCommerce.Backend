namespace QuickCommerce.Core.DTOs.Customer
{
    public class CustomerAddressDto
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pincode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}