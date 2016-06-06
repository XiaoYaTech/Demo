/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/18/2014 2:17:01 PM
 * FileName     :   RenewalLLNegotiationRecordData
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
    public class RenewalLLNegotiationRecordData:ExcelDataBase
    {
        private int RowNumber = 0;
        public RenewalLLNegotiationRecordData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "AM";
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
            string strRow = (3+RowNumber).ToString();
            worksheet.Cells["A" + strRow].Value = (RowNumber + 1).ToString();
            worksheet.Cells["B" + strRow].Value = inputInfo.McdParticipants;
            worksheet.Cells["C" + strRow].Value = inputInfo.Content;
            worksheet.Cells["D" + strRow].Value = inputInfo.LLParticipants;
            worksheet.Cells["E" + strRow].Value = inputInfo.Topic;
            worksheet.Cells["F" + strRow].Value = inputInfo.Location;
            worksheet.Cells["G" + strRow].Value = inputInfo.MeetingDate;
            worksheet.Cells["H" + strRow].Value = inputInfo.CreateDate;
            RowNumber += 1;
        }
    }
}
