using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FixedAssetInventory.Models;
using FixedAssetInventory.Services;

namespace FixedAssetInventory.Views
{
    public partial class AssetDialog : Form
    {
        private readonly IAssetService _assetService;
        private readonly Asset _asset;
        private readonly bool _isEditMode;

        public AssetDialog(IAssetService assetService, Asset asset = null)
        {
            InitializeComponent();
            _assetService = assetService;
            _asset = asset;
            _isEditMode = asset != null;

            InitializeForm();
            LoadAssetTypes();
            LoadDepartments();
            LoadStatuses();
            
            if (_isEditMode)
            {
                LoadAssetData();
            }
        }

        private void InitializeForm()
        {
            Text = _isEditMode ? "Редактирование основного средства" : "Добавление основного средства";
            btnSave.Text = _isEditMode ? "Сохранить" : "Добавить";
            
            // Настройка валидации
            inventoryNumberTextBox.Validating += ValidateInventoryNumber;
            nameTextBox.Validating += ValidateRequiredField;
            assetTypeComboBox.Validating += ValidateRequiredComboBox;
            departmentComboBox.Validating += ValidateRequiredComboBox;
        }

        private void LoadAssetTypes()
        {
            assetTypeComboBox.DataSource = _assetService.GetAllAssetTypes();
            assetTypeComboBox.DisplayMember = "Name";
            assetTypeComboBox.ValueMember = "Id";
        }

        private void LoadDepartments()
        {
            departmentComboBox.DataSource = _assetService.GetAllDepartments();
            departmentComboBox.DisplayMember = "Name";
            departmentComboBox.ValueMember = "Id";
        }

        private void LoadStatuses()
        {
            statusComboBox.DataSource = new List<string> 
            { 
                "В эксплуатации", 
                "На складе", 
                "В ремонте", 
                "Списано" 
            };
        }

        private void LoadAssetData()
        {
            inventoryNumberTextBox.Text = _asset.InventoryNumber;
            nameTextBox.Text = _asset.Name;
            serialNumberTextBox.Text = _asset.SerialNumber;
            molTextBox.Text = _asset.MOL;
            locationTextBox.Text = _asset.Location;
            notesTextBox.Text = _asset.Notes;
            
            assetTypeComboBox.SelectedValue = _asset.AssetTypeId;
            departmentComboBox.SelectedValue = _asset.DepartmentId;
            statusComboBox.SelectedItem = _asset.Status;
            
            if (_asset.PurchaseDate.HasValue)
            {
                purchaseDatePicker.Value = _asset.PurchaseDate.Value;
            }
            else
            {
                purchaseDatePicker.Checked = false;
            }
        }

        private void ValidateInventoryNumber(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inventoryNumberTextBox.Text))
            {
                errorProvider.SetError(inventoryNumberTextBox, "Инвентарный номер обязателен");
                e.Cancel = true;
            }
            else if (!_assetService.IsInventoryNumberUnique(
                inventoryNumberTextBox.Text, 
                _isEditMode ? _asset.Id : (int?)null))
            {
                errorProvider.SetError(inventoryNumberTextBox, "Инвентарный номер должен быть уникальным");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(inventoryNumberTextBox, "");
            }
        }

        private void ValidateRequiredField(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, "Поле обязательно для заполнения");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(textBox, "");
            }
        }

        private void ValidateRequiredComboBox(object sender, CancelEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedItem == null)
            {
                errorProvider.SetError(comboBox, "Необходимо выбрать значение");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(comboBox, "");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            var asset = _isEditMode ? _asset : new Asset();
            
            asset.InventoryNumber = inventoryNumberTextBox.Text.Trim();
            asset.Name = nameTextBox.Text.Trim();
            asset.SerialNumber = serialNumberTextBox.Text.Trim();
            asset.MOL = molTextBox.Text.Trim();
            asset.Location = locationTextBox.Text.Trim();
            asset.Notes = notesTextBox.Text.Trim();
            asset.AssetTypeId = (int)assetTypeComboBox.SelectedValue;
            asset.DepartmentId = (int)departmentComboBox.SelectedValue;
            asset.Status = statusComboBox.SelectedItem.ToString();
            asset.PurchaseDate = purchaseDatePicker.Checked ? purchaseDatePicker.Value : (DateTime?)null;

            try
            {
                if (_isEditMode)
                {
                    _assetService.UpdateAsset(asset);
                }
                else
                {
                    _assetService.CreateAsset(asset);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}