namespace ExcelToolStandard.Services
{
    /// <summary>
    /// Excel工具類別工廠
    /// </summary>
    public class ExcelManagerFactory
    {
        /// <summary>
        /// 創建 IExcelManager介面並注入實現類別，如果不會使用DI可以調用此方法創建實例
        /// </summary>
        /// <returns>ExcelManager 實例</returns>
        public static IExcelManager CreateExcelManager()
        {
            return new ExcelManager();
        }
    }
}
