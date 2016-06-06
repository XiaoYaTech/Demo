using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Mcdonalds.AM.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Mcdonalds.AM.DataAccess.DataTransferObjects;

namespace Mcdonalds.AM.Web.Controllers
{
    public class StoreListController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //  Store List
        
        public ActionResult StoreListQuery(string _CityName, string _NameZHCN, string _Province, string _USCode, int _PageIndex, int _PageSize)
        {
            List<VStoreList> lsStoreBasicInfo = new List<VStoreList>();

            int totalRecords = 0;
            if (_PageIndex == 0) _PageIndex = 1;
            var stores = VStoreList.GetStoreList(o => (_CityName.Length != 0 ? o.CityZHCN.Contains(_CityName) : 1 == 1) &&
                                            (_NameZHCN.Length != 0 ? o.NameZHCN.Contains(_NameZHCN) : 1 == 1) &&
                                            (_Province.Length != 0 ? o.ProvinceZHCN.Contains(_Province) : 1 == 1) &&
                                            (_USCode.Length != 0 ? o.StoreCode.Contains(_USCode) : 1 == 1));
            totalRecords = stores.Count();
            lsStoreBasicInfo = stores.OrderBy(e => e.StoreCode).Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();

            int pageTotal = Convert.ToInt32(totalRecords / _PageSize) + (totalRecords % _PageSize == 0 ? 0 : 1);

            string pageInnerHtml = "";
            int displayTabInc = 0;
            int displayTabNumber = 5;                       //  1,3,5,7,9,11,13,15,17,19
            int displayTabSubDiv2 = displayTabNumber / 2;
            int prevPage = _PageIndex - 1;
            int nextPage = _PageIndex + 1;
            ////  f - b
            //for (int j = _PageIndex - displayTabSubDiv2; j <= pageTotal && displayTabInc < displayTabNumber; j++)
            //    if (j > 0)
            //    {
            //        if (j == _PageIndex)
            //            pageInnerHtml += "[" + j.ToString() + "]" + ",";
            //        else
            //            pageInnerHtml +=   j.ToString()  + ",";
            //        displayTabInc++;
            //    }
            ////  b - f
            //if (displayTabInc < displayTabNumber)
            //    for (int j = _PageIndex - displayTabSubDiv2 - 1; j >= 1 && displayTabInc < displayTabNumber; j--)
            //        if (j > 0)
            //        {
            //            pageInnerHtml = j.ToString() + "," + pageInnerHtml;
            //            displayTabInc++;
            //        }
            ////  prevPage, nextPage
            //pageInnerHtml = "[prevPage:" + prevPage + (prevPage < 1 ? "disable" : "enabled") + "]" + " -- " +
            //                pageInnerHtml +
            //                " -- " + "[nextPage:" + nextPage + (nextPage > pageTotal ? "disable" : "enabled") + "]";

            //  f - b
            for (int j = _PageIndex - displayTabSubDiv2; j <= pageTotal && displayTabInc < displayTabNumber; j++)
                if (j > 0)
                {
                    if (j == _PageIndex)
                        pageInnerHtml += "<LI class='ng-scope active'> <A class=ng-binding href='javascript:void(0)' onclick='selectPage(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>";
                    else
                        pageInnerHtml += "<LI class='ng-scope'>         <A class=ng-binding href='javascript:void(0)' onclick='selectPage(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>";
                    //
                    displayTabInc++;
                }
            //  b - f
            if (displayTabInc < displayTabNumber)
                for (int j = _PageIndex - displayTabSubDiv2 - 1; j >= 1 && displayTabInc < displayTabNumber; j--)
                    if (j > 0)
                    {
                        pageInnerHtml = "<LI class='ng-scope'>         <A class=ng-binding href='javascript:void(0)' onclick='selectPage(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>" + pageInnerHtml;
                        displayTabInc++;
                    }
            //  prevPage, nextPage
            pageInnerHtml = "<LI class='ng-scope " + (prevPage < 1 ? "disabled" : "") + "'><A class=ng-binding href='javascript:void(0)' " + (prevPage < 1 ? "" : "onclick='selectPage(" + prevPage + ")'") + ">上一页</A></LI>" +
                            pageInnerHtml +
                            "<LI class='ng-scope " + (nextPage > pageTotal ? "disabled" : "") + "'><A class=ng-binding href='javascript:void(0)' " + (nextPage > pageTotal ? "" : "onclick='selectPage(" + nextPage + ")'") + ">下一页</A></LI>";

            var resultObj = new
            {
                totalPage = pageTotal,
                pageIndex = _PageIndex,
                dataPage = lsStoreBasicInfo,
                paginationDesc = string.Format("当前第{0}页/共{1}页 共{2}条记录", _PageIndex, pageTotal, totalRecords),
                paginationInnerHtml = pageInnerHtml
            };

            //string result = Newtonsoft.Json.JsonConvert.SerializeObject(resultObj);
            return Json(resultObj,JsonRequestBehavior.AllowGet);
        }

