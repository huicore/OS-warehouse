using System.Collections.Generic;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Services
{
    public interface IAssetService
    {
        // Asset operations
        Asset GetAsset(int id);
        Asset GetAssetByInventoryNumber(string inventoryNumber);
        IEnumerable<Asset> GetAllAssets();
        IEnumerable<Asset> SearchAssets(FilterParameters filters);
        int CreateAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(int id);

        // Asset Type operations
        IEnumerable<AssetType> GetAllAssetTypes();
        AssetType GetAssetType(int id);
        int CreateAssetType(AssetType assetType);
        bool UpdateAssetType(AssetType assetType);
        bool DeleteAssetType(int id);

        // Department operations
        IEnumerable<Department> GetAllDepartments();
        Department GetDepartment(int id);
        int CreateDepartment(Department department);
        bool UpdateDepartment(Department department);
        bool DeleteDepartment(int id);

        // History operations
        IEnumerable<AssetHistory> GetAssetHistory(int assetId);
        void LogAssetChange(int assetId, string fieldName, string oldValue, string newValue, string changedBy = null);

        // Validation
        bool ValidateAsset(Asset asset, out IEnumerable<string> validationErrors);
        bool IsInventoryNumberUnique(string inventoryNumber, int? excludeAssetId = null);
    }
}