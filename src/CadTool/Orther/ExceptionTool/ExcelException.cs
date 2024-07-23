using System;

namespace ExcelCustomizeException
{
    /// <summary>
    /// 開啟 Excel 檔案時遇到的問題
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
            : base($"Cannot Open {filePath},{innerException.Message}")
        {
            FilePath = filePath;
        }
    }
}
