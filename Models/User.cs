using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}