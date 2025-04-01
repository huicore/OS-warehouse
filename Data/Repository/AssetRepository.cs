using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Data
{
    public class AssetRepository : IAssetRepository, IDisposable
    {
        private readonly SQLiteConnection _connection;

        public AssetRepository(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

        public int AddAsset(Asset asset)
        {
            const string sql = @"INSERT INTO Assets (
                InventoryNumber, AssetTypeId, Name, SerialNumber, Status,
                DepartmentId, MOL, Location, PurchaseDate, Notes
            ) VALUES (
                @InventoryNumber, @AssetTypeId, @Name, @SerialNumber, @Status,
                @DepartmentId, @MOL, @Location, @PurchaseDate, @Notes
            );
            SELECT last_insert_rowid();";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                AddAssetParameters(cmd, asset);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool UpdateAsset(Asset asset)
        {
            const string sql = @"UPDATE Assets SET
                InventoryNumber = @InventoryNumber,
                AssetTypeId = @AssetTypeId,
                Name = @Name,
                SerialNumber = @SerialNumber,
                Status = @Status,
                DepartmentId = @DepartmentId,
                MOL = @MOL,
                Location = @Location,
                PurchaseDate = @PurchaseDate,
                Notes = @Notes,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = @Id";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                AddAssetParameters(cmd, asset);
                cmd.Parameters.AddWithValue("@Id", asset.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteAsset(int id)
        {
            const string sql = "DELETE FROM Assets WHERE Id = @Id";
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public Asset GetAsset(int id)
        {
            const string sql = @"SELECT a.*, at.Name AS AssetType, d.Name AS Department 
                              FROM Assets a
                              LEFT JOIN AssetTypes at ON a.AssetTypeId = at.Id
                              LEFT JOIN Departments d ON a.DepartmentId = d.Id
                              WHERE a.Id = @Id";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? MapAsset(reader) : null;
                }
            }
        }

        public Asset GetAssetByInventoryNumber(string inventoryNumber)
        {
            const string sql = @"SELECT a.*, at.Name AS AssetType, d.Name AS Department 
                              FROM Assets a
                              LEFT JOIN AssetTypes at ON a.AssetTypeId = at.Id
                              LEFT JOIN Departments d ON a.DepartmentId = d.Id
                              WHERE a.InventoryNumber = @InventoryNumber";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@InventoryNumber", inventoryNumber);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? MapAsset(reader) : null;
                }
            }
        }

        public List<Asset> GetAllAssets()
        {
            return GetFilteredAssets(null);
        }

        public List<Asset> GetFilteredAssets(FilterParameters filters)
        {
            var assets = new List<Asset>();
            var sql = @"SELECT a.*, at.Name AS AssetType, d.Name AS Department 
                      FROM Assets a
                      LEFT JOIN AssetTypes at ON a.AssetTypeId = at.Id
                      LEFT JOIN Departments d ON a.DepartmentId = d.Id";

            var conditions = new List<string>();
            var parameters = new List<SQLiteParameter>();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.InventoryNumber))
                {
                    conditions.Add("a.InventoryNumber LIKE @InventoryNumber");
                    parameters.Add(new SQLiteParameter("@InventoryNumber", $"%{filters.InventoryNumber}%"));
                }

                // Add other filter conditions...
            }

            if (conditions.Any())
            {
                sql += " WHERE " + string.Join(" AND ", conditions);
            }

            sql += " ORDER BY a.InventoryNumber";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddRange(parameters.ToArray());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assets.Add(MapAsset(reader));
                    }
                }
            }

            return assets;
        }

        // Implementations for AssetType, Department and History operations...

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private static void AddAssetParameters(SQLiteCommand cmd, Asset asset)
        {
            cmd.Parameters.AddWithValue("@InventoryNumber", asset.InventoryNumber);
            cmd.Parameters.AddWithValue("@AssetTypeId", asset.AssetTypeId);
            cmd.Parameters.AddWithValue("@Name", asset.Name);
            cmd.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
            cmd.Parameters.AddWithValue("@Status", asset.Status);
            cmd.Parameters.AddWithValue("@DepartmentId", asset.DepartmentId);
            cmd.Parameters.AddWithValue("@MOL", asset.MOL);
            cmd.Parameters.AddWithValue("@Location", asset.Location);
            cmd.Parameters.AddWithValue("@PurchaseDate", asset.PurchaseDate?.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Notes", asset.Notes);
        }

        private static Asset MapAsset(IDataReader reader)
        {
            return new Asset
            {
                Id = Convert.ToInt32(reader["Id"]),
                InventoryNumber = reader["InventoryNumber"].ToString(),
                AssetTypeId = Convert.ToInt32(reader["AssetTypeId"]),
                Name = reader["Name"].ToString(),
                SerialNumber = reader["SerialNumber"].ToString(),
                Status = reader["Status"].ToString(),
                DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                MOL = reader["MOL"].ToString(),
                Location = reader["Location"].ToString(),
                PurchaseDate = reader["PurchaseDate"] != DBNull.Value ? 
                    DateTime.Parse(reader["PurchaseDate"].ToString()) : (DateTime?)null,
                Notes = reader["Notes"].ToString(),
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString())
            };
        }
    }
}