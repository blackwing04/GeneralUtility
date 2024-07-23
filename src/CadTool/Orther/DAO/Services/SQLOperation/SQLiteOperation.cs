using Microsoft.Data.Sqlite;
using StaticUtil.Generic;
using StaticUtil.Models.DAO;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DAO.Services.SQLOperation
{
    public class SQLiteOperation : IDbOperation
    {
        #region 建構式
        private readonly SqliteConnection _connection;

        public SQLiteOperation(DbConnection connection)
        {
            string exMessage = $"{ResultString.ConnectionTransformFailedOrEmpty}SqliteConnection";
            _connection = connection as SqliteConnection ?? throw new ArgumentException(exMessage);
            //啟用SQLite
            SQLitePCL.Batteries.Init();
        }
        #endregion 建構式
        public async Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string dbPath)
        {
            DbQueryResultModel<bool> databaseResult = new(){
                Result = false
            };
            var connectionString = 
            new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();

            using var connection = new SqliteConnection(connectionString);
            try {
                await connection.OpenAsync();
                if (File.Exists(dbPath))
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.CreateDatabaseSuccessfully, true);
                else
                    throw new InvalidOperationException(ResultString.ExecutedButDatabaseNotCreated);
            }
            catch (Exception ex) {
                string message = $"{ResultString.CreateDatabaseFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            finally {
                await connection.CloseAsync();
            }
            return databaseResult;
        }
        public async Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema)
        {
            NonQueryResultModel databaseResult = new();
            if (tableSchema == null) {
                ResultUtil.HandleFailedResult(databaseResult, ResultString.TableSchemaIsEmpty, null);
                return databaseResult;
            }
            try {
                if (await TableExists(tableSchema.TableName))
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.DataTableIsExists);
                else {
                    await CreateTable(tableSchema);
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.DataTableCreateSuccessfully);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.CheckDataTableFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<DbQueryResultModel<int>> OperationNonQueryTransactionAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();
            using var transaction = await _connection.BeginTransactionAsync() as SqliteTransaction;
            if (transaction is null) {
                string message = $"{ResultString.TransactionTransformFailedOrEmpty}SqlTransaction";
                ResultUtil.HandleFailedResult(databaseResult, message, null);
                return databaseResult;
            }
            try {
                using SqliteCommand command = _connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = dbModel.SqlQuery.SqlQueryText;
                foreach (var param in dbModel.SqlQuery.Parameter) {
                    string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                    command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                }
                int rowsAffected = await command.ExecuteNonQueryAsync();
                // 嘗試提交事務
                await command.Transaction.CommitAsync();
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.TransactionSuccessfully, rowsAffected);
            }
            catch (Exception ex) {
                // 如果有錯誤發生，Rollback事務
                await transaction.RollbackAsync();
                string message = $"{ResultString.TransactionFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<DbQueryResultModel<T>> OperationScalarAsync<T>(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<T> databaseResult = new();
            try {
                using SqliteCommand command = _connection.CreateCommand();
                command.CommandText = dbModel.SqlQuery.SqlQueryText;
                foreach (var param in dbModel.SqlQuery.Parameter) {
                    string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                    command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                }
                var result = await command.ExecuteScalarAsync();
                //設定模型狀態
                if (result is null)
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfullyButEmpty);
                else if (result is T typedResult)
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfully, typedResult);
                else
                    ResultUtil.HandleFailedResult(databaseResult, ResultString.ResultConvertToGenericFailed);
            }
            catch (Exception ex) {
                string message = $"{ResultString.QueryFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<DbQueryResultModel<DataSet>> OperationReaderAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<DataSet> databaseResult = new();
            try {
                using SqliteCommand command = new(dbModel.SqlQuery.SqlQueryText, _connection);
                foreach (var param in dbModel.SqlQuery.Parameter) {
                    string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                    command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                }
                using SqliteDataReader reader = await command.ExecuteReaderAsync();
                DataSet dataSet = new();
                dataSet.Load(reader, LoadOption.PreserveChanges, "Result");
                //設定模型狀態
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfully, dataSet);
                else
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfullyButEmpty, dataSet);
            }
            catch (Exception ex) {
                string message = $"{ResultString.QueryFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();

            using var transaction = await _connection.BeginTransactionAsync() as SqliteTransaction;
            if (transaction is null) {
                string message = $"{ResultString.TransactionTransformFailedOrEmpty}SqliteTransaction";
                ResultUtil.HandleFailedResult(databaseResult, message, null);
                return databaseResult;
            }
            try {
                using var command = _connection.CreateCommand();
                command.Transaction = transaction;
                var columns = string.Join(", ", dbModel.BulkInsertTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                var parameters = string.Join(", ", dbModel.BulkInsertTable.Columns.Cast<DataColumn>().Select((c, index) => $"@param{index}"));
                command.CommandText = $"INSERT INTO {dbModel.BulkInsertTable.TableName} ({columns}) VALUES ({parameters})";
                int rowsAffected =0;
                foreach (DataRow row in dbModel.BulkInsertTable.Rows) {
                    command.Parameters.Clear();
                    for (int i = 0; i < dbModel.BulkInsertTable.Columns.Count; i++) {
                        command.Parameters.AddWithValue($"@param{i}", row[i] ?? DBNull.Value);
                    }
                    rowsAffected += await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.BulkInsertSuccessfully, rowsAffected);
            }
            catch (Exception ex) {
                // 如果有錯誤發生，Rollback事務
                transaction.Rollback();
                string message = $"{ResultString.BulkInsertFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new()
            {
                IsOperationSuccessful = false,
                ResultString = $"SQLite {ResultString.UnsupportedStoredProced}",
                Result=0
            };
            return await Task.FromResult(databaseResult);
        }
        #region 私有方法
        /// <summary>
        /// 檢查該資料表是否存在
        /// </summary>
        /// <param name="tableName">資料表名</param>
        /// <returns></returns>
        private async Task<bool> TableExists(string tableName)
        {
            string checkTableQuery = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            using var command = _connection.CreateCommand();
            command.CommandText = checkTableQuery;
            int count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        /// <summary>
        /// 創建資料庫
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        private async Task CreateTable(TableSchemaModel tableSchema)
        {
            StringBuilder createTableSql = new ($"CREATE TABLE {tableSchema.TableName} (");
            List<string> columnDefinitions = new ()
            {
                $"{tableSchema.TableName}Id INTEGER PRIMARY KEY AUTOINCREMENT"
            };

            foreach (var column in tableSchema.SchemaColumns) {
                var columnDef = $"\"{column.ColumnName}\" {GetSqlDbType(column.DataType)}";
                // 檢查是否允許空值
                if (!column.AllowNulls)
                    columnDef += " NOT NULL";
                // 檢查是否唯一
                if (column.Unique)
                    columnDef += " UNIQUE";
                // 檢查是否有默認值並加入 DEFAULT 子句
                if (column.DefaultValue != null) {
                    // 根據數據類型格式化默認值
                    var formattedDefaultValue = FormatDefaultValue(column.DataType, column.DefaultValue);
                    if (formattedDefaultValue != null)
                        columnDef += $" DEFAULT {formattedDefaultValue}";
                }
                columnDefinitions.Add(columnDef);
            }

            createTableSql.Append(string.Join(", ", columnDefinitions));
            // 檢查並添加複合唯一約束
            if (tableSchema.CompositeUnique != null && tableSchema.CompositeUnique.Count > 0) {
                foreach (var compositeUnique in tableSchema.CompositeUnique) {
                    string constraintName = compositeUnique.Key;
                    List<string> columns = compositeUnique.Value;
                    createTableSql.Append($", CONSTRAINT {constraintName} UNIQUE ({string.Join(", ", columns)})");
                }
            }
            createTableSql.Append(");");

            using var command = _connection.CreateCommand();
            command.CommandText = createTableSql.ToString();
            await command.ExecuteNonQueryAsync();
        }

        private static string GetSqlDbType(Type type)
        {
            string strType = "TEXT";
            if (type == typeof(int))
                strType = "INTEGER";
            else if (type == typeof(decimal))
                strType = "REAL";
            else if (type == typeof(bool))
                strType = "BOOLEAN";
            else if (type == typeof(DateTime))
                strType = "DATETIME";
            return strType;
        }
        private static string? FormatDefaultValue(Type dataType, object defaultValue)
        {
            // 檢查數據類型並格式化默認值
            if (dataType == typeof(string) || dataType == typeof(char))
                // 字符串和字符類型需要用單引號括起來
                return $"'{defaultValue}'";
            else if (dataType == typeof(DateTime))
                // DateTime 類型，格式化為 SQLite 可接受的日期字符串
                return $"'{(DateTime)defaultValue:yyyy-MM-dd HH:mm:ss}'";
            else if (dataType == typeof(bool))
                // 布爾類型，在 SQLite 中用 1 和 0 表示
                return (bool)defaultValue ? "1" : "0";
            else
                // 其他所有類型，默認情況下直接轉換為字符串
                return defaultValue.ToString();
        }
        #endregion 私有方法
    }
}
