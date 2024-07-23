using System;

namespace DAOCustomizeException
{
    /// <summary>
    /// 連線發生的例外
    /// </summary>
    public class ConnectionException : Exception
    {
        /// <summary>
        /// 切換資料庫發生的例外
        /// </summary>
        public ConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 切換資料庫發生的例外
    /// </summary>
    public class ChangeDatabaseException : Exception
    {
        /// <summary>
        /// 切換資料庫發生的例外
        /// </summary>
        public ChangeDatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
