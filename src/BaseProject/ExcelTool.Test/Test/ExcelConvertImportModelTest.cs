namespace ExcelTool.Test;

public class ExcelConvertImportModelTest
{

    /// <summary>
    /// 模擬Excel轉換成ImportModel成功的結果
    /// </summary>
    [Fact]
    public async Task ConvertTest_ReturnsSuccessfulResult()
    {
        // Arrange
        var schemaColumn = GlobalUtil.GetSchemaColumns();
        var columnMapping = GlobalUtil.GetColumnMapping();
        var contents = GlobalUtil.GetContents();

        //暫存Excel內容
        TempExcel tempExcel = new(){
            Header= columnMapping.Keys.ToList(),
            Contents = contents,
        };

        //驗證設定
        ExcelMapperSetting excelMapper=new(){
            SchemaColumn=schemaColumn,
            ColumnMapping=columnMapping,
        };
        ExcelInfo mockInfo = new()
        {
            ExcelFilePath = ExcelContent.CreateTempExcelFile(GlobalUtil.sheetName,tempExcel),
            WorkSheetName = GlobalUtil.sheetName,
            ExcelMapper = excelMapper,
        };

        try {
            // Act
            var result = await GlobalUtil.ExcelManager.ExcelConvertToImportModelAsync(mockInfo);

            // Assert
            DataTable resultTable = GlobalUtil.CreateDataTable(schemaColumn, contents);
            Assert.True(DataTableHelper.IsDataTablesEqual(resultTable, result.ResultTable));
        }
        finally {
            // 删除臨時文件
            File.Delete(mockInfo.ExcelFilePath);
        }
    }
    /// <summary>
    /// 模擬Excel轉換成ImportModel時發生找不到檔案的結果
    /// </summary>
    [Fact]
    public async Task FileNotFound_ThrowsExcelFileLoadException()
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
        var result = await Assert.ThrowsAsync<ExcelFileLoadException>
            (()=>GlobalUtil.ExcelManager.ExcelConvertToImportModelAsync(mockInfo));

        // Assert
        Assert.Equal(exMessage, result.Message);
        Assert.Equal(excelFilePath, result.FilePath);
    }
    /// <summary>
    /// 模擬Excel轉換成ImportModel時發生讀取檔案錯誤的結果
    /// </summary>
    [Fact]
    public async Task IOException_ThrowsExcelFileLoadException()
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
            var result = await Assert.ThrowsAsync<ExcelFileLoadException>
                (()=>GlobalUtil.ExcelManager.ExcelConvertToImportModelAsync(mockInfo));

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
    /// 模擬Excel轉換成ImportModel過程中發生錯誤的結果
    /// </summary>
    [Fact]
    public async Task Exception_ThrowsExcelConvertToImportModelException()
    {
        //錯誤訊息
        const string exMessage ="Excel convert to import model Failed.Error:For Mock Test";
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
        var result = await Assert.ThrowsAsync<ExcelConvertToImportModelException>
            (()=>GlobalUtil.ExcelManager.ExcelConvertToImportModelAsync(mockInfo));
        // Assert
        //檢查內部錯誤是模擬例外
        var innerException = Assert.IsType<MockTestException>(result.InnerException);
        //確認錯誤訊息與預期的一致
        Assert.Equal(exMessage, result.Message);
    }
}