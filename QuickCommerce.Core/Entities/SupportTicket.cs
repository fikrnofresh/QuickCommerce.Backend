using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("support_tickets")]
    public class SupportTicket
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("status")]
        public string Status { get; set; } = "OPEN";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}