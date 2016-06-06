using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public class ExcelHanlder
    {
        private Application excelApp;
        private Workbook workBook;
        private Worksheet workSheet;

        public ExcelHanlder(string filePath, string sheetName)
        {
            excelApp = new Application();
            workBook = excelApp.Workbooks.Open(filePath);

            foreach (Worksheet sheet in workBook.Sheets)
            {
                if (sheet.Name == sheetName)
                {
                    workSheet = sheet;
                }
            }
        }

        public void Dispose()
        {
            workBook.Close(false);
            excelApp.Quit();
        }

        public object GetCellValue(string cell)
        {
            var range = workSheet.get_Range(cell);
            return range.Value;
        }
    }
}