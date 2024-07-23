using DAO.StaticUtil.Enums;
using System.Data;
using System.Data.Common;

namespace DAO.StaticUtil.Models
{
    /// <summary>
    /// 資料庫設定模型
    /// </summary>
    public class DatabaseConfigureModel
    {
        /// <summary>
        /// 資料庫連線物件
        /// </summary>
        public DbConnection DbConnection { get; set; } = null!;
        /// <summary>
        /// 資料庫類型
        /// </summary>
        public DbTypeEnum DatabaseType { get; set; }
        /// <summary>
        /// SQL查詢語句模型
        /// </summary>
        public SqlQueryModel SqlQuery { get; set; } = new SqlQueryModel();
        /// <summary>
        /// 資料來源表，內存要上傳到資料庫的資料
        /// </summary>
        public DataTable SourceDataTable { get; set; } = new DataTable();
        /// <summary>
        /// 比對的欄位名，在做新增或更新的方法時必須提供
        /// </summary>
        /// <remarks>在做新增或更新時必須提供比對欄位名，否則方法會不知道該拿哪個欄位數值(唯一)做比對</remarks>
        public List<string> ComparisonColumn { get; set; } = new List<string>();
        /// <summary>
        /// 調用預存函數方法使用的資料
        /// </summary>
        public StoredProcedureModel StoredProcedureData { get; set; } = new StoredProcedureModel();
        /// <summary>
        /// 模擬物件模型
        /// </summary>
        public MockObjectModel MockObject { get; set; } = new();
    }
    /// <summary>
    /// SQL查詢語句模型
    /// </summary>
    public class SqlQueryModel
    {
        /// <summary>
        /// Sql查詢文本，如果只是要做查詢請把查詢語法寫進此欄位
        /// </summary>
        public string SqlQueryText { get; set; } = string.Empty;
        /// <summary>
        /// 用於存儲 SQL 查詢中的參數值對應
        /// </summary>
        public Dictionary<string, object> Parameter { get; set; } = new Dictionary<string, object>();
    }
    /// <summary>
    /// 連線結果
    /// </summary>
    public class ConnectionResultModel
    {
        /// <summary>
        /// 資料庫連線狀態
        /// </summary>
        public bool IsDbConnected { get; set; }
        /// <summary>
        /// 連線結果訊息
        /// </summary>
        public string ConnectionResultString { get; set; } = string.Empty;
    }
    /// <summary>
    /// 資料庫查詢結果，不含具體查詢內容
    /// </summary>
    public class NonQueryResultModel
    {
        /// <summary>
        /// 內部錯誤
        /// </summary>
        public Exception? InnerException { get; set; }
        /// <summary>
        /// 最後操作結果訊息
        /// </summary>
        public string ResultString { get; set; } = string.Empty;
        /// <summary>
        /// 最後操作是否成功
        /// </summary>
        public bool IsOperationSuccessful { get; set; }
    }
    /// <summary>
    /// 資料庫查詢結果，內含查詢內容
    /// </summary>
    public class DbQueryResultModel<T>
    {
        /// <summary>
        /// 資料庫查詢的結果
        /// </summary>
        public T? Result { get; set; }
        /// <summary>
        /// 內部錯誤
        /// </summary>
        public Exception? InnerException { get; set; }
        /// <summary>
        /// 最後操作結果訊息
        /// </summary>
        public string ResultString { get; set; } = string.Empty;
        /// <summary>
        /// 最後操作是否成功
        /// </summary>
        public bool IsOperationSuccessful { get; set; }
    }
    /// <summary>
    /// 預存函數模型
    /// </summary>
    public class StoredProcedureModel
    {
        /// <summary>
        /// 預存函數名稱
        /// </summary>
        public string StoredProcedureName { get; set; } = string.Empty;
        /// <summary>
        /// 預存函數要帶入的參數對應字典
        /// </summary>
        public Dictionary<string, object> Parameter { get; set; } =new Dictionary<string, object>();

    }
    /// <summary>
    /// 測試模擬物件模型
    /// </summary>
    public class MockObjectModel
    {
        /// <summary>
        /// 標記測試錯誤
        /// </summary>
        public bool IsTestingError { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        /// 模擬封裝的抽象DbCommand
        /// </summary>
        public DbCommand MockDbCommand { get; set; } = null!;
        /// <summary>
        /// 模擬封裝的抽象DbCommand介面
        /// </summary>
        public IDbCommand MockIDbCommand { get; set; } = null!;
        /// <summary>
        /// 模擬封裝的抽象DbTransaction
        /// </summary>
        public DbTransaction? MockDbTransaction { get; set; } = null;
    }
}
