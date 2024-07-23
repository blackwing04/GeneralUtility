using DAO.StaticUtil.Enums;
using Generic.StaticUtil;
using System.Data.Common;

namespace DAO.Services.SQLOperation
{
    /// <summary>
    /// DB操作類別工廠
    /// </summary>
    public class OperationFactory
    {
        /// <summary>
        /// 工廠模式，取得對應資料庫的基類
        /// </summary>
        /// <param name="dbType">資料庫種類</param>
        /// <param name="connection">資料庫連線的抽象類(DbConnection)</param>
        /// <returns>返回對應資料庫基類</returns>
        /// <exception cref="NotSupportedException">不支援的資料庫類型</exception>
        public static IDbOperation GetOperationHandler(DbTypeEnum dbType, DbConnection connection)
        {
            return dbType switch {
                DbTypeEnum.MSSQL => new SqlServerOperation(connection),
                DbTypeEnum.SQLite => new SQLiteOperation(connection),
                DbTypeEnum.MySQL => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                DbTypeEnum.PostgreSQL => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                DbTypeEnum.Oracle => throw new NotSupportedException(ResultString.UnsupportedDatabase),
                DbTypeEnum.Mock => new MockOperation(),
                _ => throw new NotSupportedException(ResultString.UnsupportedDatabase),
            };
        }
    }
}
