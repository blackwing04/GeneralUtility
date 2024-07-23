using System;
using System.Collections.Generic;

namespace ExcelCustomizeException
{
    /// <summary>
    /// 開啟Excel檔案時遇到的問題
    /// </summary>
    public class ExcelFileLoadException : Exception
    {
        public string FilePath { get; private set; }
        /// <summary>
        /// 處理開啟 Excel 檔案時遇到的問題，例如文件不存在或損壞。
        /// </summary>
        /// <param name="filePath">無法加載的 Excel 文件的路徑。</param>
        /// <param name="innerException">內部原生例外</param>
        public ExcelFileLoadException(string filePath, Exception innerException)
            : base($"Cannot Open {filePath}.Error:{innerException.Message}")
        {
            FilePath = filePath;
        }
    }
    /// <summary>
    /// 取得Excel的表頭名稱時遇到的問題
    /// </summary>
    public class GetExcelHeaderException : Exception
    {
        /// <summary>
        /// 取得Excel的表頭名稱時遇到的問題
        /// </summary>
        /// <param name="innerException">內部原生例外</param>
        public GetExcelHeaderException(Exception innerException)
            : base($"Get Excel Header Failed.Error:{innerException.Message}", innerException)
        {
        }
    }
    /// <summary>
    /// Excel轉換成ImportModel時遇到的問題
    /// </summary>
    public class ExcelConvertToImportModelException : Exception
    {
        /// <summary>
        /// Excel轉換成ImportModel時遇到的問題
        /// </summary>
        /// <param name="innerException">內部原生例外</param>
        public ExcelConvertToImportModelException(Exception innerException)
            : base($"Excel convert to import model Failed.Error:{innerException.Message}", innerException)
        {
        }
    }
    /// <summary>
    /// 發現有重複的表頭名稱的錯誤
    /// </summary>
    public class DuplicateHeaderNameException : Exception
    {
        public IEnumerable<string> HeaderNames { get; private set; }
        /// <summary>
        /// 發現有重複的表頭名稱的錯誤。
        /// </summary>
        /// <param name="headerNames">欄位名稱列表</param>
        public DuplicateHeaderNameException(IEnumerable<string> headerNames)
            : base($"The column names are duplicated: { string.Join(", ", headerNames)}")
        {
            HeaderNames = headerNames;
        }
    }

    /// <summary>
    /// DataTable轉換成Excel時發生的錯誤
    /// </summary>
    public class DataTableConvertExcelException : Exception
    {
        /// <summary>
        ///  DataTable轉換成Excel時發生的錯誤
        /// </summary>
        /// <param name="rowNumber">行號</param>
        /// <param name="columnNumber">欄號</param>
        /// <param name="innerException">內部原生例外</param>
        public DataTableConvertExcelException(int rowNumber, int columnNumber, Exception innerException)
       : base($"Error converting DataTable to Excel at row {rowNumber}," +
             $" column {columnNumber}. Error: {innerException.Message}", innerException)
        {
        }
    }

    /// <summary>
    /// List轉換成Excel時發生的錯誤
    /// </summary>
    public class ListConvertExcelException : Exception
    {
        /// <summary>
        /// 列表轉換成Excel時發生的錯誤
        /// </summary>
        /// <param name="entryIndex">條目索引</param>
        /// <param name="subEntryIndex">子條目索引</param>
        /// <param name="innerException">內部原生例外</param>
        public ListConvertExcelException(int entryIndex, int subEntryIndex, Exception innerException)
           : base($"Error converting list to Excel at entry {entryIndex}," +
                 $" sub-entry {subEntryIndex}. Error: {innerException.Message}", innerException)
        {
        }
    }

    /// <summary>
    /// DataTable轉換成Excel時無資料所發生的錯誤
    /// </summary>
    public class DataTableIsEmptyException : Exception
    {
        /// <summary>
        ///  DataTable轉換成Excel時無資料所發生的錯誤
        /// </summary>
        public DataTableIsEmptyException()
       : base("The DataTable is empty. Please ensure the DataTable contains data before converting to Excel.")
        {
        }
    }


    /// <summary>
    /// List轉換成Excel時無資料所發生的錯誤
    /// </summary>
    public class ListIsEmptyException : Exception
    {
        /// <summary>
        ///  List轉換成Excel時無資料所發生的錯誤
        /// </summary>
        public ListIsEmptyException()
       : base("The List is empty. Please ensure the List contains data before converting to Excel.")
        {
        }
    }

    /// <summary>
    /// 缺少儲存目標所發生的錯誤
    /// </summary>
    public class MissingSaveTargetException : Exception
    {
        /// <summary>
        ///  缺少儲存目標所發生的錯誤
        /// </summary>
        public MissingSaveTargetException()
       : base("No valid save target specified. Please provide a file path or a memory stream.")
        {
        }
    }
}
