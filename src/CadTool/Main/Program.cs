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
        // �ϥ� 'LibraryImportAttribute' �ӫD 'DllImportAttribute' �b�sĶ�ɶ����� P/Invoke �ʰe�B�z�{���X
#pragma warning disable SYSLIB1054 
        static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();
#pragma warning restore SYSLIB1054 
        // �ϥ� 'LibraryImportAttribute' �ӫD 'DllImportAttribute' �b�sĶ�ɶ����� P/Invoke �ʰe�B�z�{���X

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
        /// �]�w�̿�`�J�A��
        /// </summary>
        /// <param name="services">�̿�`�J�A�Ȯe��</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Main>();
            //�s���r����H�K���A�᭱�A����
            string conStr= new SqliteConnectionStringBuilder { DataSource = "D:/" }.ToString();
            var cryptoConStr =Crypto.CreatEncryptMode(conStr);
            cryptoConStr.DatabaseType = DbTypeEnum.SQLite;
            services.AddScoped(
                provider => DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr)
            );
        }
        private static async void RunApp(IList<string> args)
        {
            //�����Ҧ����ի��O
#if DEBUG
            args = new List<string>() {
             "/s",
            "/src", "D:\\Release\\CadDAO\\Demo.json",
            "/targ", "D:\\Sqlite\\Demo.db"
            };
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            //�N�ѼƤ��ά��r��
            var parameters = args.Select((value, index) => new { Index = index, Value = value })
                    .Where(p => p.Value.StartsWith("/") && p.Index + 1 < args.Count && !args[p.Index + 1].StartsWith("/"))
                    .ToDictionary(p => p.Value, p => args[p.Index + 1],StringComparer.OrdinalIgnoreCase);
            // �ˬd�R�q�Ҧ�
            if (args.Contains("/s")) {
                // ���t����x���f
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.WriteLine();
                // ����L�����ާ@
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
                // �S���ѼƩΥ����w�R�q�Ҧ��A�Ұʼз� GUI
                Application.Run(new Main());
            }
        }
        #region �p����k
        /// <summary>
        /// �N�T���o�ܤ��h�D���x�H��Log��x�ɮ�
        /// </summary>
        /// <param name="messages">�T��</param>
        private static void ConsoleWriteAndLog(string messages)
        {
            Console.WriteLine(messages);
            LogFileHelper.Log(messages);
        }
        /// <summary>
        /// ���ҰѼƸ��|
        /// </summary>
        /// <param name="parameters">�ѼƦr��</param>
        /// <param name="sourcePath">�ӷ����|</param>
        /// <param name="targetPath">�ؼи��|</param>
        /// <returns>�^�Ǹ��|�r�ꪺ���ĩ�(�D��)</returns>
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
            // �ˬd�ӷ����|
            if (parameters.TryGetValue("/src", out string? tempSourcePath) && !string.IsNullOrEmpty(tempSourcePath)) {
                sourcePath = tempSourcePath;
                ConsoleWriteAndLog($"Source file path: {sourcePath}");
            }
            else {
                errorMsg = "Source file path is missing or empty.";
                ConsoleWriteAndLog(errorMsg);
                throw new Exception(errorMsg);
            }
            // �ˬd�ؼи��|
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
        /// �ˬd�ؼХؿ��O�_�s�b�A�p�G���s�b�|�Ыب�ؿ�
        /// </summary>
        /// <param name="targetPath">�ؼи��|</param>
        private static void CreateDirectoryIfNotExists(string targetPath)
        {
            string? directoryPath = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directoryPath) && !string.IsNullOrEmpty(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
        }
        /// <summary>
        /// �]�m��Ʈw�X�ݹ�H
        /// </summary>
        /// <param name="sqlitePath">SQLite��Ʈw���|</param>
        /// <returns>��^��Ʈw�X�ݹ�H�������</returns>
        private static IDatabaseDAO SetupDatabaseDAO(string sqlitePath)
        {
            var conStrBuilder = new SqliteConnectionStringBuilder { DataSource = sqlitePath };
            var cryptoConStr = Crypto.CreatEncryptMode(conStrBuilder.ToString());
            cryptoConStr.DatabaseType = DbTypeEnum.SQLite;
            return DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr);
        }
        /// <summary>
        /// �������ഫ
        /// </summary>
        /// <param name="sourcePath">�ӷ��ؿ�</param>
        /// <param name="targetPath">�ؼХؿ�</param>
        /// <returns></returns>
        private static async Task ExecuteDataConversionAsync(string sourcePath, string targetPath)
        {
            CreateDirectoryIfNotExists(targetPath);
            string sourceExtension = Path.GetExtension(sourcePath).ToLower();
            string targetExtension = Path.GetExtension(targetPath).ToLower();
            if (sourceExtension == ".json" && targetExtension == ".db") {
                //SQLite���ؼХؿ�
                var databaseDAO = SetupDatabaseDAO(targetPath);
                DataConvert convert = new (sourcePath, targetPath, databaseDAO);
                await convert.JsonToSQLiteAsync();
            }
            else if (sourceExtension == ".db" && targetExtension == ".json") {
                //SQLite���ӷ��ؿ�
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
        /// �B�z���~
        /// </summary>
        /// <param name="ex">���~�ҥ~</param>
        private static void HandleError(Exception ex)
        {
            ConsoleWriteAndLog($"Something wrong. Error: {ex.Message}");
            Environment.Exit(1);
        }
        #endregion �p����k
    }
}