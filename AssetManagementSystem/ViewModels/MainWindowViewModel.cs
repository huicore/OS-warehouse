public MainWindowViewModel()
{
    MenuItems = new List<MenuItem>
    {
        new MenuItem { Title = "Основные средства", Icon = PackIconKind.Database, PageType = typeof(AssetListPage) },
        new MenuItem { Title = "Пользователи", Icon = PackIconKind.AccountGroup, PageType = typeof(UserManagementPage) }
    };

    if (CurrentUser.Instance.User?.Role?.CanViewAuditLogs == true)
    {
        MenuItems.Add(new MenuItem
        {
            Title = "Журнал аудита",
            Icon = PackIconKind.History,
            PageType = typeof(AuditLogPage)
        });
    }

    // ... остальная инициализация
}