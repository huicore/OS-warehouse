using System;

namespace FixedAssetInventory.Models
{
    public class FilterParameters
    {
        public string InventoryNumber { get; set; }
        public int? AssetTypeId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public int? DepartmentId { get; set; }
        public string MOL { get; set; }
        public string Location { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }

        public bool HasFilters
        {
            get
            {
                return !string.IsNullOrWhiteSpace(InventoryNumber) ||
                       AssetTypeId.HasValue ||
                       !string.IsNullOrWhiteSpace(Name) ||
                       !string.IsNullOrWhiteSpace(SerialNumber) ||
                       !string.IsNullOrWhiteSpace(Status) ||
                       DepartmentId.HasValue ||
                       !string.IsNullOrWhiteSpace(MOL) ||
                       !string.IsNullOrWhiteSpace(Location) ||
                       PurchaseDateFrom.HasValue ||
                       PurchaseDateTo.HasValue;
            }
        }

        public void Clear()
        {
            InventoryNumber = null;
            AssetTypeId = null;
            Name = null;
            SerialNumber = null;
            Status = null;
            DepartmentId = null;
            MOL = null;
            Location = null;
            PurchaseDateFrom = null;
            PurchaseDateTo = null;
        }
    }
}