namespace ExcelTool.Test;

public class GetExcelHeaderNameTest
{
    /// <summary>
    /// 模擬取得Excel表頭名稱(第一行是表頭)成功的結果
    /// </summary>
    [Fact]
    public void FirstRowIsHeaderConvertTest_ReturnsSuccessfulResult()
    {
        // Arrange
        List<string> headerName = new();
        for (int i = 1; i < 30; i++) {
            headerName.Add($"ColName{i}");
        }
        //準備暫存Excel內容
        TempExcel tempExcel = new(){
            Header = headerName
        };
        ExcelInfo mockInfo = new()
        {
            ExcelFilePath = ExcelContent.CreateTempExcelFile(GlobalUtil.sheetName,tempExcel),
            WorkSheetName = GlobalUtil.sheetName
        };
        try {
            // Act
            List<string> result = GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo);

            // Assert
            Assert.Equal(headerName, result);
        }
        finally {
            // 删除臨時文件
            File.Delete(mockInfo.ExcelFilePath);
        }
    }
    /// <summary>
    /// 模擬取得Excel表頭名稱(第一行不是表頭)成功的結果
    /// </summary>
    [Fact]
    public void FirstRowNotHeaderConvertTest_ReturnsSuccessfulResult()
    {
        // Arrange
        List<string> content = new();
        List<string> resultHeaderName = new();
        for (int i = 1; i < 30; i++) {
            content.Add($"Content{i}");
            resultHeaderName.Add(ExcelHeader.GetExcelColumnName(i));
        }
        //準備暫存Excel內容
        TempExcel tempExcel = new(){
            Contents = new List<List<string>>(){ content },
            FirstRowIsHeader = false
        };
        ExcelInfo mockInfo = new()
        {
            ExcelFilePath = ExcelContent.CreateTempExcelFile(GlobalUtil.sheetName,tempExcel),
            WorkSheetName = GlobalUtil.sheetName,
            FirstRowIsHeader = false
        };
        try {
            // Act
            List<string> result = GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo);

            // Assert
            Assert.Equal(resultHeaderName, result);
        }
        finally {
            // 删除臨時文件
            File.Delete(mockInfo.ExcelFilePath);
        }
    }
    /// <summary>
    /// 模擬取得Excel表頭名稱時發生表頭名稱重複的結果
    /// </summary>
    [Fact]
    public void DuplicateHeaderName_ThrowsDuplicateHeaderNameException()
    {
        // Arrange
        List<string> headerName = new();
        for (int i = 1; i < 10; i++) {
            headerName.Add($"ColName");
        }
        //重複的表頭
        var duplicates = ListStringHelper.GetDuplicateItems(headerName);
        //錯誤訊息
        string exMessage = $"The column names are duplicated: { string.Join(", ", duplicates)}";
        //準備暫存Excel內容
        TempExcel tempExcel = new(){
            Header =headerName
        };
        ExcelInfo mockInfo = new()
        {
            ExcelFilePath = ExcelContent.CreateTempExcelFile(GlobalUtil.sheetName,tempExcel),
            WorkSheetName = GlobalUtil.sheetName,
        };
        try {
            // Act
            var result = Assert.Throws<DuplicateHeaderNameException>(()=>GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo));

            // Assert
            Assert.Equal(exMessage, result.Message);
            Assert.Equal(duplicates, result.HeaderNames);
        }
        finally {
            // 删除臨時文件
            File.Delete(mockInfo.ExcelFilePath);
        }
    }
    /// <summary>
    /// 模擬取得Excel表頭名稱時發生找不到檔案的結果
    /// </summary>
    [Fact]
    public void FileNotFound_ThrowsExcelFileLoadException()
    {
        //Excel路徑
        string excelFilePath =Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");
        //錯誤訊息
        string exMessage = $"Cannot Open {excelFilePath}.Error:Could not find document";
        // Arrange
        ExcelInfo mockInfo = new()
        {
            //隨便生成一個Excel路徑但不創建
            ExcelFilePath= excelFilePath,
            WorkSheetName = GlobalUtil.sheetName,
        };
        // Act
        var result = Assert.Throws<ExcelFileLoadException>(()=>GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo));

        // Assert
        Assert.Equal(exMessage, result.Message);
        Assert.Equal(excelFilePath, result.FilePath);
    }
    /// <summary>
    /// 模擬取得Excel表頭名稱時發生讀取檔案錯誤的結果
    /// </summary>
    [Fact]
    public void IOException_ThrowsExcelFileLoadException()
    {
        //Excel路徑
        string excelFilePath =ExcelContent.CreateTempExcelFile(GlobalUtil.sheetName);

        //錯誤訊息
        string exMessage = $"Cannot Open {excelFilePath}." +
            $"Error:The process cannot access the file '{excelFilePath}' because it is being used by another process.";
        try {
            // 模擬文件正在被使用
            using var stream = new FileStream(excelFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            // Arrange
            ExcelInfo mockInfo = new()
            {
                ExcelFilePath= excelFilePath,
                WorkSheetName = GlobalUtil.sheetName,
            };

            // Act
            var result = Assert.Throws<ExcelFileLoadException>(()=>GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo));

            // Assert
            Assert.Equal(exMessage, result.Message);
            Assert.Equal(excelFilePath, result.FilePath);
        }
        finally {
            // 删除臨時文件
            File.Delete(excelFilePath);
        }
    }
    /// <summary>
    /// 模擬取得Excel表頭名稱過程中發生錯誤的結果
    /// </summary>
    [Fact]
    public void Exception_ThrowsGetExcelHeaderException()
    {
        //錯誤訊息
        const string exMessage ="Get Excel Header Failed.Error:For Mock Test";
        //Excel路徑
        string excelFilePath =Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xlsx");

        // Arrange
        ExcelInfo mockInfo = new()
        {
            //隨便生成一個Excel路徑但不創建
            ExcelFilePath= excelFilePath,
            WorkSheetName = GlobalUtil.sheetName,
            MockException=true
        };
        // Act
        var result = Assert.Throws<GetExcelHeaderException>(()=>GlobalUtil.ExcelManager.GetExcelHeaderName(mockInfo));
        // Assert
        //檢查內部錯誤是模擬例外
        var innerException = Assert.IsType<MockTestException>(result.InnerException);
        //確認錯誤訊息與預期的一致
        Assert.Equal(exMessage, result.Message);
    }
}