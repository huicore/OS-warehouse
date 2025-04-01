using System;
using System.ComponentModel;

namespace FixedAssetInventory.Models
{
    public class AssetHistory : INotifyPropertyChanged
    {
        private int _id;
        private int _assetId;
        private string _changedField;
        private string _oldValue;
        private string _newValue;
        private DateTime _changedAt;
        private string _changedBy;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public int AssetId
        {
            get => _assetId;
            set { _assetId = value; OnPropertyChanged(nameof(AssetId)); }
        }

        public string ChangedField
        {
            get => _changedField;
            set { _changedField = value; OnPropertyChanged(nameof(ChangedField)); }
        }

        public string OldValue
        {
            get => _oldValue;
            set { _oldValue = value; OnPropertyChanged(nameof(OldValue)); }
        }

        public string NewValue
        {
            get => _newValue;
            set { _newValue = value; OnPropertyChanged(nameof(NewValue)); }
        }

        public DateTime ChangedAt
        {
            get => _changedAt;
            set { _changedAt = value; OnPropertyChanged(nameof(ChangedAt)); }
        }

        public string ChangedBy
        {
            get => _changedBy;
            set { _changedBy = value; OnPropertyChanged(nameof(ChangedBy)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string FormattedChange
        {
            get
            {
                return $"{ChangedAt:dd.MM.yyyy HH:mm} | {ChangedField}: " +
                       $"'{OldValue}' → '{NewValue}' ({ChangedBy})";
            }
        }
    }
}