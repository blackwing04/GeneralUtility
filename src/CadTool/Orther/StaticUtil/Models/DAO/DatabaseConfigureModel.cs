using StaticUtil.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace StaticUtil.Models.DAO
{
    /// <summary>
    /// 資料庫設定模型
    /// </summary>
    public class DatabaseConfigureModel
    {
        /// <summary>
        /// 資料庫連線物件
        /// </summary>
        public DbConnection DbConnection { get; set; }
        /// <summary>
        /// 資料庫類型
        /// </summary>
        public DbTypeEnum DatabaseType { get; set; }
        /// <summary>
        /// SQL查詢語句模型
        /// </summary>
        public SqlQueryModel SqlQuery { get; set; } = new SqlQueryModel();
        /// <summary>
        /// 一次性插入多筆資料的資料表
        /// </summary>
        public DataTable BulkInsertTable { get; set; } = new DataTable();
        /// <summary>
        /// 調用預存函數方法使用的資料
        /// </summary>
        public StoredProcedureModel StoredProcedureData { get; set; } = new StoredProcedureModel();
        /// <summary>
        /// 模擬物件模型
        /// </summary>
        public MockObjectModel MockObject { get; set; } = null;
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
    /// 資料表結構
    /// </summary>
    public class TableSchemaModel
    {
        /// <summary>
        /// 資料表名稱
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public List<SchemaColumnModel> SchemaColumns { get; set; }
        /// <summary>
        /// 複合唯一設定(key:複合約束的名稱,value:複合約束包含欄位名稱)
        /// </summary>
        public Dictionary<string,List<string>> CompositeUnique { get; set; }
    }
    /// <summary>
    /// 欄位結構
    /// </summary>
    public class SchemaColumnModel
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;
        /// <summary>
        /// 資料類型(C#)
        /// </summary>
        public Type DataType { get; set; } = typeof(string);
        /// <summary>
        /// 預設值
        /// </summary>
        public string DefaultValue { get; set; } = null;
        /// <summary>
        /// 必須唯一
        /// </summary>
        public bool Unique { get; set; } 
        /// <summary>
        /// 允許空值
        /// </summary>
        public bool AllowNulls { get; set; } = true;
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
        public Exception InnerException { get; set; }
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
        public T Result { get; set; }
        /// <summary>
        /// 內部錯誤
        /// </summary>
        public Exception InnerException { get; set; }
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
        public DbCommand MockDbCommand { get; set; }
        /// <summary>
        /// 模擬封裝的抽象DbCommand介面
        /// </summary>
        public IDbCommand MockIDbCommand { get; set; }
        /// <summary>
        /// 模擬封裝的抽象DbTransaction
        /// </summary>
        public DbTransaction MockDbTransaction { get; set; }
    }
    /// <summary>
    /// 封裝加密後的資料庫字串用於解析資料庫連接字串，Crypto裡有創建的方法。
    /// </summary>
    public class CryptoConnectionStringModel
    {
        /// <summary>
        /// 資料庫類型(預設MSSQL)
        /// </summary>
        public DbTypeEnum DatabaseType { get; set; }
        /// <summary>密鑰(key)</summary>
        public string K1 { get; set; } = string.Empty;
        /// <summary>向量值(IV)</summary>
        public string V2 { get; set; } = string.Empty;
        /// <summary>加密連接字段(ConnectionString)</summary>
        public string CS { get; set; } = string.Empty;
    }
}
