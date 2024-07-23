using CadDAO.Model;
using ConvertCustomizeException;
using DAO.Services;
using Microsoft.Data.Sqlite;
using StaticUtil.Enums;
using StaticUtil.Generic;
using StaticUtil.Models.DAO;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CadDAO.Util
{
    public class DataConvert
    {
        #region 建構式
        private readonly string _jsonPath;
        private readonly IDatabaseDAO _databaseDAO;

        /// <summary>
        /// 初始化 DataConvert 類別的新實例
        /// </summary>
        /// <param name="sourcePath">來源檔案路徑</param>
        /// <param name="targetPath">目標檔案路徑</param>
        /// <param name="databaseDAO">資料庫訪問對象</param>
        public DataConvert(string sourcePath, string targetPath, IDatabaseDAO databaseDAO)
        {
            try {
                (string jsonPath, string sqlitePath) = DetermineFilePaths(sourcePath, targetPath);
                _jsonPath = jsonPath;
                _databaseDAO = databaseDAO;
            }
            catch (Exception ex) {
                throw new Exception($"Initializer DataConvert failed.Error:{ex.Message}", ex);
            }
        }
        #endregion 建構式
        /// <summary>
        /// 將 JSON 檔案轉換為 SQLite 資料庫
        /// </summary>
        public async Task JsonToSQLiteAsync()
        {
            try {
                var jsonData = JsonReader();
                foreach (var table in jsonData.Tables) {
                    await CreateAndPopulateTableAsync(table);
                }
            }
            catch (Exception ex) {
                throw new Exception($"Json to sqlite failed.Error:{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 將 SQLite 資料庫轉換為 JSON 檔案
        /// </summary>
        /// <param name="outputPath">輸出 JSON 檔案的路徑</param>
        public async Task SQLiteToJsonAsync(string outputPath)
        {
            try {
                var databaseModel = await ExtractDataFromSqliteAsync();
                await WriteJsonToFileAsync(databaseModel, outputPath);
            }
            catch (Exception ex) {
                throw new Exception($"SQLite to JSON failed. Error：{ex.Message}", ex);
            }
        }

        #region 私有方法

        /// <summary>
        /// 依據路徑副檔名確定 JSON 和 SQLite 檔案的路徑
        /// </summary>
        /// <param name="sourcePath">來源檔案路徑</param>
        /// <param name="targetPath">目標檔案路徑</param>
        /// <returns>Json檔案路徑和SQLite資料庫路徑</returns>
        private static (string jsonPath, string sqlitePath) DetermineFilePaths(string sourcePath, string targetPath)
        {
            string sourceExtension = Path.GetExtension(sourcePath).ToLower();
            string targetExtension = Path.GetExtension(targetPath).ToLower();

            if (sourceExtension == ".json" && targetExtension == ".db") {
                return (sourcePath, targetPath);
            }
            else if (sourceExtension == ".db" && targetExtension == ".json") {
                return (targetPath, sourcePath);
            }
            else {
                throw new NotSupportedException("Unsupported file extension.");
            }
        }
        /// <summary>
        /// 讀取 JSON 檔案並反序欄位化
        /// </summary>
        /// <returns>回傳序欄位化好的JSON模型</returns>
        private DatabaseModel JsonReader()
        {
            if (!File.Exists(_jsonPath)) throw new NullReferenceException("Json file does not exist");
            string jsonContent = File.ReadAllText(_jsonPath);
            DatabaseModel? databaseModel = JsonSerializer.Deserialize<DatabaseModel>(jsonContent);
            return databaseModel ?? throw new FormatException("Json content convert to database model failed");
        }

        /// <summary>
        /// 創建並填充資料表
        /// </summary>
        /// <param name="table">包含資料的資料表模型</param>
        private async Task CreateAndPopulateTableAsync(TableModel table)
        {
            var schema = CreateTableSchema(table.TablesName);
            await EnsureTableExistsAsync(schema);
            var dataTable = CreateDataTable(schema, table);
            await InsertOrUpdateDataAsync(dataTable);
        }

        /// <summary>
        /// 創建資料表結構
        /// </summary>
        /// <param name="tableName">資料表名稱</param>
        private static TableSchemaModel CreateTableSchema(string tableName)
        {
            return new TableSchemaModel {
                TableName = tableName,
                SchemaColumns = CreateDefaultSchemaColumns(),
                CompositeUnique = new Dictionary<string, List<string>>
                {
                    { tableName, new List<string> { "Key", "GroupName" } }
                }
            };
        }

        /// <summary>
        /// 確保資料表存在
        /// </summary>
        /// <param name="schema">資料表結構</param>
        private async Task EnsureTableExistsAsync(TableSchemaModel schema)
        {
            var checkResult = await _databaseDAO.CheckDataTableExistsAsync(schema);
            if (!checkResult.IsOperationSuccessful)
                throw new Exception($"Create dataTable or check dataTable exists failed.Error:{checkResult.ResultString}");
        }

        /// <summary>
        /// 創建資料表並填充資料
        /// </summary>
        /// <param name="schema">資料表結構</param>
        /// <param name="table">資料表內容</param>
        private static DataTable CreateDataTable(TableSchemaModel schema, TableModel table)
        {
            DataTable dataTable = new() { TableName = table.TablesName };
            foreach (var column in schema.SchemaColumns) {
                dataTable.Columns.Add(column.ColumnName, typeof(string));
            }

            foreach (var column in table.Columns) {
                ValidateColumnData(column, table.TablesName);
                DataRow newRow = dataTable.NewRow();
                newRow["Key"] = column.Key;
                newRow["Value"] = column.Value;
                newRow["DataType"] = column.DataType;
                newRow["GroupName"] = column.GroupName;
                newRow["Remark"] = column.Remark;
                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        /// <summary>
        /// 驗證欄位資料
        /// </summary>
        /// <param name="column">內含資料的欄位模型</param>
        /// <param name="tableName">資料表名稱</param>
        private static void ValidateColumnData(ColumnModel column, string tableName)
        {
            if (string.IsNullOrEmpty(column.Key))
                throw new CellBlankErrorException($"Table Name:{tableName},Column:Key");
            if (string.IsNullOrEmpty(column.Value))
                throw new CellBlankErrorException($"Table Name:{tableName},Column:Value");
            if (string.IsNullOrEmpty(column.DataType))
                throw new CellBlankErrorException($"Table Name:{tableName},Column:DataType");
        }

        /// <summary>
        /// 插入或更新資料
        /// </summary>
        /// <param name="data">包含完整資料的原始資料表</param>
        private async Task InsertOrUpdateDataAsync(DataTable data)
        {
            SqlQueryModel sqlQuery = new()
            {
                SqlQueryText = GenerateSQL(data)
            };
            var result = await _databaseDAO.OperationNonQueryTransactionAsync(sqlQuery);
            if (!result.IsOperationSuccessful)
                throw new Exception($"Insert or update failed.Error:{result.ResultString}");
        }

        /// <summary>
        /// 創建預設欄位結構
        /// </summary>
        ///<returns>回傳預設的欄位結構</returns>
        private static List<SchemaColumnModel> CreateDefaultSchemaColumns()
        {
            return new List<SchemaColumnModel>
            {
                new SchemaColumnModel { ColumnName = "Key", AllowNulls = false },
                new SchemaColumnModel { ColumnName = "DataType", AllowNulls = false },
                new SchemaColumnModel { ColumnName = "Value", AllowNulls = false },
                new SchemaColumnModel { ColumnName = "GroupName" },
                new SchemaColumnModel { ColumnName = "Remark" }
            };
        }

        /// <summary>
        /// 生成插入或更新的 SQL 語句
        /// </summary>
        /// <param name="data">包含完整資料的原始資料表</param>
        private static string GenerateSQL(DataTable data)
        {
            StringBuilder sql = new();
            foreach (DataRow row in data.Rows) {
                string columns = string.Join(", ", data.Columns.Cast<DataColumn>().Select(column => column.ColumnName));
                string values = string.Join(", ", data.Columns.Cast<DataColumn>().Select(column => $"'{row[column.ColumnName]}'"));

                sql.Append($"INSERT INTO {data.TableName} ({columns}) VALUES ({values}) ");
                sql.Append("ON CONFLICT(Key, GroupName) DO UPDATE SET ");
                // 生成更新SQL語句欄位賦值的部分
                var updateColumnAssignments = data.Columns.Cast<DataColumn>()
                    .Select(column => $"{column.ColumnName} = excluded.{column.ColumnName}");
                sql.Append(string.Join(", ", updateColumnAssignments));
                sql.Append(';');
            }
            return sql.ToString();
        }

        /// <summary>
        /// 從 SQLite 中提取資料
        /// </summary>
        /// <returns></returns>
        private async Task<DatabaseModel> ExtractDataFromSqliteAsync()
        {
            var databaseModel = new DatabaseModel();
            var tableNames = await GetTableNamesAsync();

            foreach (DataRow tableNameRow in tableNames.Rows) {
                var tableName = tableNameRow[0].ToString() ?? throw new Exception("Table Name is empty");
                var tableModel = new TableModel { TablesName = tableName };
                var tableData = await GetTableDataAsync(tableName);

                foreach (DataRow row in tableData.Rows) {
                    tableModel.Columns.Add(CreateColumnModel(row));
                }

                databaseModel.Tables.Add(tableModel);
            }

            return databaseModel;
        }

        /// <summary>
        /// 獲取所有資料表名稱
        /// </summary>
        private async Task<DataTable> GetTableNamesAsync()
        {
            SqlQueryModel sqlQuery = new()
            {
                SqlQueryText = "SELECT name FROM sqlite_master WHERE type='table' and name <> 'sqlite_sequence';"
            };
            var getTableNames = await _databaseDAO.OperationReaderAsync(sqlQuery);
            if (!getTableNames.IsOperationSuccessful || getTableNames.Result.Tables.Count <= 0) {
                string errorMessage = $"Get table name failed," +
                    $"Table count：{getTableNames.Result.Tables.Count}." +
                    $"Error：{getTableNames.ResultString}";
                throw new Exception(errorMessage);
            }
            return getTableNames.Result.Tables[0];
        }

        /// <summary>
        /// 獲取指定資料表的資料
        /// </summary>
        private async Task<DataTable> GetTableDataAsync(string tableName)
        {
            SqlQueryModel sqlQuery = new()
            {
                SqlQueryText = $"SELECT * from {tableName}"
            };
            var dataTable = await _databaseDAO.OperationReaderAsync(sqlQuery);
            if (!dataTable.IsOperationSuccessful || dataTable.Result.Tables.Count <= 0)
                throw new Exception($"Get table data failed.Error：{dataTable.ResultString}");

            return dataTable.Result.Tables[0];
        }

        /// <summary>
        /// 創建欄位模型
        /// </summary>
        private static ColumnModel CreateColumnModel(DataRow row)
        {
            return new ColumnModel {
                Key = row["Key"].ToString() ?? throw new Exception("Key 為空"),
                Value = row["Value"].ToString() ?? throw new Exception("Value 為空"),
                DataType = row["DataType"].ToString() ?? throw new Exception("DataType 為空"),
                GroupName = row["GroupName"].ToString(),
                Remark = row["Remark"].ToString()
            };
        }

        /// <summary>
        /// 將資料模型寫入 JSON 檔案
        /// </summary>
        private static async Task WriteJsonToFileAsync(DatabaseModel databaseModel, string outputPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var jsonString = JsonSerializer.Serialize(databaseModel, options);
            await File.WriteAllTextAsync(outputPath, jsonString);
        }

        #endregion 私有變數
    }
}