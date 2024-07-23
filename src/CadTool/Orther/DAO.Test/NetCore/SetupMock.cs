using DAO.Services;

namespace DAO.Test.NetCore
{
    public class SetupMock
    {
        /// <summary>
        /// 設定模擬環境
        /// </summary>
        /// <returns>回傳設定好的模擬物件</returns>
        public static (Mock<DbConnection>, IDatabaseDAO) SetupDatabaseDAO()
        {
            var connectionStringModel = Crypto.CreatEncryptMode(Global.ConnectionString);
            connectionStringModel.DatabaseType = DbTypeEnum.Mock;
            var mockDbConnection = new Mock<DbConnection>();
            mockDbConnection.Setup(m => m.OpenAsync(It.IsAny<CancellationToken>()))
                .Callback(() => mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open))
                .Returns(Task.CompletedTask);

            mockDbConnection.Setup(m => m.CloseAsync())
                .Callback(() => mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed))
                .Returns(Task.CompletedTask);

            mockDbConnection.Setup(m => m.Open())
                .Callback(() => mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open));

            mockDbConnection.Setup(m => m.Close())
                .Callback(() => mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed));

            var databaseDAO = DatabaseDAOFactory.CreateDatabaseDAO(connectionStringModel);

            return (mockDbConnection, databaseDAO);
        }
    }
}
