using ClosedXML.Excel;
using ConvertCustomizeException;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using static Generic.StaticUtil.Models.DataModel;
using ExcelToolStandard.StaticUtil.Models;
using ExcelCustomizeException;

namespace ExcelToolStandard.StaticUtils
{
    /// <summary>
    /// 處理Excel內容
    /// </summary>
    public class ExcelContent
    {
        #region 公開函式
        /// <summary>
        /// 處理 Excel 工作表，將每一行轉換為 DataRow。
        /// </summary>
        /// <param name="workSheet">Excel 工作表。</param>
        /// <param name="importModel">匯入的模型</param>
        /// <param name="excelMapper">Excel內容驗證與名稱映射的設定。</param>
        /// <param name="skipFirstRow">跳過第一行標題行</param>
        public static void ProcessWorksheet(IXLWorksheet workSheet, ExcelMapperSetting excelMapper
            , ImportModel importModel, bool skipFirstRow)
        {
            //如果需要跳過第一行(標題行)
            var rows = workSheet.RowsUsed().Skip(skipFirstRow ? 1 : 0);
            foreach (var row in rows)
            {
                var dataRow = CreateDataRow(row, importModel, excelMapper,skipFirstRow);
                if (dataRow != null)
                    importModel.ResultTable.Rows.Add(dataRow);
            }
        }
        /// <summary>
        /// 創建一個模擬用的臨時Excel
        /// </summary>
        /// <param name="sheetName">工作頁名稱</param>
        /// <param name="tempExcel">Excel的內容</param>
        /// <returns>回傳臨時Excel路徑</returns>
        /// <remarks>如果沒提供內容則創建空白工作頁</remarks>
        public static string CreateTempExcelFile(string sheetName, TempExcel tempExcel = null)
        {
            // 創建一個臨時的Excel
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);
                if (tempExcel != null)
                {
                    if (tempExcel.FirstRowIsHeader)
                    {
                        AddHeaderRow(worksheet, tempExcel.Header);
                        AddContentRowsWithHeader(worksheet, tempExcel.Contents);
                    }
                    else
                    {
                        AddContentRowsWithoutHeader(worksheet, tempExcel.Contents);
                    }
                }
                workbook.SaveAs(filePath);
            }
            return filePath;
        }
        /// <summary>
        /// 配置儲存格的值和樣式。
        /// </summary>
        /// <param name="cell">需要配置的儲存格。</param>
        /// <param name="cellValue">設定給儲存格的值。</param>
        /// <param name="fontColor">儲存格文字顏色。(預設黑色)</param>
        /// <param name="backgroundColor">儲存格背景顏色。(預設灰色)</param>
        public static void SetCell(IXLCell cell, string cellValue, XLColor fontColor = null, XLColor backgroundColor = null)
        {
            cell.Value = cellValue;
            cell.Style.Fill.BackgroundColor = backgroundColor ?? XLColor.LightGray;
            cell.Style.Font.FontColor = fontColor ?? XLColor.Black;
        }


        /// <summary>
        /// 儲存工作簿到指定的文件路徑或流。
        /// </summary>
        /// <param name="workbook">要保存的工作簿。</param>
        /// <param name="filePath">文件路徑，如果需要保存到文件。</param>
        /// <param name="stream">數據流，如果需要保存到流。</param>
        public static void SaveWorkbook(XLWorkbook workbook, string filePath, Stream stream)
        {
            if (filePath is null && stream is null) {
                throw new MissingSaveTargetException();
            }
            if (filePath != null) {
                workbook.SaveAs(filePath);
            }
            if (stream != null) {
                workbook.SaveAs(stream);
                stream.Position = 0;
            }
        }
        #endregion 公開函式

        #region 資料處理
        /// <summary>
        /// 添加表頭行
        /// </summary>
        /// <param name="worksheet">工作頁</param>
        /// <param name="headers">表頭列表</param>
        private static void AddHeaderRow(IXLWorksheet worksheet, List<string> headers)
        {
            for (int i = 0; i < headers.Count; i++) {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }
        }

        /// <summary>
        /// 添加帶表頭的內容行
        /// </summary>
        /// <param name="worksheet">工作頁</param>
        /// <param name="contents">內容列表</param>
        private static void AddContentRowsWithHeader(IXLWorksheet worksheet, List<List<string>> contents)
        {
            int rowIndex = 2; // 從第二行開始
            foreach (var content in contents) {
                AddContentRow(worksheet, rowIndex, content);
                rowIndex++;
            }
        }

        /// <summary>
        /// 添加不帶表頭的內容行
        /// </summary>
        /// <param name="worksheet">工作頁</param>
        /// <param name="contents">內容列表</param>
        private static void AddContentRowsWithoutHeader(IXLWorksheet worksheet, List<List<string>> contents)
        {
            int rowIndex = 1; // 從第一行開始
            foreach (var content in contents) {
                AddContentRow(worksheet, rowIndex, content);
                rowIndex++;
            }
        }

        /// <summary>
        /// 添加單行內容
        /// </summary>
        /// <param name="worksheet">工作頁</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="content">內容</param>
        private static void AddContentRow(IXLWorksheet worksheet, int rowIndex, List<string> content)
        {
            for (int i = 0; i < content.Count; i++) {
                worksheet.Cell(rowIndex, i + 1).Value = content[i];
            }
        }
        /// <summary>
        /// 將 Excel 行數據新增到匯入模型。
        /// </summary>
        /// <param name="row">Excel 行。</param>
        /// <param name="importModel">要填充的模型。</param>
        /// <param name="excelMapper">欄位結構描述。</param>
        /// <param name="skipFirstRow">跳過第一行標題行</param>
        private static DataRow CreateDataRow(IXLRow row, ImportModel importModel
            , ExcelMapperSetting excelMapper, bool skipFirstRow)
        {
            var dataRow = importModel.ResultTable.NewRow();
            ExcelRowProcessingModel processingModel = new ExcelRowProcessingModel(importModel.ResultTable.Columns.Count);
            bool isValidRow = true;
            int excelColumnIndex = processingModel.FirstColumnIndex;
            // 用於緩存欄裡每一個單元格內容的字典
            Dictionary<int, List<string>> columnValuesCache = new Dictionary<int, List<string>>();
            foreach (IXLCell cell in row.Cells(processingModel.FirstColumnIndex, processingModel.LastColumnIndex)) {
                try {
                    var excelHeaderName = skipFirstRow
                    ? cell.WorksheetRow().Worksheet.FirstRowUsed().Cell(excelColumnIndex).GetString()
                    : cell.Address.ColumnLetter;

                    if (!excelMapper.ColumnMapping.TryGetValue(excelHeaderName, out string dtColumnName)) {
                        throw new Exception($"No database column mapping found for Excel column '{excelHeaderName}'.");
                    }

                    var schemaColumn = excelMapper.SchemaColumn.FirstOrDefault(sc => sc.ColumnName == dtColumnName)
                               ?? throw new Exception($"The corresponding Column schema {dtColumnName} cannot be found");

                    int dtColumnIndex =importModel.ResultTable.Columns.IndexOf(dtColumnName);

                    processingModel.CellValue = cell.Value.ToString();
                    // 檢查和更新欄緩存
                    if (!columnValuesCache.TryGetValue(excelColumnIndex, out List<string> columnList)) {
                        columnList = cell.WorksheetColumn().CellsUsed().Skip(skipFirstRow ? 1 : 0)
                            .Select(c => c.GetValue<string>()).ToList();
                        columnValuesCache[excelColumnIndex] = columnList;
                    }
                    dataRow[dtColumnIndex] = DataConverter.ConvertCellValue(cell, schemaColumn
                        , processingModel.CellValue, columnList);
                }
                catch (Exception ex) {
                    LogRowError(row, importModel, processingModel, ex);
                    isValidRow = false;
                    break; // 發生任何異常時停止處理當前行，並記錄錯誤
                }
                excelColumnIndex++;
            }

            return isValidRow ? dataRow : null;
        }
        #endregion 資料處理

        

        #region 處理錯誤
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
            int excelColumnIndex = processingModel.FirstColumnIndex;
            //從該行第一列到最後一列
            foreach (IXLCell cell in row.Cells(processingModel.FirstColumnIndex, processingModel.LastColumnIndex))
            {
                int dtColumnIndex = excelColumnIndex - 1; // Excel索引從1開始，DataTable的列索引從0開始，因此需要調整

                if (dtColumnIndex >= importModel.ResultTable.Columns.Count)
                    break;
                errorRow[dtColumnIndex] = cell.Value.ToString();
                excelColumnIndex++;
            }

            string errorMessage = $"Error processing value '{processingModel.CellValue}': {ex.Message}";
            errorRow["ErrorMessage"] = errorMessage;
            importModel.ErrorTable.Rows.Add(errorRow);
            importModel.IsErrorOccurred = true;
        }
        #endregion 處理錯誤
    }
}
