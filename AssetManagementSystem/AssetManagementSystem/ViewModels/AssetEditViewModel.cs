using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class AssetEditViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private Asset _asset;

        public Asset Asset
        {
            get => _asset;
            set => SetField(ref _asset, value);
        }

        public string Title => Asset.Id == 0 ? "Добавить ОС" : "Редактировать ОС";
        public List<AssetType> AssetTypes { get; } = new List<AssetType>();
        public List<string> Statuses { get; } = new List<string>
        {
            "В работе",
            "На складе",
            "Требуется ремонт",
            "Вышел из строя"
        };

        public ICommand SaveCommand { get; }

        public AssetEditViewModel(AppDbContext context, Asset asset)
        {
            _context = context;
            Asset = asset;

            LoadAssetTypes();

            SaveCommand = new RelayCommand(_ => Save());
        }

        private async void LoadAssetTypes()
        {
            var types = await _context.AssetTypes.ToListAsync();
            AssetTypes.Clear();
            AssetTypes.AddRange(types);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Asset.InventoryNumber) ||
                string.IsNullOrWhiteSpace(Asset.Name) ||
                Asset.AssetTypeId == 0)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Asset.Id == 0)
            {
                _context.Assets.Add(Asset);
            }

            _context.SaveChanges();

            MessageBox.Show("Данные сохранены", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            CloseWindow(true);
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