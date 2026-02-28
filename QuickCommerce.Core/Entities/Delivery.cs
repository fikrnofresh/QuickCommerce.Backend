using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("deliveries")]
    public class Delivery
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("orderid")]
        public int OrderId { get; set; }

        [Column("deliverypartnerid")]
        public int DeliveryPartnerId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("assignedat")]
        public DateTime? AssignedAt { get; set; }

        [Column("acceptedat")]
        public DateTime? AcceptedAt { get; set; }

        [Column("pickedupat")]
        public DateTime? PickedUpAt { get; set; }

        [Column("deliveredat")]
        public DateTime? DeliveredAt { get; set; }

        [Column("distanceinkm")]
        public decimal? DistanceInKm { get; set; }

        [Column("deliveryfee")]
        public decimal? DeliveryFee { get; set; }

        [Column("partnerearnings")]
        public decimal? PartnerEarnings { get; set; }

        [Column("rating")]
        public int? Rating { get; set; }

        [Column("feedback")]
        public string? Feedback { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedat")]
        public DateTime UpdatedAt { get; set; }

        [Column("codcollectedamount")]
        public decimal? CodCollectedAmount { get; set; }

        [Column("codcollectedat")]
        public DateTime? CodCollectedAt { get; set; }

        [Column("iscodsettled")]
        public bool? IsCodSettled { get; set; }

        // Navigation Properties (NO ForeignKey attributes)
        public virtual Order Order { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
    }
}
