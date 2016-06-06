using System;
using System.IO;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ExcelDataImportDirector
    {
        private readonly ExcelDataBase _excelData;
        private readonly ExcelDataImportType _importType;

        public delegate void FillEntityEventHandler(BaseAbstractEntity entity);

        public FillEntityEventHandler FillEntityEvent;
        public ExcelDataBase ExcelData
        {
            get { return _excelData; }
        }

        public ExcelDataImportDirector(FileInfo fileInfo, ExcelDataImportType importType)
        {
            _importType = importType;
            switch (_importType)
            {
                case ExcelDataImportType.WriteOffAmount:
                    _excelData = new WriteOffAmountExcelData(fileInfo);
                    break;
                case ExcelDataImportType.ReinvestmentCost:
                    _excelData = new ReinvestmentCostExcelData(fileInfo);
                    break;
                case ExcelDataImportType.RenewalTool:
                    _excelData = new RenewalToolExcelData(fileInfo);
                    break;
                case ExcelDataImportType.RenewalAnalysis:
                    _excelData = new RenewalAnalysisExcelData(fileInfo);
                    break;
                case ExcelDataImportType.ClosureWOCheckList:
                    _excelData = new ClosureWOCheckListExcelData(fileInfo);
                    break;
                case ExcelDataImportType.FinancialPreAnalysis:
                    _excelData = new FinancialPreAnalysisExcelData(fileInfo);
                    break;
            }
        }

        public ExcelDataImportDirector(ExcelDataBase excelData)
        {
            _excelData = excelData;
        }


        public void Parse()
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];
                if (worksheet == null)
                {
                    throw new Exception(string.Format("Could not find the Sheet Name {0}", _excelData.SheetName));
                }

                for (var row = _excelData.StartRow; row <= _excelData.EndRow; row++)
                {
                    _excelData.Parse(worksheet, row);
                }
            }
        }


        public void Import()
        {
            if (FillEntityEvent != null)
            {
                FillEntityEvent(_excelData.Entity);
            }
            _excelData.Import();
        }

        public void ParseAndImport()
        {
            Parse();
            //if (FillEntityEvent != null)
            //{
            //    FillEntityEvent(_excelData.Entity);
            //}
            Import();
        }

        public string GetCellValue(int row, string col)
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];

                if (worksheet == null)
                    return "";

                return ExcelHelper.GetExcelRange<string>(worksheet.Cells[row, ExcelHelper.ToExcelIndex(col)]);
            }
        }

    }

    public enum ExcelDataImportType
    {
        ReinvestmentCost,
        WriteOffAmount,
        RenewalTool,
        RenewalAnalysis,
        ClosureWOCheckList,
        FinancialPreAnalysis
    }
}
