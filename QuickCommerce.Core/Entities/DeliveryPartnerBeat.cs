using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("deliverypartnerbeats")]
    public class DeliveryPartnerBeat
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("AssignedToPartnerId")]
        public int AssignedToPartnerId { get; set; }

        [ForeignKey("AssignedToPartnerId")]
        public DeliveryPartner DeliveryPartner { get; set; } = null!;

        [Column("beatname")]
        public string BeatName { get; set; } = null!;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }
    }
}
