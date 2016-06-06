using System;
using System.IO;
using System.Linq;
using Mcdonalds.AM.DataAccess.Entities;
using OfficeOpenXml;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Common.Extensions;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ReimageSummaryExcelData : ExcelDataBase
    {
        public ReimageSummaryExcelData(FileInfo fileInfo)
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
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == inputInfo.ProjectId);
            if (projectInfo == null)
            {
                throw new Exception("Cannot find the project info!");
            }
            var store = StoreBasicInfo.Search(e => e.StoreCode == projectInfo.USCode).FirstOrDefault();
            if (store == null)
            {
                throw new Exception("Cannot find Store info!");
            }

            var reimageSummary = BaseWFEntity.GetWorkflowEntity(inputInfo.ProjectId, FlowCode.Reimage_Summary) as ReimageSummary;
            if (reimageSummary == null)
            {
                throw new Exception("Cannot find Reimage Summary Info!");
            }
            var storeSTLocation = StoreSTLocation.GetStoreSTLocation(projectInfo.USCode);
            var storeLeaseInfo = new StoreProfitabilityAndLeaseInfo();
            var reimageConsInfo = ReimageConsInfo.GetConsInfo(inputInfo.ProjectId);
            var reinvestment = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
            if (reimageSummary != null)
            {
                storeLeaseInfo =
                   StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == reimageSummary.Id);
                if (storeLeaseInfo != null)
                {
                    if (storeLeaseInfo.LastRemodelDate.HasValue && storeLeaseInfo.LastRemodelDate.Value.ToString("yyyy-MM-dd") != "1900-01-01")
                        worksheet.Cells["B43"].Value = storeLeaseInfo.LastRemodelDate;
                    worksheet.Cells["B44"].Value = storeLeaseInfo.RemainingLeaseYears;

                    worksheet.Cells["B46"].Value = storeLeaseInfo.TTMSOIPercent / 100;
                    worksheet.Cells["B47"].Value = storeLeaseInfo.AsOf;
                    worksheet.Cells["B48"].Value = reimageSummary.OperationRequirements;
                    var financial = FinancialPreanalysis.FirstOrDefault(e => e.RefId == reimageSummary.Id);
                    worksheet.Cells["B49"].Value = TryParseDecimal(financial.TotalSalesInc);
                    worksheet.Cells["B45"].Value = financial.TTMSales;
                    worksheet.Cells["B50"].Value = TryParseDecimal(financial.StoreCM) * 100 + "%";
                    worksheet.Cells["B51"].Value = financial.CurrentPriceTier;
                    worksheet.Cells["B52"].Value = string.IsNullOrEmpty(financial.SPTAR) ? 0 : financial.SPTAR.As<decimal>();
                    worksheet.Cells["B53"].Value = financial.PriceTierafterReimage;
                }
                //worksheet.Cells["E4"].Value = financial.MarginInc;
            }
            else
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == inputInfo.ProjectId);
                var storeBasicInfo = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                var storeContractInfo = StoreContractInfo.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                int Year = 0;
                if (storeContractInfo != null)
                {

                    if (storeContractInfo.EndDate != null)
                    {
                        DateTime dtNow = DateTime.Now;
                        Year = int.Parse(storeContractInfo.EndDate.ToString().Split('/')[2].Substring(0, 4)) - dtNow.Year;
                    }
                }

                DateTime? dt = storeBasicInfo.ReImageDate;
                if (dt != null)
                {
                    if (dt.ToString().Substring(0, 8) == "1/1/1900")
                    {
                        dt = null;
                    }
                }
                worksheet.Cells["B43"].Value = dt;
                worksheet.Cells["B44"].Value = Year;
                //storeLeaseInfo.LastRemodelDate = dt;
                //storeLeaseInfo.RemainingLeaseYears = Year;
            }
            ReinvestmentCost cost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);

            worksheet.Cells["B2"].Value = store.Region;
            worksheet.Cells["B3"].Value = store.StoreCode;
            if (reinvestment != null)
            {

                worksheet.Cells["B8"].Value = TryParseDecimal(reinvestment.RightSizingSeatNo);
                worksheet.Cells["B11"].Value = TryParseDecimal(reinvestment.EstimatedSeatNo);
                worksheet.Cells["B41"].Value = reinvestment.GBDate;
                worksheet.Cells["B42"].Value = reinvestment.ConsCompletionDate;
            }
            if (cost != null)
            {
                worksheet.Cells["B13"].Value = TryParseDecimal(cost.DesignFee);
                worksheet.Cells["B14"].Value = TryParseDecimal(cost.PublicBudget);
                worksheet.Cells["B15"].Value = TryParseDecimal(cost.BuildingFacade);
                worksheet.Cells["B16"].Value = TryParseDecimal(cost.SiteBudget);
                worksheet.Cells["B17"].Value = TryParseDecimal(cost.BuildingWork);
                worksheet.Cells["B18"].Value = TryParseDecimal(cost.PlumbingSystem);
                worksheet.Cells["B19"].Value = TryParseDecimal(cost.ElectricalSystem);
                worksheet.Cells["B20"].Value = TryParseDecimal(cost.HVACDuctSystem);
                worksheet.Cells["B21"].Value = TryParseDecimal(cost.Signage);
                worksheet.Cells["B22"].Value = TryParseDecimal(cost.Seating);
                worksheet.Cells["B23"].Value = TryParseDecimal(cost.Decor);
                worksheet.Cells["B24"].Value = TryParseDecimal(cost.Kiosk);
                worksheet.Cells["B25"].Value = TryParseDecimal(cost.McCafe);
                worksheet.Cells["B26"].Value = TryParseDecimal(cost.MDS);
                worksheet.Cells["B27"].Value = TryParseDecimal(cost.Playland);
                worksheet.Cells["B28"].Value = TryParseDecimal(cost.KitchenCapacityUpgrade);
                worksheet.Cells["B29"].Value = TryParseDecimal(cost.BuildingWorks);
                worksheet.Cells["B30"].Value = TryParseDecimal(cost.KitchenEquipment);
                worksheet.Cells["B31"].Value = TryParseDecimal(cost.HVAC);
                worksheet.Cells["B32"].Value = TryParseDecimal(cost.Plumbing);
                worksheet.Cells["B33"].Value = TryParseDecimal(cost.ElectricDistribution);
                worksheet.Cells["B34"].Value = TryParseDecimal(cost.Structure);
                worksheet.Cells["B35"].Value = TryParseDecimal(cost.Others);
                worksheet.Cells["B36"].Value = TryParseDecimal(cost.LHIPMAct);
                worksheet.Cells["B37"].Value = TryParseDecimal(cost.SignagePMAct);
                worksheet.Cells["B38"].Value = TryParseDecimal(cost.EquipmentPMAct);
                worksheet.Cells["B39"].Value = TryParseDecimal(cost.SeatingPackagePMAct);
                worksheet.Cells["B40"].Value = TryParseDecimal(cost.DecorPMAct);
            }
            worksheet.Cells["B5"].Value = storeSTLocation.TotalArea.As<decimal>();
            worksheet.Cells["B6"].Value = store.ProvinceZHCN;
            worksheet.Cells["B7"].Value = store.NameZHCN;

            worksheet.Cells["B4"].Value = TryParseDecimal(storeSTLocation.TotalSeatsNo);
            worksheet.Cells["B9"].Value = TryParseDecimal(storeSTLocation.KitchenArea);
            worksheet.Cells["B10"].Value = store.CityZHCN;

            if (!string.IsNullOrEmpty(storeSTLocation.DesignStyle))
            {
                var dict = Dictionary.FirstOrDefault(i => i.Code == storeSTLocation.DesignStyle);
                if (dict != null)
                    worksheet.Cells["B12"].Value = dict.NameZHCN;
                else
                    worksheet.Cells["B12"].Value = storeSTLocation.DesignStyle;
            }
            //worksheet.Cells["B24"].Value = reimageSummary.ReinvestmentBasicInfo.NewKiosk;
            //worksheet.Cells["B25"].Value = reimageSummary.ReinvestmentBasicInfo.NewMcCafe;
            //worksheet.Cells["B26"].Value = reimageSummary.ReinvestmentBasicInfo.NewMDS;
            //worksheet.Cells["B41"].Value = reimageSummary.ReinvestmentBasicInfo.GBDate;
            //worksheet.Cells["B42"].Value = reimageSummary.ReinvestmentBasicInfo.ConsCompletionDate;


            worksheet.Cells["B10"].Value = store.CityZHCN;
            //worksheet.Cells["B11"].Value = inputInfo.StoreType;
            //worksheet.Cells["B12"].Value = inputInfo.StoreType;
            //worksheet.Cells["B13"].Value = inputInfo.StoreType;
            //worksheet.Cells["B14"].Value = inputInfo.StoreType;
            //worksheet.Cells["B15"].Value = inputInfo.StoreType;
            //worksheet.Cells["B16"].Value = inputInfo.StoreType;
            //worksheet.Cells["B17"].Value = inputInfo.StoreType;
            //worksheet.Cells["B18"].Value = inputInfo.StoreType;
            //worksheet.Cells["B19"].Value = inputInfo.StoreType;
            //worksheet.Cells["B20"].Value = inputInfo.StoreType;
            //worksheet.Cells["B21"].Value = inputInfo.StoreType;
            //worksheet.Cells["B22"].Value = inputInfo.StoreType;
            //worksheet.Cells["B23"].Value = inputInfo.StoreType;

            //worksheet.Cells["B27"].Value = inputInfo.StoreType;
            //worksheet.Cells["B28"].Value = inputInfo.StoreType;
            //worksheet.Cells["B29"].Value = inputInfo.StoreType;
            //worksheet.Cells["B30"].Value = inputInfo.StoreType;
            //worksheet.Cells["B31"].Value = inputInfo.StoreType;
            //worksheet.Cells["B32"].Value = inputInfo.StoreType;
            //worksheet.Cells["B33"].Value = inputInfo.StoreType;
            //worksheet.Cells["B34"].Value = inputInfo.StoreType;
            //worksheet.Cells["B35"].Value = inputInfo.StoreType;
            //worksheet.Cells["B36"].Value = inputInfo.StoreType;
            //worksheet.Cells["B37"].Value = inputInfo.StoreType;
            //worksheet.Cells["B38"].Value = inputInfo.StoreType;
            //worksheet.Cells["B39"].Value = inputInfo.StoreType;
            //worksheet.Cells["B40"].Value = inputInfo.StoreType;

            //if (reimageSummary.StoreProfitabilityAndLeaseInfo != null)
            //{
            //    worksheet.Cells["B43"].Value = reimageSummary.StoreProfitabilityAndLeaseInfo.LastRemodelDate;
            //    worksheet.Cells["B44"].Value = reimageSummary.StoreProfitabilityAndLeaseInfo.RemainingLeaseYears;
            //    worksheet.Cells["B45"].Value = reimageSummary.StoreProfitabilityAndLeaseInfo.TTMSales;
            //    worksheet.Cells["B46"].Value = reimageSummary.StoreProfitabilityAndLeaseInfo.TTMSOIPercent;
            //    worksheet.Cells["B47"].Value = reimageSummary.StoreProfitabilityAndLeaseInfo.AsOf;
            //}

            ////worksheet.Cells["B48"].Value = reimageSummary.OperationRequirements;
            //if (reimageSummary.FinancialPreanalysis != null)
            //{
            //    worksheet.Cells["B49"].Value = reimageSummary.FinancialPreanalysis.TotalSalesInc;
            //    worksheet.Cells["B50"].Value = reimageSummary.FinancialPreanalysis.StoreCM;
            //    worksheet.Cells["B51"].Value = reimageSummary.FinancialPreanalysis.CurrentPriceTier;
            //    worksheet.Cells["B52"].Value = reimageSummary.FinancialPreanalysis.ISDWIP;
            //    worksheet.Cells["B53"].Value = reimageSummary.FinancialPreanalysis.SPTAR;

            //}

        }

        private decimal TryParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            decimal _v;
            if (!decimal.TryParse(value, out _v))
                return 0;
            return _v;
        }
    }
}
