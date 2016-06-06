using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.IO;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    class ClosureWOCheckListExcelData : ExcelDataBase
    {
        private readonly ClosureWOCheckList _closureWOCheckList;

        private readonly string _outputCol;

        public ClosureWOCheckListExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";

            StartRow = 2;
            EndRow = 43;
            StartColumn = "D";
            EndColumn = "E";

            _outputCol = "E";

            _closureWOCheckList = new ClosureWOCheckList();
            Entity = _closureWOCheckList;
        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            string output;
            try
            {
                output = GetExcelRange<string>(worksheet, currRow, _outputCol).ToString();
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteErrorLog(JsonConvert.SerializeObject(new { ex, ExcelRange = string.Format("{0}{1}", _outputCol, currRow) }));
                output = "0";
            }

            switch (currRow)
            {
                case 2:
                    _closureWOCheckList.RE_Original = TryParseDecimal(output);
                    break;
                case 3:
                    _closureWOCheckList.LHI_Original = TryParseDecimal(output);
                    break;
                case 4:
                    _closureWOCheckList.ESSD_Original = TryParseDecimal(output);
                    break;
                case 5:
                    _closureWOCheckList.Equipment_Original = TryParseDecimal(output);
                    break;
                case 6:
                    _closureWOCheckList.Signage_Original = TryParseDecimal(output);
                    break;
                case 7:
                    _closureWOCheckList.Seating_Original = TryParseDecimal(output);
                    break;
                case 8:
                    _closureWOCheckList.Decoration_Original = TryParseDecimal(output);
                    break;
                case 9:
                    _closureWOCheckList.RE_NBV = TryParseDecimal(output);
                    break;
                case 10:
                    _closureWOCheckList.LHI_NBV = TryParseDecimal(output);
                    break;
                case 11:
                    _closureWOCheckList.ESSD_NBV = TryParseDecimal(output);
                    break;
                case 12:
                    _closureWOCheckList.Equipment_NBV = TryParseDecimal(output);
                    break;
                case 13:
                    _closureWOCheckList.Signage_NBV = TryParseDecimal(output);
                    break;
                case 14:
                    _closureWOCheckList.Seating_NBV = TryParseDecimal(output);
                    break;
                case 15:
                    _closureWOCheckList.Decoration_NBV = TryParseDecimal(output);
                    break;
                case 16:
                    _closureWOCheckList.EquipmentTransfer = TryParseDecimal(output);
                    break;
                case 17:
                    _closureWOCheckList.TotalCost_Original = TryParseDecimal(output);
                    break;
                case 18:
                    _closureWOCheckList.TotalCost_NBV = TryParseDecimal(output);
                    break;
                case 19:
                    _closureWOCheckList.TotalCost_WriteOFF = TryParseDecimal(output);
                    break;
                case 20:
                    _closureWOCheckList.RECost_WriteOFF = TryParseDecimal(output);
                    break;
                case 21:
                    _closureWOCheckList.LHI_WriteOFF = TryParseDecimal(output);
                    break;
                case 22:
                    _closureWOCheckList.ESSD_WriteOFF = TryParseDecimal(output);
                    break;
                case 23:
                    _closureWOCheckList.Equipment_WriteOFF = TryParseDecimal(output);
                    break;
                case 24:
                    _closureWOCheckList.Signage_WriteOFF = TryParseDecimal(output);
                    break;
                case 25:
                    _closureWOCheckList.Seating_WriteOFF = TryParseDecimal(output);
                    break;
                case 26:
                    _closureWOCheckList.Decoration_WriteOFF = TryParseDecimal(output);
                    break;
                case 27:
                    _closureWOCheckList.ClosingCost = TryParseDecimal(output);
                    break;
            }

        }

        private decimal TryParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            decimal _v;
            if (!decimal.TryParse(value, out _v))
                PluploadHandler.WriteErrorMsg("输入格式错误");
            return _v;
        }


        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.Region;
            worksheet.Cells["B3"].Value = inputInfo.Market;
            worksheet.Cells["B4"].Value = inputInfo.StoreName;
            worksheet.Cells["B5"].Value = inputInfo.USCode;
            worksheet.Cells["B6"].Value = inputInfo.StoreTypeName;
            worksheet.Cells["B7"].Value = inputInfo.ActualCloseDate;
            worksheet.Cells["B8"].Value = inputInfo.PMNameENUS;
        }

        public override void Import()
        {
            var woCheckList = ClosureWOCheckList.FirstOrDefault(e => e.Id.ToString().Equals(_closureWOCheckList.Id.ToString()));
            if (woCheckList != null)
            {
                woCheckList.RE_Original = _closureWOCheckList.RE_Original;
                woCheckList.LHI_Original = _closureWOCheckList.LHI_Original;
                woCheckList.ESSD_Original = _closureWOCheckList.ESSD_Original;
                woCheckList.Equipment_Original = _closureWOCheckList.Equipment_Original;
                woCheckList.Signage_Original = _closureWOCheckList.Signage_Original;
                woCheckList.Seating_Original = _closureWOCheckList.Seating_Original;
                woCheckList.Decoration_Original = _closureWOCheckList.Decoration_Original;
                woCheckList.RE_NBV = _closureWOCheckList.RE_NBV;
                woCheckList.LHI_NBV = _closureWOCheckList.LHI_NBV;
                woCheckList.ESSD_NBV = _closureWOCheckList.ESSD_NBV;
                woCheckList.Equipment_NBV = _closureWOCheckList.Equipment_NBV;
                woCheckList.Signage_NBV = _closureWOCheckList.Signage_NBV;
                woCheckList.Seating_NBV = _closureWOCheckList.Seating_NBV;
                woCheckList.Decoration_NBV = _closureWOCheckList.Decoration_NBV;
                woCheckList.EquipmentTransfer = _closureWOCheckList.EquipmentTransfer;
                woCheckList.TotalCost_Original = _closureWOCheckList.TotalCost_Original;
                woCheckList.TotalCost_NBV = _closureWOCheckList.TotalCost_NBV;
                woCheckList.TotalCost_WriteOFF = _closureWOCheckList.TotalCost_WriteOFF;
                woCheckList.RECost_WriteOFF = _closureWOCheckList.RECost_WriteOFF;
                woCheckList.LHI_WriteOFF = _closureWOCheckList.LHI_WriteOFF;
                woCheckList.ESSD_WriteOFF = _closureWOCheckList.ESSD_WriteOFF;
                woCheckList.Equipment_WriteOFF = _closureWOCheckList.Equipment_WriteOFF;
                woCheckList.Signage_WriteOFF = _closureWOCheckList.Signage_WriteOFF;
                woCheckList.Seating_WriteOFF = _closureWOCheckList.Seating_WriteOFF;
                woCheckList.Decoration_WriteOFF = _closureWOCheckList.Decoration_WriteOFF;
                woCheckList.ClosingCost = _closureWOCheckList.ClosingCost;

                ClosureWOCheckList.Update(woCheckList);
            }
            else
            {
                ClosureWOCheckList.Add(_closureWOCheckList);
            }
        }
    }
}
