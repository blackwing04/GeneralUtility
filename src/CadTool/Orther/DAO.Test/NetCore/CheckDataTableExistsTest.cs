namespace DAO.Test.NetCore
{
    /// <summary>
    /// 檢查資料表存在測試
    /// </summary>
    public class CheckDataTableExistsTest
    {
        /// <summary>
        /// 模擬檢查已存在的資料表結果
        /// </summary>
        [Fact]
        public async Task CheckDataTableExistsAsync_TableExists_ReturnsSuccessfulResult()
        {
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            //配置模擬連線物件
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);

            var tableSchema = new TableSchemaModel
            {
                TableName = "ExistsTable",
                SchemaColumns = new List<SchemaColumnModel>()
            };
            //設置模擬物件到主方法
            databaseDAO.SetMockConnection(mockDbConnection.Object);

            // Act
            var result = await databaseDAO.CheckDataTableExistsAsync(tableSchema);

            // Assert
            Assert.True(result.IsOperationSuccessful);
            Assert.Equal(ResultString.DataTableIsExists, result.ResultString);

            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 模擬檢查不存在的資料表結果(創建)
        /// </summary>
        [Fact]
        public async Task CheckDataTableExistsAsync_TableExistsForCreate_ReturnsSuccessfulResult()
        {
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            //配置模擬連線物件
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
            var tableSchema = new TableSchemaModel
            {
                TableName = "NonExistentTable",
                SchemaColumns = new List<SchemaColumnModel>()
            };
            //設置模擬物件到主方法
            databaseDAO.SetMockConnection(mockDbConnection.Object);

            // Act
            var result = await databaseDAO.CheckDataTableExistsAsync(tableSchema);

            // Assert
            Assert.True(result.IsOperationSuccessful);
            Assert.Equal(ResultString.DataTableCreateSuccessfully, result.ResultString);

            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
        /// <summary>
        /// 模擬檢查過程發生錯誤的結果
        /// </summary>
        [Fact]
        public async Task CheckDataTableExistsAsync_TableExists_ReturnsFailsResult()
        {
           const string exMessage = "Test for check dataTable failed";
            // Arrange
            var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
            //配置模擬連線物件
            mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);

            //設置模擬物件到主方法
            databaseDAO.SetMockConnection(mockDbConnection.Object);

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => databaseDAO.CheckDataTableExistsAsync(null!));
            // Assert
            Assert.Equal($"{ResultString.CheckDataTableFailed}{exMessage}", result.Message);

            // Verify that OpenAsync and CloseAsync were called once
            mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        }
    }
}
