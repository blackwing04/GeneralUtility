using System;

namespace Generic.StaticUtil
{
    /// <summary>
    /// 提供通用的枚舉解析方法
    /// </summary>
    public static class EnumParser
    {
        /// <summary>
        /// 解析字串為指定的列舉類型
        /// </summary>
        /// <typeparam name="T">列舉類型</typeparam>
        /// <param name="value">要解析的字串</param>
        /// <returns>解析得到的列舉值</returns>
        public static T ParseStringToEnum<T>(string value) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                throw new AggregateException("Value cannot be null or empty.");

            if (!Enum.TryParse<T>(value, true, out T result))
                throw new AggregateException($"Invalid value for enum {typeof(T).Name}: {value}");

            return result;
        }

        /// <summary>
        /// 從字串解析出列舉的整數值
        /// </summary>
        /// <typeparam name="T">列舉類型</typeparam>
        /// <param name="value">要解析的字串</param>
        /// <returns>列舉的整數值</returns>
        public static int ParseStringToEnumValue<T>(string value) where T : struct, Enum
        {
            T enumValue = ParseStringToEnum<T>(value);
            return Convert.ToInt32(enumValue);
        }
    }
}
