namespace UtilityUseDemo.Utils
{
    public static class MsgBoxHelper
    {
        #region 一般訊息方塊的方法
        /// <summary>
        /// 顯示一個標準的訊息框，可以選取是否自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的訊息的語言鍵或訊息。</param>
        /// <param name="title">訊息框的標題的語言鍵或標題。</param>
        public static void ShowMessage(string message, string title)
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            MessageBox.Show(translatedMessage, translatedTitle);
        }
        /// <summary>
        /// 顯示一個錯誤訊息框，可以選取是否自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的錯誤訊息的語言鍵或訊息。</param>
        /// <param name="title">錯誤訊息框的標題的語言鍵或標題。(預設為"錯誤")</param>
        public static void ShowError(string message, string title = "Error")
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            MessageBox.Show(translatedMessage, translatedTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 顯示一個警告訊息框，訊息將自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的警告訊息的語言鍵或訊息。</param>
        /// <param name="title">警告訊息框的標題的語言鍵或標題。(預設為"警告")</param>
        public static void ShowWarning(string message, string title = "Warning")
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            MessageBox.Show(translatedMessage, translatedTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 顯示一個信息訊息框，訊息將自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的訊息的語言鍵或訊息。</param>
        /// <param name="title">訊息框的標題的語言鍵或標題。(預設為"訊息")</param>
        public static void ShowInformation(string message, string title = "Message")
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            MessageBox.Show(translatedMessage, translatedTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion 一般訊息方塊的方法
        #region DialogResult的方法
        /// <summary>
        /// 顯示一個問題訊息框(YesNo)，並返回用戶的響應，訊息將自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的問題訊息的語言鍵或訊息。</param>
        /// <param name="title">問題訊息框的標題的語言鍵或標題。</param>
        /// <returns>用戶響應的 DialogResult。</returns>
        public static DialogResult ShowQuestion(string message, string title)
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            return MessageBox.Show(translatedMessage, translatedTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        /// <summary>
        /// 顯示一個確認訊息框(YesNoCancel)，並返回用戶的響應，訊息將自動進行語言轉換。
        /// </summary>
        /// <param name="message">要顯示的確認訊息的語言鍵或訊息。</param>
        /// <param name="title">確認訊息框的標題的語言鍵或標題。(預設為"確定?")</param>
        /// <returns>用戶響應的 DialogResult。</returns>
        public static DialogResult ShowConfirmation(string message, string title = "Confirm?")
        {
            string translatedMessage = LangHelper.T(message);
            string translatedTitle = LangHelper.T(title);
            return MessageBox.Show(translatedMessage, translatedTitle,
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
        #endregion DialogResult的方法
    }
}
