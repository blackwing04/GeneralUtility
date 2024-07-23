namespace DAO.Test.NetCore
{
    public class OperationScalarTest
    {
        //配置測試物件
        /// <summary>
        /// Sql查詢物件
        /// </summary>
        private readonly SqlQueryModel SqlQuery = new (){
            SqlQueryText="Test",
            Parameter=new Dictionary<string, object> { { "param1", "value1" }, { "@param2", 123 } }
        };

        /// <summary>
        /// 模擬執行指令並將單一資料封裝進模型的Result中成功的結果
        /// </summary>
        [Fact]
        public async Task OperationReaderTestAsync_OperationScalarAsyncOfInt_ReturnsSuccessfulResult()
        {
            int testForResult =10;
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            // 配置 mockDbConnection 以建立 mockDbCommand
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
            //設置模擬物件到主方法
            MockObjectModel objectModel =new(){
                MockDbCommand = GetMockObject(testForResult).Object,
            };
            databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

            // Act
            var result = await databaseDAO.OperationScalarAsync<int>(SqlQuery);

            // Assert
            Assert.True(result.IsOperationSuccessful);
            Assert.Equal(ResultString.QuerySuccessfully, result.ResultString);
            Assert.Equal(testForResult, result.Result);
            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 模擬執行指令並將資料集封裝進模型中成功的結果(內容為空)
        /// </summary>
        [Fact]
        public async Task OperationReaderTestAsync_OperationScalarAsyncOfString_ReturnsSuccessfulResultt()
        {
            string testForResult ="Test";
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            // 配置 mockDbConnection 以建立 mockDbCommand
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
            //設置模擬物件到主方法
            MockObjectModel objectModel =new(){
                MockDbCommand = GetMockObject(testForResult).Object,
            };
            databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

            // Act
            var result = await databaseDAO.OperationScalarAsync<string>(SqlQuery);

            // Assert
            Assert.True(result.IsOperationSuccessful);
            Assert.Equal(ResultString.QuerySuccessfully, result.ResultString);
            Assert.Equal(testForResult, result.Result);
            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 模擬執行指令並將資料集封裝進模型中失敗的結果
        /// </summary>
        [Fact]
        public async Task OperationReaderTestAsync_OperationReaderTest_ReturnsSuccessfullyButEmptyResult()
        {
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            // 配置 mockDbConnection 以建立 mockDbCommand
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
            //設置模擬物件到主方法
            MockObjectModel objectModel =new(){
                MockDbCommand = GetMockObject(null).Object,
            };
            databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

            // Act
            var result = await databaseDAO.OperationScalarAsync<int>(SqlQuery);

            // Assert
            Assert.True(result.IsOperationSuccessful);
            Assert.Equal(ResultString.QuerySuccessfullyButEmpty, result.ResultString);
            Assert.Equal(0, result.Result);
            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 模擬執行指令並將資料集封裝進模型中失敗的結果
        /// </summary>
        [Fact]
        public async Task OperationReaderTestAsync_OperationReaderTest_ReturnsSuccessfullyFailsResult()
        {
            const string exMessage ="Test for operation reader failed";
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            // 配置 mockDbConnection 以建立 mockDbCommand
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
            //設置模擬物件到主方法
            MockObjectModel objectModel =new(){
                IsTestingError=true,
                ErrorMessage=exMessage
            };
            databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationScalarAsync<int>(SqlQuery));

            // Assert
            Assert.Equal($"{ResultString.QueryFailed}{exMessage}", result.Message);

            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 建立模擬物件
        /// </summary>
        /// <returns>返回預設設定的模擬物件</returns>
        private static Mock<DbCommand> GetMockObject(object? returnObject)
        {
            // 配置 DbCommand 的屬性和方法
            var mockDbCommand = new Mock<DbCommand>();
            var cancellationToken = new CancellationToken(false);
            mockDbCommand.Setup(cmd => cmd.ExecuteScalarAsync(cancellationToken)).ReturnsAsync(returnObject);
            return mockDbCommand;
        }
    }
}
