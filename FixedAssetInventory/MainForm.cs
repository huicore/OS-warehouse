public partial class MainForm : Form
{
    // Современный UI с панелью инструментов и темным/светлым режимом
    public MainForm()
    {
        InitializeComponent();
        ApplyModernStyling();
        InitializeComponents();
    }

    private void ApplyModernStyling()
    {
        // Современные стили для формы
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void InitializeComponents()
    {
        // Основной контейнер
        var mainContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            ColumnStyles = 
            {
                new ColumnStyle(SizeType.Absolute, 200),
                new ColumnStyle(SizeType.Percent, 100)
            }
        };

        // Левая панель с кнопками
        var leftPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(240, 240, 240)
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Padding = new Padding(10)
        };

        // Создание функциональных кнопок с иконками
        var buttons = new[]
        {
            CreateToolButton("Добавить ОС", "add", AddAsset_Click),
            CreateToolButton("Редактировать ОС", "edit", EditAsset_Click),
            CreateToolButton("Удалить ОС", "delete", DeleteAsset_Click),
            CreateToolButton("Фильтр", "filter", Filter_Click),
            CreateToolButton("Сбросить фильтр", "clear", ClearFilter_Click),
            CreateToolButton("Печать", "print", Print_Click),
            CreateToolButton("Экспорт в Excel", "export", Export_Click),
            CreateToolButton("Выход", "exit", Exit_Click)
        };

        foreach (var btn in buttons)
        {
            buttonPanel.Controls.Add(btn);
        }

        leftPanel.Controls.Add(buttonPanel);
        
        // Правая панель с таблицей
        var rightPanel = new Panel { Dock = DockStyle.Fill };
        
        // Современная таблица с улучшенным стилем
        dataGridView = new CustomDataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            EnableHeadersVisualStyles = false,
            GridColor = Color.FromArgb(240, 240, 240),
            MultiSelect = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        // Настройка столбцов
        dataGridView.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Инв. №", Name = "InventoryNumber", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Тип ОС", Name = "AssetType", Width = 120 },
            new DataGridViewTextBoxColumn { HeaderText = "Наименование", Name = "Name", Width = 250 },
            new DataGridViewTextBoxColumn { HeaderText = "Серийный №", Name = "SerialNumber", Width = 120 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "Status", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Подразделение", Name = "Department", Width = 150 },
            new DataGridViewTextBoxColumn { HeaderText = "МОЛ", Name = "MOL", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Местоположение", Name = "Location", Width = 150 },
            new DataGridViewTextBoxColumn { HeaderText = "Дата постановки", Name = "PurchaseDate", Width = 120 },
            new DataGridViewTextBoxColumn { HeaderText = "Примечания", Name = "Notes", Width = 200 }
        );

        // Стили для заголовков
        dataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft
        };

        // Стили для строк
        dataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(240, 240, 240)
        };

        rightPanel.Controls.Add(dataGridView);
        
        // Добавление панелей в основной контейнер
        mainContainer.Controls.Add(leftPanel, 0, 0);
        mainContainer.Controls.Add(rightPanel, 1, 0);
        
        this.Controls.Add(mainContainer);
        
        // Создание меню
        InitializeMenu();
    }

    private Button CreateToolButton(string text, string iconName, EventHandler clickHandler)
    {
        var btn = new Button
        {
            Text = " " + text,
            Image = Image.FromFile($"Resources/Icons/{iconName}.png"),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            Height = 40,
            Width = 180,
            Margin = new Padding(0, 0, 0, 5),
            Cursor = Cursors.Hand
        };

        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
        btn.Click += clickHandler;
        
        return btn;
    }

    private void InitializeMenu()
    {
        var menuStrip = new MenuStrip
        {
            BackColor = Color.White,
            Renderer = new ToolStripProfessionalRenderer(new CustomColorTable())
        };

        // Меню "Справочники"
        var referenceMenu = new ToolStripMenuItem("Справочники");
        referenceMenu.DropDownItems.Add("Управление типами ОС", null, ManageAssetTypes_Click);
        referenceMenu.DropDownItems.Add("Управление подразделениями", null, ManageDepartments_Click);
        
        // Меню "Настройки"
        var settingsMenu = new ToolStripMenuItem("Настройки");
        var themeSubMenu = new ToolStripMenuItem("Тема");
        themeSubMenu.DropDownItems.Add("Светлая", null, (s, e) => SetTheme("light"));
        themeSubMenu.DropDownItems.Add("Темная", null, (s, e) => SetTheme("dark"));
        settingsMenu.DropDownItems.Add(themeSubMenu);
        settingsMenu.DropDownItems.Add("Сменить базу данных...", null, ChangeDatabase_Click);
        
        menuStrip.Items.Add(referenceMenu);
        menuStrip.Items.Add(settingsMenu);
        
        this.Controls.Add(menuStrip);
        this.MainMenuStrip = menuStrip;
    }

    // ... остальные методы формы
}