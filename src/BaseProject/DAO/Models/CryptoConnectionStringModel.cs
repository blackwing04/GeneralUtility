using DAO.StaticUtil.Enums;

namespace DAO.Models
{
    /// <summary>
    /// 封裝加密後的資料庫字串用於解析資料庫連接字串，Crypto裡有創建的方法。
    /// </summary>
    public class CryptoConnectionStringModel
    {
        /// <summary>
        /// 資料庫類型(預設MSSQL)
        /// </summary>
        public DbTypeEnum DatabaseType { get; set; }
        /// <summary>密鑰(key)</summary>
        public string K1 { get; set; } = string.Empty;
        /// <summary>向量值(IV)</summary>
        public string V2 { get; set; } = string.Empty;
        /// <summary>加密連接字段(ConnectionString)</summary>
        public string CS { get; set; } = string.Empty;
    }
}
