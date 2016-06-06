/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   11/4/2014 10:43:22 AM
 * FileName     :   RenewalPackageCoverExcelData
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class RenewalCoverExcelData:ExcelDataBase
    {
        public RenewalCoverExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            this.SheetName = "PMT";
        }
        public override void Parse(OfficeOpenXml.ExcelWorksheet worksheet, int currRow)
        {
            throw new NotImplementedException();
        }

        public override void Import()
        {
            throw new NotImplementedException();
        }

        public override void Input(OfficeOpenXml.ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.Region;
            worksheet.Cells["B3"].Value = inputInfo.Province;
            worksheet.Cells["B4"].Value = inputInfo.City;
            worksheet.Cells["B5"].Value = inputInfo.Market;
            worksheet.Cells["B6"].Value = inputInfo.StoreName;
            worksheet.Cells["B7"].Value = inputInfo.USCode;
            worksheet.Cells["B8"].Value = inputInfo.OpenDate.ToString("yyyy-MM-dd");
            worksheet.Cells["B9"].Value = inputInfo.LeaseExpirationDate.ToString("yyyy-MM-dd");
            worksheet.Cells["B10"].Value = inputInfo.Priority;
            
        }
    }
}
