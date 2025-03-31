using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AssetManagementSystem.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private User _selectedUser;

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
        public User SelectedUser
        {
            get => _selectedUser;
            set => SetField(ref _selectedUser, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }

        public UserManagementViewModel(AppDbContext context)
        {
            _context = context;
            AddCommand = new RelayCommand(_ => AddUser());
            EditCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);

            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private void AddUser()
        {
            // Реализация добавления пользователя
        }

        private void EditUser()
        {
            // Реализация редактирования пользователя
        }
    }
}