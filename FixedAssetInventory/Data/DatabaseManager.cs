using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using FixedAssetInventory.Models;

namespace FixedAssetInventory.Data
{
    public class DatabaseManager : IDisposable
    {
        private SQLiteConnection _connection;
        private readonly string _dbPath;

        public DatabaseManager(string dbPath)
        {
            _dbPath = dbPath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(_dbPath);

            _connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            _connection.Open();

            if (isNewDatabase)
            {
                CreateTables();
                SeedInitialData();
            }
        }

        private void CreateTables()
        {
            using (var cmd = new SQLiteCommand(_connection))
            {
                cmd.CommandText = @"
                CREATE TABLE AssetTypes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT
                );

                CREATE TABLE Departments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT
                );

                CREATE TABLE Assets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    InventoryNumber TEXT NOT NULL UNIQUE,
                    AssetTypeId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    SerialNumber TEXT,
                    Status TEXT NOT NULL,
                    DepartmentId INTEGER NOT NULL,
                    MOL TEXT,
                    Location TEXT,
                    PurchaseDate TEXT,
                    Notes TEXT,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (AssetTypeId) REFERENCES AssetTypes(Id),
                    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
                );

                CREATE TABLE AssetHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AssetId INTEGER NOT NULL,
                    ChangedField TEXT NOT NULL,
                    OldValue TEXT,
                    NewValue TEXT,
                    ChangedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    ChangedBy TEXT,
                    FOREIGN KEY (AssetId) REFERENCES Assets(Id) ON DELETE CASCADE
                );

                CREATE INDEX idx_asset_history_asset_id ON AssetHistory(AssetId);
                CREATE INDEX idx_assets_inventory_number ON Assets(InventoryNumber);
                CREATE INDEX idx_assets_asset_type ON Assets(AssetTypeId);
                CREATE INDEX idx_assets_department ON Assets(DepartmentId);";

                cmd.ExecuteNonQuery();
            }
        }

        private void SeedInitialData()
        {
            // Insert default asset types
            ExecuteNonQuery("INSERT INTO AssetTypes (Name) VALUES ('Компьютер'), ('Принтер'), ('Сервер'), ('Сетевое оборудование'), ('Мебель')");

            // Insert default departments
            ExecuteNonQuery("INSERT INTO Departments (Name) VALUES ('IT отдел'), ('Бухгалтерия'), ('Отдел кадров'), ('Производство'), ('Логистика')");
        }

        public List<Asset> GetAssets(FilterParameters filters = null)
        {
            var assets = new List<Asset>();
            string query = @"
                SELECT a.*, at.Name AS AssetType, d.Name AS Department 
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

                // Add other filter conditions similarly...
            }

            if (conditions.Count > 0)
            {
                query += " WHERE " + string.Join(" AND ", conditions);
            }

            query += " ORDER BY a.InventoryNumber";

            using (var cmd = new SQLiteCommand(query, _connection))
            {
                cmd.Parameters.AddRange(parameters.ToArray());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assets.Add(new Asset
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
                        });
                    }
                }
            }

            return assets;
        }

        public int AddAsset(Asset asset)
        {
            string query = @"
                INSERT INTO Assets (
                    InventoryNumber, AssetTypeId, Name, SerialNumber, Status,
                    DepartmentId, MOL, Location, PurchaseDate, Notes
                ) VALUES (
                    @InventoryNumber, @AssetTypeId, @Name, @SerialNumber, @Status,
                    @DepartmentId, @MOL, @Location, @PurchaseDate, @Notes
                );
                SELECT last_insert_rowid();";

            using (var cmd = new SQLiteCommand(query, _connection))
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

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void LogAssetChange(int assetId, string changedField, string oldValue, string newValue, string changedBy = null)
        {
            string query = @"
                INSERT INTO AssetHistory (
                    AssetId, ChangedField, OldValue, NewValue, ChangedBy
                ) VALUES (
                    @AssetId, @ChangedField, @OldValue, @NewValue, @ChangedBy
                )";

            ExecuteNonQuery(query, 
                new SQLiteParameter("@AssetId", assetId),
                new SQLiteParameter("@ChangedField", changedField),
                new SQLiteParameter("@OldValue", oldValue),
                new SQLiteParameter("@NewValue", newValue),
                new SQLiteParameter("@ChangedBy", changedBy));
        }

        private void ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var cmd = new SQLiteCommand(query, _connection))
            {
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}