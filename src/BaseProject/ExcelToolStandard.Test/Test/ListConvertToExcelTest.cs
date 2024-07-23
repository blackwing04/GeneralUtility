using ExcelToolStandard.Test;

namespace ExcelTool.Test;

public class ListConvertToExcelTest
{
    /// <summary>
    /// 模擬ListConvertExcelModel轉換成Excel成功的結果
    /// </summary>
    [Fact]
    public async Task ValidListAsync_ReturnsSuccessfulResult()
    {
        // Arrange
        var header = GlobalUtil.GetColumnMapping().Keys.ToList();
        var contents = GlobalUtil.GetContents();
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");

        ListConvertExcelModel sourceData = new(contents,header);
        try {
            //Act
            await GlobalUtil.ExcelManager.ListConvertToExcelAsync(sourceData, filePath);

            //Assert
            Assert.True(GlobalUtil.CompareExcelWithListModel(sourceData, filePath));
        }
        finally {
            // 删除臨時文件
            File.Delete(filePath);
        }
    }
    /// <summary>
    /// 模擬ListConvertExcelModel轉換成Excel時帶入空Content後發生的結果
    /// </summary>
    [Fact]
    public async Task EmptyListAsync_CreatesEmptyExcelFile()
    {
        // Arrange
        //錯誤訊息
        const string exMessage ="The List is empty. Please ensure the List contains data before converting to Excel.";
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");
        ListConvertExcelModel listConvertExcel = new(new List<List<string>>());

        // Act 
        var result = await Assert.ThrowsAsync<ListIsEmptyException>(() =>
        GlobalUtil.ExcelManager.ListConvertToExcelAsync(listConvertExcel, filePath));

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
        var contents = GlobalUtil.GetContents();

        ListConvertExcelModel listConvertExcel = new(contents);

        // Act
        var result = await Assert.ThrowsAsync<MissingSaveTargetException>
            (()=>GlobalUtil.ExcelManager.ListConvertToExcelAsync(listConvertExcel));

        // Assert
        Assert.Equal(exMessage, result.Message);
    }
    /// <summary>
    /// 模擬ListConvertExcelModel轉換成Excel過程中發生錯誤的結果
    /// </summary>
    [Fact]
    public async Task ExceptionAsync_ThrowsListConvertExcelException()
    {

        // Arrange
        var contents = GlobalUtil.GetContents();
        var invalidFilePath  = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        ListConvertExcelModel listConvertExcel = new(contents);

        // Act 
        var result = await Assert.ThrowsAsync<ListConvertExcelException>(() =>
        GlobalUtil.ExcelManager.ListConvertToExcelAsync(listConvertExcel, invalidFilePath));

        // Assert
        Assert.True((result.Message.Contains("Error converting list to Excel")
            && result.Message.Contains("is not supported.")));
    }
}