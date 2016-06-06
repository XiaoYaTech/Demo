using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Collections.Generic;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public abstract class ExcelDataBase
    {
        private readonly FileInfo _fileInfo;
        private string _sheetName;

        private int _startRow;
        private int _endRow;
        private string _startColumn;
        private string _endColumn;

        public BaseAbstractEntity Entity;

        protected ExcelDataBase(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }


        public FileInfo FileInfo
        {
            get { return _fileInfo; }
        }

        public string SheetName
        {
            get { return _sheetName; }
            set { _sheetName = value; }
        }

        public int StartRow
        {
            get { return _startRow; }
            set { _startRow = value; }
        }

        public int EndRow
        {
            get { return _endRow; }
            set { _endRow = value; }
        }

        public string StartColumn
        {
            get { return _startColumn; }
            set { _startColumn = value; }
        }

        public string EndColumn
        {
            get { return _endColumn; }
            set { _endColumn = value; }
        }

        public abstract void Parse(ExcelWorksheet worksheet, int currRow);

        public abstract void Import();

        internal static T GetExcelRange<T>(ExcelWorksheet worksheet, int row, string col)
        {
            return ExcelHelper.GetExcelRange<T>(worksheet.Cells[row, ExcelHelper.ToExcelIndex(col)]);
        }

        public abstract void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo);

        public virtual void Edit(ExcelWorksheet worksheet, ExcelInputDTO excelInput) { }

    }

    public class ExcelHelper
    {
        public static int ToExcelIndex(string primaryKeyValue)
        {
            return ToIndex(primaryKeyValue) + 1;
        }

        public static string ToExcelTagName(int index)
        {
            return ToTagName(index - 1);
        }

        public static int ToIndex(string primaryKeyValue)
        {
            var tags = primaryKeyValue.ToCharArray();
            int index;
            if (tags.Length == 1)
            {
                index = tags[0] % 'A';
            }
            else
            {
                var position = tags.Select((t, i) => (ToIndex(t.ToString(CultureInfo.InvariantCulture)) + 1) * (int)Math.Pow(26, tags.Length - 1 - i)).Sum();
                index = position - 1;
            }
            return index;
        }

        public static string ToTagName(int index)
        {
            if ((index - index % 26) / 26 == 0)
            {
                return ((char)(index % 26 + 'A')).ToString(CultureInfo.InvariantCulture);
            }
            return ToTagName((index - index % 26) / 26 - 1) + ToTagName(index % 26);
        }


        internal static T GetExcelRange<T>(ExcelRange fieldValue)
        {
            var val = fieldValue.GetValue<T>();
            object result = val;

            if (val is string)
            {
                var strVal = val as string;
                result = !string.IsNullOrEmpty(strVal) ? strVal : string.Empty;
            }
            else if (val is int)
            {
                var intVal = val as int?;
                result = intVal.Equals(default(int)) ? null : intVal;
            }
            else if (val is DateTime)
            {
                var dateVal = val as DateTime?;
                result = dateVal == default(DateTime) ? null : dateVal.Value.ToString("yyyyMM");
            }

            return (T)result;
        }

        public static string GetExcelVersionNumber(string filePath)
        {
            using (var package = System.IO.Packaging.ZipPackage.Open(filePath))
            {
                return package.PackageProperties.Version;
            }
        }

        public static string GetExcelVersionNumber(Stream fileStream)
        {
            using (var package = System.IO.Packaging.ZipPackage.Open(fileStream))
            {
                return package.PackageProperties.Version;
            }
        }

        public static void UpdateExcelVersionNumber(string filePath, string version)
        {
            using (var package = System.IO.Packaging.ZipPackage.Open(filePath))
            {
                package.PackageProperties.Version = version;
            }
        }

        public static bool MatchVersionNumber(string filePath1, string filePath2)
        {
            var version1 = GetExcelVersionNumber(filePath1);
            var version2 = GetExcelVersionNumber(filePath2);
            return version1 == version2;
        }

        public static bool MatchVersionNumber(string templatePath, Stream uploadStream)
        {
            var versionTemplate = GetExcelVersionNumber(templatePath);
            var versionUpload = GetExcelVersionNumber(uploadStream);
            return versionTemplate == versionUpload;
        }

        public static string ResolveBooleanTypeFieldValue(bool? field)
        {
            var boolStr = "No";
            if (field.HasValue)
            {
                boolStr = field.Value ? "Yes" : "No";
            }

            return boolStr;
        }

    }
}
