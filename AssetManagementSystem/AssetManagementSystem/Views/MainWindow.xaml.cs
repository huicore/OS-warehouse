using AssetManagementSystem.Models;
using AssetManagementSystem.Views.Pages;
using AssetManagementSystem.Views.Windows;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Page _currentPage;

        public User CurrentUser => CurrentUser.Instance.User;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetField(ref _currentPage, value);
        }

        public List<MenuItem> MenuItems { get; }
        public ICommand LogoutCommand { get; }

        public MainWindowViewModel()
        {
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Title = "Основные средства", Icon = PackIconKind.Database, PageType = typeof(AssetListPage) },
                new MenuItem { Title = "Пользователи", Icon = PackIconKind.AccountGroup, PageType = typeof(UserManagementPage) }
            };

            LogoutCommand = new RelayCommand(_ => Logout());
            CurrentPage = new AssetListPage();
        }

        private void Logout()
        {
            CurrentUser.Instance.Clear();
            new LoginWindow().Show();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public PackIconKind Icon { get; set; }
        public System.Type PageType { get; set; }
    }
}