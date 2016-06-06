using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ClosureCoverExcelData : ExcelDataBase
    {
        public ClosureCoverExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";
        }

        public override void Parse(OfficeOpenXml.ExcelWorksheet worksheet, int currRow)
        {
            throw new NotImplementedException();
        }

        public override void Import()
        {
            throw new NotImplementedException();
        }

        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.StoreNameEN;
            worksheet.Cells["B3"].Value = inputInfo.USCode;
            worksheet.Cells["B5"].Value = inputInfo.City;
            worksheet.Cells["B6"].Value = inputInfo.Market;
            worksheet.Cells["B7"].Value = inputInfo.ActualCloseDate;
        }
    }
}
