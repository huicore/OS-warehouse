using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services;
using AssetManagementSystem.Views.Dialogs;
using AssetManagementSystem.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class AssetListViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private readonly ExportService _exportService;
        private readonly AssetService _assetService;
        private Asset _selectedAsset;
        private FilterCriteria _currentFilters = new FilterCriteria();
        private string _searchText;

        public ObservableCollection<Asset> Assets { get; } = new ObservableCollection<Asset>();

        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set => SetField(ref _selectedAsset, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                ApplyFilters();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public AssetListViewModel(AppDbContext context, ExportService exportService, AssetService assetService)
        {
            _context = context;
            _exportService = exportService;
            _assetService = assetService;

            AddCommand = new RelayCommand(_ => AddAsset(), _ => CurrentUser.Instance.User?.Role?.CanAddAssets ?? false);
            EditCommand = new RelayCommand(_ => EditAsset(), _ => SelectedAsset != null && (CurrentUser.Instance.User?.Role?.CanEditAssets ?? false));
            DeleteCommand = new RelayCommand(_ => DeleteAsset(), _ => SelectedAsset != null && (CurrentUser.Instance.User?.Role?.CanDeleteAssets ?? false));
            ExportCommand = new RelayCommand(_ => ExportToExcel());
            ShowHistoryCommand = new RelayCommand(_ => ShowHistory(), _ => SelectedAsset != null);
            FilterCommand = new RelayCommand(_ => ShowFilterDialog());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            LoadAssets();
        }

        private async void LoadAssets()
        {
            try
            {
                var assets = await _context.Assets
                    .Include(a => a.AssetType)
                    .ToListAsync();

                Assets.Clear();
                foreach (var asset in assets)
                {
                    Assets.Add(asset);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAsset()
        {
            var dialog = new AssetEditDialog(new Asset());
            if (dialog.ShowDialog() == true)
            {
                LoadAssets();
            }
        }

        private void EditAsset()
        {
            var dialog = new AssetEditDialog(SelectedAsset);
            if (dialog.ShowDialog() == true)
            {
                LoadAssets();
            }
        }

        private void DeleteAsset()
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить ОС '{SelectedAsset.InventoryNumber}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Assets.Remove(SelectedAsset);
                    _context.SaveChanges();
                    LoadAssets();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportToExcel()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Основные средства_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    _exportService.ExportToExcel(Assets, saveDialog.FileName);
                    MessageBox.Show("Экспорт завершен успешно", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowHistory()
        {
            var window = new AssetHistoryWindow(SelectedAsset.Id)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            };
            window.ShowDialog();
        }

        private void ShowFilterDialog()
        {
            var dialog = new FilterWindow
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive),
                DataContext = new FilterViewModel(_context.AssetTypes.ToList())
            };

            ((FilterViewModel)dialog.DataContext).FiltersApplied += filters =>
            {
                _currentFilters = filters;
                ApplyFilters();
            };

            dialog.ShowDialog();
        }

        private void ApplyFilters()
        {
            var filteredAssets = _assetService.GetFilteredAssets(_currentFilters)
                .Where(a => string.IsNullOrEmpty(SearchText) ||
                            a.InventoryNumber.Contains(SearchText) ||
                            a.Name.Contains(SearchText) ||
                            (a.AssetType?.Name?.Contains(SearchText) ?? false) ||
                            a.Status.Contains(SearchText))
                .ToList();

            Assets.Clear();
            foreach (var asset in filteredAssets)
            {
                Assets.Add(asset);
            }
        }

        private void ClearFilters()
        {
            _currentFilters = new FilterCriteria();
            SearchText = string.Empty;
            LoadAssets();
        }
    }
}