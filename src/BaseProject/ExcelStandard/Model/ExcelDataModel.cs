using static Generic.StaticUtil.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ExcelToolStandard.StaticUtil.Models
{
    #region 轉換匯入模型
    public class ImportModel
    {
        /// <summary>
        /// 轉換結果的資料表
        /// </summary>
        public DataTable ResultTable { get; set; } = new DataTable();
        /// <summary>
        /// 轉換失敗的暫存資料表
        /// </summary>
        public DataTable ErrorTable { get; set; } = new DataTable();
        /// <summary>
        /// 是否轉換過程有錯誤發生，如果有錯誤詳細看ErrorTable內容
        /// </summary>
        public bool IsErrorOccurred { get; set; } = false;
    }
    /// <summary>
    /// Excel處理行資料使用的模型
    /// </summary>
    public class ExcelRowProcessingModel
    {
        public string CellValue { get; set; } = string.Empty;
        public int FirstColumnIndex { get; set; } = 1;
        public int LastColumnIndex { get; set; }

        public ExcelRowProcessingModel(int lastIndex)
        {
            LastColumnIndex = lastIndex;
        }

    }
    #endregion 轉換匯入模型
    #region 通用
    /// <summary>
    /// Excel通用模組，用於封裝Excel資訊
    /// </summary>
    public class ExcelInfo
    {
        /// <summary>
        /// Excel檔案實體位置
        /// </summary>
        public string ExcelFilePath { get; set; }
        /// <summary>
        /// 工作頁名稱(如果沒指定預設打開第一頁)
        /// </summary>
        public string WorkSheetName { get; set; } = string.Empty;
        /// <summary>
        /// 第一行是否為表頭(預設True)
        /// </summary>
        public bool FirstRowIsHeader { get; set; } = true;
        /// <summary>
        /// 用於對Excel資料進行欄位的內容驗證與名稱映射的設定。
        /// </summary>
        public ExcelMapperSetting ExcelMapper { get; set; } = new ExcelMapperSetting();
        /// <summary>
        /// 用於觸發單完測試模擬環境丟出例外，正常使用時請忽略這個欄位
        /// </summary>
        public bool MockException { get; set; }
    }

    /// <summary>
    /// ExcelToDatabaseMapper 用於驗證和映射 Excel 資料至資料庫模型。
    /// </summary>
    /// <remarks>主要用來確保 Excel 欄位符合資料庫結構並進行適當的欄位名稱映射。</remarks>
    public class ExcelMapperSetting
    {
        /// <summary>
        /// 資料庫欄位結構，驗證 Excel 資料是否符合規範。
        /// </summary>
        public List<SchemaColumnModel> SchemaColumn { get; set; } = new List<SchemaColumnModel>();

        /// <summary>
        /// 欄位映射字典，對應 Excel 表頭至資料庫欄位名。
        /// </summary>
        /// <remarks>Key是對應的是Excel的表頭，Value是對應資料表欄位名稱</remarks>
        public Dictionary<string, string> ColumnMapping { get; set; } = new Dictionary<string, string>();
    }
    /// <summary>
    /// 列表轉換成Excel使用的模型
    /// </summary>
    public class ListConvertExcelModel
    {
        /// <summary>
        /// 工作表名稱(預設Sheet1)
        /// </summary>
        public string SheetName { get; set; } = "Sheet1";
        /// <summary>
        /// 表頭
        /// </summary>
        public List<string> Header { get; }
        /// <summary>
        /// 內容列表
        /// </summary>
        public List<List<string>> ContentList { get; }

        /// <summary>
        /// 初始化 ListConvertExcelModel 並檢查 Header 和 Content 的一致性
        /// </summary>
        /// <param name="content">內容</param>
        /// <param name="header">表頭(可空)</param>
        /// <exception cref="ArgumentException">如果 Header 和 Content 的列數不一致，拋出異常</exception>
        public ListConvertExcelModel(List<List<string>> content, List<string> header = null)
        {
            if (header != null && content.Any(row => row.Count != header.Count))
            {
                throw new ArgumentException("All rows in content must have the same number of columns as the header.");
            }

            Header = header;
            ContentList = content;
        }
    }
    #endregion 通用
    #region 模擬單元測試
    public class TempExcel
    {
        /// <summary>
        /// 表頭
        /// </summary>
        public List<string> Header { get; set; } = new List<string>();
        /// <summary>
        /// 內容
        /// </summary>
        public List<List<string>> Contents { get; set; } = new List<List<string>>();
        /// <summary>
        /// 第一行為表頭(預設True)
        /// </summary>
        public bool FirstRowIsHeader { get; set; } = true;
    }
    #endregion 模擬單元測試
}
