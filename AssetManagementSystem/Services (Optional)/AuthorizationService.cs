using AssetManagementSystem.Models;

namespace AssetManagementSystem.Services
{
    public class AuthorizationService
    {
        public bool CheckAccess(User user, string requiredPermission)
        {
            if (user == null || user.Role == null)
                return false;

            return requiredPermission switch
            {
                "CanManageAssets" => user.Role.CanManageAssets,
                "CanManageUsers" => user.Role.CanManageUsers,
                "CanViewReports" => user.Role.CanViewReports,
                "CanExportData" => user.Role.CanExportData,
                _ => false
            };
        }

        public void VerifyAccess(User user, string requiredPermission)
        {
            if (!CheckAccess(user, requiredPermission))
            {
                throw new UnauthorizedAccessException(
                    $"У пользователя {user.Username} нет прав для выполнения этой операции");
            }
        }
    }
}