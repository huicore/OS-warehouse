using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AssetManagementSystem.ViewModels
{
    public class AssetHistoryViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private readonly int _assetId;

        public List<AssetHistory> History { get; } = new List<AssetHistory>();

        public AssetHistoryViewModel(AppDbContext context, int assetId)
        {
            _context = context;
            _assetId = assetId;

            LoadHistory();
        }

        private async void LoadHistory()
        {
            var history = await _context.AssetHistories
                .Include(h => h.ChangedBy)
                .Where(h => h.AssetId == _assetId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            History.Clear();
            History.AddRange(history);
        }
    }
}