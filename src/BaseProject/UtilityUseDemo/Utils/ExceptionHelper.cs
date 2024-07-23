using Generic.StaticUtil;
using System.Diagnostics;

namespace UtilityUseDemo.Utils
{
    public class ExceptionHelper
    {
        /// <summary>
        /// 將異常訊息和自定義日誌訊息記錄到日誌檔案。
        /// </summary>
        /// <param name="ex">要記錄的異常。</param>
        /// <param name="customerLog">額外的自定義日誌訊息，可選。</param>
        /// <remarks>
        /// 此方法提取異常的堆棧跟踪訊息，包括錯誤發生的檔案名和行號。
        /// 如果無法獲取這些訊息，將僅記錄異常的消息。
        /// </remarks>
        public static void RecordExceptionToLag(Exception ex, string customerLog = "")
        {
            //ToDo:未來用此方法自己寫錯誤行號跟錯誤的檔案名
            if (!string.IsNullOrEmpty(customerLog))
                customerLog = $"{customerLog}";
            StackTrace trace = new (ex, true);
            StackFrame? frame = trace.GetFrame(0); // 獲取引發異常的第一個堆棧框架
            if (frame != null) {
                string? fileName = frame.GetFileName(); // 出錯的文件名
                int line = frame.GetFileLineNumber(); // 出錯的行號
                LogFileHelper.Log(GlobalVariables.AppDataFolder,
                    $"{customerLog} [錯誤位置] 檔案：{fileName}, 行號：{line}, 錯誤信息：{ex.Message}");
            }
            else {
                LogFileHelper.Log(GlobalVariables.AppDataFolder, 
                    $"{customerLog} 錯誤信息：{ex.Message}");
            }
        }
    }
}
