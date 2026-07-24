using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("notification_recipients")]
    public class NotificationRecipient
    {
        [Key]
        public int Id { get; set; }

        [Column("notificationid")]
        public int NotificationId { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }

        [Column("userid")]
        public int? UserId { get; set; }

        [Column("storeid")]
        public int? StoreId { get; set; }

        [Column("AssignedToPartnerId")]
        public int? AssignedToPartnerId { get; set; }

        [Column("isread")]
        public bool IsRead { get; set; } = false;

        [Column("readat")]
        public DateTime? ReadAt { get; set; }
    }
}