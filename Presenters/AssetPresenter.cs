using System;
using System.Collections.Generic;
using FixedAssetInventory.Models;
using FixedAssetInventory.Services;
using FixedAssetInventory.Views;

namespace FixedAssetInventory.Presenters
{
    public class AssetPresenter
    {
        private readonly IAssetView _view;
        private readonly IAssetService _service;
        private Asset _currentAsset;

        public AssetPresenter(IAssetView view, IAssetService service)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.ViewLoading += OnViewLoading;
            _view.SaveRequested += OnSaveRequested;
            _view.ValidationRequired += OnValidationRequired;
        }

        private void OnViewLoading(object sender, EventArgs e)
        {
            try
            {
                if (_view.IsEditMode && _view.AssetId.HasValue)
                {
                    _currentAsset = _service.GetAsset(_view.AssetId.Value);
                    _view.DisplayAsset(_currentAsset);
                }
                else
                {
                    _currentAsset = new Asset();
                }

                LoadReferenceData();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadReferenceData()
        {
            _view.PopulateAssetTypes(_service.GetAllAssetTypes());
            _view.PopulateDepartments(_service.GetAllDepartments());
            _view.PopulateStatuses(new List<string> { "В эксплуатации", "На складе", "В ремонте", "Списано" });
        }

        private void OnSaveRequested(object sender, EventArgs e)
        {
            try
            {
                if (!_view.ValidateForm())
                    return;

                var asset = _view.GetAssetData();

                if (_view.IsEditMode)
                {
                    _service.UpdateAsset(asset);
                    _view.ShowSuccess("Основное средство успешно обновлено");
                }
                else
                {
                    _service.CreateAsset(asset);
                    _view.ShowSuccess("Основное средство успешно добавлено");
                }

                _view.CloseWithSuccess();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void OnValidationRequired(object sender, AssetValidationEventArgs e)
        {
            try
            {
                e.IsValid = true;

                if (string.IsNullOrWhiteSpace(e.Asset.InventoryNumber))
                {
                    e.ErrorMessage = "Инвентарный номер обязателен";
                    e.IsValid = false;
                }
                else if (!_service.IsInventoryNumberUnique(
                    e.Asset.InventoryNumber, 
                    _view.IsEditMode ? e.Asset.Id : (int?)null))
                {
                    e.ErrorMessage = "Инвентарный номер должен быть уникальным";
                    e.IsValid = false;
                }

                if (string.IsNullOrWhiteSpace(e.Asset.Name))
                {
                    e.ErrorMessage = "Наименование обязательно";
                    e.IsValid = false;
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Ошибка валидации: {ex.Message}");
                e.IsValid = false;
            }
        }
    }
}