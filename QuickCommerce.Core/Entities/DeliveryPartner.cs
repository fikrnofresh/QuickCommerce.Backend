using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("deliverypartners")]
    public class DeliveryPartner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("vehicletype")]
        public string? VehicleType { get; set; }

        [Column("vehiclenumber")]
        public string? VehicleNumber { get; set; }

        [Column("drivinglicensenumber")]
        public string? DrivingLicenseNumber { get; set; }

        [Column("aadharnumber")]
        public string? AadharNumber { get; set; }

        [Column("isavailable")]
        public bool IsAvailable { get; set; }

        [Column("currentlatitude")]
        public decimal? CurrentLatitude { get; set; }

        [Column("currentlongitude")]
        public decimal? CurrentLongitude { get; set; }

        [Column("lastlocationupdate")]
        public DateTime? LastLocationUpdate { get; set; }

        [Column("ratingaverage")]
        public decimal? RatingAverage { get; set; }

        [Column("totaldeliveries")]
        public int TotalDeliveries { get; set; }

        [Column("totalearnings")]
        public decimal TotalEarnings { get; set; }

        [Column("isverified")]
        public bool IsVerified { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedat")]
        public DateTime UpdatedAt { get; set; }

        [Column("partnertype")]
        public string? PartnerType { get; set; }

        [Column("commissionperdelivery")]
        public decimal? CommissionPerDelivery { get; set; }

        [Column("isactive")]
        public bool IsActive { get; set; }

        [Column("currentworkload")]
        public int CurrentWorkload { get; set; }

        [Column("rating")]
        public decimal? Rating { get; set; }

        [Column("totalratings")]
        public int TotalRatings { get; set; }

        // 🔥 Correct navigation properties
        public ICollection<Delivery> Deliveries { get; set; }
            = new List<Delivery>();

        public ICollection<DeliveryPartnerBeat> Beats { get; set; }
            = new List<DeliveryPartnerBeat>();
    }
}
