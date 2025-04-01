public partial class AssetDialog : Form
{
    public AssetDialog()
    {
        InitializeComponent();
        ApplyModernStyling();
        InitializeComponents();
    }

    private void ApplyModernStyling()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.Padding = new Padding(20);
    }

    private void InitializeComponents()
    {
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 10,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 10)
        };

        // ���������� ����� �����
        AddFormField(mainLayout, "���. �:", new TextBox { Dock = DockStyle.Fill }, 0);
        AddFormField(mainLayout, "��� ��:", CreateAssetTypeComboBox(), 1);
        AddFormField(mainLayout, "������������:", new TextBox { Dock = DockStyle.Fill }, 2);
        AddFormField(mainLayout, "�������� �:", new TextBox { Dock = DockStyle.Fill }, 3);
        AddFormField(mainLayout, "������:", CreateStatusComboBox(), 4);
        AddFormField(mainLayout, "�������������:", CreateDepartmentComboBox(), 5);
        AddFormField(mainLayout, "���:", new TextBox { Dock = DockStyle.Fill }, 6);
        AddFormField(mainLayout, "��������������:", new TextBox { Dock = DockStyle.Fill }, 7);
        AddFormField(mainLayout, "���� ����������:", new TextBox { Dock = DockStyle.Fill }, 8);
        AddFormField(mainLayout, "����������:", new TextBox { Dock = DockStyle.Fill }, 9);

        // ������
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Padding = new Padding(0, 10, 0, 0)
        };

        var okButton = new Button 
        { 
            Text = "OK", 
            DialogResult = DialogResult.OK,
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width = 80
        };

        var cancelButton = new Button 
        { 
            Text = "������", 
            DialogResult = DialogResult.Cancel,
            BackColor = Color.FromArgb(240, 240, 240),
            FlatStyle = FlatStyle.Flat,
            Width = 80
        };

        buttonPanel.Controls.Add(okButton);
        buttonPanel.Controls.Add(cancelButton);

        this.Controls.Add(mainLayout);
        this.Controls.Add(buttonPanel);
    }

    private void AddFormField(TableLayoutPanel panel, string labelText, Control control, int row)
    {
        panel.Controls.Add(new Label 
        { 
            Text = labelText, 
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 10, 5)
        }, 0, row);

        panel.Controls.Add(control, 1, row);
    }

    private Control CreateAssetTypeComboBox()
    {
        var combo = new ComboBox { Dock = DockStyle.Fill };
        combo.Items.AddRange(new[] { "�������", "��������� ����", "�������", "�������", "���" });
        return combo;
    }

    // ... ����������� ������ ��� ������ �����������
}