        public string GetPaging(int pageTotal, int _PageIndex, int _PageSize, string jsFunciontName)
        {

            string pageInnerHtml = "";
            int displayTabInc = 0;
            int displayTabNumber = 5;                       //  1,3,5,7,9,11,13,15,17,19
            int displayTabSubDiv2 = displayTabNumber / 2;
            int prevPage = _PageIndex - 1;
            int nextPage = _PageIndex + 1;

            //  f - b
            for (int j = _PageIndex - displayTabSubDiv2; j <= pageTotal && displayTabInc < displayTabNumber; j++)
                if (j > 0)
                {
                    if (j == _PageIndex)
                        pageInnerHtml += "<LI class='ng-scope active'> <A class=ng-binding href='javascript:void(0)' onclick='" + jsFunciontName + "(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>";
                    else
                        pageInnerHtml += "<LI class='ng-scope'>         <A class=ng-binding href='javascript:void(0)' onclick='" + jsFunciontName + "(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>";
                    //
                    displayTabInc++;
                }
            //  b - f
            if (displayTabInc < displayTabNumber)
                for (int j = _PageIndex - displayTabSubDiv2 - 1; j >= 1 && displayTabInc < displayTabNumber; j--)
                    if (j > 0)
                    {
                        pageInnerHtml = "<LI class='ng-scope'>         <A class=ng-binding href='javascript:void(0)' onclick='" + jsFunciontName + "(" + j.ToString() + ")'>" + j.ToString() + "</A></LI>" + pageInnerHtml;
                        displayTabInc++;
                    }
            //  prevPage, nextPage
            pageInnerHtml = "<LI class='ng-scope " + (prevPage < 1 ? "disabled" : "") + "'><A class=ng-binding href='javascript:void(0)' " + (prevPage < 1 ? "" : "onclick='" + jsFunciontName + "(" + prevPage + ")'") + ">上一页</A></LI>" +
                            pageInnerHtml +
                            "<LI class='ng-scope " + (nextPage > pageTotal ? "disabled" : "") + "'><A class=ng-binding href='javascript:void(0)' " + (nextPage > pageTotal ? "" : "onclick='" + jsFunciontName + "(" + nextPage + ")'") + ">下一页</A></LI>";

            return pageInnerHtml;

        }

        public ActionResult StoreProjectRowsCount(string _USCode)
        {
            var resultData = new
            {
                ClosureRowsCount = ClosureInfo.Search(o => o.USCode == _USCode).ToList<ClosureInfo>().Count,
                RenewalRowsCount = RenewalInfo.Search(o => o.USCode == _USCode).ToList<RenewalInfo>().Count,
                RebuildRowsCount = RebuildInfo.Search(o => o.USCode == _USCode).ToList<RebuildInfo>().Count,
                MajorLeaseRowsCount = MajorLeaseInfo.Search(o => o.USCode == _USCode).ToList<MajorLeaseInfo>().Count,
                ReimageRowsCount = ReimageInfo.Search(o => o.USCode == _USCode).ToList<ReimageInfo>().Count,
                TempClosureRowsCount = TempClosureInfo.Search(o => o.USCode == _USCode).ToList<TempClosureInfo>().Count
            };

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(resultData);

            return Content(result);
        }

        public ActionResult FlowInfoQuery(string _USCode, string _FlowCode, int _TopRowCount)
        {
            string result = string.Empty;

            int totalRecords = 0;

            var lsProjectInfo = new List<VProject>();
            if (_TopRowCount == -1)
            {
                lsProjectInfo = VProject.Search(i => i.USCode == _USCode && i.FlowCode == _FlowCode).OrderByDescending(i => i.ProjectId).ToList();
            }
            else
            {
                lsProjectInfo = VProject.Search(i => i.USCode == _USCode && i.FlowCode == _FlowCode, obp => obp.ProjectId, 1, _TopRowCount, out totalRecords, true).ToList();
            }

            result = Newtonsoft.Json.JsonConvert.SerializeObject(lsProjectInfo);
            return Content(result);
        }

