using DAOStandard.Models;
namespace DAOStandard.Services
{
    /// <summary>
    /// 資料庫DAO類別工廠
    /// </summary>
    public class DatabaseDAOFactory
    {
        /// <summary>
        /// 創建 DatabaseDAO 的實例，如果不會使用DI可以調用此方法創建實例
        /// </summary>
        /// <param name="connectionString">加密連接字串</param>
        /// <returns>DatabaseDAO 實例</returns>
        public static DatabaseDAO CreateDatabaseDAO(CryptoConnectionStringModel connectionString)
        {
            return new DatabaseDAO(connectionString);
        }
    }
}
