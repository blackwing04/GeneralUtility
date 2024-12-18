using DAO.StaticUtil.Models;
using Generic.StaticUtil;
using System.Data;
using System.Data.Common;
using DAO.StaticUtil;
using static Generic.StaticUtil.Models.DataModel;

namespace DAO.Services.SQLOperation
{
    public class MockOperation : IDbOperation
    {
        public async Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseName)
        {
            DbQueryResultModel<bool> databaseResult = new()
            {
                IsOperationSuccessful = false,
                ResultString = $"SQLite {ResultString.UnsupportedStoredProcedure}",
                Result=false
            };
            return await Task.FromResult(databaseResult);
        }
        public async Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema)
        {
            NonQueryResultModel databaseResult = new();
            try {
                if (tableSchema == null)
                    throw new Exception("Test for check dataTable failed");
                if (tableSchema.TableName == "ExistsTable")
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.DataTableIsExists);
                else
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.DataTableCreateSuccessfully);
            }
            catch (Exception ex) {
                string message = $"{ResultString.CheckDataTableFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return await Task.FromResult(databaseResult);
        }

        public async Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();
            try {
                if (dbModel.MockObject.IsTestingError)
                    throw new Exception(dbModel.MockObject.ErrorMessage);
                using (DbCommand command = dbModel.MockObject.MockDbCommand) {
                    //格式化參數
                    FormatAndSetParameters(command, dbModel.StoredProcedureData.Parameter);
                    var cancellationToken = new CancellationToken(false);
                    int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                    //設定模型狀態
                    ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.ExecutedStoredProcedureSuccessfully, rowsAffected);
                };
            }
            catch (Exception ex) {
                string message = $"{ResultString.ExecutedStoredProcedureFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return databaseResult;
        }

        public async Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();
            try {
                if (dbModel.MockObject.IsTestingError)
                    throw new Exception(dbModel.MockObject.ErrorMessage);
                using DbCommand command = dbModel.MockObject.MockDbCommand;
                var cancellationToken = new CancellationToken(false);
                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                //設定模型狀態
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.BulkInsertSuccessfully, rowsAffected);
            }
            catch (Exception ex) {
                string message = $"{ResultString.BulkInsertFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
            }
            return await Task.FromResult(databaseResult);
        }

        public async Task<DbQueryResultModel<int>> OperationNonQueryTransactionAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<int> databaseResult = new();
            try {
                if (dbModel.MockObject.MockDbTransaction is null) {
                    string message = $"{ResultString.TransactionTransformFailedOrEmpty}Mock Test";
                    ResultUtil.HandleFailedResult(databaseResult, message, null);
                    return databaseResult;
                }
                if (dbModel.MockObject.IsTestingError)
                    throw new Exception(dbModel.MockObject.ErrorMessage);

                using DbCommand command = dbModel.MockObject.MockDbCommand;
                FormatAndSetParameters(command, dbModel.SqlQuery.Parameter);
                var cancellationToken = new CancellationToken(false);
                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                // 嘗試提交事務
                await dbModel.MockObject.MockDbTransaction.CommitAsync();
                //設定模型狀態
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.TransactionSuccessfully, rowsAffected);
            }
            catch (Exception ex) {
                string message = $"{ResultString.TransactionFailed}{ex.Message}";
                ResultUtil.HandleFailedResult(databaseResult, message, ex);
                // 嘗試還原事務
                if (dbModel.MockObject.MockDbTransaction is not null)
                    await dbModel.MockObject.MockDbTransaction.RollbackAsync();
            }
            return await Task.FromResult(databaseResult);
        }

        public async Task<DbQueryResultModel<DataSet>> OperationReaderAsync(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<DataSet> databaseResult = new();
            try {
                if (dbModel.MockObject.IsTestingError)
                    throw new Exception(dbModel.MockObject.ErrorMessage);
                using IDbCommand command = dbModel.MockObject.MockIDbCommand;
                FormatAndSetParameters(command, dbModel.SqlQuery.Parameter);
                var commandBehavior= CommandBehavior.Default;
                using IDataReader reader = command.ExecuteReader(commandBehavior);
                DataSet dataSet = new();
                dataSet.Load(reader, LoadOption.PreserveChanges, "Test");
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
            return await Task.FromResult(databaseResult);
        }

        public async Task<DbQueryResultModel<T>> OperationScalarAsync<T>(DatabaseConfigureModel dbModel)
        {
            DbQueryResultModel<T> databaseResult = new();
            try {
                if (dbModel.MockObject.IsTestingError)
                    throw new Exception(dbModel.MockObject.ErrorMessage);
                using DbCommand command = dbModel.MockObject.MockDbCommand;
                FormatAndSetParameters(command, dbModel.SqlQuery.Parameter);
                var cancellationToken = new CancellationToken(false);
                var result = await command.ExecuteScalarAsync(cancellationToken);
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
            return await Task.FromResult(databaseResult);
        }

        /// <summary>
        /// 格式化並設定存儲過程的參數
        /// </summary>
        /// <param name="command">要設置參數的資料庫命令物件</param>
        /// <param name="Parameter">包含參數資料字典</param>
        private static void FormatAndSetParameters(DbCommand command, Dictionary<string, object> Parameter)
        {
            // 格式化參數
            var standardizedParameters = Parameter.ToDictionary(p => $"@{p.Key.TrimStart('@')}", p => p.Value);

            // 模擬設定參數
            foreach (var param in standardizedParameters) {
                if (!param.Key.StartsWith("@"))
                    throw new InvalidOperationException($"Parameter {param.Key} validation failed.");

                var dbParam = command.CreateParameter();
                if (dbParam != null) {
                    dbParam.ParameterName = param.Key;
                    dbParam.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParam);
                }
            }
        }
        /// <summary>
        /// 格式化並設定存儲過程的參數
        /// </summary>
        /// <param name="command">要設置參數的資料庫命令物件</param>
        /// <param name="Parameter">包含參數資料字典</param>
        private static void FormatAndSetParameters(IDbCommand command, Dictionary<string, object> Parameter)
        {
            // 格式化參數
            var standardizedParameters = Parameter.ToDictionary(p => $"@{p.Key.TrimStart('@')}", p => p.Value);

            // 模擬設定參數
            foreach (var param in standardizedParameters) {
                if (!param.Key.StartsWith("@"))
                    throw new InvalidOperationException($"Parameter {param.Key} validation failed.");

                var dbParam = command.CreateParameter();
                if (dbParam != null) {
                    dbParam.ParameterName = param.Key;
                    dbParam.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParam);
                }
            }
        }

        public Task<DbQueryResultModel<int>> OperationInsertOrUpdateAsync(DatabaseConfigureModel dbModel)
        {
            throw new NotImplementedException();
        }
    }
}
