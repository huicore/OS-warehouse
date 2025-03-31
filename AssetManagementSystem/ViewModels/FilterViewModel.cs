using AssetManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        public List<AssetType> AssetTypes { get; }
        public List<string> Statuses { get; }

        public string InventoryNumberFilter { get; set; }
        public int? SelectedAssetTypeId { get; set; }
        public string NameFilter { get; set; }
        public string SelectedStatus { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }

        public ICommand ApplyCommand { get; }
        public ICommand ResetCommand { get; }

        public event Action<FilterCriteria> FiltersApplied;

        public FilterViewModel(IEnumerable<AssetType> assetTypes)
        {
            AssetTypes = new List<AssetType>(assetTypes);
            Statuses = new List<string> { "В работе", "На складе", "Требуется ремонт", "Вышел из строя" };

            ApplyCommand = new RelayCommand(_ => ApplyFilters());
            ResetCommand = new RelayCommand(_ => ResetFilters());
        }

        private void ApplyFilters()
        {
            var criteria = new FilterCriteria
            {
                InventoryNumber = InventoryNumberFilter,
                AssetTypeId = SelectedAssetTypeId,
                Name = NameFilter,
                Status = SelectedStatus,
                PurchaseDateFrom = PurchaseDateFrom,
                PurchaseDateTo = PurchaseDateTo
            };

            FiltersApplied?.Invoke(criteria);
            CloseWindow();
        }

        private void ResetFilters()
        {
            InventoryNumberFilter = null;
            SelectedAssetTypeId = null;
            NameFilter = null;
            SelectedStatus = null;
            PurchaseDateFrom = null;
            PurchaseDateTo = null;

            FiltersApplied?.Invoke(new FilterCriteria());
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }
    }

    public class FilterCriteria
    {
        public string InventoryNumber { get; set; }
        public int? AssetTypeId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
    }
}