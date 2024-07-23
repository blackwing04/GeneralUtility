using StaticUtil.Generic;
using StaticUtil.Models.DAO;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace DAO.Services.SQLOperation
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
            string exMessage = $"{ResultString.ConnectionTransformFailedOrEmpty}SqlConnection";
            _connection = connection as SqlConnection ?? throw new ArgumentException(exMessage);
        }
        #endregion 建構式
        public async Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseName)
        {
            DbQueryResultModel<bool> databaseResult = new(){
                Result = false
            };
            if (await DatabaseExists(databaseName)) {
                ResultUtil.HandleFailedResult(databaseResult, ResultString.DatabaseAlreadyExists);
                return databaseResult;
            }
            string sqlCreateDBQuery = $"CREATE DATABASE {databaseName}";
            try {
                using SqlCommand command = _connection.CreateCommand();
                command.CommandText = sqlCreateDBQuery;
                await command.ExecuteNonQueryAsync();
                if (await DatabaseExists(databaseName))
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.CreateDatabaseSuccessfully, true);
                else
                    throw new InvalidOperationException(ResultString.ExecutedButDatabaseNotCreated);
            }
            catch (Exception ex) {
                string message = $"{ResultString.CreateDatabaseFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
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
            using var transaction = await _connection.BeginTransactionAsync() as SqlTransaction;
            if (transaction is null) {
                string message = $"{ResultString.TransactionTransformFailedOrEmpty}SqlTransaction";
                ResultUtil.HandleFailedResult(databaseResult, message, null);
                return databaseResult;
            }
            try {
                using SqlCommand command = _connection.CreateCommand();
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
                using SqlCommand command = _connection.CreateCommand();
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
                using SqlCommand command = new(dbModel.SqlQuery.SqlQueryText, _connection);

                foreach (var param in dbModel.SqlQuery.Parameter) {
                    string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                    command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                }
                using SqlDataReader reader = await command.ExecuteReaderAsync();
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
            DataTable sourceTable =dbModel.BulkInsertTable;
            int rowsAffected =0;
            try {

                using SqlBulkCopy bulkCopy = new(_connection){
                    DestinationTableName = sourceTable.TableName
                };
                bulkCopy.NotifyAfter = sourceTable.Rows.Count;
                bulkCopy.SqlRowsCopied += (sender, e) => {
                    rowsAffected += (int)e.RowsCopied;  //更新影響行数
                };

                await bulkCopy.WriteToServerAsync(sourceTable);
                //設定模型狀態
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.BulkInsertSuccessfully, rowsAffected);
            }
            catch (Exception ex) {
                string message = $"{ResultString.BulkInsertFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();
            string storedProcedureName = dbModel.StoredProcedureData.StoredProcedureName;
            var parameter = dbModel.StoredProcedureData.Parameter;
            try {
                using SqlCommand command = new(storedProcedureName, _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                foreach (var param in parameter) {
                    string paramName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                    command.Parameters.AddWithValue(paramName, param.Value ?? DBNull.Value);
                }
                int rowsAffected = await command.ExecuteNonQueryAsync();
                //設定模型狀態
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.ExecutedStoredProcedureSuccessfully, rowsAffected);
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
            using var command = _connection.CreateCommand();
            command.CommandText = checkTableQuery;
            command.Parameters.AddWithValue("@databaseName", databaseName);
            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value;
        }
        /// <summary>
        /// 檢查該資料表是否存在
        /// </summary>
        /// <param name="tableName">資料表名稱</param>
        /// <returns></returns>
        private async Task<bool> TableExists(string tableName)
        {
            string checkTableQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
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
                $"{tableSchema.TableName}Id INT IDENTITY(1,1) PRIMARY KEY"
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
        private static string? FormatDefaultValue(Type dataType, object defaultValue)
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
