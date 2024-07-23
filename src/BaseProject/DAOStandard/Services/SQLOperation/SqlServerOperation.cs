using DAOStandard.StaticUtil;
using Generic.StaticUtil;
using DAOStandard.StaticUtil.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Generic.StaticUtil.Models.DataModel;

namespace DAOStandard.Services.SQLOperation
{
    public class SqlServerOperation : IDbOperation
    {
        #region 建構式
        /// <summary>
        /// 資料庫連線物件
        /// </summary>
        private readonly SqlConnection _connection;

        public SqlServerOperation(DbConnection connection)
        {
            _connection = connection as SqlConnection
                     ?? throw new ArgumentException($"{ResultString.ConnectionTransformFailedOrEmpty}SqlConnection");
        }
        #endregion 建構式
        public async Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseName)
        {
            DbQueryResultModel<bool> databaseResult = new DbQueryResultModel<bool> (){
                Result = false
            };
            if (await DatabaseExists(databaseName)) {
                ResultUtil.HandleFailedResult(databaseResult, ResultString.DatabaseAlreadyExists);
                return databaseResult;
            }
            string sqlCreateDBQuery = $"CREATE DATABASE {databaseName}";
            try {
                using (SqlCommand command = _connection.CreateCommand()) {
                    command.CommandText = sqlCreateDBQuery;
                    await command.ExecuteNonQueryAsync();
                    if (await DatabaseExists(databaseName))
                        ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.CreateDatabaseSuccessfully, true);
                    else
                        throw new InvalidOperationException(ResultString.ExecutedButDatabaseNotCreated);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.CreateDatabaseFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema)
        {
            NonQueryResultModel databaseResult = new NonQueryResultModel();
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
            DbQueryResultModel<int> databaseResult = new DbQueryResultModel<int>();
            using (var transaction = _connection.BeginTransaction()) {
                if (transaction is null) {
                    string message = $"{ResultString.TransactionTransformFailedOrEmpty}SqlTransaction";
                    ResultUtil.HandleFailedResult(databaseResult, message, null);
                    return databaseResult;
                }
                try {
                    using (SqlCommand command = _connection.CreateCommand()) {
                        command.Transaction = transaction;
                        command.CommandText = dbModel.SqlQuery.SqlQueryText;
                        foreach (var param in dbModel.SqlQuery.Parameter) {
                            string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                            command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                        }
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // 嘗試提交事務
                        command.Transaction.Commit();
                        ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfully, rowsAffected);
                    }
                }
                catch (Exception ex) {
                    // 如果有錯誤發生，Rollback事務
                    transaction.Rollback();
                    string message = $"{ResultString.TransactionFailed}{ex.Message}";
                    ResultUtil.HandleFailedResult(databaseResult, message, ex);
                }
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<T>> OperationScalarAsync<T>(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<T> databaseResult = new DbQueryResultModel<T>();
            try {
                using (SqlCommand command = _connection.CreateCommand()) {
                    command.CommandText = dbModel.SqlQuery.SqlQueryText;
                    foreach (var param in dbModel.SqlQuery.Parameter) {
                        string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                        command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                    }
                    var result = await command.ExecuteScalarAsync();
                    //設定模型狀態
                    if (result is T typedResult)
                        ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfully, typedResult);
                    else
                        ResultUtil.HandleFailedResult(databaseResult, ResultString.ResultConvertToGenericFailed);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.QueryFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<DataSet>> OperationReaderAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<DataSet> databaseResult = new DbQueryResultModel<DataSet>();
            try {
                using (SqlCommand command = new SqlCommand(dbModel.SqlQuery.SqlQueryText, _connection)) {
                    foreach (var param in dbModel.SqlQuery.Parameter) {
                        string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                        command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                    }
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        DataSet dataSet = new DataSet();
                        dataSet.Load(reader, LoadOption.PreserveChanges, "Result");
                        //設定模型狀態
                        ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.QuerySuccessfully, dataSet);
                    }
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.QueryFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new DbQueryResultModel<int>();
            DataTable sourceTable =dbModel.SourceDataTable;
            int rowsAffected =0;
            try {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection) {
                    DestinationTableName = sourceTable.TableName
                };
                using (bulkCopy) {
                    bulkCopy.NotifyAfter = sourceTable.Rows.Count;
                    bulkCopy.SqlRowsCopied += (sender, e) => {
                        rowsAffected += (int)e.RowsCopied;  //更新影響行数
                    };
                    await bulkCopy.WriteToServerAsync(sourceTable);
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.BulkInsertSuccessfully, rowsAffected);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.BulkInsertFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<DbQueryResultModel<int>> OperationInsertOrUpdateAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new DbQueryResultModel<int>();
            try {
                if (dbModel.ComparisonColumn.Count < 1) throw new Exception(ResultString.ComparisonColumnIsEmpty);
                using (var command = _connection.CreateCommand()) {

                    var columns = string.Join(", ", dbModel.SourceDataTable.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName));
                    var updateSet = string.Join(", ", dbModel.SourceDataTable.Columns.Cast<DataColumn>()
                    .Select(c => $"[{c.ColumnName}] = @{c.ColumnName}"));
                    var keyWhere = string.Join(" AND ", dbModel.ComparisonColumn
                    .Select(col => $"[{col}] = @{col}"));

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("BEGIN TRAN;");

                    int paramIndex = 0;
                    foreach (DataRow row in dbModel.SourceDataTable.Rows) {
                        var values = string.Join(", ", dbModel.SourceDataTable.Columns.Cast<DataColumn>()
                        .Select(col => $"@param{paramIndex}_{col.ColumnName}"));

                        sb.AppendLine($@"
                        IF EXISTS (SELECT 1 FROM {dbModel.SourceDataTable.TableName} WHERE {keyWhere})
                            UPDATE {dbModel.SourceDataTable.TableName} SET {updateSet} WHERE {keyWhere};
                        ELSE
                            INSERT INTO {dbModel.SourceDataTable.TableName} ({columns}) VALUES ({values});");

                        foreach (DataColumn col in dbModel.SourceDataTable.Columns) {
                            command.Parameters.AddWithValue($"@param{paramIndex}_{col.ColumnName}", row[col] ?? DBNull.Value);
                        }
                        paramIndex++;
                    }

                    sb.AppendLine("COMMIT TRAN;");
                    command.CommandText = sb.ToString();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.InsertOrUpdateSuccessfully, rowsAffected);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.InsertOrUpdateFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        public async Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new DbQueryResultModel<int>();
            string storedProcedureName = dbModel.StoredProcedureData.StoredProcedureName;
            var parameter = dbModel.StoredProcedureData.Parameter;
            try {
                SqlCommand command = new SqlCommand(storedProcedureName, _connection) {
                    CommandType = CommandType.StoredProcedure
                };
                using (command) {
                    foreach (var param in parameter) {
                        string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                        command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                    }
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.ExecutedStoredProcedureSuccessfully, rowsAffected);
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.ExecutedStoredProcedureFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }
        #region 私有方法
        /// <summary>
        /// 檢查該資料庫是否存在
        /// </summary>
        /// <param name="databaseName">資料庫名稱</param>
        /// <returns></returns>
        private async Task<bool> DatabaseExists(string databaseName)
        {
            string checkTableQuery = "SELECT db_id(@databaseName)";
            using (var command = _connection.CreateCommand()) {
                command.CommandText = checkTableQuery;
                command.Parameters.AddWithValue("@databaseName", databaseName);
                var result = await command.ExecuteScalarAsync();
                return result != DBNull.Value;
            }
        }
        /// <summary>
        /// 檢查該資料表是否存在
        /// </summary>
        /// <param name="tableName">資料表名</param>
        /// <returns></returns>
        private async Task<bool> TableExists(string tableName)
        {
            string checkTableQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            using (var command = _connection.CreateCommand()) {
                command.CommandText = checkTableQuery;
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }
        /// <summary>
        /// 創建資料庫
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        private async Task CreateTable(TableSchemaModel tableSchema)
        {
            StringBuilder createTableSql = new StringBuilder ($"CREATE TABLE {tableSchema.TableName} (");
            List<string> columnDefinitions = new List<string>()
            {
                $"{tableSchema.TableName}Id INT IDENTITY(1,1) PRIMARY KEY"
            };

            foreach (var column in tableSchema.SchemaColumns) {
                var columnDef = $"\"{column.ColumnName}\" {GetSqlDbType(column.DataType)}";
                if (!column.AllowNulls)
                    columnDef += " NOT NULL";
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

            using (var command = _connection.CreateCommand()) {
                command.CommandText = createTableSql.ToString();
                await command.ExecuteNonQueryAsync();
            }
        }
        private static string GetSqlDbType(Type type)
        {
            string strType ="NVARCHAR(255)";
            if (type == typeof(int))
                strType = "INT";
            else if (type == typeof(decimal))
                strType = "DECIMAL(18,2)";
            else if (type == typeof(bool))
                strType = "BIT";
            else if (type == typeof(DateTime))
                strType = "DATETIME";
            else if (type == typeof(string))
                strType = "NVARCHAR(255)";
            return strType;
        }
        private static string FormatDefaultValue(Type dataType, object defaultValue)
        {
            // 檢查數據類型並格式化默認值
            if (dataType == typeof(string) || dataType == typeof(char))
                // 字符串和字符類型需要用單引號括起來
                return $"'{defaultValue}'";
            else if (dataType == typeof(DateTime))
                // DateTime 類型，格式化為 SQL Server 可接受的日期字符串
                return $"'{(DateTime)defaultValue:yyyy-MM-dd HH:mm:ss}'";
            else if (dataType == typeof(bool))
                // 布爾類型，在 SQL Server 中用 1 和 0 表示
                return (bool)defaultValue ? "1" : "0";
            else
                // 其他所有類型，默認情況下直接轉換為字符串
                return defaultValue.ToString();
        }
        #endregion 私有方法
    }
}
