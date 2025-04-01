using System;
using System.Collections.Generic;
using FixedAssetInventory.Models;
using FixedAssetInventory.Services;
using FixedAssetInventory.Views;

namespace FixedAssetInventory.Presenters
{
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly IAssetService _assetService;
        private List<Asset> _currentAssets;

        public MainPresenter(IMainView view, IAssetService assetService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _assetService = assetService ?? throw new ArgumentNullException(nameof(assetService));

            WireUpEvents();
            LoadInitialData();
        }

        private void WireUpEvents()
        {
            _view.Load += OnViewLoad;
            _view.AddAsset += OnAddAsset;
            _view.EditAsset += OnEditAsset;
            _view.DeleteAsset += OnDeleteAsset;
            _view.ApplyFilter += OnApplyFilter;
            _view.ResetFilter += OnResetFilter;
            _view.ExportToExcel += OnExportToExcel;
            _view.ManageAssetTypes += OnManageAssetTypes;
            _view.ManageDepartments += OnManageDepartments;
        }

        private void LoadInitialData()
        {
            try
            {
                _currentAssets = new List<Asset>(_assetService.GetAllAssets());
                _view.DisplayAssets(_currentAssets);
                _view.SetStatusMessage($"Загружено записей: {_currentAssets.Count}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void OnViewLoad(object sender, EventArgs e)
        {
            LoadInitialData();
        }

        private void OnAddAsset(object sender, EventArgs e)
        {
            try
            {
                var result = _view.ShowAssetDialog(new Asset());
                if (result == DialogResult.OK)
                {
                    var newAsset = _view.GetAssetData();
                    _assetService.CreateAsset(newAsset);
                    RefreshData();
                    _view.ShowSuccess("Основное средство успешно добавлено");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private void OnEditAsset(object sender, EventArgs e)
        {
            if (_view.SelectedAssetId == null)
            {
                _view.ShowWarning("Не выбрано основное средство для редактирования");
                return;
            }

            try
            {
                var asset = _assetService.GetAsset(_view.SelectedAssetId.Value);
                var result = _view.ShowAssetDialog(asset);
                if (result == DialogResult.OK)
                {
                    var updatedAsset = _view.GetAssetData();
                    _assetService.UpdateAsset(updatedAsset);
                    RefreshData();
                    _view.ShowSuccess("Изменения сохранены успешно");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка при редактировании: {ex.Message}");
            }
        }

        private void OnDeleteAsset(object sender, EventArgs e)
        {
            if (_view.SelectedAssetId == null)
            {
                _view.ShowWarning("Не выбрано основное средство для удаления");
                return;
            }

            try
            {
                if (_view.ConfirmDelete("Вы действительно хотите удалить это основное средство?"))
                {
                    _assetService.DeleteAsset(_view.SelectedAssetId.Value);
                    RefreshData();
                    _view.ShowSuccess("Основное средство удалено успешно");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void OnApplyFilter(object sender, FilterParameters filters)
        {
            try
            {
                _currentAssets = new List<Asset>(_assetService.SearchAssets(filters));
                _view.DisplayAssets(_currentAssets);
                _view.SetStatusMessage($"Найдено записей: {_currentAssets.Count}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private void OnResetFilter(object sender, EventArgs e)
        {
            LoadInitialData();
        }

        private void OnExportToExcel(object sender, EventArgs e)
        {
            try
            {
                var filePath = _view.ShowSaveFileDialog("Excel Files|*.xlsx");
                if (!string.IsNullOrEmpty(filePath))
                {
                    ExcelExporter.Export(_currentAssets, filePath);
                    _view.ShowSuccess($"Данные успешно экспортированы в {filePath}");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка экспорта: {ex.Message}");
            }
        }

        private void OnManageAssetTypes(object sender, EventArgs e)
        {
            try
            {
                _view.ShowAssetTypeManager();
                RefreshData();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка управления типами ОС: {ex.Message}");
            }
        }

        private void OnManageDepartments(object sender, EventArgs e)
        {
            try
            {
                _view.ShowDepartmentManager();
                RefreshData();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка управления подразделениями: {ex.Message}");
            }
        }

        private void RefreshData()
        {
            _currentAssets = new List<Asset>(_assetService.GetAllAssets());
            _view.DisplayAssets(_currentAssets);
            _view.SetStatusMessage($"Загружено записей: {_currentAssets.Count}");
        }
    }
}