using System;
using System.Collections.Generic;

namespace Generic.StaticUtil
{
    /// <summary>
    /// 提供通用列舉靜態方法
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 解析字串為指定的列舉類型
        /// </summary>
        /// <typeparam name="T">列舉類型</typeparam>
        /// <param name="value">要解析的字串</param>
        /// <param name="ignoreCase">是否忽略大小寫(預設忽略)</param>
        /// <returns>解析得到的列舉值</returns>
        public static T ParseStringToEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                throw new AggregateException("Value cannot be null or empty.");

            if (!Enum.TryParse<T>(value, ignoreCase, out T result))
                throw new AggregateException($"Invalid value for enum {typeof(T).Name}: {value}");

            return result;
        }

        /// <summary>
        /// 從字串解析出列舉的整數值
        /// </summary>
        /// <typeparam name="T">列舉類型</typeparam>
        /// <param name="ignoreCase">是否忽略大小寫(預設忽略)</param>
        /// <param name="value">要解析的字串</param>
        /// <returns>列舉的整數值</returns>
        public static int ParseStringToEnumValue<T>(string value, bool ignoreCase = true) where T : struct, Enum
        {
            T enumValue = ParseStringToEnum<T>(value, ignoreCase);
            return Convert.ToInt32(enumValue);
        }

        /// <summary>
        /// 將列舉內容轉換為排序字典
        /// </summary>
        /// <typeparam name="T">列舉類型</typeparam>
        /// <returns>回傳列舉的排序字典</returns>
        public static SortedDictionary<int, string> EnumToSortedDictionary<T>() where T : Enum
        {
            SortedDictionary<int, string> sortedDict = new SortedDictionary<int, string>();

            foreach (T enumValue in Enum.GetValues(typeof(T))) {
                sortedDict.Add(Convert.ToInt32(enumValue), enumValue.ToString());
            }

            return sortedDict;
        }
        /// <summary>
        /// 驗證字串是否符合指定的列舉內容
        /// </summary>
        /// <typeparam name="TEnum">指定的列舉(泛型)</typeparam>
        /// <param name="value">驗證的字串</param>
        /// <param name="ignoreCase">是否忽略大小寫</param>
        /// <returns>是否符合指定的列舉內容</returns>
        public static bool IsValidEnumValue<TEnum>(string value, bool ignoreCase = false) where TEnum : struct, Enum
        {
            if (ignoreCase)
            {
                foreach (var name in Enum.GetNames(typeof(TEnum)))
                {
                    if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            return Enum.IsDefined(typeof(TEnum), value);
        }
        /// <summary>
        /// 驗證指定的整數值是否是列舉的有效值
        /// </summary>
        /// <typeparam name="TEnum">列舉類型</typeparam>
        /// <param name="value">整數值</param>
        /// <returns>是否是列舉的有效值</returns>
        public static bool IsValidEnumValue<TEnum>(int value) where TEnum : struct, Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        /// <summary>
        /// 將整數安全轉換為 Enum
        /// </summary>
        public static T ValueToEnum<T>(int value) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)(object)value;
            else
                throw new ArgumentException($"無效的值 {value} 對於 {typeof(T).Name}");
        }

        /// <summary>
        /// 嘗試轉換，失敗時回傳預設值
        /// </summary>
        public static T ValueToEnumOrDefault<T>(int value, T defaultValue = default) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value) ? (T)(object)value : defaultValue;
        }

    }
}
