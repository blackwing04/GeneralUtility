using ClosedXML.Excel;
using System.Data;

namespace Generic.StaticUtil
{
    public static class DataTableHelper
    {
        /// <summary>
        /// 比較兩個DataTable是否相等
        /// </summary>
        /// <param name="expected">期望的DataTable</param>
        /// <param name="actual">實際的DataTable</param>
        /// <returns>如果兩個DataTable相等則返回True，否則返回False</returns>
        public static bool IsDataTablesEqual(DataTable expected, DataTable actual)
        {
            // 比較Column數量
            if (expected.Columns.Count != actual.Columns.Count)
                return false;

            // 比較Row數量
            if (expected.Rows.Count != actual.Rows.Count)
                return false;

            // 比較每個Column的名稱和資料類型
            for (int col = 0; col < expected.Columns.Count; col++) {
                if (expected.Columns[col].ColumnName != actual.Columns[col].ColumnName)
                    return false;
                if (expected.Columns[col].DataType != actual.Columns[col].DataType)
                    return false;
            }

            // 比較每個Row的內容
            for (int row = 0; row < expected.Rows.Count; row++) {
                for (int col = 0; col < expected.Columns.Count; col++) {
                    if (!expected.Rows[row][col].Equals(actual.Rows[row][col]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 比對 Excel 文件和 DataTable 的內容。
        /// </summary>
        /// <param name="sourceData">原始 DataTable</param>
        /// <param name="filePath">Excel 文件的路徑</param>
        /// <returns>如果內容一致，返回 True，否則返回 False。</returns>
        public static bool CompareExcelWithDataTable(DataTable sourceData, string filePath)
        {
            using (var workbook = new XLWorkbook(filePath)) {
                var worksheet = workbook.Worksheet(1);

                // 比對表頭
                for (int colIndex = 0; colIndex < sourceData.Columns.Count; colIndex++) {
                    string expectedHeader = sourceData.Columns[colIndex].ColumnName;
                    string actualHeader = worksheet.Cell(1, colIndex + 1).Value.ToString();
                    if (expectedHeader != actualHeader) {
                        return false;
                    }
                }

                // 比對內容
                for (int rowIndex = 0; rowIndex < sourceData.Rows.Count; rowIndex++) {
                    for (int colIndex = 0; colIndex < sourceData.Columns.Count; colIndex++) {
                        string expectedValue = sourceData.Rows[rowIndex][colIndex].ToString();
                        string actualValue = worksheet.Cell(rowIndex + 2, colIndex + 1).Value.ToString();
                        if (expectedValue != actualValue) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
