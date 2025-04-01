using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FixedAssetInventory.Models;
using FixedAssetInventory.Services;

namespace FixedAssetInventory.Views
{
    public partial class ReferenceManagerDialog : Form
    {
        private readonly IAssetService _assetService;
        private readonly ReferenceType _referenceType;
        private BindingSource _bindingSource = new BindingSource();

        public ReferenceManagerDialog(IAssetService assetService, ReferenceType referenceType)
        {
            InitializeComponent();
            _assetService = assetService;
            _referenceType = referenceType;
            
            InitializeForm();
            LoadData();
            SetupDataGridView();
        }

        private void InitializeForm()
        {
            Text = _referenceType == ReferenceType.AssetType 
                ? "Управление типами ОС" 
                : "Управление подразделениями";

            btnAdd.Text = "Добавить";
            btnEdit.Text = "Редактировать";
            btnDelete.Text = "Удалить";
            
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void LoadData()
        {
            try
            {
                if (_referenceType == ReferenceType.AssetType)
                {
                    _bindingSource.DataSource = _assetService.GetAllAssetTypes().ToList();
                }
                else
                {
                    _bindingSource.DataSource = _assetService.GetAllDepartments().ToList();
                }
                dataGridView.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.RowHeadersVisible = false;
            
            dataGridView.Columns.Clear();
            
            var columns = new[]
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Name", 
                    HeaderText = "Наименование", 
                    DataPropertyName = "Name", 
                    Width = 250 
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Description", 
                    HeaderText = "Описание", 
                    DataPropertyName = "Description", 
                    Width = 350,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill 
                }
            };

            dataGridView.Columns.AddRange(columns);
            dataGridView.ClearSelection();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new ReferenceItemEditDialog(_referenceType);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (_referenceType == ReferenceType.AssetType)
                    {
                        var assetType = new AssetType 
                        { 
                            Name = form.ItemName, 
                            Description = form.Description 
                        };
                        _assetService.CreateAssetType(assetType);
                    }
                    else
                    {
                        var department = new Department 
                        { 
                            Name = form.ItemName, 
                            Description = form.Description 
                        };
                        _assetService.CreateDepartment(department);
                    }
                    
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0) return;

            var selectedItem = dataGridView.SelectedRows[0].DataBoundItem;
            
            string currentName = "";
            string currentDescription = "";
            
            if (_referenceType == ReferenceType.AssetType)
            {
                var assetType = (AssetType)selectedItem;
                currentName = assetType.Name;
                currentDescription = assetType.Description;
            }
            else
            {
                var department = (Department)selectedItem;
                currentName = department.Name;
                currentDescription = department.Description;
            }

            var form = new ReferenceItemEditDialog(_referenceType, currentName, currentDescription);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (_referenceType == ReferenceType.AssetType)
                    {
                        var assetType = (AssetType)selectedItem;
                        assetType.Name = form.ItemName;
                        assetType.Description = form.Description;
                        _assetService.UpdateAssetType(assetType);
                    }
                    else
                    {
                        var department = (Department)selectedItem;
                        department.Name = form.ItemName;
                        department.Description = form.Description;
                        _assetService.UpdateDepartment(department);
                    }
                    
                    _bindingSource.ResetCurrentItem();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при редактировании: {ex.Message}", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0) return;

            var selectedItem = dataGridView.SelectedRows[0].DataBoundItem;
            string itemName = _referenceType == ReferenceType.AssetType 
                ? ((AssetType)selectedItem).Name 
                : ((Department)selectedItem).Name;

            if (MessageBox.Show($"Вы действительно хотите удалить '{itemName}'?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_referenceType == ReferenceType.AssetType)
                    {
                        var assetType = (AssetType)selectedItem;
                        _assetService.DeleteAssetType(assetType.Id);
                    }
                    else
                    {
                        var department = (Department)selectedItem;
                        _assetService.DeleteDepartment(department.Id);
                    }
                    
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\n{ex.InnerException?.Message}", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public enum ReferenceType
    {
        AssetType,
        Department
    }
}