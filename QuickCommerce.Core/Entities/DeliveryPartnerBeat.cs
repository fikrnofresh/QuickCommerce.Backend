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

        [Column("deliverypartnerid")]
        public int DeliveryPartnerId { get; set; }

        [ForeignKey("DeliveryPartnerId")]
        public DeliveryPartner DeliveryPartner { get; set; } = null!;

        [Column("beatname")]
        public string BeatName { get; set; } = null!;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }
    }
}
