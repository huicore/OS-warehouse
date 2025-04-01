using System;
using System.Collections.Generic;
using System.Linq;
using FixedAssetInventory.Data;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repository;

        public AssetService(IAssetRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Asset GetAsset(int id) => _repository.GetAsset(id);

        public Asset GetAssetByInventoryNumber(string inventoryNumber) => 
            _repository.GetAssetByInventoryNumber(inventoryNumber);

        public IEnumerable<Asset> GetAllAssets() => _repository.GetAllAssets();

        public IEnumerable<Asset> SearchAssets(FilterParameters filters) => 
            _repository.GetFilteredAssets(filters);

        public int CreateAsset(Asset asset)
        {
            if (!ValidateAsset(asset, out _))
                throw new InvalidOperationException("Asset validation failed");

            return _repository.AddAsset(asset);
        }

        public bool UpdateAsset(Asset asset)
        {
            if (!ValidateAsset(asset, out _))
                return false;

            return _repository.UpdateAsset(asset);
        }

        public bool DeleteAsset(int id) => _repository.DeleteAsset(id);

        public IEnumerable<AssetType> GetAllAssetTypes() => _repository.GetAllAssetTypes();

        public AssetType GetAssetType(int id) => _repository.GetAssetType(id);

        public int CreateAssetType(AssetType assetType) => _repository.AddAssetType(assetType);

        public bool UpdateAssetType(AssetType assetType) => _repository.UpdateAssetType(assetType);

        public bool DeleteAssetType(int id) => _repository.DeleteAssetType(id);

        public IEnumerable<Department> GetAllDepartments() => _repository.GetAllDepartments();

        public Department GetDepartment(int id) => _repository.GetDepartment(id);

        public int CreateDepartment(Department department) => _repository.AddDepartment(department);

        public bool UpdateDepartment(Department department) => _repository.UpdateDepartment(department);

        public bool DeleteDepartment(int id) => _repository.DeleteDepartment(id);

        public IEnumerable<AssetHistory> GetAssetHistory(int assetId) => 
            _repository.GetAssetHistory(assetId);

        public void LogAssetChange(int assetId, string fieldName, string oldValue, string newValue, string changedBy = null) => 
            _repository.LogAssetChange(assetId, fieldName, oldValue, newValue, changedBy);

        public bool ValidateAsset(Asset asset, out IEnumerable<string> validationErrors)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(asset.InventoryNumber))
                errors.Add("Inventory number is required");

            if (string.IsNullOrWhiteSpace(asset.Name))
                errors.Add("Name is required");

            if (!IsInventoryNumberUnique(asset.InventoryNumber, asset.Id))
                errors.Add("Inventory number must be unique");

            validationErrors = errors;
            return !errors.Any();
        }

        public bool IsInventoryNumberUnique(string inventoryNumber, int? excludeAssetId = null)
        {
            var asset = _repository.GetAssetByInventoryNumber(inventoryNumber);
            return asset == null || asset.Id == excludeAssetId;
        }
    }
}