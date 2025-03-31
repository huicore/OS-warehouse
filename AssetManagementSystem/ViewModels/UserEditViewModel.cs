using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private string _validationError;

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
        public string PasswordHint => User.Id == 0 ? "Обязательное поле" : "Оставьте пустым, чтобы не менять";

        public List<Role> Roles { get; } = new List<Role>();
        public string ValidationError
        {
            get => _validationError;
            set => SetField(ref _validationError, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserEditViewModel(AppDbContext context, User user)
        {
            _context = context;
            User = user ?? throw new ArgumentNullException(nameof(user));

            LoadRoles();
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private async void LoadRoles()
        {
            try
            {
                var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
                Roles.Clear();
                Roles.AddRange(roles);
                OnPropertyChanged(nameof(Roles));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(User.Username))
                errors.Add("Логин обязателен");
            else if (User.Username.Length < 4)
                errors.Add("Логин должен быть не менее 4 символов");

            if (User.Id == 0 && string.IsNullOrWhiteSpace(Password))
                errors.Add("Пароль обязателен для нового пользователя");
            else if (!string.IsNullOrWhiteSpace(Password) && Password.Length < 6)
                errors.Add("Пароль должен быть не менее 6 символов");

            if (string.IsNullOrWhiteSpace(User.FullName))
                errors.Add("ФИО обязательно");

            if (!new EmailAddressAttribute().IsValid(User.Email))
                errors.Add("Некорректный email");

            if (User.RoleId == 0)
                errors.Add("Выберите роль");

            // Проверка уникальности логина и email
            if (_context.Users.Any(u => u.Id != User.Id && u.Username == User.Username))
                errors.Add("Логин уже занят");

            if (_context.Users.Any(u => u.Id != User.Id && u.Email == User.Email))
                errors.Add("Email уже используется");

            ValidationError = errors.Any() ? string.Join("\n", errors) : null;
            return !errors.Any();
        }

        private void Save()
        {
            if (!Validate())
                return;

            try
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                }
                else if (User.Id == 0)
                {
                    throw new InvalidOperationException("Пароль не может быть пустым для нового пользователя");
                }

                if (User.Id == 0)
                {
                    User.CreatedAt = DateTime.Now;
                    _context.Users.Add(User);
                }
                else
                {
                    _context.Users.Update(User);
                }

                _context.SaveChanges();
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            if (User.Id == 0 || MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CloseWindow(false);
            }
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