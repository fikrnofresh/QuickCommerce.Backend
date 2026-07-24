using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("wallet_transaction")]
    public class WalletTransaction
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("type")]
        public string Type { get; set; } = "CREDIT"; // CREDIT / DEBIT

        [Column("reference")]
        public string? Reference { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}