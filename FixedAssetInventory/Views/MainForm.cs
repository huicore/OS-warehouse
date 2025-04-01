using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FixedAssetInventory.Models;
using FixedAssetInventory.Services;
using FixedAssetInventory.Data;

namespace FixedAssetInventory
{
    public partial class MainForm : Form
    {
        private readonly IAssetService _assetService;
        private List<Asset> _assets;
        private FilterParameters _currentFilters;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            _assetService = new AssetService(new AssetRepository(Properties.Settings.Default.DatabaseConnectionString));
            InitializeUI();
            LoadData();
            ApplyTheme();
        }

        private void InitializeDatabaseConnection()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.DatabaseConnectionString))
            {
                var dbDialog = new DatabaseConnectionDialog();
                if (dbDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.DatabaseConnectionString = dbDialog.ConnectionString;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Close();
                }
            }
        }

        private void InitializeUI()
        {
            // Настройка DataGridView
            assetsDataGridView.AutoGenerateColumns = false;
            assetsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            assetsDataGridView.MultiSelect = false;
            assetsDataGridView.AllowUserToResizeRows = false;
            assetsDataGridView.RowHeadersVisible = false;
            assetsDataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 9);

            // Настройка столбцов
            var columns = new[]
            {
                new DataGridViewTextBoxColumn { Name = "InventoryNumber", HeaderText = "Инв. №", DataPropertyName = "InventoryNumber", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "AssetType", HeaderText = "Тип ОС", DataPropertyName = "AssetType", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Наименование", DataPropertyName = "Name", Width = 250, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Статус", DataPropertyName = "Status", Width = 100 }
            };

            assetsDataGridView.Columns.AddRange(columns);

            // Настройка кнопок
            addButton.Click += (s, e) => AddAsset();
            editButton.Click += (s, e) => EditAsset();
            deleteButton.Click += (s, e) => DeleteAsset();
            filterButton.Click += (s, e) => ShowFilterDialog();
            refreshButton.Click += (s, e) => RefreshData();
            exportButton.Click += (s, e) => ExportToExcel();
        }

        private void LoadData()
        {
            try
            {
                _assets = new List<Asset>(_assetService.GetAllAssets());
                assetsDataGridView.DataSource = _assets;
                statusLabel.Text = $"Загружено записей: {_assets.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddAsset()
        {
            var form = new AssetEditForm(_assetService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshData();
            }
        }

        private void EditAsset()
        {
            if (assetsDataGridView.SelectedRows.Count == 0) return;

            var assetId = (int)assetsDataGridView.SelectedRows[0].Cells["Id"].Value;
            var asset = _assetService.GetAsset(assetId);

            var form = new AssetEditForm(_assetService, asset);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshData();
            }
        }

        private void DeleteAsset()
        {
            if (assetsDataGridView.SelectedRows.Count == 0) return;

            var assetId = (int)assetsDataGridView.SelectedRows[0].Cells["Id"].Value;
            var asset = _assetService.GetAsset(assetId);

            if (MessageBox.Show($"Удалить актив {asset.InventoryNumber}?", "Подтверждение", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _assetService.DeleteAsset(assetId);
                RefreshData();
            }
        }

        private void ShowFilterDialog()
        {
            var form = new FilterForm(_assetService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _currentFilters = form.GetFilters();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_currentFilters == null) return;

            _assets = new List<Asset>(_assetService.SearchAssets(_currentFilters));
            assetsDataGridView.DataSource = _assets;
            statusLabel.Text = $"Найдено записей: {_assets.Count}";
        }

        private void RefreshData()
        {
            _currentFilters = null;
            LoadData();
        }

        private void ExportToExcel()
        {
            // Реализация экспорта в Excel
        }

        private void ApplyTheme()
        {
            // Применение выбранной темы
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _assetService.Dispose();
        }
    }
}