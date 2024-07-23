namespace DAO.Test;

/// <summary>
/// �s�u����
/// </summary>
public class ConnectionTest
{
    #region �s�u����
    /// <summary>
    /// �����}�ҳs�u���\�����G
    /// </summary>
    [Fact]
    public async Task OpenConnectionAsync_ConnectionOpensSuccessfully_ReturnsSuccessfulResult()
    {
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.OpenConnectionAsync();

        // Assert
        Assert.True(result.IsDbConnected);
        Assert.Equal(ResultString.ConnectionOpenSuccessfully, result.ConnectionResultString);
    }
    /// <summary>
    /// ���������s�u���\�����G
    /// </summary>
    [Fact]
    public async Task CloseConnectionAsync_ConnectionCloseSuccessfully_ReturnsSuccessfulResult()
    {
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open);
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.CloseConnectionAsync();

        // Assert
        Assert.False(result.IsDbConnected);
        Assert.Equal(ResultString.ConnectionCloseSuccessfully, result.ConnectionResultString);
    }
    /// <summary>
    /// �����}�ҳs�u���Ѫ����G
    /// </summary>
    [Fact]
    public async Task OpenConnectionAsync_ConnectionFails_ReturnsFailedResult()
    {
        const string exMessage = "Test for connection failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.OpenAsync(It.IsAny<CancellationToken>())).Throws(new Exception(exMessage));
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await Assert.ThrowsAsync<ConnectionException>(databaseDAO.OpenConnectionAsync);

        // Assert
        Assert.Equal($"{ResultString.ConnectionOpenFailed}{exMessage}", result.Message);
    }
    /// <summary>
    /// ���������s�u���Ѫ����G
    /// </summary>
    [Fact]
    public async Task CloseConnectionAsync_ConnectionFails_ReturnsFailedResult()
    {
        const string exMessage = "Test for close connection failed";

        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.CloseAsync()).Throws(new Exception(exMessage));
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Open);
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await Assert.ThrowsAsync<ConnectionException>(databaseDAO.CloseConnectionAsync);

        // Assert
        Assert.Equal($"{ResultString.ConnectionCloseFailed}{exMessage}", result.Message);
    }
    #endregion �s�u����
    #region ������Ʈw����
    /// <summary>
    /// ����������Ʈw���\
    /// </summary>
    [Fact]
    public async Task ChangeDatabaseAsync_ShouldChangeDatabaseSuccessfully()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        mockDbConnection.Setup(m => m.ChangeDatabase(It.IsAny<string>()))
                        .Verifiable("The database should change without errors.");
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act
        var result = await databaseDAO.ChangeDatabaseAsync("NewDatabase");

        // Assert
        Assert.True(result.IsOperationSuccessful, "The result should indicate success.");
        mockDbConnection.Verify();
    }
    /// <summary>
    /// ����������Ʈw����
    /// </summary>
    [Fact]
    public async Task ChangeDatabaseAsync_ShouldHandleException()
    {
        const string exMessage = "Database change failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        //�t�m�����s�u����
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        mockDbConnection.Setup(m => m.ChangeDatabase(It.IsAny<string>()))
                        .Throws(new Exception(exMessage));
        //�]�m���������D��k
        databaseDAO.SetMockConnection(mockDbConnection.Object);

        // Act & Assert
        var result = await Assert.ThrowsAsync<ChangeDatabaseException>(
            () => databaseDAO.ChangeDatabaseAsync("NewDatabase"));
        // Assert
        Assert.Equal($"{ResultString.ChangeDatabaseFailed}{exMessage}", result.Message);
    }
    #endregion ������Ʈw����
}