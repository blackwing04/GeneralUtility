namespace Generic.StaticUtil
{
    public static class ResultString
    {
        #region 錯誤
        /// <summary>
        /// 不支援的資料庫類型
        /// </summary>
        public const string UnsupportedDatabase ="Unsupported database type.";
        /// <summary>
        /// 資料庫類型不支援預存函數
        /// </summary>
        public const string UnsupportedStoredProcedure ="does not support stored procedures.";
        /// <summary>
        /// 連線失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string FailedOpenConnection ="Failed to open connection. Error:";
        /// <summary>
        /// Db連線物件不可為空
        /// </summary>
        public const string DbConnectionIsEmpty ="Db Connection cannot be Empty.";
        /// <summary>
        /// 資料表結構不可為空
        /// </summary>
        public const string TableSchemaIsEmpty ="Table schema cannot be null.";
        /// <summary>
        /// 資料表結構不可為空
        /// </summary>
        public const string ResultConvertToGenericFailed ="Result object convert to generic T failed.";
        #endregion 錯誤
        #region 連線操作
        /// <summary>
        /// 切換資料庫成功
        /// </summary>
        public const string ChangeDatabaseSuccessfully ="Change database successfully.";
        /// <summary>
        /// 切換資料庫失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string ChangeDatabaseFailed ="Failed to change database. Error:";
        /// <summary>
        /// 開啟連線成功
        /// </summary>
        public const string ConnectionOpenSuccessfully ="Connection opened successfully.";
        /// <summary>
        /// 開啟連線失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string ConnectionOpenFailed ="Failed to open connection. Error:";
        /// <summary>
        /// 關閉連線成功
        /// </summary>
        public const string ConnectionCloseSuccessfully ="Connection closed successfully.";
        /// <summary>
        /// 關閉連線失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string ConnectionCloseFailed ="Failed to close connection. Error:";
        #endregion 連線操作
        #region 資料庫操作
        /// <summary>
        /// 資料庫已經存在
        /// </summary>
        public const string DatabaseAlreadyExists
            ="Database already exists.";
        /// <summary>
        /// 創建資料庫失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string ExecutedButDatabaseNotCreated 
            ="The SQL command was executed, but the database was not successfully created.";
        /// <summary>
        /// 創建資料庫失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string CreateDatabaseFailed ="Create database failed. Error:";
        /// <summary>
        /// 創建資料庫成功
        /// </summary>
        public const string CreateDatabaseSuccessfully ="Create database successfully.";
        /// <summary>
        /// 檢查資料表失敗(包含創建操作)
        /// </summary>
        public const string CheckDataTableFailed ="Check DataTable Failed. Error:";
        /// <summary>
        /// 資料存在
        /// </summary>
        public const string DataTableIsExists ="DataTable is exists.";
        /// <summary>
        /// 資料存在
        /// </summary>
        public const string DataTableCreateSuccessfully 
            ="DataTable is create successful,so dataTable now is exist.";
        /// <summary>
        /// 事務提交成功
        /// </summary>
        public const string TransactionSuccessfully ="Transaction committed.";
        /// <summary>
        /// 事務提交失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string TransactionFailed ="Transaction Failed ,transaction rolled back. Error:";
        /// <summary>
        /// 查詢操作成功，查詢內容請使用模型的Result
        /// </summary>
        public const string QuerySuccessfully 
            ="Query successful, please use the model's Result for the query content.";
        /// <summary>
        /// 查詢操作成功，但查詢到的內容是空的
        /// </summary>
        public const string QuerySuccessfullyButEmpty ="Query successful,but the query content is empty.";
        /// <summary>
        /// 查詢失敗(後方請自行帶入失敗的詳細原因)
        /// </summary>
        public const string QueryFailed ="Query Failed. Error:";
        /// <summary>
        /// 一次性大量寫入成功
        /// </summary>
        public const string BulkInsertSuccessfully = "Bulk insert executed successfully.";
        /// <summary>
        /// 一次性大量寫入失敗
        /// </summary>
        public const string BulkInsertFailed = "Bulk insert failed. Error: ";
        /// <summary>
        /// 新增或更新的操作成功
        /// </summary>
        public const string InsertOrUpdateSuccessfully = "Insert Or Update executed successfully.";
        /// <summary>
        /// 新增或更新的操作失敗
        /// </summary>
        public const string InsertOrUpdateFailed = "Insert Or Update failed. Error:";
        /// <summary>
        /// 調用預存函數成功
        /// </summary>
        public const string ExecutedStoredProcedureSuccessfully = "Stored procedure executed successfully.";
        /// <summary>
        /// 調用預存函數失敗
        /// </summary>
        public const string ExecutedStoredProcedureFailed = "Stored procedure execution failed: ";
        #endregion 資料庫操作
        #region 參數有問題的例外
        /// <summary>
        /// 事務無法轉換成目標事務或是事務為空(後方自行帶轉換甚麼類型失敗)
        /// </summary>
        public const string TransactionTransformFailedOrEmpty
            ="Transaction is empty or transaction can't transform to ";
        /// <summary>
        /// 連線物件轉換失敗或是連線物件為空(後方自行帶轉換甚麼類型失敗)
        /// </summary>
        public const string ConnectionTransformFailedOrEmpty 
            ="Database connection is empty or connection transaction can't transform to ";
        /// <summary>
        /// 查詢語法為空無法做SQL查詢
        /// </summary>
        public const string SqlQueryIsEmpty 
            ="SQL query cannot be performed because the query syntax is empty";
        /// <summary>
        /// BulkInser時，資料來源表不可為空
        /// </summary>
        public const string BulkInserTableIsEmpty 
            ="Bulk insert cannot be performed because the source table is empty";
        /// <summary>
        /// InsertOrUpdate時，資料來源表不可為空
        /// </summary>
        public const string InsertOrUpdateTableIsEmpty
            ="Insert or update cannot be performed because the source table is empty";
        /// <summary>
        /// 比對欄位為空無法做新增或更新的方法
        /// </summary>
        public const string ComparisonColumnIsEmpty
            ="Insert or update cannot be performed because the comparison column is empty";
        /// <summary>
        /// 執行預存函數時，模型不可為空
        /// </summary>
        public const string StoredProcedureModelIsEmpty 
            ="Execute Stored Procedure cannot be performed because the source table is empty";
        #endregion 參數有問題的例外
    }
}
