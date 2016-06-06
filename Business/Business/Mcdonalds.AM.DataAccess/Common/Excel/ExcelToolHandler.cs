using Microsoft.Office.Interop.Excel;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ExcelToolHandler
    {
        private Application excelApp;
        private Workbook workBook;
        private Worksheet workSheet;

        public ExcelToolHandler(string filePath, string sheetName)
        {
            excelApp = new Application();
            workBook = excelApp.Workbooks.Open(filePath, CorruptLoad: true);

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
