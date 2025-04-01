using System;
using System.ComponentModel;

namespace FixedAssetInventory.Models
{
    public class Asset : INotifyPropertyChanged
    {
        private int _id;
        private string _inventoryNumber;
        private int _assetTypeId;
        private string _name;
        private string _serialNumber;
        private string _status;
        private int _departmentId;
        private string _mol;
        private string _location;
        private DateTime? _purchaseDate;
        private string _notes;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string InventoryNumber
        {
            get => _inventoryNumber;
            set { _inventoryNumber = value; OnPropertyChanged(nameof(InventoryNumber)); }
        }

        public int AssetTypeId
        {
            get => _assetTypeId;
            set { _assetTypeId = value; OnPropertyChanged(nameof(AssetTypeId)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set { _serialNumber = value; OnPropertyChanged(nameof(SerialNumber)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public int DepartmentId
        {
            get => _departmentId;
            set { _departmentId = value; OnPropertyChanged(nameof(DepartmentId)); }
        }

        public string MOL
        {
            get => _mol;
            set { _mol = value; OnPropertyChanged(nameof(MOL)); }
        }

        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(nameof(Location)); }
        }

        public DateTime? PurchaseDate
        {
            get => _purchaseDate;
            set { _purchaseDate = value; OnPropertyChanged(nameof(PurchaseDate)); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(nameof(CreatedAt)); }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(nameof(UpdatedAt)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}