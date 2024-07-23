using StaticUtil.Generic;
using System.Runtime.InteropServices;
using DAO.Services;
using CadDAO.Util;
using StaticUtil.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.VisualBasic.Logging;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using System.Reflection;

namespace CadDAO
{
    public static class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        [DllImport("kernel32.dll", SetLastError = true)]
        // 使用 'LibraryImportAttribute' 而非 'DllImportAttribute' 在編譯時間產生 P/Invoke 封送處理程式碼
#pragma warning disable SYSLIB1054 
        static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();
#pragma warning restore SYSLIB1054 
        // 使用 'LibraryImportAttribute' 而非 'DllImportAttribute' 在編譯時間產生 P/Invoke 封送處理程式碼

        // Constants for AttachConsole
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            RunApp(args);
        }
        /// <summary>
        /// 設定依賴注入服務
        /// </summary>
        /// <param name="services">依賴注入服務容器</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Main>();
            //連結字串先隨便給，後面再切換
            string conStr= new SqliteConnectionStringBuilder { DataSource = "D:/" }.ToString();
            var cryptoConStr =Crypto.CreatEncryptMode(conStr);
            cryptoConStr.DatabaseType = DbTypeEnum.SQLite;
            services.AddScoped(
                provider => DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr)
            );
        }
        private static async void RunApp(IList<string> args)
        {
            //偵錯模式測試指令
#if DEBUG
            args = new List<string>() {
             "/s",
            "/src", "D:\\Release\\CadDAO\\Demo.json",
            "/targ", "D:\\Sqlite\\Demo.db"
            };
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            //將參數分割為字典
            var parameters = args.Select((value, index) => new { Index = index, Value = value })
                    .Where(p => p.Value.StartsWith("/") && p.Index + 1 < args.Count && !args[p.Index + 1].StartsWith("/"))
                    .ToDictionary(p => p.Value, p => args[p.Index + 1],StringComparer.OrdinalIgnoreCase);
            // 檢查靜默模式
            if (args.Contains("/s")) {
                // 分配控制台窗口
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.WriteLine();
                // 執行無介面操作
                ConsoleWriteAndLog("Running in silent mode...");
                try {
                    if (LogAndValidateFilePaths(parameters, out string sourcePath, out string targetPath)) {
                        if (ServiceProvider is not null) {
                            await ExecuteDataConversionAsync(sourcePath, targetPath);
                            ConsoleWriteAndLog("Done");
                        }
                    }
                }
                catch (Exception ex) {
                    HandleError(ex);
                }
                finally {
                    FreeConsole();
                }
                Environment.Exit(0);
            }
            else {
                // 沒有參數或未指定靜默模式，啟動標準 GUI
                Application.Run(new Main());
            }
        }
        #region 私有方法
        /// <summary>
        /// 將訊息發至父層主控台以及Log日誌檔案
        /// </summary>
        /// <param name="messages">訊息</param>
        private static void ConsoleWriteAndLog(string messages)
        {
            Console.WriteLine(messages);
            LogFileHelper.Log(messages);
        }
        /// <summary>
        /// 驗證參數路徑
        /// </summary>
        /// <param name="parameters">參數字典</param>
        /// <param name="sourcePath">來源路徑</param>
        /// <param name="targetPath">目標路徑</param>
        /// <returns>回傳路徑字串的有效性(非空)</returns>
        private static bool LogAndValidateFilePaths(Dictionary<string, string>? parameters
            , out string sourcePath, out string targetPath)
        {
            sourcePath = string.Empty;
            targetPath = string.Empty;
            string errorMsg;

            if (parameters is null) {
                errorMsg = "Parameters are null.";
                ConsoleWriteAndLog(errorMsg);
                throw new Exception(errorMsg);
            }
            bool isValid = true;
            // 檢查來源路徑
            if (parameters.TryGetValue("/src", out string? tempSourcePath) && !string.IsNullOrEmpty(tempSourcePath)) {
                sourcePath = tempSourcePath;
                ConsoleWriteAndLog($"Source file path: {sourcePath}");
            }
            else {
                errorMsg = "Source file path is missing or empty.";
                ConsoleWriteAndLog(errorMsg);
                throw new Exception(errorMsg);
            }
            // 檢查目標路徑
            if (parameters.TryGetValue("/targ", out string? tempTargetPath) && !string.IsNullOrEmpty(tempTargetPath)) {
                targetPath = tempTargetPath;
                ConsoleWriteAndLog($"Target file path: {targetPath}");

            }
            else {
                errorMsg = "Target file path is missing or empty.";
                ConsoleWriteAndLog(errorMsg);
                throw new Exception(errorMsg);
            }
            return isValid;
        }
        /// <summary>
        /// 檢查目標目錄是否存在，如果不存在會創建其目錄
        /// </summary>
        /// <param name="targetPath">目標路徑</param>
        private static void CreateDirectoryIfNotExists(string targetPath)
        {
            string? directoryPath = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directoryPath) && !string.IsNullOrEmpty(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
        }
        /// <summary>
        /// 設置資料庫訪問對象
        /// </summary>
        /// <param name="sqlitePath">SQLite資料庫路徑</param>
        /// <returns>返回資料庫訪問對象介面實例</returns>
        private static IDatabaseDAO SetupDatabaseDAO(string sqlitePath)
        {
            var conStrBuilder = new SqliteConnectionStringBuilder { DataSource = sqlitePath };
            var cryptoConStr = Crypto.CreatEncryptMode(conStrBuilder.ToString());
            cryptoConStr.DatabaseType = DbTypeEnum.SQLite;
            return DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr);
        }
        /// <summary>
        /// 執行資料轉換
        /// </summary>
        /// <param name="sourcePath">來源目錄</param>
        /// <param name="targetPath">目標目錄</param>
        /// <returns></returns>
        private static async Task ExecuteDataConversionAsync(string sourcePath, string targetPath)
        {
            CreateDirectoryIfNotExists(targetPath);
            string sourceExtension = Path.GetExtension(sourcePath).ToLower();
            string targetExtension = Path.GetExtension(targetPath).ToLower();
            if (sourceExtension == ".json" && targetExtension == ".db") {
                //SQLite為目標目錄
                var databaseDAO = SetupDatabaseDAO(targetPath);
                DataConvert convert = new (sourcePath, targetPath, databaseDAO);
                await convert.JsonToSQLiteAsync();
            }
            else if (sourceExtension == ".db" && targetExtension == ".json") {
                //SQLite為來源目錄
                var databaseDAO = SetupDatabaseDAO(sourcePath);
                DataConvert convert = new (sourcePath, targetPath, databaseDAO);
                await convert.SQLiteToJsonAsync(targetPath);
            }
            else {
                ConsoleWriteAndLog("Invalid source and target file types.");
                return;
            }
        }
        /// <summary>
        /// 處理錯誤
        /// </summary>
        /// <param name="ex">錯誤例外</param>
        private static void HandleError(Exception ex)
        {
            ConsoleWriteAndLog($"Something wrong. Error: {ex.Message}");
            Environment.Exit(1);
        }
        #endregion 私有方法
    }
}