using ExcelToolStandard.Test;

namespace ExcelTool.Test;

public class DataTableConvertToExcelTest
{
    /// <summary>
    /// 模擬DataTable轉換成Excel成功的結果
    /// </summary>
    [Fact]
    public async Task ValidDataAsync_ReturnsSuccessfulResult()
    {
        // Arrange
        var schemaColumn = GlobalUtil.GetSchemaColumns();
        var contents = GlobalUtil.GetContents();
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");
        DataTable sourceData = GlobalUtil.CreateDataTable(schemaColumn, contents);

        try {
            // Act
            await GlobalUtil.ExcelManager.DataTableConvertToExcelAsync(sourceData, filePath);

            // Assert
            Assert.True(DataTableHelper.CompareExcelWithDataTable(sourceData, filePath));
        }
        finally {
            // 無論測試是否成功，都執行删除臨時文件
            File.Delete(filePath);
        }
    }
    /// <summary>
    /// 模擬DataTable轉換成Excel時帶入空DataTable後發生的結果
    /// </summary>
    [Fact]
    public async Task EmptyDataTableAsync_CreatesEmptyExcelFile()
    {
        // Arrange
        //錯誤訊息
        const string exMessage ="The DataTable is empty. " +
            "Please ensure the DataTable contains data before converting to Excel.";
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");

        DataTable sourceData = new();

        
        // Act
        var result = await Assert.ThrowsAsync<DataTableIsEmptyException>
            (()=>GlobalUtil.ExcelManager.DataTableConvertToExcelAsync(sourceData, filePath));

        // Assert
        Assert.Equal(exMessage, result.Message);
    }
    /// <summary>
    /// 模擬遺失存檔目標發生的結果
    /// </summary>
    [Fact]
    public async Task EmptySaveTargetAsync_MissingSaveTarget()
    {
        //錯誤訊息
        const string exMessage ="No valid save target specified. Please provide a file path or a memory stream.";

        // Arrange
        var schemaColumn = GlobalUtil.GetSchemaColumns();
        var contents = GlobalUtil.GetContents();

        DataTable sourceData = GlobalUtil.CreateDataTable(schemaColumn, contents);

        // Act
        var result = await Assert.ThrowsAsync<MissingSaveTargetException>
            (()=>GlobalUtil.ExcelManager.DataTableConvertToExcelAsync(sourceData));

        // Assert
        Assert.Equal(exMessage, result.Message);
    }
    /// <summary>
    /// 模擬DataTable轉換成Excel過程中發生錯誤的結果
    /// </summary>
    [Fact]
    public async Task ExceptionAsync_ThrowsDataTableConvertExcelException()
    {
        // Arrange
        var schemaColumn = GlobalUtil.GetSchemaColumns();
        var contents = GlobalUtil.GetContents();
        var invalidFilePath  = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        DataTable sourceData = GlobalUtil.CreateDataTable(schemaColumn, contents);

        // Act 
        var result = await Assert.ThrowsAsync<DataTableConvertExcelException>(() =>
        GlobalUtil.ExcelManager.DataTableConvertToExcelAsync(sourceData, invalidFilePath));

        // Assert
        Assert.True((result.Message.Contains("Error converting DataTable to Excel")
            && result.Message.Contains("is not supported.")));
    }
}