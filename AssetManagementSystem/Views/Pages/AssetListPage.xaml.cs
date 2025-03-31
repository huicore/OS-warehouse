using AssetManagementSystem.Data;
using AssetManagementSystem.ViewModels;
using System.Windows.Controls;

namespace AssetManagementSystem.Views.Pages
{
    public partial class AssetListPage : Page
    {
        public AssetListPage()
        {
            InitializeComponent();
            DataContext = new AssetListViewModel(
                ((App)Application.Current).Services.GetRequiredService<AppDbContext>());
        }
    }
}