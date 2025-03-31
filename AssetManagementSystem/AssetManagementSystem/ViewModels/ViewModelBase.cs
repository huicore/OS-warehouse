using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssetManagementSystem.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void ApplyFilters()
        {
            var filteredAssets = _assetService.GetFilteredAssets(_currentFilters);
            Assets.Clear();
            foreach (var asset in filteredAssets)
            {
                Assets.Add(asset);
            }
        }
    }
}