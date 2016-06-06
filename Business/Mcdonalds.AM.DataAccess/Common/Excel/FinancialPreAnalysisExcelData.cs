using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    class FinancialPreAnalysisExcelData : ExcelDataBase
    {
        private readonly FinancialPreanalysis _financialPreanalysis;

        private readonly string _outputCol;
        public FinancialPreAnalysisExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";

            StartRow = 2;
            EndRow = 4;
            StartColumn = "E";
            EndColumn = "E";

            _outputCol = "E";

            _financialPreanalysis = new FinancialPreanalysis();
            Entity = _financialPreanalysis;
        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            var output = GetExcelRange<decimal>(worksheet, currRow, _outputCol).ToString();

            switch (currRow)
            {
                case 2:
                    _financialPreanalysis.ROI =(Convert.ToDecimal(output)*100).ToString() ;
                    break;
                case 3:
                    _financialPreanalysis.PaybackYears = output;
                    break;
                case 4:
                    _financialPreanalysis.MarginInc = output;
                    break;
                 

            }

        }


        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            //worksheet.Cells["B2"].Value = inputInfo.Region;
            //worksheet.Cells["B3"].Value = inputInfo.Market;
            //worksheet.Cells["B4"].Value = inputInfo.StoreName;
            //worksheet.Cells["B5"].Value = inputInfo.USCode;
            //worksheet.Cells["B6"].Value = inputInfo.StoreType;
            //if (inputInfo.ClosureDate != null && inputInfo.ClosureDate != default(DateTime))
            //{
            //    worksheet.Cells["B7"].Value = inputInfo.ClosureDate;
            //}

            //worksheet.Cells["B8"].Value = inputInfo.Region;
        }

        public override void Import()
        {
            var financialPreanalysis =
                    FinancialPreanalysis.FirstOrDefault(e => e.RefId.ToString().Equals(_financialPreanalysis.RefId.ToString()));
            if (financialPreanalysis != null)
            {
                _financialPreanalysis.Id = financialPreanalysis.Id;
            }
            if (FinancialPreanalysis.Any(a => a.Id == _financialPreanalysis.Id))
            {
                FinancialPreanalysis.Update(_financialPreanalysis);
            }
            else
            {
                FinancialPreanalysis.Add(_financialPreanalysis);
            }
        }
    }
}

