using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using AssetManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManagementSystem.Services
{
    public class AssetService
    {
        private readonly AppDbContext _context;

        public AssetService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Asset> GetFilteredAssets(FilterCriteria criteria)
        {
            var query = _context.Assets
                .Include(a => a.AssetType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(criteria.InventoryNumber))
                query = query.Where(a => a.InventoryNumber.Contains(criteria.InventoryNumber));

            if (criteria.AssetTypeId.HasValue)
                query = query.Where(a => a.AssetTypeId == criteria.AssetTypeId.Value);

            if (!string.IsNullOrEmpty(criteria.Name))
                query = query.Where(a => a.Name.Contains(criteria.Name));

            if (!string.IsNullOrEmpty(criteria.Status))
                query = query.Where(a => a.Status == criteria.Status);

            if (criteria.PurchaseDateFrom.HasValue)
                query = query.Where(a => a.CreatedAt >= criteria.PurchaseDateFrom.Value);

            if (criteria.PurchaseDateTo.HasValue)
                query = query.Where(a => a.CreatedAt <= criteria.PurchaseDateTo.Value);

            return query.ToList();
        }
    }
}