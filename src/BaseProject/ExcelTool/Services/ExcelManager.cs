using ClosedXML.Excel;
using System.Data;
using ExcelTool.StaticUtil.Models;
using ExcelCustomizeException;
using CustomizeException;
using ExcelTool.StaticUtil;


namespace ExcelTool.Services
{
    public class ExcelManager : IExcelManager
    {
        public List<string> GetExcelHeaderName(ExcelInfo excelInfo, MemoryStream? stream = null)
        {
            try {
                //單元測試
                if (excelInfo.MockException) throw new MockTestException();
                XLWorkbook workBook;
                if (stream != null)
                    workBook = new(stream);
                else
                    workBook = new(excelInfo.ExcelFilePath);

                using (workBook) {
                    IXLWorksheet workSheet;
                    if (string.IsNullOrWhiteSpace(excelInfo.WorkSheetName))
                        workSheet = workBook.Worksheet(1);
                    else
                        workSheet = workBook.Worksheet(excelInfo.WorkSheetName);
                    return ExcelHeader.GetFirstRowData(workSheet, excelInfo.FirstRowIsHeader);
                }
            }
            catch (FileNotFoundException ex) {
                throw new ExcelFileLoadException(excelInfo.ExcelFilePath, ex);
            }
            catch (IOException ex) {
                throw new ExcelFileLoadException(excelInfo.ExcelFilePath, ex);
            }
            catch (DuplicateHeaderNameException) {
                throw;
            }
            catch (Exception ex) {
                throw new GetExcelHeaderException(ex);
            }
        }

        public async Task<ImportModel> ExcelConvertToImportModelAsync(ExcelInfo excelInfo, MemoryStream? stream = null)
        {
            //取得資料庫欄位結構
            ImportModel importModel = ExcelHeader.InitializeDataTable(excelInfo.ExcelMapper);
            try {
                //單元測試
                if (excelInfo.MockException) throw new MockTestException();
                await Task.Run(() => {
                    XLWorkbook workBook;
                    if (stream != null)
                        workBook = new(stream);
                    else
                        workBook = new(excelInfo.ExcelFilePath);

                    using (workBook) {
                        IXLWorksheet workSheet;
                        if (string.IsNullOrWhiteSpace(excelInfo.WorkSheetName))
                            workSheet = workBook.Worksheet(1);
                        else
                            workSheet = workBook.Worksheet(excelInfo.WorkSheetName);
                        ExcelContent.ProcessWorksheet(workSheet, excelInfo.ExcelMapper, importModel, excelInfo.FirstRowIsHeader);
                    }
                });

                return importModel;
            }
            catch (FileNotFoundException ex) {
                throw new ExcelFileLoadException(excelInfo.ExcelFilePath, ex);
            }
            catch (IOException ex) {
                throw new ExcelFileLoadException(excelInfo.ExcelFilePath, ex);
            }
            catch (Exception ex) {
                throw new ExcelConvertToImportModelException(ex);
            }
        }

        public async Task DataTableConvertToExcelAsync(DataTable sourceData, string? filePath = null, MemoryStream? stream = null)
        {
            int rowIndex = 0;
            int colIndex = 0;
            try {
                if (sourceData is null || sourceData.Rows.Count < 1) throw new DataTableIsEmptyException();

                using var workbook = new XLWorkbook();
                await Task.Run(() => {
                    var worksheet = ExcelHeader.InitializeWorksheet(workbook, sourceData.TableName);
                    // 寫入表頭，Excel是從1開始計算非0，所以索引要+1
                    string? cellValue;
                    for (colIndex = 0; colIndex < sourceData.Columns.Count; colIndex++) {
                        var cell = worksheet.Cell(1, colIndex + 1);
                        cellValue = sourceData.Columns[colIndex].ColumnName;
                        XLColor fontColor = sourceData.Columns[colIndex].AllowDBNull ? XLColor.Black : XLColor.Blue;
                        ExcelContent.SetCell(cell, cellValue, fontColor);
                    }
                    // 寫入資料，Excel是從1開始計算非0，所以索引要+1而行要+2，因為第一行是表頭
                    for (rowIndex = 0; rowIndex < sourceData.Rows.Count; rowIndex++) {
                        for (colIndex = 0; colIndex < sourceData.Columns.Count; colIndex++) {
                            cellValue = sourceData.Rows[rowIndex][colIndex].ToString();
                            worksheet.Cell(rowIndex + 2, colIndex + 1).Value = cellValue;
                        }
                    }
                    ExcelContent.SaveWorkbook(workbook, filePath, stream);
                });
            }
            catch (DataTableIsEmptyException) {
                throw;
            }
            catch (MissingSaveTargetException) {
                throw;
            }
            catch (Exception ex) {
                throw new DataTableConvertExcelException(rowIndex, colIndex, ex);
            }
        }
        public async Task ListConvertToExcelAsync(ListConvertExcelModel sourceData, string? filePath = null
            , ExcelMapperSetting? excelMapper = null, MemoryStream? stream = null)
        {
            //條目索引
            int entryIndex = 0;
            //子條目索引
            int subEntryIndex = 0;
            try {
                if (sourceData.ContentList is null || sourceData.ContentList.Count < 1) throw new ListIsEmptyException();
                await Task.Run(() => {
                    using var workbook = new XLWorkbook();
                    var worksheet = ExcelHeader.InitializeWorksheet(workbook, sourceData.SheetName);
                    // 寫入表頭，如果存在
                    int headerOffset = 0;
                    string cellValue;
                    if (sourceData.Header != null && sourceData.Header.Count > 0) {
                        for (subEntryIndex = 0; subEntryIndex < sourceData.Header.Count; subEntryIndex++) {
                            // 寫入表頭，Excel是從1開始計算非0，所以索引要+1
                            var cell = worksheet.Cell(1, subEntryIndex + 1);
                            cellValue = sourceData.Header[subEntryIndex];
                            // 檢查映射到的欄位是否不允許空值，並設置為必填欄位的字體顏色
                            bool isRequiredColumn = ExcelHeader.IsRequiredColumn(sourceData.Header[subEntryIndex], excelMapper);
                            XLColor fontColor = isRequiredColumn ? XLColor.Blue : XLColor.Black;
                            ExcelContent.SetCell(cell, cellValue, fontColor);
                        }
                        headerOffset = 1;  // 如果有表頭，數據從第二行開始寫
                    }

                    // 寫入數據
                    for (entryIndex = 0; entryIndex < sourceData.ContentList.Count; entryIndex++) {
                        for (subEntryIndex = 0; subEntryIndex < sourceData.ContentList[entryIndex].Count; subEntryIndex++) {
                            cellValue = sourceData.ContentList[entryIndex][subEntryIndex];
                            worksheet.Cell(entryIndex + 1 + headerOffset, subEntryIndex + 1).Value = cellValue;
                        }
                    }
                    ExcelContent.SaveWorkbook(workbook, filePath, stream);
                });
            }
            catch (ListIsEmptyException) {
                throw;
            }
            catch (MissingSaveTargetException) {
                throw;
            }
            catch (Exception ex) {
                throw new ListConvertExcelException(entryIndex, subEntryIndex, ex);
            }
        }
    }
}