        public ActionResult StoreSearchListQuery(string _SearchType, string _SearchValue)
        {
            int totalRecords = 0;
            List<StoreBasicInfo> lsStore = new List<StoreBasicInfo>();
            List<string> lsString = new List<string>();
            if (_SearchType == "USCode")
            {
                lsStore = StoreBasicInfo.Search(o => o.StoreCode.Contains(_SearchValue),
                                        od => od.StoreCode,
                                        1, 5, out totalRecords, false).ToList<StoreBasicInfo>();
                lsString = (from o in lsStore select o.StoreCode).Distinct().ToList<string>();
            }
            if (_SearchType == "NameZHCN")
            {
                lsStore = StoreBasicInfo.Search(o => o.NameZHCN.Contains(_SearchValue),
                                        od => od.NameZHCN,
                                        1, 5, out totalRecords, false).ToList<StoreBasicInfo>();
                lsString = (from o in lsStore select o.NameZHCN).Distinct().ToList<string>();
            }
            if (_SearchType == "Province")
            {
                lsStore = StoreBasicInfo.Search(o => o.ProvinceZHCN.Contains(_SearchValue),
                                        od => od.ProvinceZHCN,
                                        1, 5, out totalRecords, false).ToList<StoreBasicInfo>();
                lsString = (from o in lsStore select o.ProvinceZHCN).Distinct().ToList<string>();
            }
            if (_SearchType == "CityName")
            {
                lsStore = StoreBasicInfo.Search(o => o.CityZHCN.Contains(_SearchValue),
                                        od => od.CityZHCN,
                                        1, 5, out totalRecords, false).ToList<StoreBasicInfo>();
                lsString = (from o in lsStore select o.CityZHCN).Distinct().ToList<string>();
            }

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(lsString);

            return Content(result);
        }

        #region //  Store Management

        public ActionResult StoreQuery(string _USCode)
        {
            List<StoreBasicInfo> lsStore = new List<StoreBasicInfo>();
            StoreBasicInfo store = new StoreBasicInfo();

            lsStore = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreBasicInfo>();
            if (lsStore.Count > 0) store = lsStore[0];

            string result = JsonConvert.SerializeObject(store);

            return Content(result);

        }

        public ActionResult StoreBasicInfoQuery(string _USCode)
        {
            List<StoreBasicInfo> lsStoreBasicInfo = new List<StoreBasicInfo>();
            StoreBasicInfo storeBasicInfo = new StoreBasicInfo();

            lsStoreBasicInfo = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreBasicInfo>();
            if (lsStoreBasicInfo.Count > 0) storeBasicInfo = lsStoreBasicInfo[0];

            string result = JsonConvert.SerializeObject(storeBasicInfo);

            return Content(result);

        }

        public ActionResult StoreDevelopQuery(string _USCode)
        {
            List<StoreDevelop> lsStoreBasicInfo = new List<StoreDevelop>();
            StoreDevelop storeDevelop = new StoreDevelop();

            lsStoreBasicInfo = StoreDevelop.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreDevelop>();
            if (lsStoreBasicInfo.Count > 0) storeDevelop = lsStoreBasicInfo[0];

            string result = JsonConvert.SerializeObject(storeDevelop);

            return Content(result);

        }

        public ActionResult StoreOpQuery(string _USCode)
        {
            List<StoreOps> lsStoreOp = new List<StoreOps>();
            StoreOps storeOp = new StoreOps();

            lsStoreOp = StoreOps.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreOps>();
            if (lsStoreOp.Count > 0) storeOp = lsStoreOp[0];

            string result = JsonConvert.SerializeObject(storeOp);

            return Content(result);

        }

        #endregion

