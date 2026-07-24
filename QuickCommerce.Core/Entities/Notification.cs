using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("module")]
        public string Module { get; set; }

        [Column("entityname")]
        public string EntityName { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<NotificationRecipient> Recipients { get; set; }
        [Column("is_read")]
        public bool IsRead { get; set; } = false;
    }
}