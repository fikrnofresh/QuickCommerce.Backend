using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("addresses")]
    public class Address
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("label")]
        public string? Label { get; set; }

        [Column("housenumber")]
        public string HouseNumber { get; set; } = string.Empty;

        [Column("street")]
        public string Street { get; set; } = string.Empty;

        [Column("landmark")]
        public string? Landmark { get; set; }

        [Column("area")]
        public string Area { get; set; } = string.Empty;

        [Column("city")]
        public string City { get; set; } = string.Empty;

        [Column("state")]
        public string State { get; set; } = string.Empty;

        [Column("pincode")]
        public string Pincode { get; set; } = string.Empty;

        [Column("latitude")]
        public decimal? Latitude { get; set; }

        [Column("longitude")]
        public decimal? Longitude { get; set; }

        [Column("isdefault")]
        public bool IsDefault { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }
    }
}