        #region //  Store RealEstate
        public ActionResult StoreRealEstateQuery(string _USCode)
        {
            List<StoreAssetHandover> lsStoreAssetHandover = new List<StoreAssetHandover>();
            StoreAssetHandover mStoreAssetHandover = new StoreAssetHandover();

            lsStoreAssetHandover = StoreAssetHandover.Search(o => o.StoreCode == _USCode).ToList<StoreAssetHandover>();
            if (lsStoreAssetHandover.Count > 0) mStoreAssetHandover = lsStoreAssetHandover[0];

            string result = JsonConvert.SerializeObject(mStoreAssetHandover);

            return Content(result);
        }
        public ActionResult StoreRealEstateAttachmentQuery(string _USCode)
        {
            List<StoreAssetHandoverAttachment> lsStoreAssetHandoverAttachment = new List<StoreAssetHandoverAttachment>();

            lsStoreAssetHandoverAttachment = StoreAssetHandoverAttachment.Search(
                                            o => o.StoreCode == _USCode).ToList<StoreAssetHandoverAttachment>().OrderBy(o => (o.SequenceNum)).ThenBy(o => (o.Attachment)).ToList<StoreAssetHandoverAttachment>();

            string result = JsonConvert.SerializeObject(lsStoreAssetHandoverAttachment);

            return Content(result);
        }
        #endregion

        #region //  商圈 Store TA

        public ActionResult StoreMMInfoQuery(string _USCode)
        {
            var mStoreMMInfo = StoreMMInfo.GetStoreMMInfo(_USCode);
            string result = JsonConvert.SerializeObject(mStoreMMInfo);

            return Content(result);
        }

        #endregion

        #region //  场地 Store Construction | StoreSTLocation
        public ActionResult StoreSTLocationQuery(string _USCode)
        {
            var mStoreSTLocation = StoreSTLocation.GetStoreSTLocationStoreList(_USCode);
            string result = JsonConvert.SerializeObject(mStoreSTLocation);

            return Content(result);
        }
        #endregion

        #region //  业务拓展
        public ActionResult StoreBEInfoQuery(string _USCode)
        {
            List<StoreBEInfo> lsStoreBEInfo = new List<StoreBEInfo>();
            lsStoreBEInfo = StoreBEInfo.Search(o => o.StoreCode == _USCode).ToList<StoreBEInfo>();

            string result = JsonConvert.SerializeObject(lsStoreBEInfo);

            return Content(result);
        }

        public ActionResult StoreBEContractInfoQuery(string _USCode)
        {
            List<StoreBEContractInfo> lsStoreBEContractInfo = new List<StoreBEContractInfo>();
            lsStoreBEContractInfo = StoreBEContractInfo.SqlQuery<StoreBEContractInfo>(
                "select * from StoreBEContractInfo where BEID in(select BEID from StoreBEInfo where StoreCode = '" + _USCode + "')",
                null).ToList<StoreBEContractInfo>();

            foreach (var item in lsStoreBEContractInfo)
            {
                var dict = Dictionary.FirstOrDefault(i => i.Code == item.McDLegalEntity);
                if (dict != null)
                    item.McDLegalEntity = dict.NameZHCN;
            }

            string result = JsonConvert.SerializeObject(lsStoreBEContractInfo);

            return Content(result);
        }
        #endregion

        #region //  FinanceInfo

