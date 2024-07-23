using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;

namespace StaticUtil.Models.Excel
{
    #region 匯入
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
        /// <summary>
        /// 匯入資料庫是否成功
        /// </summary>
        public bool IsDbImportSuccess { get; set; } = false;
    }
    /// <summary>
    /// Excel處理行資料使用的模型
    /// </summary>
    public class ExcelRowProcessingModel
    {
        public int ColumnIndex { get; set; } = 0;
        public string CellValue { get; set; } = string.Empty;
        public int FirstColumnIndex { get; set; }
        public int LastColumnIndex { get; set; }

        public ExcelRowProcessingModel(IXLRow row)
        {
            FirstColumnIndex = row.FirstCellUsed()?.Address.ColumnNumber ?? 0;
            LastColumnIndex = row.LastCellUsed()?.Address.ColumnNumber ?? 0;
        }

    }
    #endregion 匯入
    #region 匯出
    /// <summary>
    /// Excel資料通用的模型，對應表頭跟內容
    /// </summary>
    public class ExcelDataModle
    {
        /// <summary>
        /// 表頭
        /// </summary>
        public List<ExcelHead> Head { get; set; } = new List<ExcelHead>();
        /// <summary>
        /// 內容
        /// </summary>
        public List<string[]> Content { get; set; } = new List<string[]>();
        /// <summary>
        /// 工作簿
        /// </summary>
        public XLWorkbook ExcelWorkbook { get; set; } = new XLWorkbook();
    }
    /// <summary>
    /// Excel資料表頭模型 包含型態
    /// </summary>
    public class ExcelHead
    {

        public string HeadName { get; set; } = string.Empty;
        public Type ColumnDataType { get; set; } = typeof(string);
    }
    /// <summary>
    /// 原始資料模型，用於封裝準備處理的資料
    /// </summary>
    public class RawData
    {
        public Dictionary<int, string> ColumnName { get; set; } = new Dictionary<int, string>();

        public Type ColumnOfType { get; set; } = typeof(string);
    }
    #endregion 匯出
    #region 通用
    /// <summary>
    /// Excel通用模組，用於封裝Excel資訊
    /// </summary>
    public class ExcelInfo
    {
        /// <summary>
        /// Excel檔案實體位置
        /// </summary>
        public string ExcelFilepath { get; set; }
        /// <summary>
        /// 工作頁名稱(如果沒指定預設打開第一頁)
        /// </summary>
        public string WorkSheetName { get; set; }= string.Empty;
        /// <summary>
        /// 目的資料表名稱
        /// </summary>
        public string DestinationTableName { get; set; } = string.Empty;
        /// <summary>
        /// 第一行為表頭
        /// </summary>
        public bool FirstRowIsHeader { get; set; } = true;
    }
    #endregion 通用
}
