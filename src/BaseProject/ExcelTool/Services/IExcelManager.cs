using ExcelTool.StaticUtil.Models;
using System.Data;

namespace ExcelTool.Services
{
    public interface IExcelManager
    {
        /// <summary>
        /// 取得Excel的表頭名稱
        /// </summary>
        /// <param name="excelInfo">Excel檔案訊息包含匹配驗證資料的設定</param>
        /// <param name="stream">內存數據</param>
        /// <returns>返回表頭名稱列表</returns>
        List<string> GetExcelHeaderName(ExcelInfo excelInfo, MemoryStream? stream = null);

        /// <summary>
        /// excel轉換成匯入模型，內含轉換DataTable的結果
        /// </summary>
        /// <param name="excelInfo">Excel檔案訊息包含匹配驗證資料的設定</param> 
        /// <param name="stream">內存數據</param>
        /// <returns>返回轉換模型</returns>
        /// <remarks>如果有提供內存數據則從內存取得Excel資料</remarks>
        Task<ImportModel> ExcelConvertToImportModelAsync(ExcelInfo excelInfo, MemoryStream? stream = null);

        /// <summary>
        /// DataTable轉換成Excel
        /// </summary>
        /// <param name="sourceData">資料來源(DataTable)</param>
        /// <param name="filePath">匯出路徑(可空)</param>
        /// <param name="stream">內存數據(可空)</param>
        /// <remarks>匯出路徑或內存數據容器必須至少提供一個來作保存</remarks>
        Task DataTableConvertToExcelAsync(DataTable sourceData, string? filePath = null, MemoryStream? stream = null);

        /// <summary>
        /// 將List模型轉換成Excel
        /// </summary>
        /// <param name="sourceData">資料來源(List模型)></param>
        /// <param name="excelMapper">驗證和映射 Excel 資料</param>
        /// <param name="filePath">匯出路徑</param>
        /// <param name="stream">內存數據</param>
        /// <remarks>匯出路徑或內存數據容器必須至少提供一個來作保存</remarks>
        Task ListConvertToExcelAsync(ListConvertExcelModel sourceData, string? filePath = null
            , ExcelMapperSetting? excelMapper = null, MemoryStream? stream = null);
    }
}
