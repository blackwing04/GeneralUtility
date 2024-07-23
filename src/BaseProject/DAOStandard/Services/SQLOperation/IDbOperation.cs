using DAOStandard.StaticUtil.Models;
using System.Data;
using System.Threading.Tasks;
using static Generic.StaticUtil.Models.DataModel;

namespace DAOStandard.Services.SQLOperation
{
    public interface IDbOperation
    {
        /// <summary>
        /// 創建資料庫
        /// </summary>
        /// <param name="databaseInfo">資料庫訊息</param>
        /// <remarks>有關資料庫訊息如果是SQLite請帶入完整的dbPath，其餘則給db名子</remarks>
        Task<DbQueryResultModel<bool>> CreateDatabaseAsync(string databaseInfo);
        /// <summary>
        /// 檢查DB中的資料表是否存在，如不存在自動創建
        /// </summary>
        /// <param name="tableSchema">資料表結構</param>
        Task<NonQueryResultModel> CheckDataTableExistsAsync(TableSchemaModel tableSchema);
        /// <summary>
        /// 執行指令(不返回資料)並提交事務
        /// </summary>
        Task<DbQueryResultModel<int>> OperationNonQueryTransactionAsync(DatabaseConfigureModel dbModel);
        /// <summary>
        /// 執行指令並將單一資料封裝進模型的Result中
        /// </summary>
        /// <typeparam name="T">指定Result的類型</typeparam>
        Task<DbQueryResultModel<T>> OperationScalarAsync<T>(DatabaseConfigureModel dbModel);
        /// <summary>
        /// 執行指令並將資料集封裝進模型中(DataSet)
        /// </summary>
        Task<DbQueryResultModel<DataSet>> OperationReaderAsync(DatabaseConfigureModel dbModel);
        /// <summary>
        /// 一次性大量寫入資料
        /// </summary>
        Task<DbQueryResultModel<int>> OperationBulkInsertAsync(DatabaseConfigureModel dbModel);
        /// <summary>
        /// 依據資料源做新增或更新
        /// </summary>
        Task<DbQueryResultModel<int>> OperationInsertOrUpdateAsync(DatabaseConfigureModel dbModel);
        /// <summary>
        /// 執行預存函數
        /// </summary>
        Task<DbQueryResultModel<int>> ExecuteStoredProcedureAsync(DatabaseConfigureModel dbModel);
    }
}
