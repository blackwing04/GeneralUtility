using DAO.Services;
using Microsoft.Extensions.DependencyInjection;
using DAO.StaticUtil;
using DAO.StaticUtil.Enums;

namespace UtilityUseDemo
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Form的依賴注入比較複雜，更下面註解有網頁專案的注入方式
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var mainForm = ServiceProvider.GetRequiredService<Main>();
            Application.Run(mainForm);

        }
        /// <summary>
        /// 設定依賴注入服務
        /// </summary>
        /// <param name="services">依賴注入服務容器</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            // 注入依賴服務與視窗
            services.AddScoped<Main>();
            /*所有需要使用依賴注入服務的對象都要在這裡注冊，例如:
               - Form 類型的視窗，如設置視窗、關於視窗等
               - UserControl 類型的使用者元件，用於各種界面功能
               - 其他服務，如日誌、配置管理等
               
               範例:
               services.AddScoped<SettingsForm>(); // 設置視窗
               services.AddScoped<AboutBox>(); // 關於視窗
               services.AddScoped<UserControl1>(); // 使用者元件1
               services.AddScoped<UserControl2>(); // 使用者元件2
               
               註冊服務不僅限於視窗和元件，所有在應用程序中需要依賴注入的組件都應在此註冊。
             */
            //創建服務建構式所需的參數
            //正常連結字串不該明文寫在程式碼中，實際使用時請盡量從其他文件讀出來或使用官方推薦的方法例如IConfiguration 
            string conStr = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;";
            var cryptoConStr =CryptoHelper.CreateEncryptModel(conStr);
            cryptoConStr.DatabaseType= DbTypeEnum.MSSQL;
            services.AddScoped(
                provider => DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr)
            );
            /*如果不想用依賴注入
             * 也可以在實際要使用的地方直接用DatabaseDAOFactory.CreateDatabaseDAO建構實例
             * EX. var databaseDAO =DatabaseDAOFactory.CreateDatabaseDAO(cryptoConStr)
             * 但建議還是使用.NET推崇的依賴注入來服務會比較好
            */
        }

        /*ASP.NET Web在Program.cs的注入方式
        // 註冊 DatabaseDAO 並提供建構式參數
        builder.Services.AddScoped<IDatabaseDAO>(serviceProvider => {
            //創建服務建構式所需的參數
            //正常連結字串不該明文寫在程式碼中，實際使用時請盡量從其他文件讀出來或使用官方推薦的方法例如IConfiguration
            string conStr = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;";
            var cryptoConStr =Crypto.CreateEncryptMode(conStr);
            cryptoConStr.DatabaseType= DbTypeEnum.MSSQL;
            // 使用解密的連接字串來建立 DatabaseDAO
            return new DatabaseDAO(cryptoConStr);
        });*/
    }
}