        public ActionResult StoreSTMonthlyFinaceInfoQuery(string _USCode)
        {
            McdAMEntities amdb = new McdAMEntities();
            var storeId = amdb.StoreBasicInfo.Where(s => s.StoreCode.Equals(_USCode)).Select(id => id.StoreID).FirstOrDefault();
            var item = amdb.DataSync_LDW_AM_STMonthlyFinaceInfo.Where(f => f.StoreID == storeId).OrderByDescending(f => f.Year).FirstOrDefault();

            var yearMonthObj = amdb.StoreSTMonthlyFinaceInfoTTM.FirstOrDefault();
            var financeYear = GetLatestYear();
            var financeMonth = GetLatestMonth();
            if (yearMonthObj != null && !string.IsNullOrEmpty(yearMonthObj.TTMValue))
            {
                financeYear = yearMonthObj.TTMValue.Substring(0, yearMonthObj.TTMValue.IndexOf('-'));
                financeMonth = yearMonthObj.TTMValue.Substring(yearMonthObj.TTMValue.IndexOf('-') + 1);
            }
            var financeData = amdb.DataSync_LDW_AM_STFinanceData.Where(f => f.UsCode == _USCode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();
            var financeData2 = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == _USCode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();

            if (item == null)
                item = new DataSync_LDW_AM_STMonthlyFinaceInfo();

            //  界面展示逻辑调整
            // （金额精确到小数点后2位）
            item.NetProductSales = Convert.ToString(Math.Round(Convert.ToDecimal(item.NetProductSales), 2));
            item.CashFlow = Convert.ToString(Math.Round(Convert.ToDecimal(item.CashFlow), 2));
            item.SOI = Convert.ToString(Math.Round(Convert.ToDecimal(item.SOI), 2));
            item.Rent = Convert.ToString(Math.Round(Convert.ToDecimal(item.Rent), 2));
            item.McOpCoMargin = Convert.ToString(Math.Round(Convert.ToDecimal(item.McOpCoMargin), 2));
            item.Rent_inclAdjustment = Convert.ToString(Math.Round(Convert.ToDecimal(item.Rent_inclAdjustment), 2));
            item.LHINBV = Convert.ToString(Math.Round(Convert.ToDecimal(item.LHINBV), 2));
            item.ESSDNBV = Convert.ToString(Math.Round(Convert.ToDecimal(item.ESSDNBV), 2));
            item.TotalNBV = Convert.ToString(Math.Round(Convert.ToDecimal(item.TotalNBV), 2));
            item.SOIPreYr1 = Convert.ToString(Math.Round(Convert.ToDecimal(item.SOIPreYr1), 2));
            item.SOIPreYr2 = Convert.ToString(Math.Round(Convert.ToDecimal(item.SOIPreYr2), 2));
            item.SOIPreYr3 = Convert.ToString(Math.Round(Convert.ToDecimal(item.SOIPreYr3), 2));
            item.CashFlowPreYr1 = Convert.ToString(Math.Round(Convert.ToDecimal(item.CashFlowPreYr1), 2));
            item.CashFlowPreYr2 = Convert.ToString(Math.Round(Convert.ToDecimal(item.CashFlowPreYr2), 2));
            item.CashFlowPreYr3 = Convert.ToString(Math.Round(Convert.ToDecimal(item.CashFlowPreYr3), 2));
            item.RentalPaidtoLLPreYr1 = Convert.ToString(Math.Round(Convert.ToDecimal(item.RentalPaidtoLLPreYr1), 2));
            item.RentalPaidtoLLPreYr2 = Convert.ToString(Math.Round(Convert.ToDecimal(item.RentalPaidtoLLPreYr2), 2));
            item.RentalPaidtoLLPreYr3 = Convert.ToString(Math.Round(Convert.ToDecimal(item.RentalPaidtoLLPreYr3), 2));

            // (百分比精确到小数点后1位)
            item.SOIPct = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.SOIPct), 1));
            item.Rent_inclAdjustmentPct = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.Rent_inclAdjustmentPct), 1));
            //item.CompsSales = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.CompsSales), 1));
            item.CompSalePreYr1 = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.CompSalePreYr1), 1));
            item.CompSalePreYr2 = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.CompSalePreYr2), 1));
            item.CompSalePreYr3 = Convert.ToString(Math.Round(100 * Convert.ToDecimal(item.CompSalePreYr3), 1));
            item.CompsSales = Convert.ToString(Math.Round(100 * Convert.ToDecimal(financeData2.comp_sales_ttm), 1));

            string result = JsonConvert.SerializeObject(item);

            return Content(result);
        }

        public ActionResult StoreSTMonthlyFinaceInfoTTMQuery()
        {
            List<StoreSTMonthlyFinaceInfoTTM> lsStoreSTMonthlyFinaceInfoTTM = new List<StoreSTMonthlyFinaceInfoTTM>();
            StoreSTMonthlyFinaceInfoTTM mStoreSTMonthlyFinaceInfoTTM = new StoreSTMonthlyFinaceInfoTTM();

            lsStoreSTMonthlyFinaceInfoTTM = StoreSTMonthlyFinaceInfoTTM.Search(o => 1 == 1).ToList<StoreSTMonthlyFinaceInfoTTM>();
            if (lsStoreSTMonthlyFinaceInfoTTM.Count > 0) mStoreSTMonthlyFinaceInfoTTM = lsStoreSTMonthlyFinaceInfoTTM[0];

            string result = JsonConvert.SerializeObject(mStoreSTMonthlyFinaceInfoTTM);

            return Content(result);
        }

        private static string GetLatestYear()
        {
            string rtnStr = string.Empty;
            if (DateTime.Now.Month == 1)
            {
                rtnStr = DateTime.Now.AddYears(-1).Year.ToString();
            }
            else
            {
                rtnStr = DateTime.Now.Year.ToString();
            }
            return rtnStr;
        }

        private static string GetLatestMonth()
        {
            string rtnStr = string.Empty;
            if (DateTime.Now.Month == 1)
            {
                rtnStr = "12";
            }
            else
            {
                rtnStr = DateTime.Now.AddMonths(-1).Month.ToString();
                if (rtnStr.Length < 2)
                {
                    rtnStr = "0" + rtnStr;
                }
            }
            return rtnStr;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region //  合同信息 Store ContractInfo

        //  租赁历史[from StoreContractInfo]
        public ActionResult StoreContractInfoListQuery(string _USCode)
        {
            var lsStoreContractInfo = new List<StoreContractInfo>();
            var bll = new StoreContractInfo();
            lsStoreContractInfo = bll.GetStoreContractInfo(_USCode);

            string result = JsonConvert.SerializeObject(lsStoreContractInfo);

            return Content(result);
        }

        //  合同信息[from StoreContractInfo]
        public ActionResult StoreContractInfoByIDQuery(Guid _ID)
        {
            StoreContractInfo mStoreContractInfo = new StoreContractInfo();

            mStoreContractInfo = mStoreContractInfo.GetStoreContractInfo(_ID);
            string result = JsonConvert.SerializeObject(mStoreContractInfo);

            return Content(result);
        }

        //  修订信息[from StoreContractRevision]
        public ActionResult StoreContractRevisionQuery(int _LeaseRecapID)
        {
            List<StoreContractRevision> lsStoreContractRevision = new List<StoreContractRevision>();

            lsStoreContractRevision = StoreContractRevision.Search(o => o.LeaseRecapID == _LeaseRecapID).ToList<StoreContractRevision>();

            string result = JsonConvert.SerializeObject(lsStoreContractRevision);

            return Content(result);
        }

        //  附件记录[from StoreContractInfoAttached]
        public ActionResult StoreContractInfoAttachedQuery(string _LeaseRecapID)
        {
            List<StoreContractInfoAttached> lsStoreContractInfoAttached = new List<StoreContractInfoAttached>();

            lsStoreContractInfoAttached = StoreContractInfoAttached.Search(o => o.LeaseRecapID == _LeaseRecapID).ToList<StoreContractInfoAttached>().OrderByDescending(o => o.CreateDate).ToList();

            string result = JsonConvert.SerializeObject(lsStoreContractInfoAttached);

            return Content(result);
        }

        #endregion

        #region //  业主联系记录 Store OwnerContactInfo
        //  业主联系记录
        public ActionResult StoreSTLLRecordQuery(string _USCode)
        {
            List<StoreSTLLRecord> lsStoreContractInfo = new List<StoreSTLLRecord>();
            StoreSTLLRecord mStoreSTLLRecord = new StoreSTLLRecord();

            lsStoreContractInfo = StoreSTLLRecord.Search(o => o.StoreCode == _USCode).OrderByDescending(o => o.CreatedTime).ToList<StoreSTLLRecord>();
            if (lsStoreContractInfo.Count > 0) mStoreSTLLRecord = lsStoreContractInfo[0];

            string result = JsonConvert.SerializeObject(mStoreSTLLRecord);

            return Content(result);
        }

        //  业主联系记录-谈判记录
        public ActionResult StoreSTNegotiationQuery(string _USCode)
        {
            List<StoreSTNegotiation> lsStoreSTNegotiation = new List<StoreSTNegotiation>();

            lsStoreSTNegotiation = StoreSTNegotiation.Search(o => o.StoreCode == _USCode).OrderByDescending(o => o.DateTime).ToList<StoreSTNegotiation>();

            string result = JsonConvert.SerializeObject(lsStoreSTNegotiation);

            return Content(result);
        }
        #endregion

        #region //  Licenses
        public ActionResult StoreSTLicenseQuery(string _USCode)
        {
            List<StoreSTLicense> lsStoreSTLicense = new List<StoreSTLicense>();
            lsStoreSTLicense = StoreSTLicense.Search(o => o.StoreCode == _USCode).OrderBy(o => o.Title).ToList<StoreSTLicense>();

            if (lsStoreSTLicense.Count == 0)
            {
                var licenseList = new List<StoreSTLicense>();
                var store = StoreBasicInfo.FirstOrDefault(i => i.StoreCode == _USCode);
                foreach (var dicItem in licenseDict)
                {
                    var storeLicense = new StoreSTLicense();
                    storeLicense.Id = Guid.NewGuid();
                    storeLicense.StoreID = store.StoreID;
                    storeLicense.StoreCode = _USCode;
                    storeLicense.Title = dicItem.Key;
                    storeLicense.CreatedTime = DateTime.Now;
                    storeLicense.DocType = dicItem.Value;
                    licenseList.Add(storeLicense);
                }
                StoreSTLicense.Add(licenseList.ToArray());
                lsStoreSTLicense = licenseList;
            }

            string result = JsonConvert.SerializeObject(lsStoreSTLicense);

            return Content(result);
        }

        private Dictionary<string, int> licenseDict
        {
            get
            {
                var result = new Dictionary<string, int>();
                result.Add("1. 施工报建", 660);
                result.Add("1.1 施工许可证", 661);
                result.Add("1.2 临时占用道路申请表,许可证", 662);
                result.Add("1.3 建设工程安全质量监督申报表", 663);
                result.Add("2. 消防", 664);
                result.Add("2.1 消防设计审核意见", 665);
                result.Add("2.2 消防设计审核意见书", 666);
                result.Add("2.3 餐厅开业消防许可证", 667);
                result.Add("3. 环评", 668);
                result.Add("3.1 室内环境质量检测报告", 669);
                result.Add("3.2 环境影响报告表", 670);
                result.Add("3.3 环保审核批复", 671);
                result.Add("3.4 环保竣工验收合格意见书", 672);
                result.Add("4. 门面/招牌许可文件", 673);
                result.Add("5. 营业执照", 674);
                result.Add("6. 餐饮服务许可证", 675);
                return result;
            }
        }
        #endregion

        /////////////////////////////////////////////////////////////////////////////////


        #region //  StoreAllInfoQuery Test
        public ActionResult StoreAllInfoQuery(string _USCode)
        {
            List<StoreBasicInfo> lsStoreBasicInfo = new List<StoreBasicInfo>();
            StoreBasicInfo mStoreBasicInfo = new StoreBasicInfo();
            lsStoreBasicInfo = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreBasicInfo>();
            if (lsStoreBasicInfo.Count > 0) mStoreBasicInfo = lsStoreBasicInfo[0];

            List<StoreDevelop> lsStoreDevelop = new List<StoreDevelop>();
            StoreDevelop mStoreDevelop = new StoreDevelop();
            lsStoreDevelop = StoreDevelop.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreDevelop>();
            if (lsStoreDevelop.Count > 0) mStoreDevelop = lsStoreDevelop[0];

            List<StoreOps> lsStoreOp = new List<StoreOps>();
            StoreOps mStoreOp = new StoreOps();
            lsStoreOp = StoreOps.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreOps>();
            if (lsStoreOp.Count > 0) mStoreOp = lsStoreOp[0];

            List<StoreContractInfo> lsStoreContractInfo = new List<StoreContractInfo>();
            StoreContractInfo mStoreContractInfo = new StoreContractInfo();
            lsStoreContractInfo = StoreContractInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreContractInfo>();
            if (lsStoreContractInfo.Count > 0) mStoreContractInfo = lsStoreContractInfo[0];

            var resultStoreAllInfo = new
            {
                StoreBasicInfo = mStoreBasicInfo,
                StoreDevelop = mStoreDevelop,
                StoreOp = mStoreOp,
                StoreContractInfo = new
                {
                    StoreContractInfo = mStoreContractInfo,
                    StoreContractRevision = "",
                    StoreContractInfoAttached = ""
                }
            };

            string result = JsonConvert.SerializeObject(resultStoreAllInfo);

            return Content(result);

        }
        #endregion


        #region Sumarry Update

        public ActionResult StoreSummaryDetail(string flowCode)
        {
            return Content(flowCode);
        }

        public ActionResult AddSummary()
        {
            return View();
        }

        public ActionResult SummaryByClosure()
        {
            return View();
        }

        public ActionResult SummaryByMajorLease()
        {
            return View();
        }

        public ActionResult SummaryByRebuild()
        {
            return View();
        }

        public ActionResult SummaryByReimage()
        {
            return View();
        }

        public ActionResult SummaryByRenewal()
        {
            return View();
        }
        #endregion
    }
}


