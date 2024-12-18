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
    /// <summary>
    /// 無效的日期格式異常
    /// </summary>
    public class InvalidDateTimeFormatException : Exception
    {
        /// <summary>
        /// 無效的日期格式異常
        /// </summary>
        public InvalidDateTimeFormatException()
            : base("Invalid DateTime format.")
        {
        }
    }
    /// <summary>
    /// 超過最大允許長度的異常
    /// </summary>
    public class ExceedsMaximumLengthException : Exception
    {
        /// <summary>
        /// 超過最大允許長度的異常
        /// </summary>
        public ExceedsMaximumLengthException()
            : base("The length of the cell value exceeds the maximum allowed length.")
        {
        }
    }
    /// <summary>
    /// 無法將數值轉化成布林異常
    /// </summary>
    public class UnableToConvertToBooleanException : Exception
    {
        /// <summary>
        /// 無法將數值轉換成布林異常
        /// </summary>
        public UnableToConvertToBooleanException()
            : base("Unable to convert value to True/False.")
        {
        }
    }
}


