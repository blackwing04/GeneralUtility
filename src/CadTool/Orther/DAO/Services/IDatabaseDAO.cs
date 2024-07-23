using StaticUtil.Models.DAO;
using System.Data;
using System.Data.Common;

namespace DAO.Services
{
    /// <summary>
    /// 資料庫DAO對外介面
    /// </summary>
    public interface IDatabaseDAO
    {
        /// <summary>
        /// 切換資料庫，用於連接不同的資料庫
        /// </summary>
        /// <param name="newConnectionString">新的加密字串物件</param>
        /// <remarks>如果要切換資料庫引擎，記得模型內要設定新的資料庫類型</remarks>
        void ChangeDatabaseConnection(CryptoConnectionStringModel newConnectionString);
        /// <summary>
        /// 用於設定單元測地的模擬連線物件
        /// </summary>
        /// <param name="dbConnection">db連線</param>
        /// <param name="mockObject">模擬物件</param>
        void SetMockConnection(DbConnection dbConnection, MockObjectModel? mockObject = null);
        /// <summary>
        /// 創建資料庫
        /// </summary>
        /// <param name="databaseInfo">資料庫訊息</param>
        /// <remarks>有關資料庫訊息如果是SQLite請帶入完整的dbPath，其餘則給db名子</remarks>
        Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseInfo);
        /// <summary>
        /// 切換資料庫
        /// </summary>
        /// <param name="databaseName">要切換的資料庫名稱</param>
        Task<NonQueryResultModel> ChangeDatabaseAsync(string databaseName);
        /// <summary>
        /// 開啟資料庫連接。
        /// </summary>
        /// <returns>連線結果</returns>
        ConnectionResultModel OpenConnection();
        /// <summary>
        /// 異步開啟資料庫連接。
        /// </summary>
        /// <returns>連線結果</returns>
        Task<ConnectionResultModel> OpenConnectionAsync();
        /// <summary>
        /// 關閉資料庫連接。
        /// </summary>
        /// <returns>連線結果</returns>
        ConnectionResultModel CloseConnection();
        /// <summary>
        /// 異步開啟資料庫連接。
        /// </summary>
        /// <returns>連線結果</returns>
        Task<ConnectionResultModel> CloseConnectionAsync();
        /// <summary>
        /// 檢查DB中的資料表是否存在，如不存在自動創建
        /// </summary>
        /// <returns>操作結果，如果不存在且無法創立則直接丟出例外</returns>
        Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema);
        /// <summary>
        /// 執行指令(不返回資料)並提交事務
        /// </summary>
        /// <param name="sqlQuery">Sql語句模型</param>
        /// <remarks>當SQL查詢中有參數時，請盡量用 SqlQueryModel 的 Parameter 屬性傳遞參數值。</remarks>
        /// <returns>資料庫操作結果，模型結果紀錄多少行受到影響</returns>
        Task<DbQueryResultModel<int>> OperationNonQueryTransactionAsync(SqlQueryModel sqlQuery);
        /// <summary>
        /// 執行指令並將單一資料封裝進模型的Result中
        /// </summary>
        /// <typeparam name="T">指定Result的類型</typeparam>
        /// <param name="sqlQuery">Sql語句模型</param>
        /// <remarks>當SQL查詢中有參數時，請盡量用 SqlQueryModel 的 Parameter 屬性傳遞參數值。</remarks>
        /// <returns>資料庫操作結果，模型內包含查詢結果</returns>
        Task<DbQueryResultModel<T>> OperationScalarAsync<T>(SqlQueryModel sqlQuery);
        /// <summary>
        /// 執行指令並將資料集封裝進模型中
        /// </summary>
        /// <param name="sqlQuery">Sql語句模型</param>
        /// <remarks>當SQL查詢中有參數時，請盡量用 SqlQueryModel 的 Parameter 屬性傳遞參數值。</remarks>
        /// <returns>資料庫操作結果，模型內包含查詢結果(DataSet)</returns>
        Task<DbQueryResultModel<DataSet>> OperationReaderAsync(SqlQueryModel sqlQuery);
        /// <summary>
        /// 一次性大量寫入資料
        /// </summary>
        /// <param name="bulkInserTable">要寫入的資料表</param>
        /// <returns>資料庫操作結果，模型結果紀錄多少行受到影響</returns>
        Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DataTable bulkInserTable);
        /// <summary>
        /// 執行預存函數
        /// </summary>
        /// <param name="storedProcedureData">要執行的預存函數模型</param>
        /// <returns>資料庫操作結果，模型結果紀錄多少行受到影響</returns>
        Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(StoredProcedureModel storedProcedureData);
    }
}
