using ClosedXML.Excel;
using ConvertCustomizeException;
using static Generic.StaticUtil.Models.DataModel;

namespace ExcelTool.StaticUtils
{
    public static class DataConverter
    {
        /// <summary>
        /// 轉換儲存格的值至目標數據類型，並處理潛在的錯誤。
        /// </summary>
        /// <param name="cell">Excel 儲存格。</param>
        /// <param name="schemaColumn">欄位結構描述。</param>
        /// <param name="cellValue">儲存格內容。</param>
        /// <param name="columnList">該欄所有資料，用於檢查唯一性</param>
        /// <returns>轉換後的值。</returns>
        public static object ConvertCellValue(IXLCell cell, SchemaColumnModel schemaColumn
            , string cellValue, List<string> columnList)
        {
            Type targetType = schemaColumn.DataType;
            try {
                string? defaultValue = CheckedDefaultValueToDataType(targetType, schemaColumn.DefaultValue);
                bool hasDuplicate = columnList.Count(c => c == cellValue) > 1;
                //檢查重複
                if (schemaColumn.Unique && hasDuplicate)
                    throw new DuplicateDataException(schemaColumn.ColumnName, cellValue);
                //檢查空值
                if (string.IsNullOrWhiteSpace(cellValue) && !schemaColumn.AllowNulls)
                    cellValue = defaultValue ?? throw new CellBlankErrorException(schemaColumn.ColumnName);
                //如果通過空值檢查，數值為空則回傳DBNull
                if (string.IsNullOrWhiteSpace(cellValue)) {
                    return DBNull.Value;
                }
                //如果目標是時間型態
                if (targetType == typeof(DateTime)) {
                    if (cell.DataType == XLDataType.Number)
                        return ConvertToDateTime(cellValue);
                    else {
                        if (DateTime.TryParse(cellValue, out DateTime result))
                            return result; // 成功解析則返回 DateTime 對象
                        else
                            throw new InvalidDateTimeFormatException(); // 解析失敗拋出異常
                    }
                }
                //如果目標是布林型態
                if (targetType == typeof(bool))
                    return ConvertToBoolean(cellValue);
                //如果目標是字串型態
                if (targetType == typeof(string) && cellValue.Length > schemaColumn.Length)
                    throw new ExceedsMaximumLengthException();
                //如果必須大等於0
                if (schemaColumn.IsNonNegative) {
                    if (!float.TryParse(cellValue, out float val) || val < 0) {
                        throw new InvalidNumberFormatException();
                    }
                }
                //如果是必須為正數
                else if (schemaColumn.IsPositive) {
                    if (!float.TryParse(cellValue, out float val) || val <= 0) {
                        throw new InvalidNumberFormatException();
                    }
                }

                return Convert.ChangeType(cellValue, targetType);
            }
            catch { throw; }
        }

        /// <summary>
        /// 檢查預設值是否能轉換為指定的類型。
        /// 轉換成功則返回預設字串，轉換失敗則返回 null。
        /// </summary>
        /// <param name="targetType">要轉換到的目標類型。</param>
        /// <param name="defaultValue">要檢查的字串預設值。</param>
        /// <returns>轉換後的字串，或在無法轉換時返回 null。</returns>
        private static string? CheckedDefaultValueToDataType(Type targetType, string defaultValue)
        {
            // 如果預設值為空或只包含空白字符，則無需進行轉換
            if (string.IsNullOrWhiteSpace(defaultValue)) {
                return null;
            }

            try {
                // 獲取非可空的基底類型，以處理可空類型
                var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // 嘗試將字串預設值轉換為指定的類型
                var convertedValue = Convert.ChangeType(defaultValue, nonNullableType);

                // 如果轉換成功，返回轉換後的字串值
                return defaultValue;
            }
            catch {
                // 如果轉換過程中出現任何異常，則返回 null
                return null;
            }
        }

        /// <summary>
        /// 轉換字串為布林值。接受 "Y", "YES", "N", "NO" 等值，不區分大小寫。
        /// </summary>
        /// <param name="input">待轉換的字串。</param>
        /// <returns>對應的布林值，如果輸入無效返回 null。</returns>
        private static bool ConvertToBoolean(string input)
        {
            var booleanMappings = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
            {
                {"Y", true}, {"YES", true},
                {"N", false}, {"NO", false},
                {"TRUE", true}, {"FALSE", false},
                {"1", true}, {"0", false}
            };

            if (booleanMappings.TryGetValue(input.Trim(), out bool result)) {
                return result;
            }
            throw new UnableToConvertToBooleanException();
        }
        /// <summary>
        /// 將值轉成時間型態
        /// </summary>
        /// <param name="cellValue">欄位值</param>
        /// <returns>返回轉換好的時間型態</returns>
        private static DateTime ConvertToDateTime(string cellValue)
        {
            string errorMessage;
            if (double.TryParse(cellValue, out double numericValue)) {
                try {
                    return DateTime.FromOADate(numericValue);
                }
                catch (ArgumentException ex) {
                    errorMessage = $"Convert {cellValue} To DateTime failed,Error: {ex.Message}";
                    throw new ArgumentException(errorMessage, ex);
                }
            }
            errorMessage = $"Convert {cellValue} To DateTime failed";
            throw new ArgumentException(errorMessage);
        }


    }
}
