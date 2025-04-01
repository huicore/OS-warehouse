using System.Collections.Generic;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Data
{
    public interface IAssetRepository
    {
        // Asset operations
        int AddAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(int id);
        Asset GetAsset(int id);
        Asset GetAssetByInventoryNumber(string inventoryNumber);
        List<Asset> GetAllAssets();
        List<Asset> GetFilteredAssets(FilterParameters filters);

        // Asset Type operations
        int AddAssetType(AssetType assetType);
        bool UpdateAssetType(AssetType assetType);
        bool DeleteAssetType(int id);
        AssetType GetAssetType(int id);
        List<AssetType> GetAllAssetTypes();

        // Department operations
        int AddDepartment(Department department);
        bool UpdateDepartment(Department department);
        bool DeleteDepartment(int id);
        Department GetDepartment(int id);
        List<Department> GetAllDepartments();

        // History operations
        void LogAssetChange(int assetId, string changedField, string oldValue, string newValue, string changedBy = null);
        List<AssetHistory> GetAssetHistory(int assetId);
    }
}