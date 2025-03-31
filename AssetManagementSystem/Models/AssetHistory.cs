using System;

namespace AssetManagementSystem.Models
{
    public class AssetHistory
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        public string ChangedField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        public int ChangedById { get; set; }
        public User ChangedBy { get; set; }
    }
}