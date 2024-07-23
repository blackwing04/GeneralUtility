namespace CadDAO.Model
{
    /// <summary>
    /// 資料庫模型
    /// </summary>
    public class DatabaseModel
    {
        /// <summary>
        /// 資料表列表
        /// </summary>
        public List<TableModel> Tables { get; set; } = new();
    }
    /// <summary>
    /// 資料表模型
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// 資料表名稱
        /// </summary>
        public string TablesName { get; set; } = string.Empty;
        /// <summary>
        /// 欄位列表
        /// </summary>
        public List<ColumnModel> Columns { get; set; } = new();
    }
    /// <summary>
    /// 欄位模型
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// 數值
        /// </summary>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// 資料類型
        /// </summary>
        public string DataType { get; set; } = string.Empty;
        /// <summary>
        /// 組別名稱
        /// </summary>
        public string? GroupName { get; set; } =null;
        /// <summary>
        /// 註解
        /// </summary>
        public string? Remark { get; set; } = null;
    }
}
