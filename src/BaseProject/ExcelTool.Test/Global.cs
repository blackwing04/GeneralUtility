global using Xunit;
global using Generic.StaticUtil;
global using ExcelCustomizeException;
global using static Generic.StaticUtil.Models.DataModel;
global using System.Data;
global using CustomizeException;
global using ExcelTool.Services;
global using ExcelTool.StaticUtil.Models;
global using ExcelTool.StaticUtil;
using ClosedXML.Excel;


namespace ExcelTool.Test
{
    public class GlobalUtil
    {
        public static readonly IExcelManager ExcelManager = ExcelManagerFactory.CreateExcelManager();
        public const string sheetName= "TestSheetName";


        /// <summary>
        /// 獲取欄位結構。
        /// </summary>
        /// <returns>返回欄位結構列表。</returns>
        internal static List<SchemaColumnModel> GetSchemaColumns()
        {
            return new List<SchemaColumnModel>
            {
                new SchemaColumnModel { ColumnName = "StringCol", DataType = typeof(string), Unique = true },
                new SchemaColumnModel { ColumnName = "IntCol", DataType = typeof(int), AllowNulls = false },
                new SchemaColumnModel { ColumnName = "DoubleCol", DataType = typeof(double),
                    DefaultValue = "0.01", AllowNulls = false },
                new SchemaColumnModel { ColumnName = "DateTimeCol", DataType = typeof(DateTime) }
            };
        }
        /// <summary>
        /// 獲取欄位映射。
        /// </summary>
        /// <returns>返回欄位映射字典。</returns>
        internal static Dictionary<string, string> GetColumnMapping()
        {
            return new Dictionary<string, string>
            {
                { "TestStringAndUnique", "StringCol" },
                { "TestIntAndNotAllowNulls", "IntCol" },
                { "TestDouble", "DoubleCol" },
                { "TestDateTime", "DateTimeCol" }
            };
        }

        /// <summary>
        /// 獲取全部內容。
        /// </summary>
        /// <returns>返回內容列表。</returns>
        internal static List<List<string>> GetContents()
        {
            return new List<List<string>>
            {
                new List<string> { "TestStringContent1", "123", "45.67", "2024-07-11" },
                new List<string> { "TestStringContent2", "123", "0.01", "" }
            };
        }

        /// <summary>
        /// 將內容轉換為DataTable。
        /// </summary>
        /// <param name="schemaColumns">模式列的列表，用於定義每列的數據結構。</param>
        /// <param name="contents">每行的數據。</param>
        /// <returns>生成的DataTable。</returns>
        internal static DataTable CreateDataTable(List<SchemaColumnModel> schemaColumns, List<List<string>> contents)
        {
            DataTable dataTable = new();

            // 添加列到DataTable
            foreach (var schema in schemaColumns) {
                dataTable.Columns.Add(schema.ColumnName, schema.DataType);
            }

            // 添加行到DataTable
            foreach (var content in contents) {
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < content.Count; i++) {
                    if (string.IsNullOrEmpty(content[i])) {
                        // 如果內容為空，設置為 DBNull
                        row[i] = DBNull.Value;
                    }
                    else {
                        row[i] = Convert.ChangeType(content[i], dataTable.Columns[i].DataType);
                    }
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// 比對 Excel 文件和 ListConvertExcelModel 的內容。
        /// </summary>
        /// <param name="model">列表轉換成 Excel 的模型</param>
        /// <param name="filePath">Excel 文件的路徑</param>
        /// <returns>如果內容一致，返回 True，否則返回 False。</returns>
        public static bool CompareExcelWithListModel(ListConvertExcelModel model, string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(model.SheetName);

            // 比對表頭，如果有的話
            if (model.Header != null && model.Header.Count > 0) {
                for (int colIndex = 0; colIndex < model.Header.Count; colIndex++) {
                    string expectedHeader = model.Header[colIndex];
                    string actualHeader = worksheet.Cell(1, colIndex + 1).Value.ToString();
                    if (expectedHeader != actualHeader) {
                        return false;
                    }
                }
            }

            // 比對內容
            for (int rowIndex = 0; rowIndex < model.ContentList.Count; rowIndex++) {
                for (int colIndex = 0; colIndex < model.ContentList[rowIndex].Count; colIndex++) {
                    string expectedValue = model.ContentList[rowIndex][colIndex];
                    // 索引偏移量+2因為Excel從1開始且第一行是表頭
                    string actualValue = worksheet.Cell(rowIndex + 2, colIndex + 1).Value.ToString();
                    if (expectedValue != actualValue) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}