using System.Collections.Generic;
using System.Linq;

namespace Generic.StaticUtil
{
    /// <summary>
    /// 提供<![CDATA[List<string>]]>的方法擴充
    /// </summary>
    public static class ListStringHelper
    {
        /// <summary>
        /// 獲取列表中的重複項
        /// </summary>
        /// <param name="items">要檢查的字符串列表</param>
        /// <returns>包含重複項的列表</returns>
        public static List<string> GetDuplicateItems(List<string> items)
        {
            return items.GroupBy(x => x)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();
        }
        /// <summary>
        /// 檢查列表中是否包含指定的字符串
        /// </summary>
        /// <param name="list">字符串列表</param>
        /// <param name="value">要檢查的字符串</param>
        /// <returns>如果找到指定的字符串，返回True；否則返回False。</returns>
        public static bool ContainsValue(List<string> list, string value)
        {
            return list.Contains(value);
        }
        /// <summary>
        /// 移除列表中的所有空白或Null元素
        /// </summary>
        /// <param name="list">字符串列表</param>
        public static void RemoveNullOrWhiteSpace(List<string> list)
        {
            list.RemoveAll(string.IsNullOrWhiteSpace);
        }
        /// <summary>
        /// 找到列表中最長的字符串
        /// </summary>
        /// <param name="list">字符串列表</param>
        /// <returns>列表中最長的字符串</returns>
        public static string GetLongestString(List<string> list)
        {
            return list.OrderByDescending(s => s.Length).FirstOrDefault();
        }
        /// <summary>
        /// 使用指定分隔符合併列表中的所有字符串
        /// </summary>
        /// <param name="list">字符串列表</param>
        /// <param name="separator">分隔符</param>
        /// <returns>合併後的字符串</returns>
        public static string JoinStrings(List<string> list, string separator)
        {
            return string.Join(separator, list);
        }
        /// <summary>
        /// 將列表中的每個字符串按指定分隔符進行分割，然後將結果展平為單一列表
        /// </summary>
        /// <param name="list">原始字符串列表</param>
        /// <param name="separator">分隔符</param>
        /// <returns>展平後的字符串列表</returns>
        public static List<string> SplitAndFlatten(List<string> list, char separator)
        {
            return list.SelectMany(s => s.Split(separator)).ToList();
        }

    }
}
