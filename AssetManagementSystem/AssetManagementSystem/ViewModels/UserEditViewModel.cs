using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class UserEditViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private User _user;
        private string _password;

        public User User
        {
            get => _user;
            set => SetField(ref _user, value);
        }

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        public string Title => User.Id == 0 ? "Добавить пользователя" : "Редактировать пользователя";
        public string PasswordHint => User.Id == 0 ? "Пароль (обязательно)" : "Пароль (оставьте пустым чтобы не менять)";
        public List<Role> Roles { get; } = new List<Role>();

        public ICommand SaveCommand { get; }

        public UserEditViewModel(AppDbContext context, User user)
        {
            _context = context;
            User = user;

            LoadRoles();

            SaveCommand = new RelayCommand(_ => Save());
        }

        private async void LoadRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            Roles.Clear();
            Roles.AddRange(roles);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(User.Username) ||
                string.IsNullOrWhiteSpace(User.FullName) ||
                User.RoleId == 0 ||
                (User.Id == 0 && string.IsNullOrWhiteSpace(Password)))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (User.Id == 0)
            {
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                _context.Users.Add(User);
            }
            else if (!string.IsNullOrWhiteSpace(Password))
            {
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            }

            _context.SaveChanges();

            MessageBox.Show("Данные сохранены", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            CloseWindow(true);
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}