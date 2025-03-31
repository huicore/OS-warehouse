using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagementSystem.Models
{
    public class Asset
    {
        public int Id { get; set; }

        [Required]
        public string InventoryNumber { get; set; }

        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        [Required]
        public string Name { get; set; }

        public string Status { get; set; } = "В работе";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}