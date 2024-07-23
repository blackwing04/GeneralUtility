namespace UtilityUseDemo.Models
{
    internal class UserSetting
    {
        /// <summary>
        /// 紀錄當前選取的語言
        /// </summary>
        public static string CurrentLanguage { get; set; }
            = Thread.CurrentThread.CurrentUICulture.ToString(); // 預設值
    }
}
