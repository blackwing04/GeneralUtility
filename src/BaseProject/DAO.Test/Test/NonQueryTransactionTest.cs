namespace DAO.Test;

public class NonQueryTransactionTest
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
    /// 模擬執行成功(不返回資料)並提交事務的結果
    /// </summary>
    [Fact]
    public async Task NonQueryTransactionTestAsync_NonQueryTransactionTest_ReturnsSuccessfulResult()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection 以建立 mockDbCommand
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        (var mockDbCommand, var mockTransaction) = GetMockObject();

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockDbCommand = mockDbCommand.Object,
            MockDbTransaction = mockTransaction.Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);
        
        // Act
        var result = await databaseDAO.OperationNonQueryTransactionAsync(SqlQuery);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.TransactionSuccessfully, result.ResultString);
        Assert.Equal(RowsAffected, result.Result);
        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
    /// <summary>
    /// 模擬執行指令(不返回資料)發生事物為空的結果
    /// </summary>
    [Fact]
    public async Task NonQueryTransactionTestAsync_NonQueryTransactionTest_ReturnsTransactionFailedOrEmpty()
    {
        const string exMessage = $"{ResultString.TransactionTransformFailedOrEmpty}Mock Test";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);

        //建立模擬物件
        (var mockDbCommand, var mockTransaction) = GetMockObject();


        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockDbCommand = mockDbCommand.Object,
            MockDbTransaction = null,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationNonQueryTransactionAsync(SqlQuery));

        // Assert
        Assert.Equal(exMessage, result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
    /// <summary>
    /// 模擬執行失敗(不返回資料)並提交事務的結果(包含事務滾回)
    /// </summary>
    [Fact]
    public async Task NonQueryTransactionTestAsync_NonQueryTransactionTest_ReturnsFailsResult()
    {
        const string exMessage ="Test for NonQuery failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //建立模擬物件
        (var mockDbCommand, var mockTransaction) = GetMockObject();

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            IsTestingError=true,
            ErrorMessage=exMessage,
            MockDbCommand = mockDbCommand.Object,
            MockDbTransaction = mockTransaction.Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationNonQueryTransactionAsync(SqlQuery));

        // Assert
        Assert.Equal($"{ResultString.TransactionFailed}{exMessage}", result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
        mockTransaction.Verify(m => m.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    /// <summary>
    /// 建立模擬物件
    /// </summary>
    /// <returns>返回預設設定的模擬物件</returns>
    private (Mock<DbCommand>, Mock<DbTransaction>) GetMockObject()
    {
        // 配置 DbCommand 的屬性和方法
        var mockDbCommand = new Mock<DbCommand>();
        var cancellationToken = new CancellationToken(false);
        mockDbCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(cancellationToken)).ReturnsAsync(RowsAffected);
        // 配置 DBTransaction
        var mockTransaction =new Mock<DbTransaction>();
        mockTransaction.Setup(m => m.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockTransaction.Setup(m => m.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        return (mockDbCommand, mockTransaction);
    }
}
