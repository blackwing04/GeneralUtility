using Microsoft.Win32;
using UtilityUseDemo.Utils;

namespace UtilityUseDemo
{
#pragma warning disable CA2211
    public class GlobalVariables
    {
        /// <summary>
        /// 程式版本號
        /// </summary>
        public const string AppVersion = "1.0";

        /// <summary>
        /// 用於儲存設定的註冊表位置
        /// </summary>
        private const string RegistryPath = "Software\\GBR Co., Ltd.\\dbtemplate";

        /// <summary>
        /// 註冊表值名稱，用於存儲 UserSetting 變量
        /// </summary>
        public const string UserSettingRegistryValueName = "UserSetting";

        /// <summary>
        /// 註冊表值名稱，用於存儲 ConnString 變量
        /// </summary>
        public const string ConnStringRegistryValueName = "ConnString";

        /// <summary>
        /// 產品名
        /// </summary>
        public const string ProductName = "EDRP";
        /// <summary>
        /// 當前用戶在語言選取列表中選中的語言索引
        /// </summary>
        public static int LanguageIndex { get; set; }
        /// /// <summary>
        /// 儲存當前選取的DT節點索引
        /// </summary>
        public static int CurrentDTNodeIndex { get; set; }

        private static bool _edited;
        /// /// <summary>
        /// 紀錄是否有修改
        /// </summary>
        public static bool Edited
        {
            get => _edited;
            set
            {
                if (_edited != value) {
                    _edited = value;
                    OnEditedChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler? OnEditedChanged;

        /// <summary>
        /// Json檔(樣板)目錄
        /// </summary>
        public static string Folder { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// Variable to save registry key
        /// </summary>
        public static RegistryKey? UserRegistryKey
        {
            get
            {
                try {
                    var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
                    var key = baseKey.CreateSubKey(RegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    return key;
                }
                catch (Exception e) {
                    ExceptionHelper.RecordExceptionToLag(e);
                }
                return null;
            }
        }
        /// <summary>
        /// 紀錄在登路檔的連接字串
        /// </summary>
        public static string ConnectionStringFromRegistry =string.Empty;
        /// <summary>
        /// Connection string to connect db
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionStringFromRegistry)) return ConnectionStringFromRegistry;
                // get value from registry and decrypt connection string key
                try {
                    if (UserRegistryKey == null) return ConnectionStringFromRegistry;
                    var o = UserRegistryKey.GetValue(ConnStringRegistryValueName);
                    if (o != null) ConnectionStringFromRegistry = CryptokiHelper.Decrypt(o.ToString()!);
                }
                catch (Exception e) {
                    ExceptionHelper.RecordExceptionToLag(e);
                    ConnectionStringFromRegistry = "";
                }
                return ConnectionStringFromRegistry;
            }
            set
            {
                try {
                    if (string.IsNullOrEmpty(value)) return;
                    // encrypt ConnectionString string and save it to registry
                    var encryptedString = CryptokiHelper.Encrypt(value);
                    var key = UserRegistryKey;

                    if (key != null) {
                        key.SetValue(ConnStringRegistryValueName, encryptedString);
                        key.Close();
                    }
                    ConnectionStringFromRegistry = value;
                }
                catch (Exception e) {
                    ExceptionHelper.RecordExceptionToLag(e);
                }
            }
        }
        private static readonly string TempFolderPath = Path.GetTempPath() + @"\GBRCo\dbtemplate";

        public static string TempFolder
        {
            get
            {
                Directory.CreateDirectory(TempFolderPath);
                return TempFolderPath;
            }
        }

        private static readonly string AppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GBRCo\dbtemplate\";

        /// <summary>
        /// Temp folder to store log, and picture
        /// </summary>
        public static string AppDataFolder
        {
            get
            {
                Directory.CreateDirectory(AppDataFolderPath);
                return AppDataFolderPath;
            }
        }
    }
#pragma warning restore CA2211
}
