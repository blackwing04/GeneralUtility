namespace DAO.Test;

public class StoredProcedureTest
{
    //測試模型
    private readonly StoredProcedureModel StoredProcedureData = new(){
        StoredProcedureName="TestStoredProcedure",
        Parameter = new Dictionary<string, object> { { "param1", "value1" }, { "@param2", 123 } },
    };

    /// <summary>
    /// 模擬執行預存函數成功的結果
    /// </summary>
    [Fact]
    public async Task ExecuteStoredProcedureAsync_ExecuteStoredProcedure_ReturnsSuccessfulResult()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();

        // 配置 mockDbConnection 以建立 mockDbCommand
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        // 配置 DbCommand 的屬性和方法
        var mockDbCommand = new Mock<DbCommand>();
        mockDbCommand.SetupProperty(cmd => cmd.CommandType, CommandType.StoredProcedure);
        var cancellationToken = new CancellationToken(false);
        int rowsAffected =100;
        mockDbCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(cancellationToken)).ReturnsAsync(rowsAffected);
        //配置參數
        var parameters = new Dictionary<string, object> { { "param1", "value1" }, { "@param2", 123 } };

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockDbCommand = mockDbCommand.Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await databaseDAO.ExecuteStoredProcedureAsync(StoredProcedureData);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.ExecutedStoredProcedureSuccessfully, result.ResultString);
        Assert.Equal(rowsAffected, result.Result);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
    /// <summary>
    /// 模擬執行預存函數失敗的結果
    /// </summary>
    [Fact]
    public async Task ExecuteStoredProcedureAsync_ExecuteStoredProcedure_ReturnsFailsResult()
    {
        const string exMessage = "Test for execute stored procedure failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //配置參數
        var parameters = new Dictionary<string, object> { { "param1", "value1" }, { "@param2", 123 } };
        MockObjectModel objectModel = new(){
         IsTestingError = true,
         ErrorMessage = exMessage,
        };
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);
        // Act
        var result = await Assert.ThrowsAsync<Exception>(
            ()=>databaseDAO.ExecuteStoredProcedureAsync(StoredProcedureData));

        // Assert
        Assert.Equal($"{ResultString.ExecutedStoredProcedureFailed}{exMessage}", result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
}
