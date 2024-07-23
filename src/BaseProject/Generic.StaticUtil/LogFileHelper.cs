using System;
using System.IO;

namespace Generic.StaticUtil
{
    public class LogFileHelper
    {
        /// <summary>
        /// 將訊息紀錄至日誌檔
        /// </summary>
        /// <param name="logPath">日誌路徑</param>
        /// <param name="value">日誌訊息</param>
        public static void Log(string logPath, string value)
        {
            var fileName = Path.Combine(logPath, "logs_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

            // 訊息只記錄一次
            if (!File.Exists(fileName)) {
                // 建立一個日誌檔案
                using (var sw = File.CreateText(fileName)) {
                    sw.WriteLine("{0:yyyy-MM-dd HH:mm:ss} # GBRCO_LOG_FILE", DateTime.Now);
                }
            }
            // 此文字每次執行都會被新增，若不刪除則會使檔案逐漸變長。
            using (var sw = File.AppendText(fileName)) {
                sw.WriteLine("{0:yyyy-MM-dd HH:mm:ss} # {1}", DateTime.Now, value);
            }
        }
    }
}
