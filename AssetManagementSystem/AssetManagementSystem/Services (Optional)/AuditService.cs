using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using System;

namespace AssetManagementSystem.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public void LogAction(int userId, string action, string details)
        {
            try
            {
                var logEntry = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    Details = details,
                    Timestamp = DateTime.Now
                };

                _context.AuditLogs.Add(logEntry);
                _context.SaveChanges();
            }
            catch
            {
                // В реальном приложении следует добавить резервное логирование
            }
        }
    }
}