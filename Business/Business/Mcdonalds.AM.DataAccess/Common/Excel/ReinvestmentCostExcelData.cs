using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using NTTMNC.BPM.Fx.Core.Json;
using NTTMNC.BPM.Fx.K2;
using OfficeOpenXml;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ReinvestmentCostExcelData : ExcelDataBase
    {

        private readonly ReinvestmentCost _entity;

        private readonly string _outputCol;
        public ReinvestmentCostExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";

            StartRow = 4;
            EndRow = 184;

            _outputCol = "E";

            _entity = new ReinvestmentCost();
            Entity = _entity;
        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            string output;
            try
            {
                output = GetExcelRange<decimal>(worksheet, currRow, _outputCol).ToString();
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteErrorLog(JsonConvert.SerializeObject(new { ex, ExcelRange = string.Format("{0}{1}", _outputCol, currRow) }));
                output = "0";
            }


            switch (currRow)
            {
                case 4:
                    _entity.LHINorm = output;
                    break;
                case 5:
                    _entity.AESFeesNorm = output;
                    break;
                case 6:
                    _entity.SurveyTestFeesNorm = output;
                    break;
                case 7:
                    _entity.PermitsUtilityFeesNorm = output;
                    break;
                case 8:
                    _entity.DemolitionCostNorm = output;
                    break;
                case 9:
                    _entity.SiteDevelopmentCostNorm = output;
                    break;
                case 10:
                    _entity.BuildingConstructionCostNorm = output;
                    break;
                case 11:
                    _entity.GeneralContractWorkNorm = output;
                    break;
                case 12:
                    _entity.InteriorDecorationNorm = output;
                    break;
                case 13:
                    _entity.ESCELFNorm = output;
                    break;
                case 14:
                    _entity.ELFNorm = output;
                    break;
                case 15:
                    _entity.PSSCNorm = output;
                    break;
                case 16:
                    _entity.HVACSubContractNorm = output;
                    break;
                case 17:
                    _entity.FireProtectionServiceNorm = output;
                    break;
                case 18:
                    _entity.EnvironmentProtectionNorm = output;
                    break;
                case 19:
                    _entity.RECostNorm = output;
                    break;
                case 20:
                    _entity.WallOnyDTNorm = output;
                    break;
                case 21:
                    _entity.ESSDNorm = output;
                    break;
                case 22:
                    _entity.SignageNorm = output;
                    break;
                case 23:
                    _entity.EquipmentNorm = output;
                    break;
                case 24:
                    _entity.SeatingPackageNorm = output;
                    break;
                case 25:
                    _entity.DecorNorm = output;
                    break;
                case 26:
                    _entity.TotalReinvestmentNorm = output;
                    break;
                case 27:
                    _entity.REMcCafeRECostNorm = output;
                    break;
                case 28:
                    _entity.REMcCafeLHINorm = output;
                    break;
                case 29:
                    _entity.REMcCafeESSDNorm = output;
                    break;
                case 30:
                    _entity.REMcCafeTotalNorm = output;
                    break;
                case 31:
                    _entity.REMDSRECostNorm = output;
                    break;
                case 32:
                    _entity.REMDSLHINorm = output;
                    break;
                case 33:
                    _entity.REMDSESSDNorm = output;
                    break;
                case 34:
                    _entity.REMDSTotalNorm = output;
                    break;
                case 35:
                    _entity.RERemoteKioskRECostNorm = output;
                    break;
                case 36:
                    _entity.RERemoteKioskLHINorm = output;
                    break;
                case 37:
                    _entity.RERemoteKioskESSDNorm = output;
                    break;
                case 38:
                    _entity.RERemoteKioskTotalNorm = output;
                    break;
                case 39:
                    _entity.REAttachedKioskRECostNorm = output;
                    break;
                case 40:
                    _entity.REAttachedKioskLHINorm = output;
                    break;
                case 41:
                    _entity.REAttachedKiosESSDNorm = output;
                    break;
                case 42:
                    _entity.REAttachedKiosTotalNorm = output;
                    break;

                case 47:
                    _entity.LHIBudget = output;
                    break;
                case 48:
                    _entity.AESFeesBudget = output;
                    break;
                case 49:
                    _entity.SurveyTestFeesBudget = output;
                    break;
                case 50:
                    _entity.PermitsUtilityFeesBudget = output;
                    break;
                case 51:
                    _entity.DemolitionCostBudget = output;
                    break;
                case 52:
                    _entity.SiteDevelopmentCostBudget = output;
                    break;
                case 53:
                    _entity.BuildingConstructionCostBudget = output;
                    break;
                case 54:
                    _entity.GeneralContractWorkBudget = output;
                    break;
                case 55:
                    _entity.InteriorDecorationBudget = output;
                    break;
                case 56:
                    _entity.ESCELFBudget = output;
                    break;
                case 57:
                    _entity.ELFBudget = output;
                    break;
                case 58:
                    _entity.PSSCBudget = output;
                    break;
                case 59:
                    _entity.HVACSubContractBudget = output;
                    break;
                case 60:
                    _entity.FireProtectionServiceBudget = output;
                    break;
                case 61:
                    _entity.EnvironmentProtectionBudget = output;
                    break;
                case 62:
                    _entity.RECostBudget = output;
                    break;
                case 63:
                    _entity.WallOnyDTBudget = output;
                    break;
                case 64:
                    _entity.ESSDBudget = output;
                    break;
                case 65:
                    _entity.SignageBudget = output;
                    break;
                case 66:
                    _entity.EquipmentBudget = output;
                    break;
                case 67:
                    _entity.SeatingPackageBudget = output;
                    break;
                case 68:
                    _entity.DecorBudget = output;
                    break;
                case 69:
                    _entity.TotalReinvestmentBudget = output;
                    break;
                case 70:
                    _entity.REMcCafeRECostBudget = output;
                    break;
                case 71:
                    _entity.REMcCafeLHIBudget = output;
                    break;
                case 72:
                    _entity.REMcCafeESSDBudget = output;
                    break;
                case 73:
                    _entity.REMcCafeTotalBudget = output;
                    break;
                case 74:
                    _entity.REMDSRECostBudget = output;
                    break;
                case 75:
                    _entity.REMDSLHIBudget = output;
                    break;
                case 76:
                    _entity.REMDSESSDBudget = output;
                    break;
                case 77:
                    _entity.REMDSTotalBudget = output;
                    break;
                case 78:
                    _entity.RERemoteKioskRECostBudget = output;
                    break;
                case 79:
                    _entity.RERemoteKioskLHIBudget = output;
                    break;
                case 80:
                    _entity.RERemoteKioskESSDBudget = output;
                    break;
                case 81:
                    _entity.RERemoteKioskTotalBudget = output;
                    break;
                case 82:
                    _entity.REAttachedKioskRECostBudget = output;
                    break;
                case 83:
                    _entity.REAttachedKioskLHIBudget = output;
                    break;
                case 84:
                    _entity.REAttachedKiosESSDBudget = output;
                    break;
                case 85:
                    _entity.REAttachedKiosTotalBudget = output;
                    break;

                case 87:
                    _entity.LHIPMAct = output;
                    break;
                case 88:
                    _entity.AESFeesPMAct = output;
                    break;
                case 89:
                    _entity.SurveyTestFeesPMAct = output;
                    break;
                case 90:
                    _entity.PermitsUtilityFeesPMAct = output;
                    break;
                case 91:
                    _entity.DemolitionCostPMAct = output;
                    break;
                case 92:
                    _entity.SiteDevelopmentCostPMAct = output;
                    break;
                case 93:
                    _entity.BuildingConstructionCostPMAct = output;
                    break;
                case 94:
                    _entity.GeneralContractWorkPMAct = output;
                    break;
                case 95:
                    _entity.InteriorDecorationPMAct = output;
                    break;
                case 96:
                    _entity.ESCELFPMAct = output;
                    break;
                case 97:
                    _entity.ELFPMAct = output;
                    break;
                case 98:
                    _entity.PSSCPMAct = output;
                    break;
                case 99:
                    _entity.HVACSubContractPMAct = output;
                    break;
                case 100:
                    _entity.FireProtectionServicePMAct = output;
                    break;
                case 101:
                    _entity.EnvironmentProtectionPMAct = output;
                    break;
                case 102:
                    _entity.RECostPMAct = output;
                    break;
                case 103:
                    _entity.WallOnyDTPMAct = output;
                    break;
                case 104:
                    _entity.ESSDPMAct = output;
                    break;
                case 105:
                    _entity.SignagePMAct = output;
                    break;
                case 106:
                    _entity.EquipmentPMAct = output;
                    break;
                case 107:
                    _entity.SeatingPackagePMAct = output;
                    break;
                case 108:
                    _entity.DecorPMAct = output;
                    break;
                case 109:
                    _entity.TotalReinvestmentPMAct = output;
                    break;
                case 110:
                    _entity.REMcCafeRECostPMAct = output;
                    break;
                case 111:
                    _entity.REMcCafeLHIPMAct = output;
                    break;
                case 112:
                    _entity.REMcCafeESSDPMAct = output;
                    break;
                case 113:
                    _entity.REMcCafeTotalPMAct = output;
                    break;
                case 114:
                    _entity.REMDSRECostPMAct = output;
                    break;
                case 115:
                    _entity.REMDSLHIPMAct = output;
                    break;
                case 116:
                    _entity.REMDSESSDPMAct = output;
                    break;
                case 117:
                    _entity.REMDSTotalPMAct = output;
                    break;
                case 118:
                    _entity.RERemoteKioskRECostPMAct = output;
                    break;
                case 119:
                    _entity.RERemoteKioskLHIPMAct = output;
                    break;
                case 120:
                    _entity.RERemoteKioskESSDPMAct = output;
                    break;
                case 121:
                    _entity.RERemoteKioskTotalPMAct = output;
                    break;
                case 122:
                    _entity.REAttachedKioskRECostPMAct = output;
                    break;
                case 123:
                    _entity.REAttachedKioskLHIPMAct = output;
                    break;
                case 124:
                    _entity.REAttachedKiosESSDPMAct = output;
                    break;
                case 125:
                    _entity.REAttachedKiosTotalPMAct = output;
                    break;
                case 126:
                    _entity.LHIFAAct = output;
                    break;
                case 127:
                    _entity.AESFeesFAAct = output;
                    break;
                case 128:
                    _entity.SurveyTestFeesFAAct = output;
                    break;
                case 129:
                    _entity.PermitsUtilityFeesFAAct = output;
                    break;
                case 130:
                    _entity.DemolitionCostFAAct = output;
                    break;
                case 131:
                    _entity.SiteDevelopmentCostFAAct = output;
                    break;
                case 132:
                    _entity.BuildingConstructionCostFAAct = output;
                    break;
                case 133:
                    _entity.GeneralContractWorkFAAct = output;
                    break;
                case 134:
                    _entity.InteriorDecorationFAAct = output;
                    break;
                case 135:
                    _entity.ESCELFFAAct = output;
                    break;
                case 136:
                    _entity.ELFFAAct = output;
                    break;
                case 137:
                    _entity.PSSCFAAct = output;
                    break;
                case 138:
                    _entity.HVACSubContractFAAct = output;
                    break;
                case 139:
                    _entity.FireProtectionServiceFAAct = output;
                    break;
                case 140:
                    _entity.EnvironmentProtectionFAAct = output;
                    break;
                case 141:
                    _entity.RECostFAAct = output;
                    break;
                case 142:
                    _entity.WallOnyDTFAAct = output;
                    break;
                case 143:
                    _entity.ESSDFAAct = output;
                    break;
                case 144:
                    _entity.SignageFAAct = output;
                    break;
                case 145:
                    _entity.EquipmentFAAct = output;
                    break;
                case 146:
                    _entity.SeatingPackageFAAct = output;
                    break;
                case 147:
                    _entity.DecorFAAct = output;
                    break;
                case 148:
                    _entity.TotalReinvestmentFAAct = output;
                    break;
                case 150:
                    _entity.DesignFee = output;
                    break;
                case 151:
                    _entity.PublicBudget = output;
                    break;
                case 152:
                    _entity.BuildingFacade = output;
                    break;
                case 153:
                    _entity.SiteBudget = output;
                    break;
                case 154:
                    _entity.BuildingWork = output;
                    break;
                case 155:
                    _entity.PlumbingSystem = output;
                    break;
                case 156:
                    _entity.ElectricalSystem = output;
                    break;
                case 157:
                    _entity.HVACDuctSystem = output;
                    break;
                case 158:
                    _entity.Signage = output;
                    break;
                case 159:
                    _entity.Seating = output;
                    break;
                case 160:
                    _entity.Decor = output;
                    break;
                case 161:
                    _entity.Kiosk = output;
                    break;
                case 162:
                    _entity.McCafe = output;
                    break;
                case 163:
                    _entity.MDS = output;
                    break;
                case 164:
                    _entity.Playland = output;
                    break;
                case 165:
                    _entity.KitchenCapacityUpgrade = output;
                    break;
                case 166:
                    _entity.TotalSalBldingInvstBudget = output;
                    break;
                case 167:
                    _entity.BuildingWorks = output;
                    break;
                case 168:
                    _entity.KitchenEquipment = output;
                    break;
                case 169:
                    _entity.HVAC = output;
                    break;
                case 170:
                    _entity.Plumbing = output;
                    break;
                case 171:
                    _entity.ElectricDistribution = output;
                    break;
                case 172:
                    _entity.Structure = output;
                    break;
                case 173:
                    _entity.Others = output;
                    break;
                case 174:
                    _entity.TotalNonSalBldingInvstBudget = output;
                    break;
                case 176:
                    _entity.TotalSalBldingInvstBudgetPMAct = output;
                    break;
                case 177:
                    _entity.TotalNonSalBldingInvstBudgetPMAct = output;
                    break;
                case 178:
                    _entity.TotalSalBldingInvstAccntAct = output;
                    break;
                case 179:
                    _entity.TotalNonSalBldingInvstAccntAct = output;
                    break;

                case 181:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _entity.ExpFAActVsReCost = output;
                    break;
                case 182:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _entity.ExpFAActVsLHI = output;
                    break;
                case 183:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _entity.ExpFAActVsESSD = output;
                    break;
                case 184:
                    output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                    _entity.ExpFAActVsTotal = output;
                    break;


            }
        }

        public override void Import()
        {
            _entity.LHIVariance = (_entity.LHIBudget.As<decimal>() - _entity.LHINorm.As<decimal>()).ToString();
            _entity.AESFeesVariance = (_entity.AESFeesBudget.As<decimal>() - _entity.AESFeesNorm.As<decimal>()).ToString();
            _entity.SurveyTestFeesVariance = (_entity.SurveyTestFeesBudget.As<decimal>() - _entity.SurveyTestFeesNorm.As<decimal>()).ToString();
            _entity.PermitsUtilityFeesVariance = (_entity.PermitsUtilityFeesBudget.As<decimal>() - _entity.PermitsUtilityFeesNorm.As<decimal>()).ToString();
            _entity.DemolitionCostVariance = (_entity.DemolitionCostBudget.As<decimal>() - _entity.DemolitionCostNorm.As<decimal>()).ToString();
            _entity.SiteDevelopmentCostVariance = (_entity.SiteDevelopmentCostBudget.As<decimal>() - _entity.SiteDevelopmentCostNorm.As<decimal>()).ToString();
            _entity.BuildingConstructionCostVariance = (_entity.BuildingConstructionCostBudget.As<decimal>() - _entity.BuildingConstructionCostNorm.As<decimal>()).ToString();
            _entity.GeneralContractWorkVariance = (_entity.GeneralContractWorkBudget.As<decimal>() - _entity.GeneralContractWorkNorm.As<decimal>()).ToString();
            _entity.InteriorDecorationVariance = (_entity.InteriorDecorationBudget.As<decimal>() - _entity.InteriorDecorationNorm.As<decimal>()).ToString();
            _entity.ESCELFVariance = (_entity.ESCELFBudget.As<decimal>() - _entity.ESCELFNorm.As<decimal>()).ToString();
            _entity.ELFVariance = (_entity.ELFBudget.As<decimal>() - _entity.ELFNorm.As<decimal>()).ToString();
            _entity.PSSCVariance = (_entity.PSSCBudget.As<decimal>() - _entity.PSSCNorm.As<decimal>()).ToString();
            _entity.HVACSubContractVariance = (_entity.HVACSubContractBudget.As<decimal>() - _entity.HVACSubContractNorm.As<decimal>()).ToString();
            _entity.FireProtectionServiceVariance = (_entity.FireProtectionServiceBudget.As<decimal>() - _entity.FireProtectionServiceNorm.As<decimal>()).ToString();
            _entity.EnvironmentProtectionVariance = (_entity.EnvironmentProtectionBudget.As<decimal>() - _entity.EnvironmentProtectionNorm.As<decimal>()).ToString();
            _entity.RECostVariance = (_entity.RECostBudget.As<decimal>() - _entity.RECostNorm.As<decimal>()).ToString();
            _entity.WallOnyDTVariance = (_entity.WallOnyDTBudget.As<decimal>() - _entity.WallOnyDTNorm.As<decimal>()).ToString();
            _entity.ESSDVariance = (_entity.ESSDBudget.As<decimal>() - _entity.ESSDNorm.As<decimal>()).ToString();
            _entity.SignageVariance = (_entity.SignageBudget.As<decimal>() - _entity.SignageNorm.As<decimal>()).ToString();
            _entity.EquipmentVariance = (_entity.EquipmentBudget.As<decimal>() - _entity.EquipmentNorm.As<decimal>()).ToString();
            _entity.SeatingPackageVariance = (_entity.SeatingPackageBudget.As<decimal>() - _entity.SeatingPackageNorm.As<decimal>()).ToString();
            _entity.DecorVariance = (_entity.DecorBudget.As<decimal>() - _entity.DecorNorm.As<decimal>()).ToString();
            _entity.TotalReinvestmentVariance = (_entity.TotalReinvestmentBudget.As<decimal>() - _entity.TotalReinvestmentNorm.As<decimal>()).ToString();
            _entity.REMcCafeRECostVariance = (_entity.REMcCafeRECostBudget.As<decimal>() - _entity.REMcCafeRECostNorm.As<decimal>()).ToString();
            _entity.REMcCafeLHIVariance = (_entity.REMcCafeLHIBudget.As<decimal>() - _entity.REMcCafeLHINorm.As<decimal>()).ToString();
            _entity.REMcCafeESSDVariance = (_entity.REMcCafeESSDBudget.As<decimal>() - _entity.REMcCafeESSDNorm.As<decimal>()).ToString();
            _entity.REMcCafeTotalVariance = (_entity.REMcCafeTotalBudget.As<decimal>() - _entity.REMcCafeTotalNorm.As<decimal>()).ToString();
            _entity.REMDSRECostVariance = (_entity.REMDSRECostBudget.As<decimal>() - _entity.REMDSRECostNorm.As<decimal>()).ToString();
            _entity.REMDSLHIVariance = (_entity.REMDSLHIBudget.As<decimal>() - _entity.REMDSLHINorm.As<decimal>()).ToString();
            _entity.REMDSESSDVariance = (_entity.REMDSESSDBudget.As<decimal>() - _entity.REMDSESSDNorm.As<decimal>()).ToString();
            _entity.REMDSTotalVariance = (_entity.REMDSTotalBudget.As<decimal>() - _entity.REMDSTotalNorm.As<decimal>()).ToString();
            _entity.RERemoteKioskRECostVariance = (_entity.RERemoteKioskRECostBudget.As<decimal>() - _entity.RERemoteKioskRECostNorm.As<decimal>()).ToString();
            _entity.RERemoteKioskLHIVariance = (_entity.RERemoteKioskLHIBudget.As<decimal>() - _entity.RERemoteKioskLHINorm.As<decimal>()).ToString();
            _entity.RERemoteKioskESSDVariance = (_entity.RERemoteKioskESSDBudget.As<decimal>() - _entity.RERemoteKioskESSDNorm.As<decimal>()).ToString();
            _entity.REAttachedKioskRECostVariance = (_entity.REAttachedKioskRECostBudget.As<decimal>() - _entity.REAttachedKioskRECostNorm.As<decimal>()).ToString();
            _entity.REAttachedKioskLHIVariance = (_entity.REAttachedKioskLHIBudget.As<decimal>() - _entity.REAttachedKioskLHINorm.As<decimal>()).ToString();
            _entity.REAttachedKiosESSDVariance = (_entity.REAttachedKiosESSDBudget.As<decimal>() - _entity.REAttachedKiosESSDNorm.As<decimal>()).ToString();
            _entity.REAttachedKiosTotalVariance = (_entity.REAttachedKiosTotalBudget.As<decimal>() - _entity.REAttachedKiosTotalNorm.As<decimal>()).ToString();
            _entity.RERemoteKioskTotalVariance = (_entity.RERemoteKioskTotalBudget.As<decimal>() - _entity.RERemoteKioskTotalNorm.As<decimal>()).ToString();
            //_entity.DesignFee = _entity.DesignFee.As<decimal>().ToString();
            //_entity.PublicBudget = _entity.PublicBudget.As<decimal>() .ToString();
            //_entity.BuildingFacade = (_entity.BuildingFacade.As<decimal>() - _entity.BuildingFacade.As<decimal>()).ToString();
            //_entity.SiteBudget = (_entity.SiteBudget.As<decimal>() - _entity.SiteBudget.As<decimal>()).ToString();
            //_entity.BuildingWork = (_entity.BuildingWork.As<decimal>() - _entity.BuildingWork.As<decimal>()).ToString();
            //_entity.PlumbingSystem = (_entity.PlumbingSystem.As<decimal>() - _entity.PlumbingSystem.As<decimal>()).ToString();
            //_entity.ElectricalSystem = (_entity.ElectricalSystem.As<decimal>() - _entity.ElectricalSystem.As<decimal>()).ToString();
            //_entity.HVACDuctSystem = (_entity.HVACDuctSystem.As<decimal>() - _entity.HVACDuctSystem.As<decimal>()).ToString();
            //_entity.Signage = (_entity.Signage.As<decimal>() - _entity.Signage.As<decimal>()).ToString();
            //_entity.Seating = (_entity.Seating.As<decimal>() - _entity.Seating.As<decimal>()).ToString();
            //_entity.Decor = (_entity.Decor.As<decimal>() - _entity.Decor.As<decimal>()).ToString();
            //_entity.Kiosk = (_entity.Kiosk.As<decimal>() - _entity.Kiosk.As<decimal>()).ToString();
            //_entity.McCafe = (_entity.McCafe.As<decimal>() - _entity.McCafe.As<decimal>()).ToString();
            //_entity.MDS = (_entity.MDS.As<decimal>() - _entity.MDS.As<decimal>()).ToString();
            //_entity.Playland = (_entity.Playland.As<decimal>() - _entity.Playland.As<decimal>()).ToString();
            //_entity.KitchenCapacityUpgrade = (_entity.KitchenCapacityUpgrade.As<decimal>() - _entity.KitchenCapacityUpgrade.As<decimal>()).ToString();
            //_entity.BuildingWorks = (_entity.BuildingWorks.As<decimal>() - _entity.BuildingWorks.As<decimal>()).ToString();
            //_entity.KitchenEquipment = (_entity.KitchenEquipment.As<decimal>() - _entity.KitchenEquipment.As<decimal>()).ToString();
            //_entity.HVAC = (_entity.HVAC.As<decimal>() - _entity.HVAC.As<decimal>()).ToString();
            //_entity.Plumbing = (_entity.Plumbing.As<decimal>() - _entity.Plumbing.As<decimal>()).ToString();
            //_entity.ElectricDistribution = (_entity.ElectricDistribution.As<decimal>() - _entity.ElectricDistribution.As<decimal>()).ToString();
            //_entity.Structure = (_entity.Structure.As<decimal>() - _entity.Structure.As<decimal>()).ToString();
            //_entity.Others = (_entity.Others.As<decimal>() - _entity.Others.As<decimal>()).ToString();

            if (ReinvestmentCost.Any(r => r.Id == _entity.Id))
            {
                ReinvestmentCost.Update(_entity);
            }
            else
            {
                ReinvestmentCost.Add(_entity);
            }

        }
        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B4"].Value = inputInfo.Region.AsString();
            worksheet.Cells["B5"].Value = inputInfo.Market.AsString();
            worksheet.Cells["B6"].Value = inputInfo.City.AsString();
            worksheet.Cells["B7"].Value = inputInfo.USCode.AsString();
            worksheet.Cells["B8"].Value = inputInfo.StoreNameEN.AsString();
            worksheet.Cells["B9"].Value = inputInfo.StoreNameCN.AsString();
            worksheet.Cells["B10"].Value = inputInfo.OpenDate;
            if (inputInfo.GBDate.HasValue)
            {
                worksheet.Cells["B11"].Value = inputInfo.GBDate.Value;
            }
            if (inputInfo.ConsCompletionDate.HasValue)
            {
                worksheet.Cells["B12"].Value = inputInfo.ConsCompletionDate.Value;
            }
            worksheet.Cells["B13"].Value = Dictionary.ParseDisplayName(inputInfo.NewDesignType);
            worksheet.Cells["B14"].Value = inputInfo.NormType.AsString();
            worksheet.Cells["B15"].Value = inputInfo.EstimatedSeatNO.AsString();
            worksheet.Cells["B16"].Value = inputInfo.NewDTSiteArea.AsString();
            worksheet.Cells["B17"].Value = inputInfo.NewOperationArea.AsString();
            worksheet.Cells["B18"].Value = inputInfo.NewDiningArea.AsString();
            worksheet.Cells["B19"].Value = inputInfo.WallPanelArea.AsString();
            worksheet.Cells["B20"].Value = inputInfo.WallGraphicArea.AsString();
            worksheet.Cells["B21"].Value = inputInfo.FacadeACMArea.AsString();

            worksheet.Cells["B22"].Value = ExcelHelper.ResolveBooleanTypeFieldValue(inputInfo.NewRemoteKiosk);
            worksheet.Cells["B23"].Value = ExcelHelper.ResolveBooleanTypeFieldValue(inputInfo.NewAttachedKiosk);
            worksheet.Cells["B24"].Value = ExcelHelper.ResolveBooleanTypeFieldValue(inputInfo.NewMcCafe);
            worksheet.Cells["B25"].Value = ExcelHelper.ResolveBooleanTypeFieldValue(inputInfo.NewMDS);

        }



    }
}
