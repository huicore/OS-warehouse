using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using AssetManagementSystem.Views.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set => SetField(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AppDbContext context)
        {
            _context = context;
            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentUser.Instance.SetUser(user);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}