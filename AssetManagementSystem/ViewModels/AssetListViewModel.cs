using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services;
using AssetManagementSystem.Views.Dialogs;
using AssetManagementSystem.Views.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class AssetListViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private readonly ExportService _exportService;
        private readonly AuditService _auditService;
        private Asset _selectedAsset;
        private string _searchText;
        private int _currentPage = 1;
        private const int ItemsPerPage = 20;
        private int _totalPages;

        public ObservableCollection<Asset> Assets { get; } = new ObservableCollection<Asset>();
        public ICollectionView FilteredAssets => CollectionViewSource.GetDefaultView(Assets);

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
                OnPropertyChanged(nameof(PagedAssets));
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetField(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetField(ref _totalPages, value);
        }

        public ObservableCollection<Asset> PagedAssets => new ObservableCollection<Asset>(
            FilteredAssets.Cast<Asset>()
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
        );

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand RefreshCommand { get; }

        public AssetListViewModel(
            AppDbContext context,
            ExportService exportService,
            AuditService auditService)
        {
            _context = context;
            _exportService = exportService;
            _auditService = auditService;

            FilteredAssets.Filter = FilterAssets;

            AddCommand = new RelayCommand(_ => AddAsset(),
                _ => CurrentUser.Instance.User?.Role?.CanManageAssets ?? false);

            EditCommand = new RelayCommand(_ => EditAsset(),
                _ => SelectedAsset != null && (CurrentUser.Instance.User?.Role?.CanManageAssets ?? false));

            DeleteCommand = new RelayCommand(async _ => await DeleteAsset(),
                _ => SelectedAsset != null && (CurrentUser.Instance.User?.Role?.CanManageAssets ?? false));

            ExportCommand = new RelayCommand(_ => ExportToExcel(),
                _ => CurrentUser.Instance.User?.Role?.CanExportData ?? false);

            ShowHistoryCommand = new RelayCommand(_ => ShowHistory(),
                _ => SelectedAsset != null);

            NextPageCommand = new RelayCommand(_ =>
            {
                if (CurrentPage < TotalPages)
                {
                    CurrentPage++;
                    OnPropertyChanged(nameof(PagedAssets));
                }
            });

            PrevPageCommand = new RelayCommand(_ =>
            {
                if (CurrentPage > 1)
                {
                    CurrentPage--;
                    OnPropertyChanged(nameof(PagedAssets));
                }
            });

            RefreshCommand = new RelayCommand(_ => LoadAssets());

            LoadAssets();
        }

        private bool FilterAssets(object obj)
        {
            if (obj is not Asset asset) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            return asset.InventoryNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   asset.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   asset.AssetType?.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                   asset.Department?.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
        }

        private async void LoadAssets()
        {
            try
            {
                Assets.Clear();
                var assets = await _context.Assets
                    .Include(a => a.AssetType)
                    .Include(a => a.Department)
                    .Include(a => a.CreatedBy)
                    .OrderBy(a => a.InventoryNumber)
                    .ToListAsync();

                foreach (var asset in assets)
                {
                    Assets.Add(asset);
                }

                CalculatePagination();
                OnPropertyChanged(nameof(PagedAssets));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculatePagination()
        {
            TotalPages = (int)Math.Ceiling((double)FilteredAssets.Cast<object>().Count() / ItemsPerPage);
            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;
        }

        private void ApplyFilters()
        {
            FilteredAssets.Refresh();
            CalculatePagination();
            CurrentPage = 1;
        }

        private void AddAsset()
        {
            var dialog = new AssetEditDialog(new Asset());
            if (dialog.ShowDialog() == true)
            {
                _auditService.LogActionAsync(
                    CurrentUser.Instance.User.Id,
                    "Добавление ОС",
                    $"Добавлен актив: {dialog.Asset.InventoryNumber}"
                ).ConfigureAwait(false);
                LoadAssets();
            }
        }

        private void EditAsset()
        {
            var dialog = new AssetEditDialog(SelectedAsset);
            if (dialog.ShowDialog() == true)
            {
                _auditService.LogActionAsync(
                    CurrentUser.Instance.User.Id,
                    "Редактирование ОС",
                    $"Изменен актив: {SelectedAsset.InventoryNumber}"
                ).ConfigureAwait(false);
                LoadAssets();
            }
        }

        private async Task DeleteAsset()
        {
            var confirm = MessageBox.Show(
                $"Удалить актив {SelectedAsset.InventoryNumber}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _context.Assets.Remove(SelectedAsset);
                await _context.SaveChangesAsync();

                await _auditService.LogActionAsync(
                    CurrentUser.Instance.User.Id,
                    "Удаление ОС",
                    $"Удален актив: {SelectedAsset.InventoryNumber}"
                );

                LoadAssets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"Основные средства_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() != true) return;

            try
            {
                _exportService.ExportToExcel(
                    FilteredAssets.Cast<Asset>().ToList(),
                    saveDialog.FileName
                );

                _auditService.LogActionAsync(
                    CurrentUser.Instance.User.Id,
                    "Экспорт данных",
                    $"Экспортировано {FilteredAssets.Count} активов"
                ).ConfigureAwait(false);

                MessageBox.Show("Экспорт завершен успешно",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}