using StaticUtil.Models.DAO;
using System.Data;
using System.Collections.Generic;

namespace ExcelTool.StaticUtil
{
    public static class ExcelHeader
    {
        #region 匯入資料整理
        /// <summary>
        /// 初始化 DataTable 結構。
        /// </summary>
        /// <param name="schemaColumns">欄位結構描述。</param>
        /// <returns>初始化後的 DataTable。</returns>
        public static DataTable InitializeDataTable(List<SchemaColumnModel> schemaColumns)
        {
            DataTable dataTable = new DataTable();
            foreach (var column in schemaColumns) {
                dataTable.Columns.Add(column.ColumnName, column.DataType);
            }
            return dataTable;
        }
        #endregion 匯入資料整理
        #region 匯出資料整理
        /// <summary>
        /// 在翻譯字典中為 "Default" 項目添加 "(English)" 字樣。
        /// </summary>
        /// <param name="translateColName">欄位名稱翻譯字典，鍵為原始欄位名稱，值為翻譯後的欄位名稱。</param>
        /// <returns>更新後的欄位名稱翻譯字典。</returns>
        public static Dictionary<string, string> AppendEnglishToDefault(Dictionary<string, string> translateColName)
        {
            if (translateColName.ContainsKey("Default")) {
                var originalName = translateColName["Default"];
                translateColName["Default"] = $"{originalName}(English)";
            }
            return translateColName;
        }
        /// <summary>
        /// 使用排序的欄位名稱稱字典和翻譯字典，生成最終的欄位列表。
        /// </summary>
        /// <param name="sortColumnName">排序的欄位名稱字典，鍵為排序索引，值為欄位名稱。</param>
        /// <param name="translateColName">欄位名稱翻譯字典，鍵為原始欄位名稱，值為翻譯後的欄位名稱。</param>
        /// <returns>最終的欄位列表。</returns>
        public static List<string> SortedColumnName(SortedDictionary<int, string> sortColumnName
       , Dictionary<string, string> translateColName = null)
        {
            // 使用resourceList的顺序构建最终的欄位名稱列表
            List<string> ColumnNames = new List<string>();
            foreach (var entry in sortColumnName) {
                if (translateColName != null && translateColName.ContainsKey(entry.Value))
                    ColumnNames.Add(translateColName[entry.Value]);
                else
                    ColumnNames.Add(entry.Value);  // 如果沒有翻譯，加入原始名稱
            }

            return ColumnNames;
        }
        #endregion 匯出資料整理
    }
}
