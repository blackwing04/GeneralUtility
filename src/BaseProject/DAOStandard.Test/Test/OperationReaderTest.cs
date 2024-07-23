namespace DAOStandard.Test;

public class OperationReaderTest
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
    /// 模擬的資料表數量
    /// </summary>
    private readonly int TableCount = 1;
    /// <summary>
    /// 模擬的資料筆數
    /// </summary>
    private readonly int RowsCount = 3;

    /// <summary>
    /// 模擬執行成功並將資料集封裝進模型中的結果
    /// </summary>
    [Fact]
    public async Task OperationReaderTestAsync_OperationReaderTest_ReturnsSuccessfulResult()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection 以建立 mockDbCommand
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockIDbCommand = GetMockObject().Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await databaseDAO.OperationReaderAsync(SqlQuery);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.QuerySuccessfully, result.ResultString);
        Assert.Equal("Test", result.Result.Tables[0].TableName);
        Assert.Equal(TableCount, result.Result.Tables.Count);
        Assert.Equal(RowsCount, result.Result.Tables[0].Rows.Count);
        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.Close(), Times.Once);
    }
    /// <summary>
    /// 模擬執行成功並將資料集封裝進模型中的結果(內容為空)
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
            MockIDbCommand = GetMockObject(true).Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await databaseDAO.OperationReaderAsync(SqlQuery);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.QuerySuccessfullyButEmpty, result.ResultString);
        Assert.Equal("Test", result.Result.Tables[0].TableName);
        Assert.Equal(TableCount, result.Result.Tables.Count);
        Assert.Equal(0, result.Result.Tables[0].Rows.Count);
        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.Close(), Times.Once);
    }
    /// <summary>
    /// 模擬執行失敗指令並將資料集封裝進模型中的結果
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
        var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationReaderAsync(SqlQuery));

        // Assert
        Assert.Equal($"{ResultString.QueryFailed}{exMessage}", result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.Close(), Times.Once);
    }

    /// <summary>
    /// 建立模擬物件
    /// </summary>
    /// <returns>返回預設設定的模擬物件</returns>
    private static Mock<IDbCommand> GetMockObject(bool isEmpty = false)
    {
        // 配置 DbDataReader 的屬性和方法
        var mockDbDataReader = GetMockDataReader(isEmpty);
        // 配置 DbCommand 的屬性和方法
        var mockDbCommand = new Mock<IDbCommand>();
        var commandBehavior= CommandBehavior.Default;
        mockDbCommand.Setup(cmd => cmd.ExecuteReader(commandBehavior)).Returns(mockDbDataReader.Object);
        return mockDbCommand;
    }

    private static Mock<IDataReader> GetMockDataReader(bool isEmpty =false)
    {
        var mockDataReader = new Mock<IDataReader>();
        //第一次調用Read()遞增從0開始
        int callCount = -1;
        // 模擬資料列表
        var data = new List<int> { 1, 2, 3 };
        if (isEmpty)
            data.Clear();

        //模擬 Read() 方法
        mockDataReader.Setup(x => x.Read()).Returns(() => {
            callCount++;
            return callCount < data.Count;
        });

        mockDataReader.Setup(x => x["testInt"]).Returns(() => data[callCount]);
        mockDataReader.Setup(x => x.GetInt32(It.IsAny<int>())).Returns(() => data[callCount]);
        mockDataReader.Setup(x => x.FieldCount).Returns(1);
        mockDataReader.Setup(x => x.GetName(0)).Returns("testInt");
        mockDataReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));

        return mockDataReader;
    }
}

