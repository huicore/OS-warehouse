using System.ComponentModel.DataAnnotations;

namespace AssetManagementSystem.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        // Права доступа
        public bool CanManageAssets { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanViewReports { get; set; }
        public bool CanExportData { get; set; }
        public bool CanViewAuditLogs { get; set; }

        // Связи
        public ICollection<User> Users { get; set; }
    }
}