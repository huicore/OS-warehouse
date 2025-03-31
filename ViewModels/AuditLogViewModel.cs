using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AssetManagementSystem.ViewModels
{
    public class AuditLogViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;

        public List<AuditLog> LogEntries { get; } = new List<AuditLog>();

        public AuditLogViewModel(AppDbContext context)
        {
            _context = context;
            LoadLogEntries();
        }

        private async void LoadLogEntries()
        {
            var entries = await _context.AuditLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .Take(1000)
                .ToListAsync();

            LogEntries.Clear();
            LogEntries.AddRange(entries);
        }
    }
}