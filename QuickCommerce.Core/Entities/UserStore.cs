using System;
using System.ComponentModel.DataAnnotations.Schema;
using QuickCommerce.Core.Entities;


namespace QuickCommerce.Core.Entities
{
    [Table("user_stores")]
    public class UserStore
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public bool IsPrimary { get; set; } = false;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}