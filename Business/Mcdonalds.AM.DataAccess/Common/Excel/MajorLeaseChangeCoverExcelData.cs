using System;
using System.IO;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class MajorLeaseChangeCoverExcelData : ExcelDataBase
    {
        public MajorLeaseChangeCoverExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";

        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            throw new NotImplementedException();
        }

        public override void Import()
        {
            throw new NotImplementedException();
        }

        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.Region.AsString();
            worksheet.Cells["B3"].Value = inputInfo.Province.AsString();
            worksheet.Cells["B4"].Value = inputInfo.City.AsString();
            worksheet.Cells["B5"].Value = inputInfo.Market.AsString();
            worksheet.Cells["B6"].Value = inputInfo.StoreName.AsString();
            worksheet.Cells["B7"].Value = inputInfo.USCode.AsString();
            worksheet.Cells["B8"].Value = inputInfo.OpenDate;
            //worksheet.Cells["B9"].Value = inputInfo.Region.AsString();
            //worksheet.Cells["B10"].Value = inputInfo.Region.AsString();
            //worksheet.Cells["B11"].Value = inputInfo.Region.AsString();

        }
    }
}
