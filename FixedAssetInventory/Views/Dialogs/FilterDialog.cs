using System;
using System.Windows.Forms;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Views
{
    public partial class FilterDialog : Form
    {
        public FilterParameters FilterParameters { get; private set; }

        public FilterDialog(IAssetService assetService)
        {
            InitializeComponent();
            InitializeControls(assetService);
        }

        private void InitializeControls(IAssetService assetService)
        {
            // Инициализация ComboBox для типов ОС
            cmbAssetType.DataSource = assetService.GetAllAssetTypes();
            cmbAssetType.DisplayMember = "Name";
            cmbAssetType.ValueMember = "Id";
            cmbAssetType.SelectedIndex = -1;

            // Инициализация ComboBox для подразделений
            cmbDepartment.DataSource = assetService.GetAllDepartments();
            cmbDepartment.DisplayMember = "Name";
            cmbDepartment.ValueMember = "Id";
            cmbDepartment.SelectedIndex = -1;

            // Инициализация ComboBox для статусов
            cmbStatus.Items.AddRange(new object[] { "Все", "В эксплуатации", "На складе", "В ремонте", "Списано" });
            cmbStatus.SelectedIndex = 0;

            // Настройка DateTimePicker
            dtpFrom.Value = DateTime.Today.AddYears(-1);
            dtpTo.Value = DateTime.Today;
            dtpFrom.Checked = false;
            dtpTo.Checked = false;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            FilterParameters = new FilterParameters
            {
                InventoryNumber = txtInventoryNumber.Text.Trim(),
                Name = txtName.Text.Trim(),
                SerialNumber = txtSerialNumber.Text.Trim(),
                MOL = txtMOL.Text.Trim(),
                Location = txtLocation.Text.Trim()
            };

            if (cmbAssetType.SelectedIndex > -1)
                FilterParameters.AssetTypeId = (int)cmbAssetType.SelectedValue;

            if (cmbDepartment.SelectedIndex > -1)
                FilterParameters.DepartmentId = (int)cmbDepartment.SelectedValue;

            if (cmbStatus.SelectedIndex > 0)
                FilterParameters.Status = cmbStatus.SelectedItem.ToString();

            if (dtpFrom.Checked)
                FilterParameters.PurchaseDateFrom = dtpFrom.Value.Date;

            if (dtpTo.Checked)
                FilterParameters.PurchaseDateTo = dtpTo.Value.Date;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            txtInventoryNumber.Clear();
            txtName.Clear();
            txtSerialNumber.Clear();
            txtMOL.Clear();
            txtLocation.Clear();
            cmbAssetType.SelectedIndex = -1;
            cmbDepartment.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            dtpFrom.Checked = false;
            dtpTo.Checked = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}