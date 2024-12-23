using System;
using System.Collections.Generic;

namespace Generic.StaticUtil.Models
{
    public class DataModel
    {
        /// <summary>
        /// 資料表結構
        /// </summary>
        public class TableSchemaModel
        {
            /// <summary>
            /// 資料表名稱
            /// </summary>
            public string TableName { get; set; } = string.Empty;
            /// <summary>
            /// 欄位名稱
            /// </summary>
            public List<SchemaColumnModel> SchemaColumns { get; set; }
            /// <summary>
            /// 複合唯一設定(key:複合約束的名稱,value:複合約束包含欄位名稱)
            /// </summary>
            public Dictionary<string, List<string>> CompositeUnique { get; set; }
        }
        /// <summary>
        /// 欄位結構
        /// </summary>
        public class SchemaColumnModel
        {
            private bool _isNonNegative;
            private bool _isPositive;
            /// <summary>
            /// 欄位名稱
            /// </summary>
            public string ColumnName { get; set; } = string.Empty;
            /// <summary>
            /// 資料類型(C#)
            /// </summary>
            public Type DataType { get; set; } = typeof(string);
            /// <summary>
            /// 長度，只對String類別有用
            /// </summary>
            public int Length { get; set; } = int.MaxValue;
            /// <summary>
            /// 預設值
            /// </summary>
            public string DefaultValue { get; set; } = null;
            /// <summary>
            /// 必須唯一
            /// </summary>
            public bool Unique { get; set; }
            /// <summary>
            /// 允許空值
            /// </summary>
            public bool AllowNulls { get; set; } = true;
            /// <summary>
            /// 必須大於等於0
            /// </summary>
            public bool IsNonNegative
            {
                get => _isNonNegative;
                set
                {
                    if (value && _isPositive) {
                        throw new InvalidOperationException("IsNonNegative and IsPositive cannot be true at the same time.");
                    }
                    _isNonNegative = value;
                }
            }
            /// <summary>
            /// 必須大於0
            /// </summary>
            public bool IsPositive
            {
                get => _isPositive;
                set
                {
                    if (value && _isNonNegative) {
                        throw new InvalidOperationException("IsNonNegative and IsPositive cannot be true at the same time.");
                    }
                    _isPositive = value;
                }
            }

        }
    }
}
