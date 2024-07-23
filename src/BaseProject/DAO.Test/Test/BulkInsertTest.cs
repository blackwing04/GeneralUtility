namespace DAO.Test;

/// <summary>
/// 大量資料寫入測試
/// </summary>
public class BulkInsertTest
{
    /// <summary>
    /// 預設回傳影響行數
    /// </summary>
    private readonly int RowsAffected;
    /// <summary>
    /// 寫入的模擬測試表
    /// </summary>
    private readonly DataTable TestTable = new();
    /// <summary>
    /// 大量資料寫入測試
    /// </summary>
    public BulkInsertTest()
    {
        //配置測試物件
        TestTable.Columns.Add("testInt", typeof(int));
        DataRow row = TestTable.NewRow ();
        row["testInt"] = 1;
        TestTable.Rows.Add(row);
        RowsAffected = TestTable.Rows.Count;
    }
    /// <summary>
    /// 模擬執行大量寫入資料成功的結果
    /// </summary>
    [Fact]
    public async Task BulkInsertTestAsync_BulkInsertTest_ReturnsSuccessfulResult()
    {
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
      
        // 配置 mockDbConnection 以建立 mockDbCommand
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        // 配置 DbCommand 的屬性和方法
        var mockDbCommand = new Mock<DbCommand>();
        var cancellationToken = new CancellationToken(false);
        mockDbCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(cancellationToken)).ReturnsAsync(RowsAffected);

        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            MockDbCommand = mockDbCommand.Object,
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);

        // Act
        var result = await databaseDAO.OperationBulkInsertAsync(TestTable);

        // Assert
        Assert.True(result.IsOperationSuccessful);
        Assert.Equal(ResultString.BulkInsertSuccessfully, result.ResultString);
        Assert.Equal(RowsAffected, result.Result);
        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
    /// <summary>
    /// 模擬執行大量寫入資料失敗的結果
    /// </summary>
    [Fact]
    public async Task BulkInsertTestAsync_BulkInsertTest_ReturnsFailsResult()
    {
        const string exMessage = "Test for bulk insert failed";
        // Arrange
        var (mockDbConnection, databaseDAO) = SetupMock.SetupDatabaseDAO();
        // 配置 mockDbConnection
        mockDbConnection.Setup(m => m.State).Returns(ConnectionState.Closed);
        //設置模擬物件到主方法
        MockObjectModel objectModel =new(){
            IsTestingError=true,
            ErrorMessage=exMessage
        };
        databaseDAO.SetMockConnection(mockDbConnection.Object, objectModel);
        // Act
        var result = await Assert.ThrowsAsync<Exception>(()=>databaseDAO.OperationBulkInsertAsync(TestTable));

        // Assert
        Assert.Equal($"{ResultString.BulkInsertFailed}{exMessage}", result.Message);

        // Verify that OpenAsync and CloseAsync were called once
        mockDbConnection.Verify(m => m.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDbConnection.Verify(m => m.CloseAsync(), Times.Once);
    }
}
