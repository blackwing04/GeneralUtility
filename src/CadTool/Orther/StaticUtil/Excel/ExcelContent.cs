using ClosedXML.Excel;
using StaticUtil.Models.DAO;
using StaticUtil.Models.Excel;
using ConvertCustomizeException;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ExcelTool.StaticUtil
{
    /// <summary>
    /// 處理Excel內容
    /// </summary>
    public class ExcelContent
    {
        #region 匯入資料整理
        //需要寫單元測試去做測試
        /// <summary>
        /// 處理 Excel 工作表，將每一行轉換為 DataRow。
        /// </summary>
        /// <param name="workSheet">Excel 工作表。</param>
        /// <param name="importModel">匯入的模型</param>
        /// <param name="schemaColumns">欄位結構描述。</param>
        /// <param name="useParallel">是否使用並行處理</param>
        /// <param name="skipFirstRow">跳過第一行標題行(預設跳過)</param>
        public static void ProcessWorksheet(IXLWorksheet workSheet, List<SchemaColumnModel> schemaColumns
            , ImportModel importModel, bool useParallel, bool skipFirstRow = true)
        {
            //如果需要跳過第一行(標題行)
            var rows = workSheet.RowsUsed().Skip(skipFirstRow ? 1 : 0);
            if (useParallel) {
                var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                //使用並行處理來加快處理速度
                Parallel.ForEach(rows, options, row =>
                    AddRowToDataTable(row, importModel, schemaColumns)
                );
            }
            else {
                foreach (var row in rows)
                    AddRowToDataTable(row, importModel, schemaColumns);
            }
        }

        /// <summary>
        /// 將 Excel 行數據新增到匯入模型。
        /// </summary>
        /// <param name="row">Excel 行。</param>
        /// <param name="importModel">要填充的模型。</param>
        /// <param name="schemaColumns">欄位結構描述。</param>
        private static void AddRowToDataTable(IXLRow row, ImportModel importModel
            , List<SchemaColumnModel> schemaColumns)
        {
            DataRow newRow = importModel.ResultTable.NewRow();
            ExcelRowProcessingModel processingModel =new ExcelRowProcessingModel(row);
            bool isValidRow = true;
            foreach (IXLCell cell in row.Cells(processingModel.FirstColumnIndex, processingModel.LastColumnIndex)) {
                try {
                    var schemaColumn = schemaColumns[processingModel.ColumnIndex];
                    if (processingModel.ColumnIndex >= importModel.ResultTable.Columns.Count)
                        break;
                    processingModel.CellValue = cell.Value.ToString();
                    newRow[processingModel.ColumnIndex] =
                        ConvertCellValue(cell, schemaColumn, processingModel.CellValue, importModel.ResultTable);
                }
                catch (Exception ex) {
                    LogRowError(row, importModel, processingModel, ex);
                    isValidRow = false;
                    break;
                }
                processingModel.ColumnIndex++;
            }
            //鎖定ResultTable限制同一時間只能一個線程操作，只對並行有影響。
            if (isValidRow)
                lock (importModel.ResultTable)
                    importModel.ResultTable.Rows.Add(newRow);
        }
        /// <summary>
        /// 處理發生錯誤的行
        /// </summary>
        /// <param name="row">錯誤行</param>
        /// <param name="importModel">要填充的模型</param>
        /// <param name="processingModel">發生錯誤的單元格值</param>
        /// <param name="ex">發生的具體錯誤</param>
        private static void LogRowError(IXLRow row, ImportModel importModel
            , ExcelRowProcessingModel processingModel, Exception ex)
        {
            DataRow errorRow = importModel.ErrorTable.NewRow();
            processingModel.ColumnIndex = 0; //重置索引
            //從該行第一列到最後一列
            foreach (IXLCell cell in row.Cells(processingModel.FirstColumnIndex, processingModel.LastColumnIndex)) {
                if (processingModel.ColumnIndex >= importModel.ResultTable.Columns.Count)
                    break;
                errorRow[processingModel.ColumnIndex] = cell.Value.ToString();
                processingModel.ColumnIndex++;
            }

            string errorMessage = $"Error processing value '{processingModel.CellValue}': {ex.Message}";
            errorRow["ErrorMessage"] = errorMessage;
            importModel.ErrorTable.Rows.Add(errorRow);
            importModel.IsErrorOccurred = true;
        }
        /// <summary>
        /// 轉換單元格的值至目標數據類型，並處理潛在的錯誤。
        /// </summary>
        /// <param name="cell">Excel 單元格。</param>
        /// <param name="schemaColumn">欄位結構描述。</param>
        /// <param name="cellValue">單元格內容。</param>
        /// <param name="resultTable">包含原來資料的資料表，用於檢查唯一性</param>
        /// <returns>轉換後的值。</returns>
        private static object ConvertCellValue(IXLCell cell, SchemaColumnModel schemaColumn
            , string cellValue, DataTable resultTable)
        {
            Type targetType = schemaColumn.DataType;
            try {
                string defaultValue = CheckedDefaultValueToDataType(targetType,schemaColumn.DefaultValue);
                bool hasDuplicate =resultTable.AsEnumerable()
                                         .Any(r => r[schemaColumn.ColumnName].ToString() == cellValue);
                //檢查重複
                if (schemaColumn.Unique && hasDuplicate)
                    throw new DuplicateDataException(schemaColumn.ColumnName, cellValue);
                //檢查空值
                if (!schemaColumn.AllowNulls && string.IsNullOrWhiteSpace(cellValue))
                    cellValue = defaultValue ?? throw new CellBlankErrorException(schemaColumn.ColumnName);
                //如果目標是時間型態，
                if (targetType == typeof(DateTime) && cell.DataType == XLDataType.Number)
                    return ConvertToDateTime(cellValue);

                return Convert.ChangeType(cellValue, targetType);
            }
            catch { throw; }
        }

        /// <summary>
        /// 檢查預設值是否能轉換為指定的類型。
        /// 轉換成功則返回預設字串，轉換失敗則返回 null。
        /// </summary>
        /// <param name="targetType">要轉換到的目標類型。</param>
        /// <param name="defaultValue">要檢查的字串預設值。</param>
        /// <returns>轉換後的字串，或在無法轉換時返回 null。</returns>
        private static string CheckedDefaultValueToDataType(Type targetType, string defaultValue)
        {
            // 如果預設值為空或只包含空白字符，則無需進行轉換
            if (string.IsNullOrWhiteSpace(defaultValue)) {
                return null;
            }

            try {
                // 獲取非可空的基底類型，以處理可空類型
                var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // 嘗試將字串預設值轉換為指定的類型
                var convertedValue = Convert.ChangeType(defaultValue, nonNullableType);

                // 如果轉換成功，返回轉換後的字串值
                return defaultValue;
            }
            catch {
                // 如果轉換過程中出現任何異常，則返回 null
                return null;
            }
        }
        /// <summary>
        /// 將值轉成時間型態
        /// </summary>
        /// <param name="cellValue">欄位值</param>
        /// <returns>返回轉換好的時間型態</returns>
        private static DateTime ConvertToDateTime(string cellValue)
        {
            string errorMessage;
            if (double.TryParse(cellValue, out double numericValue)) {
                try {
                    return DateTime.FromOADate(numericValue);
                }
                catch (ArgumentException ex) {
                    errorMessage = $"Convert {cellValue} To DateTime failed,Error: {ex.Message}";
                    throw new ArgumentException(errorMessage, ex);
                }
            }
            errorMessage = $"Convert {cellValue} To DateTime failed";
            throw new ArgumentException(errorMessage);
        }
        #endregion 匯入資料整理
        #region 匯出資料整理
        #endregion 匯出資料整理
    }
}
