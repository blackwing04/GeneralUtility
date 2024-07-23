using System;

namespace ConvertCustomizeException
{
    /// <summary>
    /// 單位格內容不得為空
    /// </summary>
    public class CellBlankErrorException : Exception
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// 欄位內容不允許為空導致錯誤而使用的異常
        /// </summary>
        /// <param name="columnName">欄位名</param>
        public CellBlankErrorException(string columnName)
        : base($"The content of the column '{columnName}' cannot be blank.")
        {
            ColumnName = columnName;
        }
    }
    /// <summary>
    /// 該資料欄位內容不允許重複資料
    /// </summary>
    public class DuplicateDataException : Exception
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// 單元格內容
        /// </summary>
        public string CellValue { get; private set; }
        /// <summary>
        /// 欄位內容不允許重複資料導致錯誤而使用的異常
        /// </summary>
        /// <param name="columnName">欄位名</param>
        /// <param name="cellValue">不允許重複的單元格內容</param>
        public DuplicateDataException(string columnName, string cellValue)
             : base($"Duplicate data found in column '{columnName}', cell value '{cellValue}' " +
                   $"is not allowed to be duplicated.")
        {
            ColumnName = columnName;
            CellValue = cellValue;
        }
    }
  
}


