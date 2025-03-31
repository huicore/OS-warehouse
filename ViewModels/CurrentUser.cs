using AssetManagementSystem.Models;

namespace AssetManagementSystem
{
    public sealed class CurrentUser
    {
        private static CurrentUser _instance;

        public User User { get; private set; }

        private CurrentUser() { }

        public static CurrentUser Instance => _instance ??= new CurrentUser();

        public void SetUser(User user)
        {
            User = user;
        }

        public void Clear()
        {
            User = null;
        }
    }
}