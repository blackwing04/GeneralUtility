using DAOStandard.StaticUtil.Enums;
using Generic.StaticUtil;
using System;
using System.Data.Common;

namespace DAOStandard.Services.SQLOperation
{
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
            switch (dbType) {
                case DbTypeEnum.MSSQL:
                    return new SqlServerOperation(connection);
                case DbTypeEnum.SQLite:
                    return new SQLiteOperation(connection);
                case DbTypeEnum.MySQL:
                    throw new NotSupportedException(ResultString.UnsupportedDatabase);
                case DbTypeEnum.PostgreSQL:
                    throw new NotSupportedException(ResultString.UnsupportedDatabase);
                case DbTypeEnum.Oracle:
                    throw new NotSupportedException(ResultString.UnsupportedDatabase);
                case DbTypeEnum.Mock:
                    return new MockOperation();
                default:
                    throw new NotSupportedException(ResultString.UnsupportedDatabase);
            }
        }
    }
}
