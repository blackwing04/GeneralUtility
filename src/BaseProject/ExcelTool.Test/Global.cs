global using Xunit;
global using Generic.StaticUtil;
global using ExcelCustomizeException;
global using static Generic.StaticUtil.Models.DataModel;
global using System.Data;
global using CustomizeException;
global using ExcelTool.Services;
global using ExcelTool.StaticUtil.Models;
global using ExcelTool.StaticUtil;
using ClosedXML.Excel;


namespace ExcelTool.Test
{
    public class GlobalUtil
    {
        public static readonly IExcelManager ExcelManager = ExcelManagerFactory.CreateExcelManager();
        public const string sheetName= "TestSheetName";


        /// <summary>
        /// �����쵲�c�C
        /// </summary>
        /// <returns>��^��쵲�c�C��C</returns>
        internal static List<SchemaColumnModel> GetSchemaColumns()
        {
            return new List<SchemaColumnModel>
            {
                new SchemaColumnModel { ColumnName = "StringCol", DataType = typeof(string), Unique = true },
                new SchemaColumnModel { ColumnName = "IntCol", DataType = typeof(int), AllowNulls = false },
                new SchemaColumnModel { ColumnName = "DoubleCol", DataType = typeof(double),
                    DefaultValue = "0.01", AllowNulls = false },
                new SchemaColumnModel { ColumnName = "DateTimeCol", DataType = typeof(DateTime) }
            };
        }
        /// <summary>
        /// ������M�g�C
        /// </summary>
        /// <returns>��^���M�g�r��C</returns>
        internal static Dictionary<string, string> GetColumnMapping()
        {
            return new Dictionary<string, string>
            {
                { "TestStringAndUnique", "StringCol" },
                { "TestIntAndNotAllowNulls", "IntCol" },
                { "TestDouble", "DoubleCol" },
                { "TestDateTime", "DateTimeCol" }
            };
        }

        /// <summary>
        /// ����������e�C
        /// </summary>
        /// <returns>��^���e�C��C</returns>
        internal static List<List<string>> GetContents()
        {
            return new List<List<string>>
            {
                new List<string> { "TestStringContent1", "123", "45.67", "2024-07-11" },
                new List<string> { "TestStringContent2", "123", "0.01", "" }
            };
        }

        /// <summary>
        /// �N���e�ഫ��DataTable�C
        /// </summary>
        /// <param name="schemaColumns">�Ҧ��C���C��A�Ω�w�q�C�C���ƾڵ��c�C</param>
        /// <param name="contents">�C�檺�ƾڡC</param>
        /// <returns>�ͦ���DataTable�C</returns>
        internal static DataTable CreateDataTable(List<SchemaColumnModel> schemaColumns, List<List<string>> contents)
        {
            DataTable dataTable = new();

            // �K�[�C��DataTable
            foreach (var schema in schemaColumns) {
                dataTable.Columns.Add(schema.ColumnName, schema.DataType);
            }

            // �K�[���DataTable
            foreach (var content in contents) {
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < content.Count; i++) {
                    if (string.IsNullOrEmpty(content[i])) {
                        // �p�G���e���šA�]�m�� DBNull
                        row[i] = DBNull.Value;
                    }
                    else {
                        row[i] = Convert.ChangeType(content[i], dataTable.Columns[i].DataType);
                    }
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// ��� Excel ���M ListConvertExcelModel �����e�C
        /// </summary>
        /// <param name="model">�C���ഫ�� Excel ���ҫ�</param>
        /// <param name="filePath">Excel ��󪺸��|</param>
        /// <returns>�p�G���e�@�P�A��^ True�A�_�h��^ False�C</returns>
        public static bool CompareExcelWithListModel(ListConvertExcelModel model, string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(model.SheetName);

            // �����Y�A�p�G������
            if (model.Header != null && model.Header.Count > 0) {
                for (int colIndex = 0; colIndex < model.Header.Count; colIndex++) {
                    string expectedHeader = model.Header[colIndex];
                    string actualHeader = worksheet.Cell(1, colIndex + 1).Value.ToString();
                    if (expectedHeader != actualHeader) {
                        return false;
                    }
                }
            }

            // ��鷺�e
            for (int rowIndex = 0; rowIndex < model.ContentList.Count; rowIndex++) {
                for (int colIndex = 0; colIndex < model.ContentList[rowIndex].Count; colIndex++) {
                    string expectedValue = model.ContentList[rowIndex][colIndex];
                    // ���ް����q+2�]��Excel�q1�}�l�B�Ĥ@��O���Y
                    string actualValue = worksheet.Cell(rowIndex + 2, colIndex + 1).Value.ToString();
                    if (expectedValue != actualValue) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}