namespace DAO.Test;

/// <summary>
/// 連線測試
/// </summary>
public class ConnectionTest
{
    #region 連線測試
    /// <summary>
    /// 模擬開啟連線成功的結果
    /// </summary>
    [Fact]
    public async Task OpenConnectionAsync_ConnectionOpensSuccessfully_ReturnsSuccessfulResult()
    {
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.OpenConnectionAsync();

        // Assert
        Assert.True(result.IsDbConnected);
        Assert.Equal(ResultString.ConnectionOpenSuccessfully, result.ConnectionResultString);
    }
    /// <summary>
    /// 模擬關閉連線成功的結果
    /// </summary>
    [Fact]
    public async Task CloseConnectionAsync_ConnectionCloseSuccessfully_ReturnsSuccessfulResult()
    {
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open);
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.CloseConnectionAsync();

        // Assert
        Assert.False(result.IsDbConnected);
        Assert.Equal(ResultString.ConnectionCloseSuccessfully, result.ConnectionResultString);
    }
    /// <summary>
    /// 模擬開啟連線失敗的結果
    /// </summary>
    [Fact]
    public async Task OpenConnectionAsync_ConnectionFails_ReturnsFailedResult()
    {
        const string exMessage = "Test for connection failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.OpenAsync(It.IsAny<CancellationToken>())).Throws(new Exception(exMessage));
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await Assert.ThrowsAsync<ConnectionException>(databaseDAO.OpenConnectionAsync);

        // Assert
        Assert.Equal($"{ResultString.ConnectionOpenFailed}{exMessage}", result.Message);
    }
    /// <summary>
    /// 模擬關閉連線失敗的結果
    /// </summary>
    [Fact]
    public async Task CloseConnectionAsync_ConnectionFails_ReturnsFailedResult()
    {
        const string exMessage = "Test for close connection failed";

        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.CloseAsync()).Throws(new Exception(exMessage));
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open);
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await Assert.ThrowsAsync<ConnectionException>(databaseDAO.CloseConnectionAsync);

        // Assert
        Assert.Equal($"{ResultString.ConnectionCloseFailed}{exMessage}", result.Message);
    }
    #endregion 連線測試
    #region 切換資料庫測試
    /// <summary>
    /// 模擬切換資料庫成功
    /// </summary>
    [Fact]
    public async Task ChangeDatabaseAsync_ShouldChangeDatabaseSuccessfully()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        mockDbConnection.Setup(m => m.ChangeDatabase(It.IsAny<string>()))
                        .Verifiable("The database should change without errors.");
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.ChangeDatabaseAsync("NewDatabase");

        // Assert
        Assert.True(result.IsOperationSuccessful, "The result should indicate success.");
        mockDbConnection.Verify();
    }
    /// <summary>
    /// 模擬切換資料庫失敗
    /// </summary>
    [Fact]
    public async Task ChangeDatabaseAsync_ShouldHandleException()
    {
        const string exMessage = "Database change failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //配置模擬連線物件
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        mockDbConnection.Setup(m => m.ChangeDatabase(It.IsAny<string>()))
                        .Throws(new Exception(exMessage));
        //設置模擬物件到主方法
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act & Assert
        var result = await Assert.ThrowsAsync<ChangeDatabaseException>(
            () => databaseDAO.ChangeDatabaseAsync("NewDatabase"));
        // Assert
        Assert.Equal($"{ResultString.ChangeDatabaseFailed}{exMessage}", result.Message);
    }
    #endregion 切換資料庫測試
}