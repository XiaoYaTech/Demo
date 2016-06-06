using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    class WriteOffAmountExcelData : ExcelDataBase
    {
        private readonly WriteOffAmount _writeOffAmount;

        private readonly string _outputCol;
        public WriteOffAmountExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";

            StartRow = 2;
            EndRow = 43;
            StartColumn = "D";
            EndColumn = "E";

            _outputCol = "E";

            _writeOffAmount = new WriteOffAmount();
            Entity = _writeOffAmount;
        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            var output = GetExcelRange<decimal>(worksheet, currRow, _outputCol).ToString();

            switch (currRow)
            {
                case 2:
                    _writeOffAmount.REII = output;
                    break;
                case 3:
                    _writeOffAmount.LHIII = output;
                    break;
                case 4:
                    _writeOffAmount.ESSDII = output;
                    break;
                case 5:
                    _writeOffAmount.EquipmentII = output;
                    break;
                case 6:
                    _writeOffAmount.SignageII = output;
                    break;
                case 7:
                    _writeOffAmount.SeatingII = output;
                    break;
                case 8:
                    _writeOffAmount.DecorationII = output;
                    break;
                case 9:
                    _writeOffAmount.RENBV = output;
                    break;
                case 10:
                    _writeOffAmount.LHINBV = output;
                    break;
                case 11:
                    _writeOffAmount.ESSDNBV = output;
                    break;
                case 12:
                    _writeOffAmount.EquipmentNBV = output;
                    break;
                case 13:
                    _writeOffAmount.SignageNBV = output;
                    break;
                case 14:
                    _writeOffAmount.SeatingNBV = output;
                    break;
                case 15:
                    _writeOffAmount.DecorationNBV = output;
                    break;
                case 17:
                    _writeOffAmount.TotalII = output;
                    break;
                case 18:
                    _writeOffAmount.TotalNBV = output;
                    break;
                case 19:
                    _writeOffAmount.TotalWriteOff = output;
                    break;
                case 20:
                    _writeOffAmount.REWriteOff = output;
                    break;
                case 21:
                    _writeOffAmount.LHIWriteOff = output;
                    break;
                case 22:
                    _writeOffAmount.ESSDWriteOff = output;
                    break;
                case 23:
                    _writeOffAmount.EquipmentWriteOff = output;
                    break;
                case 24:
                    _writeOffAmount.SignageWriteOff = output;
                    break;
                case 25:
                    _writeOffAmount.SeatingWriteOff = output;
                    break;
                case 26:
                    _writeOffAmount.DecorationWriteOff = output;
                    break;
                case 27:
                    _writeOffAmount.ClosingCostBudegt = output;
                    break;
                case 28:
                    _writeOffAmount.ClosingCostActual = output;
                    break;
                case 29:
                    _writeOffAmount.TotalActual = output;
                    break;
                case 30:
                    _writeOffAmount.REActual = output;
                    break;
                case 31:
                    _writeOffAmount.LHIActual = output;
                    break;
                case 32:
                    _writeOffAmount.EquipmentActual = output;
                    break;
                case 33:
                    _writeOffAmount.SignageActual = output;
                    break;
                case 34:
                    _writeOffAmount.SeatingActual = output;
                    break;
                case 35:
                    _writeOffAmount.DecorationActual = output;
                    break;
                case 40:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _writeOffAmount.ExpFAActVsReCost = output;
                    break;
                case 41:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _writeOffAmount.ExpFAActVsLHI = output;
                    break;
                case 42:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _writeOffAmount.ExpFAActVsESSD = output;
                    break;
                case 43:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _writeOffAmount.ExpFAActVsTotal = output;
                    break;

            }

        }


        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.Region;
            worksheet.Cells["B3"].Value = inputInfo.Market;
            worksheet.Cells["B4"].Value = inputInfo.StoreName;
            worksheet.Cells["B5"].Value = inputInfo.USCode;
            worksheet.Cells["B6"].Value = inputInfo.StoreTypeName;
            if (inputInfo.ClosureDate != null && inputInfo.ClosureDate != default(DateTime))
            {
                worksheet.Cells["B7"].Value = inputInfo.ClosureDate;
            }

            worksheet.Cells["B8"].Value = inputInfo.PMNameENUS;
        }

        public override void Import()
        {
            var writeoff =
                    WriteOffAmount.FirstOrDefault(e => e.ConsInfoID.ToString().Equals(_writeOffAmount.ConsInfoID.ToString()));
            if (writeoff != null)
            {
                _writeOffAmount.Id = writeoff.Id;
            }
            if (WriteOffAmount.Any(a => a.Id == _writeOffAmount.Id))
            {
                WriteOffAmount.Update(_writeOffAmount);
            }
            else
            {
                WriteOffAmount.Add(_writeOffAmount);
            }
        }
    }
}
