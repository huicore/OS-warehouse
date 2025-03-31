using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class AssetEditViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private Asset _asset;
        private string _validationError;

        public Asset Asset
        {
            get => _asset;
            set => SetField(ref _asset, value);
        }

        public string Title => Asset.Id == 0 ? "Добавить ОС" : "Редактировать ОС";

        public List<AssetType> AssetTypes { get; } = new List<AssetType>();
        public List<Department> Departments { get; } = new List<Department>();
        public List<string> Statuses { get; } = new List<string>
        {
            "В эксплуатации",
            "На складе",
            "В ремонте",
            "Списано"
        };

        public string ValidationError
        {
            get => _validationError;
            set => SetField(ref _validationError, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AssetEditViewModel(AppDbContext context, Asset asset)
        {
            _context = context;
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));

            LoadReferenceData();

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private async void LoadReferenceData()
        {
            try
            {
                AssetTypes.AddRange(await _context.AssetTypes.OrderBy(at => at.Name).ToListAsync());
                Departments.AddRange(await _context.Departments.OrderBy(d => d.Name).ToListAsync());

                OnPropertyChanged(nameof(AssetTypes));
                OnPropertyChanged(nameof(Departments));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Asset.InventoryNumber))
                errors.Add("Инвентарный номер обязателен");

            if (string.IsNullOrWhiteSpace(Asset.Name))
                errors.Add("Наименование обязательно");

            if (Asset.AssetTypeId == 0)
                errors.Add("Выберите тип ОС");

            if (Asset.DepartmentId == 0)
                errors.Add("Выберите подразделение");

            if (Asset.PurchaseDate > DateTime.Now)
                errors.Add("Дата покупки не может быть в будущем");

            ValidationError = errors.Any() ? string.Join("\n", errors) : null;
            return !errors.Any();
        }

        private void Save()
        {
            if (!Validate())
                return;

            try
            {
                if (Asset.Id == 0)
                {
                    Asset.CreatedAt = DateTime.Now;
                    Asset.CreatedById = CurrentUser.Instance.User.Id;
                    _context.Assets.Add(Asset);
                }
                else
                {
                    Asset.UpdatedAt = DateTime.Now;
                    _context.Assets.Update(Asset);
                }

                _context.SaveChanges();
                CloseWindow(true);
            }
            catch (DbUpdateException ex)
            {
                string errorMessage = ex.InnerException?.Message.Contains("IX_Assets_InventoryNumber") == true
                    ? "ОС с таким инвентарным номером уже существует"
                    : $"Ошибка сохранения: {ex.Message}";

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            if (Asset.Id == 0 || MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CloseWindow(false);
            }
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}