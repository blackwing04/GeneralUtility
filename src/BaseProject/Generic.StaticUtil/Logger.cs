using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace Generic.StaticUtil
{
    /// <summary>
    /// 日誌記錄器
    /// </summary>
    /// <typeparam name="T">調用的類別，用於識別是哪個類別觸發了日誌記錄。</typeparam>
    public static class LogHelper<T> where T : class
    {
        #region 設定Serilog
        /// <summary>
        /// 設定Serilog
        /// </summary>
        /// <param name="filePath">日誌文件路徑，預設為 logs/log-.txt</param>
        /// <param name="consoleOutput">是否輸出到控制台，預設為 true</param>
        /// <param name="rollingInterval">滾動間隔，預設為每天</param>
        public static void ConfigureLogging(string filePath = "logs/log-.txt"
            , bool consoleOutput = true
            , RollingInterval rollingInterval = RollingInterval.Day)
        {
            var loggerConfig = new LoggerConfiguration();

            if (consoleOutput) {
                loggerConfig = loggerConfig.WriteTo.Console();
            }

            loggerConfig = loggerConfig.WriteTo.File(filePath, rollingInterval: rollingInterval);

            Log.Logger = loggerConfig.CreateLogger();
        }
        #endregion 設定Serilog

        #region 公開函式
        /// <summary>
        /// 處理系統例外錯誤，包含記錄詳細錯誤至伺服器日誌，使用者端轉跳通用錯誤頁面
        /// </summary>
        /// <param name="ex">系統例外</param>
        /// <param name="customerLog">自訂的錯誤內容(最好包含發生錯誤的方法名)</param>
        public static void HandleError(Exception ex, string customerLog = "")
        {
            LogErrorRecursive(ex, customerLog);
        }

        /// <summary>
        /// 記錄資訊日誌
        /// </summary>
        /// <param name="log">日誌</param>
        /// <param name="logLevel">日誌等級(預設為一般)</param>
        /// <remarks>使用內建日誌等級列舉</remarks>
        public static void HandleLogEventLevel(string log, LogEventLevel logLevel = LogEventLevel.Information)
        {
            Log.Write(logLevel, log);
        }
        /// <summary>
        /// 記錄資訊日誌
        /// </summary>
        /// <param name="log">日誌</param>
        /// <param name="logLevel">日誌等級(預設為一般)</param>
        /// <remarks>使用Serilog日誌等級列舉</remarks>
        public static void HandleLogLevel(string log, LogLevel logLevel = LogLevel.Information)
        {
            LogEventLevel logEventLevel;

            switch (logLevel) {
                case LogLevel.Information:
                    logEventLevel = LogEventLevel.Information;
                    break;
                case LogLevel.Warning:
                    logEventLevel = LogEventLevel.Warning;
                    break;
                case LogLevel.Error:
                    logEventLevel = LogEventLevel.Error;
                    break;
                case LogLevel.Debug:
                    logEventLevel = LogEventLevel.Debug;
                    break;
                default:
                    logEventLevel = LogEventLevel.Information;
                    break;
            }

            Log.Write(logEventLevel, log);
        }
        #endregion 公開函式

        #region 內部函式
        /// <summary>
        /// 遞歸記錄所有層級的內部錯誤訊息
        /// </summary>
        /// <param name="ex">當前層的例外</param>
        /// <param name="customerLog">自訂的錯誤內容</param>
        private static void LogErrorRecursive(Exception ex, string customerLog)
        {
            if (ex == null) return;

            if (ex.InnerException == null)
                Log.Error($"{customerLog}, 訊息位置:{typeof(T).FullName}, 錯誤信息：{ex.Message}");
            else {
                Log.Error($"{customerLog}, 訊息位置:{typeof(T).FullName}, 錯誤信息：{ex.Message}, 內部錯誤訊息：{ex.InnerException.Message}");
                LogErrorRecursive(ex.InnerException, customerLog);  // 遞歸處理內部錯誤
            }
        }
        #endregion 內部函式
    }
}
