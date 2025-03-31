using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagementSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}