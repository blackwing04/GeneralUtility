namespace DAOStandard.Test;

public class NonQueryTest
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
    /// 預設回傳影響行數
    /// </summary>
    private readonly int RowsAffected = 10;

    /// <summary>
    /// 模擬執行成功(不返回資料)
    /// </summary>
    [Fact]
    public async Task NonQueryTestAsync_NonQueryTest_ReturnsSuccessfulResult()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection 以建立 mockDbCommand
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockDbCommand = GetMockObject().Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);
        
        // Act
        var result = await databaseDAO.OperationNonQueryAsync(SqlQuery,false);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.TransactionSuccessfully, result.ResultString);
        Assert.Equal(RowsAffected, result.Result);
        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.Close(), Times.Once);
    }
    /// <summary>
    /// 模擬執行失敗(不返回資料)
    /// </summary>
    [Fact]
    public async Task NonQueryTestAsync_NonQueryTest_ReturnsFailsResult()
    {
        const string exMessage ="Test for NonQuery failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            IsTestingError=true,
            ErrorMessage=exMessage,
            MockDbCommand = GetMockObject().Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationNonQueryAsync(SqlQuery,false));

        // Assert
        Assert.Equal($"{ResultString.TransactionFailed}{exMessage}", result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.Close(), Times.Once);
    }
    /// <summary>
    /// 建立模擬物件
    /// </summary>
    /// <returns>返回預設設定的模擬物件</returns>
    private Mock<DbCommand> GetMockObject()
    {
        // 配置 DbCommand 的屬性和方法
        var mockDbCommand = new Mock<DbCommand>();
        var cancellationToken = new CancellationToken(false);
        mockDbCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(cancellationToken)).ReturnsAsync(RowsAffected);

        return mockDbCommand;
    }
}
