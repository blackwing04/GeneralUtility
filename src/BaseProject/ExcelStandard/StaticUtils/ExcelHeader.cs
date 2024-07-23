using System.Data;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Linq;
using System;
using ExcelCustomizeException;
using Generic.StaticUtil;
using ExcelToolStandard.StaticUtil.Models;

namespace ExcelToolStandard.StaticUtil
{
    public static class ExcelHeader
    {
        #region 初始化方法
        /// <summary>
        /// 初始化並配置工作表。
        /// </summary>
        /// <param name="workbook">工作簿實例，用於添加工作表。</param>
        /// <param name="sheetName">工作表名稱。(預設Sheet1)</param>
        /// <returns>配置好的工作表。</returns>
        public static IXLWorksheet InitializeWorksheet(IXLWorkbook workbook, string sheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
                sheetName = "Sheet1";
            var worksheet = workbook.Worksheets.Add(sheetName);
            worksheet.Style.Font.FontName = "Microsoft YaHei";  // 統一字體設置
            return worksheet;
        }
        /// <summary>
        /// 初始化 DataTable 結構，根據名稱映射來設置 DataTable 的欄位名稱。
        /// </summary>
        /// <param name="excelMapper">Excel內容驗證與名稱映射的設定。</param>
        /// <returns>初始化後的結果表與錯誤表，其欄位名稱與映射對應。</returns>
        public static ImportModel InitializeDataTable(ExcelMapperSetting excelMapper)
        {
            DataTable resultTable = new DataTable();
            DataTable errorTable = new DataTable();
            // 檢查每個 SchemaColumn 是否有對應的映射名稱，若有則使用映射名稱作為 DataTable 的列名
            foreach (var column in excelMapper.SchemaColumn) {
                string columnName = column.ColumnName;
                // 如果映射字典中包含這個列名，則使用映射後的名稱
                if (excelMapper.ColumnMapping.ContainsKey(columnName))
                    columnName = excelMapper.ColumnMapping[columnName];

                resultTable.Columns.Add(columnName, column.DataType);
                //錯誤表使用一般字串類別
                errorTable.Columns.Add(columnName, typeof(string));
            }
            //錯誤表最後加入錯誤訊息欄位
            errorTable.Columns.Add("ErrorMessage", typeof(string));
            return new ImportModel() { ResultTable = resultTable, ErrorTable = errorTable };
        }
        #endregion 初始化方法

        #region 取得Excel表頭
        /// <summary>
        /// 從指定的 IXLWorksheet 中獲取第一行使用中的單元格並轉為字串列表
        /// </summary>
        /// <param name="workSheet">要處理的 IXLWorksheet</param>
        /// <param name="firstRowIsHeader">第一行為標題行</param>
        /// <returns>第一行使用中的單元格內容列表，或者 Excel 預設的列標題</returns>
        public static List<string> GetFirstRowData(IXLWorksheet workSheet, bool firstRowIsHeader)
        {
            // 創建一個空的列表來存儲行資料
            List<string> rowData = new List<string>();

            // 獲取第一行使用中的單元格
            var firstRowCells = workSheet.RowsUsed().First().CellsUsed();

            if (firstRowIsHeader)
            {
                // 將每個單元格的值轉換為字串並添加到列表中
                foreach (var cell in firstRowCells)
                {
                    rowData.Add(cell.GetString());
                }
            }
            else
            {
                // 計算並添加 Excel 預設的列標題
                foreach (var cell in firstRowCells)
                {
                    rowData.Add(GetExcelColumnName(cell.Address.ColumnNumber));
                }
            }
            //檢查表頭名稱是不是有重複，如果重複了就丟出具體例外
            var duplicates = ListStringHelper.GetDuplicateItems(rowData);
            if (duplicates.Count > 0)
                throw new DuplicateHeaderNameException(duplicates);

            // 返回包含第一行資料的列表
            return rowData;
        }
        /// <summary>
        /// 根據欄位編號計算 Excel 的欄位標題 (例如：1 -> A, 2 -> B, ..., 27 -> AA)
        /// </summary>
        /// <param name="columnNumber">欄位編號</param>
        /// <returns>對應的 Excel欄位標題</returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
        #endregion 取得Excel表頭

        #region 檢查方法
        /// <summary>
        /// 檢查指定的表頭是否映射到一個不允許空值的欄位。
        /// </summary>
        /// <param name="header">表頭名稱</param>
        /// <param name="excelMapper">Excel映射設定</param>
        /// <returns>如果對應的欄位不允許空值，返回 true。</returns>
        public static bool IsRequiredColumn(string header, ExcelMapperSetting excelMapper)
        {
            if (excelMapper?.ColumnMapping.TryGetValue(header, out string mappingHeader) == true) {
                return excelMapper.SchemaColumn.Any(sc => sc.ColumnName == mappingHeader && !sc.AllowNulls);
            }
            return false;
        }
        #endregion 檢查方法
    }
}
