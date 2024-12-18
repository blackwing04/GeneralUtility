using DAOCustomizeException;
using Generic.StaticUtil;
using DAOStandard.StaticUtil.Models;
using DAOStandard.Services.SQLOperation;
using DAOStandard.StaticUtil.Enums;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DAOStandard.StaticUtil;
using static Generic.StaticUtil.Models.DataModel;
using DAOStandard.Models;

namespace DAOStandard.Services
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
            _connectionString = StaticUtil.CryptoHelper.DecryptString(connectionString);
            DbModel = InitializeDbModel();
            //啟用SQLite
            if (connectionString.DatabaseType == DbTypeEnum.SQLite)
                SQLitePCL.Batteries.Init();
        }
        private DatabaseConfigureModel InitializeDbModel()
        {
            DatabaseConfigureModel dbModel = new DatabaseConfigureModel();
            // 將整數轉換為列舉型態，如果無法轉換則判定不支援該資料庫
            if (!Enum.IsDefined(typeof(DbTypeEnum), _databaseTypeIndex))
                throw new ArgumentException(ResultString.UnsupportedDatabase);

            DbTypeEnum databaseType = (DbTypeEnum)_databaseTypeIndex;
            DbConnection dbConnection;
            switch (databaseType) {
                case DbTypeEnum.MSSQL:
                    dbConnection = new SqlConnection(_connectionString);
                    break;
                case DbTypeEnum.SQLite:
                    dbConnection = new SqliteConnection(_connectionString);
                    break;
                case DbTypeEnum.MySQL:
                    dbConnection = null;
                    break;
                case DbTypeEnum.PostgreSQL:
                    dbConnection = null;
                    break;
                case DbTypeEnum.Oracle:
                    dbConnection = null;
                    break;
                default:
                    dbConnection = null;
                    break;
            }
            //如果非模擬並且又無法建立連線物件依然認定為不支援
            if (dbConnection == null && databaseType != DbTypeEnum.Mock)
                throw new ArgumentException(ResultString.UnsupportedDatabase);

            //將連線物件設定至模型
            dbModel.DatabaseType = databaseType;
            dbModel.DbConnection = dbConnection;
            return dbModel;
        }
        #endregion 建構式
        #region 資料庫連線方法
        public void ChangeDatabaseConnection(CryptoConnectionStringModel newConnectionString)
        {
            _connectionString = StaticUtil.CryptoHelper.DecryptString(newConnectionString);
            if (newConnectionString.DatabaseType != DbTypeEnum.None)
                _databaseTypeIndex = (int)newConnectionString.DatabaseType;
            DbModel = InitializeDbModel();
        }
        public void SetMockConnection(DbConnection dbConnection, MockObjectModel mockObject = null)
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
                 * 而SQLite則是在內部方法做開啟連結跟關閉，
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
            NonQueryResultModel databaseResult = new NonQueryResultModel();
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
                CloseConnection();
            }
            return databaseResult;
        }
        public ConnectionResultModel OpenConnection()
        {
            ConnectionResultModel resul = new ConnectionResultModel();
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
            ConnectionResultModel resul = new ConnectionResultModel();
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
            ConnectionResultModel resul = new ConnectionResultModel();
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
                CloseConnection();
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
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
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
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
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
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
            }
        }
        public async Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DataTable bulkInserTable)
        {
            try {
                if (bulkInserTable.Rows.Count <= 0) throw new ArgumentException(ResultString.BulkInserTableIsEmpty);
                DbModel.SourceDataTable = bulkInserTable;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.OperationBulkInsertAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
            }
        }

        public async Task<DbQueryResultModel<int>> OperationInsertOrUpdateAsync(DataTable sourceDataTable)
        {
            try {
                if (sourceDataTable.Rows.Count <= 0) throw new ArgumentException(ResultString.InsertOrUpdateTableIsEmpty);
                DbModel.SourceDataTable = sourceDataTable;
                await OpenConnectionAsync();
                var handler = OperationFactory.GetOperationHandler(DbModel.DatabaseType,DbModel.DbConnection);
                var result = await handler.OperationInsertOrUpdateAsync(DbModel);
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString, result.InnerException);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
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
                if (!result.IsOperationSuccessful) throw new Exception(result.ResultString);
                return result;
            }
            catch {
                throw;
            }
            finally {
                ResetDatabaseQueryObject();
                CloseConnection();
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
            DbModel.SqlQuery = new SqlQueryModel();
            DbModel.SourceDataTable = new DataTable();
            DbModel.StoredProcedureData = new StoredProcedureModel();
        }
        #endregion 其他方法
    }
}
