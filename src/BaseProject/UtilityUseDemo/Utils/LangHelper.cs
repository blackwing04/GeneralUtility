using System.Reflection;
using System.Resources;
using UtilityUseDemo.Models;

namespace UtilityUseDemo.Utils
{
    public class LangHelper
    {
        private static readonly Dictionary<string, string> LanguageMap = new()
        {
            {"繁體中文", "zh-TW"},
            {"简体中文", "zh-CN"},
            {"English", "en-US"}
        };
        /// <summary>
        /// 改變語系地區碼
        /// </summary>
        ///  <param name="language">語系名稱</param>
        public static void ChangeLanguageCode(string language)
        {
            //轉換對應語言碼
            if (LanguageMap.TryGetValue(language, out var languageCode)) {
                UserSetting.CurrentLanguage = languageCode;
            }
            else {
                // 都匹配不到預設為用戶地區語言
                UserSetting.CurrentLanguage = Thread.CurrentThread.CurrentUICulture.ToString();
            }
        }
        /// <summary>
        /// 系統元件初始化語言時使用。
        /// </summary>
        ///  <param name="languageName">資源檔內文本的名稱</param>
        /// <returns>如果找到對應文本，則回傳資源檔中的值；否則回傳原始參數空字段。</returns>
        public static string Initialize(string languageName)
        {
            string resourcePath = $"UtilityUseDemo.Resources.{UserSetting.CurrentLanguage}";
            var resourceManager = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
            return resourceManager.GetString(languageName) ?? string.Empty;
        }
        /// <summary>
        /// 回傳語系地區碼對應的語系資源文本
        /// </summary>
        ///  <param name="languageName">資源檔內文本的名稱</param>
        /// <returns>如果找到對應文本，則回傳資源檔中的值；否則回傳原始參數languageName。</returns>
        public static string T(string languageName)
        {
            try { 
            string resourcePath = $"UtilityUseDemo.Resources.{UserSetting.CurrentLanguage}";
            var resourceManager = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
            return resourceManager.GetString(languageName) ?? languageName;
            }
            catch {
                return languageName;
            }
        }
    }
}
