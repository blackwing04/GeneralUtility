using StaticUtil.Enums;
using DAO.Services.SQLOperation;
using StaticUtil.Generic;
using StaticUtil.Models.DAO;
using DAOCustomizeException;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DAO.Services
{
    /// <summary>
    /// 資料庫DAO的基層類別
    /// </summary>
    public class DatabaseDAO : IDatabaseDAO
    {
        #region 建構式
        /// <summary>
        /// 資料庫類型的整數索引。
        /// </summary>
        private int _databaseTypeIndex;
        /// <summary>
        /// 資料庫連接字符串。
        /// </summary>
        private string _connectionString = string.Empty;
        /// <summary>
        /// 資料庫模型
        /// </summary>
        private DatabaseConfigureModel DbModel { get; set; }
        /// <summary>
        /// 資料庫DAO的基層類別
        /// </summary>
        public DatabaseDAO(CryptoConnectionStringModel connectionString)
        {
            int databaseTypeIndex =(int)connectionString.DatabaseType;
            _databaseTypeIndex = databaseTypeIndex;
            _connectionString = Crypto.DecryptString(connectionString);
            DbModel = InitializeDbModel();
            //啟用SQLite
            if (connectionString.DatabaseType == DbTypeEnum.SQLite) { 
                SQLitePCL.Batteries.Init();
            }
        }
        /// <summary>
        /// 初始化DB模組
        /// </summary>
        /// <returns>建構完成的模組</returns>
        /// <exception cref="ArgumentException">不支援的資料庫類型</exception>
        private DatabaseConfigureModel InitializeDbModel()
        {
            DatabaseConfigureModel dbModel = new();
            // 將整數轉換為列舉型態，如果無法轉換則判定不支援該資料庫
            if (!Enum.IsDefined(typeof(DbTypeEnum), _databaseTypeIndex))
                throw new ArgumentException(ResultString.UnsupportedDatabase);

            DbTypeEnum databaseType = (DbTypeEnum)_databaseTypeIndex;
            DbConnection? dbConnection = databaseType switch {
                DbTypeEnum.MSSQL => new SqlConnection(_connectionString),
                DbTypeEnum.SQLite => new SqliteConnection(_connectionString),
                DbTypeEnum.MySQL => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                DbTypeEnum.PostgreSQL => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                DbTypeEnum.Oracle => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                _ => null,
            };
            //如果非模擬並且又無法建立連線物件依然認定為不支援
            if (dbConnection == null && databaseType != DbTypeEnum.Mock)
                throw new ArgumentException(ResultString.UnsupportedDatabase);

            //將連線物件設定至模型
            dbModel.DatabaseType = databaseType;
            dbModel.DbConnection = dbConnection!;
            return dbModel;
        }
        #endregion 建構式
        #region 資料庫連線方法
        public void ChangeDatabaseConnection(CryptoConnectionStringModel newConnectionString)

        {
            _connectionString = Crypto.DecryptString(newConnectionString);
            if (newConnectionString.DatabaseType != DbTypeEnum.None)
                _databaseTypeIndex = (int)newConnectionString.DatabaseType;
            DbModel = InitializeDbModel();
        }
        public void SetMockConnection(DbConnection dbConnection, MockObjectModel? mockObject = null)
        {
            DbModel.DbConnection = dbConnection;
            if (mockObject != null) {
                DbModel.MockObject = mockObject;
            }
        }
        public async Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseInfo)
        {
            try {
                /*
                * SQLite是靠開啟連結創建資料庫，所以只有非SQLite才需要開啟連結
                * 而SQLite則是在內部方法做開啟連結跟關閉，不過實際上SQLite可能也用不太到這個方法
                * 所有方法開都會先開啟連結，那時便會一併創建了。
               */
                if (DbModel.DatabaseType != DbTypeEnum.SQLite)
                    await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.CreateDatabaseAsync(databaseInfo);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                if (DbModel.DatabaseType != DbTypeEnum.SQLite)
                    await OpenConnectionAsync();
            }
        }
        public async Task<NonQueryResultModel> ChangeDatabaseAsync(string databaseName)
        {
            NonQueryResultModel databaseResult = new ();
            try {
                await OpenConnectionAsync();
                DbModel.DbConnection.ChangeDatabase(databaseName);
                ResultUtil.HandleSuccessfulResult(databaseResult, ResultString.ChangeDatabaseSuccessfully);
            }
            catch (Exception ex) {
                string message =$"{ResultString.ChangeDatabaseFailed}{ex.Message}";
                throw new ChangeDatabaseException(message, ex);
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
            return databaseResult;
        }
        public ConnectionResultModel OpenConnection()
        {
            ConnectionResultModel resul = new();
            try {
                if (DbModel.DbConnection.State != ConnectionState.Open) {
                    DbModel.DbConnection.Open();
                    resul.ConnectionResultString = ResultString.ConnectionOpenSuccessfully;
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.ConnectionOpenFailed}{ex.Message}";
                throw new ConnectionException(message, ex);
            }
            finally {
                resul.IsDbConnected = (DbModel.DbConnection.State == ConnectionState.Open);
            }
            return resul;
        }
        public async Task<ConnectionResultModel> OpenConnectionAsync()
        {
            ConnectionResultModel resul = new();
            try {
                if (DbModel.DbConnection.State != ConnectionState.Open) {
                    await DbModel.DbConnection.OpenAsync();
                    resul.ConnectionResultString = ResultString.ConnectionOpenSuccessfully;
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.ConnectionOpenFailed}{ex.Message}";
                throw new ConnectionException(message, ex);
            }
            finally {
                resul.IsDbConnected = (DbModel.DbConnection.State == ConnectionState.Open);
            }
            return resul;
        }
        public ConnectionResultModel CloseConnection()
        {
            ConnectionResultModel resul = new();
            try {
                if (DbModel.DbConnection.State != ConnectionState.Closed) {
                    DbModel.DbConnection.Close();
                    resul.ConnectionResultString = ResultString.ConnectionCloseSuccessfully;
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.ConnectionCloseFailed}{ex.Message}";
                throw new ConnectionException(message, ex);
            }
            finally {
                resul.IsDbConnected = (DbModel.DbConnection.State == ConnectionState.Open);
            }
            return resul;
        }
        public async Task<ConnectionResultModel> CloseConnectionAsync()
        {
            ConnectionResultModel resul = new();
            try {
                if (DbModel.DbConnection.State != ConnectionState.Closed) {
                    await DbModel.DbConnection.CloseAsync();
                    resul.ConnectionResultString = ResultString.ConnectionCloseSuccessfully;
                }
            }
            catch (Exception ex) {
                string message = $"{ResultString.ConnectionCloseFailed}{ex.Message}";
                throw new ConnectionException(message, ex);
            }
            finally {
                resul.IsDbConnected = (DbModel.DbConnection.State == ConnectionState.Open);
            }
            return resul;
        }
        #endregion 資料庫連線方法
        #region 資料交互方法
        public async Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema)
        {
            try {
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.CheckDataTableExistsAsync(tableSchema);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }
        public async Task<DbQueryResultModel<int>> OperationNonQueryTransactionAsync(SqlQueryModel sqlQuery)
        {
            try {
                if (string.IsNullOrWhiteSpace(sqlQuery.SqlQueryText)) throw new ArgumentException(ResultString.SqlQueryIsEmpty);
                DbModel.SqlQuery = sqlQuery;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.OperationNonQueryTransactionAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }
        public async Task<DbQueryResultModel<T>> OperationScalarAsync<T>(SqlQueryModel sqlQuery)
        {
            try {
                if (string.IsNullOrWhiteSpace(sqlQuery.SqlQueryText)) throw new ArgumentException(ResultString.SqlQueryIsEmpty);
                DbModel.SqlQuery = sqlQuery;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result =  await handler.OperationScalarAsync<T>(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }

        public async Task<DbQueryResultModel<DataSet>> OperationReaderAsync(SqlQueryModel sqlQuery)
        {
            try {
                if (string.IsNullOrWhiteSpace(sqlQuery.SqlQueryText)) throw new ArgumentException(ResultString.SqlQueryIsEmpty);
                DbModel.SqlQuery = sqlQuery;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.OperationReaderAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }
        public async Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DataTable bulkInserTable)
        {
            try {
                if (bulkInserTable.Rows.Count <= 0) throw new ArgumentException(ResultString.BulkInserTableIsEmpty);
                DbModel.BulkInsertTable = bulkInserTable;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.OperationBulkInsertAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }
        public async Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(StoredProcedureModel storedProcedureData)
        {
            try {
                if (string.IsNullOrWhiteSpace(storedProcedureData.StoredProcedureName))
                    throw new ArgumentException(ResultString.StoredProcedureModelIsEmpty);
                if (storedProcedureData.Parameter.Count <= 0)
                    throw new ArgumentException(ResultString.StoredProcedureModelIsEmpty);
                DbModel.StoredProcedureData = storedProcedureData;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.ExecuteStoredProcedureAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                await CloseConnectionAsync();
            }
        }
        #endregion 資料交互方法
        #region 其他方法
        /// <summary>
        /// 重置資料庫模型查詢物件
        /// </summary>
        /// <returns></returns>
        private void ResetDatabaseQueryObject()
        {
            DbModel.SqlQuery = new();
            DbModel.BulkInsertTable = new();
            DbModel.StoredProcedureData = new();
        }
        #endregion 其他方法
    }
}
