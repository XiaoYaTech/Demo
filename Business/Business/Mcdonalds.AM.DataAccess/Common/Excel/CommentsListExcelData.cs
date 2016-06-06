using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class CommentsListExcelData : ExcelDataBase
    {
        public int RowNum = 2;
        public CommentsListExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "Sheet1";
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
            worksheet.Cells["A" + RowNum].Value = inputInfo.Form.AsString();
            worksheet.Cells["B" + RowNum].Value = inputInfo.Comments.AsString();
            worksheet.Cells["C" + RowNum].Value = inputInfo.CreateBy.AsString();
            worksheet.Cells["D" + RowNum].Value = inputInfo.SendTo.AsString();
            worksheet.Cells["E" + RowNum].Value = inputInfo.CreateDate.AsString();
            worksheet.Cells["F" + RowNum].Value = inputInfo.SenderPosition.AsString();
            worksheet.Cells["G" + RowNum].Value = inputInfo.IsRead.AsString();
            RowNum++;
        }
    }
}
