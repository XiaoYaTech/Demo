using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreMMInfo : BaseEntity<StoreMMInfo>
    {
        public static StoreMMInfo Get(string strUsCode)
        {
            var storeMMinfo = FirstOrDefault(e => e.StoreCode.Equals(strUsCode));
            return storeMMinfo;
        }
        public static StoreMMInfo GetStoreMMInfo(string strUsCode)
        {
            var storeMMinfo = FirstOrDefault(e => e.StoreCode.Equals(strUsCode));
            var listCode = GetDisplayFileds(storeMMinfo);
            var listDic = Dictionary.GetDictionary(listCode);
            if (listDic != null && listDic.Count > 0)
            {
                foreach (var code in listDic)
                {
                    var strDisplayName = listDic[code.Key].NameZHCN;
                    storeMMinfo.GetType().GetProperty(code.Key).SetValue(storeMMinfo, strDisplayName);
                }
            }
            return storeMMinfo;
        }

        private static Dictionary<string, string> GetDisplayFileds(StoreMMInfo storeMMinfo)
        {
            var listCode = new Dictionary<string, string>();

            listCode.Add("Priority", storeMMinfo.Priority);
            listCode.Add("TAAPriority", storeMMinfo.TAAPriority);
            listCode.Add("TAType", storeMMinfo.TAType);
            listCode.Add("Desirability", storeMMinfo.Desirability);
            listCode.Add("Maturity", storeMMinfo.Maturity);
            listCode.Add("IncomeLevel", storeMMinfo.IncomeLevel);
            listCode.Add("LocationRatingPP", storeMMinfo.LocationRatingPP);
            listCode.Add("OverallVisibility", storeMMinfo.OverallVisibility);
            listCode.Add("OverallAccessibility", storeMMinfo.OverallAccessibility);
            listCode.Add("Keyword1", storeMMinfo.Keyword1);
            listCode.Add("Keyword2", storeMMinfo.Keyword2);
            listCode.Add("Keyword3", storeMMinfo.Keyword3);
            listCode.Add("ShoppingLevel", storeMMinfo.ShoppingLevel);
            listCode.Add("WorkersProfile", storeMMinfo.WorkersProfile);
            listCode.Add("BuildingRetailActStrength", storeMMinfo.BuildingRetailActStrength);
            listCode.Add("SubwayVisibility", storeMMinfo.SubwayVisibility);
            listCode.Add("HomeIncomeLevel", storeMMinfo.HomeIncomeLevel);
            listCode.Add("AllianceName", storeMMinfo.AllianceName);
            listCode.Add("BigLLName", storeMMinfo.BigLLName);
            listCode.Add("StrategicInvestmentType", storeMMinfo.StrategicInvestmentType);
            listCode.Add("THStoreLocation", storeMMinfo.THStoreLocation);
            listCode.Add("BuildingNature", storeMMinfo.BuildingNature);
            listCode.Add("SchoolStudentProfile", storeMMinfo.SchoolStudentProfile);
            return listCode;
        }
    }
}
