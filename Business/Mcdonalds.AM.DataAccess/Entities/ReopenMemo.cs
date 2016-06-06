using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReopenMemo : BaseEntity<ReopenMemo>
    {
        public StoreInfo Store { get; set; }
        public ReinvestmentBasicInfo ReinvestInfo { get; set; }
        public RebuildInfo RbdInfo { get; set; }
        public ReimageInfo RmgInfo { get; set; }
        public WriteOffAmount WriteOff { get; set; }
        public EstimatedVsActualConstruction EAC { get; set; }
        public List<SelectItem> YearMonthList { get; set; }
        public string ExteriorAfterImgURL1 { get; set; }
        public string ExteriorAfterImgURL2 { get; set; }
        public string InteriorAfterImgURL1 { get; set; }
        public string InteriorAfterImgURL2 { get; set; }
        public string TotalReinvestmentBudget { get; set; }
        public string PriceTiter { get; set; }
        public string OriginalOperationSize { get; set; }
        public string OriginalSeatNumber { get; set; }
        public string TTMNetSalesYearMonth { get; set; }

        public static ReopenMemo GetReopenMemo(string projectId)
        {
            var memo = FirstOrDefault(e => e.ProjectId.Equals(projectId));
            bool isEmptyMemo = false;
            if (memo == null)
            {
                memo = new ReopenMemo();
                isEmptyMemo = true;
            }
            
            memo.ProjectId = projectId;
            if (projectId.ToLower().IndexOf("rebuild") >= 0)
            {
                var rbdInfo = new RebuildInfo();
                memo.RbdInfo = rbdInfo.GetRebuildInfo(projectId);
                memo.Store = StoreBasicInfo.GetStore(memo.RbdInfo.USCode);
                var consInfo = new RebuildConsInfo();
                consInfo = consInfo.GetConsInfo(projectId);
                var rein = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentBasicInfo();
                memo.ReinvestInfo = rein;
                var recos = ReinvestmentCost.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentCost();
                memo.TotalReinvestmentBudget = recos.TotalReinvestmentBudget;
                if (!memo.ReopenDate.HasValue)
                    memo.ReopenDate = memo.ReinvestInfo.ReopenDate;
                if (!memo.GBDate.HasValue)
                    memo.GBDate = memo.ReinvestInfo.GBDate;
                var gbMemo = GBMemo.GetGBMemo(projectId);
                if (!memo.CompletionDate.HasValue && gbMemo!=null)
                    memo.CompletionDate = gbMemo.ConstCompletionDate;
                //if (string.IsNullOrEmpty(memo.DesignConcept))
                //    memo.DesignConcept = memo.ReinvestInfo.NewDesignType;

                var writeoff = WriteOffAmount.GetByConsInfoId(consInfo.Id) ?? new WriteOffAmount();
                memo.WriteOff = writeoff;

            }
            else if (projectId.ToLower().IndexOf("reimage") >= 0)
            {
                memo.RmgInfo = ReimageInfo.GetReimageInfo(projectId);
                memo.Store = StoreBasicInfo.GetStore(memo.RmgInfo.USCode);
                var consInfo = ReimageConsInfo.GetConsInfo(projectId);
                var rein = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentBasicInfo();
                memo.ReinvestInfo = rein;
                var recos = ReinvestmentCost.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentCost();
                memo.TotalReinvestmentBudget = recos.TotalReinvestmentBudget;
                if (!memo.ReopenDate.HasValue)
                    memo.ReopenDate = memo.ReinvestInfo.ReopenDate;
                if (!memo.GBDate.HasValue)
                    memo.GBDate = memo.ReinvestInfo.GBDate;
                var gbMemo = ReimageGBMemo.GetGBMemo(projectId);
                if (!memo.CompletionDate.HasValue && gbMemo != null)
                    memo.CompletionDate = gbMemo.ConstCompletionDate;
                //if (string.IsNullOrEmpty(memo.DesignConcept))
                //    memo.DesignConcept = memo.ReinvestInfo.NewDesignType;
                var writeoff = WriteOffAmount.GetByConsInfoId(consInfo.Id) ?? new WriteOffAmount();
                memo.WriteOff = writeoff;
            }
            else if (projectId.ToLower().IndexOf("majorlease") >= 0)
            {
                var mjrInfo = new MajorLeaseInfo().GetMajorLeaseInfo(projectId);
                memo.ReopenDate = mjrInfo.ReopenDate;
                memo.Store = StoreBasicInfo.GetStore(mjrInfo.USCode);
                var consInfo = new MajorLeaseConsInfo().GetConsInfo(projectId);
                var rein = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentBasicInfo();
                memo.ReinvestInfo = rein;
                var recos = ReinvestmentCost.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentCost();
                memo.TotalReinvestmentBudget = recos.TotalReinvestmentBudget;
                if (!memo.ReopenDate.HasValue)
                    memo.ReopenDate = memo.ReinvestInfo.ReopenDate;
                if (!memo.GBDate.HasValue)
                    memo.GBDate = memo.ReinvestInfo.GBDate;
                var gbMemo = MajorLeaseGBMemo.GetGBMemo(projectId);
                if (!memo.CompletionDate.HasValue && gbMemo != null)
                    memo.CompletionDate = gbMemo.ConstCompletionDate;
                //if (string.IsNullOrEmpty(memo.DesignConcept))
                //    memo.DesignConcept = memo.ReinvestInfo.NewDesignType;
                var writeoff = WriteOffAmount.GetByConsInfoId(consInfo.Id) ?? new WriteOffAmount();
                memo.WriteOff = writeoff;
            }
            else if (projectId.ToLower().IndexOf("renewal") >= 0)
            {
                var renewalInfo = RenewalInfo.Get(projectId);
                memo.ReopenDate = renewalInfo.NewLeaseStartDate;
                memo.Store = StoreBasicInfo.GetStore(renewalInfo.USCode);
                var consInfo = RenewalConsInfo.Get(projectId);
                var rein = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentBasicInfo();
                memo.ReinvestInfo = rein;
                var recos = ReinvestmentCost.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentCost();
                memo.TotalReinvestmentBudget = recos.TotalReinvestmentBudget;
                if (!memo.ReopenDate.HasValue)
                    memo.ReopenDate = memo.ReinvestInfo.ReopenDate;
                if (!memo.GBDate.HasValue)
                    memo.GBDate = memo.ReinvestInfo.GBDate;
                var gbMemo = RenewalGBMemo.GetGBMemo(projectId);
                if (!memo.CompletionDate.HasValue && gbMemo != null)
                    memo.CompletionDate = gbMemo.ConstCompletionDate;
                //if (string.IsNullOrEmpty(memo.DesignConcept))
                //    memo.DesignConcept = memo.ReinvestInfo.NewDesignType;
                var writeoff = WriteOffAmount.GetByConsInfoId(consInfo.Id) ?? new WriteOffAmount();
                memo.WriteOff = writeoff;
            }
            if (isEmptyMemo)
            {
                if (memo.ReinvestInfo != null)
                {
                    if (memo.ReinvestInfo.NewMcCafe.HasValue && memo.ReinvestInfo.NewMcCafe.Value)
                    {
                        memo.NewMcCafe = true;
                    }
                    if ((memo.ReinvestInfo.NewAttachedKiosk.HasValue && memo.ReinvestInfo.NewAttachedKiosk.Value)
                        || (memo.ReinvestInfo.NewRemoteKiosk.HasValue && memo.ReinvestInfo.NewRemoteKiosk.Value))
                    {
                        memo.NewKiosk = true;
                    }
                    if (memo.ReinvestInfo.NewMDS.HasValue && memo.ReinvestInfo.NewMDS.Value)
                    {
                        memo.NewMDS = true;
                    }
                    if (memo.ReinvestInfo.NewTwientyFourHour.HasValue && memo.ReinvestInfo.NewTwientyFourHour.Value)
                    {
                        memo.Is24H = true;
                    }
                    //if (!string.IsNullOrEmpty(memo.ReinvestInfo.NewOperationSize))
                    //{
                    //    memo.AftOperationSize = memo.ReinvestInfo.NewOperationSize;
                    //}
                    //if (!string.IsNullOrEmpty(memo.ReinvestInfo.EstimatedSeatNo))
                    //{
                    //    memo.AftARSN = memo.ReinvestInfo.EstimatedSeatNo;
                    //}
                }
                memo.TTMNetSales = GetTTFinanceData(memo.Store.StoreBasicInfo.StoreCode);
            }
            memo.PriceTiter = GetPriceTier(memo.Store.StoreBasicInfo.StoreCode);
            //if (string.IsNullOrEmpty(memo.AftARPT))
            //{
            //    memo.AftARPT = memo.PriceTiter;
            //}
            //if (memo.Store != null)
            //{
            //    memo.OriginalOperationSize = memo.Store.StoreSTLocation.TotalArea;
            //    memo.OriginalSeatNumber = memo.Store.StoreSTLocation.TotalSeatsNo;
            //}
            memo.YearMonthList = GetSelectYearMonth(memo.Store.StoreBasicInfo.StoreCode);
            if (isEmptyMemo)
                SaveReopenMemo(memo);
            return memo;
        }

        public static string GetTTFinanceData(string uscode)
        {
            var financeData = DataSync_LDW_AM_STFinanceData.Search(f => f.UsCode == uscode)
                .OrderByDescending(e=>e.FinanceYear)
                .ThenByDescending(e=>e.FinanceMonth)
                .FirstOrDefault();
            
            string returnVal="";

            if (financeData != null)
            {
                returnVal = financeData.Total_Sales_TTM;
            }
            return returnVal;
        }

        public static string GetPriceTier(string uscode)
        {
            var financeData = DataSync_LDW_AM_STFinanceData2.Search(f => f.UsCode == uscode)
                .OrderByDescending(e => e.FinanceYear)
                .ThenByDescending(e => e.FinanceMonth)
                .FirstOrDefault();
            string returnVal = "";

            if (financeData != null)
            {
                returnVal = financeData.Price_Tier;
            }
            return returnVal;
        }

        public static List<SelectItem> GetSelectYearMonth(string uscode)
        {
            var financeData = DataSync_LDW_AM_STFinanceData.Search(f => f.UsCode == uscode)
                .Distinct()
                .OrderByDescending(e=>e.FinanceYear)
                .ThenByDescending(e=>e.FinanceMonth).Take(12).ToList();
            List<SelectItem> returnList = new List<SelectItem>();
            var index = 0;
            foreach (var item in financeData)
            {
                SelectItem sItem = new SelectItem();
                var yearMonth = item.FinanceYear + "-" + item.FinanceMonth;
                sItem.Name = yearMonth;
                sItem.Value = item.Total_Sales_TTM;
                if (index==0)
                    sItem.Selected = true;
                else
                    sItem.Selected = false;
                index++;
                returnList.Add(sItem);
            }
            return returnList;
        }

        public static void SaveReopenMemo(ReopenMemo memo)
        {
            memo.LastUpdateTime = DateTime.Now;
            memo.LastUpdateUserAccount = ClientCookie.UserCode;
            memo.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
            memo.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;

            //if (!string.IsNullOrEmpty(memo.ExteriorAfterImgURL1)
            //    && memo.ExteriorAfterImgURL1.ToLower().IndexOf("mcd_logo") == -1)
            //    memo.ExteriorAfterImg1 = memo.ExteriorAfterImgURL1.Split('/')[2];

            //if (!string.IsNullOrEmpty(memo.ExteriorAfterImgURL2)
            //    && memo.ExteriorAfterImgURL2.ToLower().IndexOf("mcd_logo") == -1)
            //    memo.ExteriorAfterImg2 = memo.ExteriorAfterImgURL2;

            //if (!string.IsNullOrEmpty(memo.InteriorAfterImgURL1)
            //    && memo.InteriorAfterImgURL1.ToLower().IndexOf("mcd_logo") == -1)
            //    memo.InteriorAfterImg1 = memo.InteriorAfterImgURL1;

            //if (!string.IsNullOrEmpty(memo.InteriorAfterImgURL2)
            //    && memo.InteriorAfterImgURL2.ToLower().IndexOf("mcd_logo") == -1)
            //    memo.InteriorAfterImg2 = memo.InteriorAfterImgURL2;

            if (memo.Id == Guid.Empty)
            {
                memo.Id = Guid.NewGuid();
                memo.CreateTime = DateTime.Now;
                memo.LastUpdateTime = DateTime.Now;
                Add(memo);
            }
            else
            {
                Update(memo);
            }
        }
        public static void Submit(ReopenMemo memo)
        {
            string strFlowCode = "";
            string strInputNodeCode = "";
            string strSendNodeCode = "";
            string strTypeCode = "";
            if (memo.ProjectId.ToLower().IndexOf("rebuild") != -1)
            {
                strFlowCode = FlowCode.Rebuild;
                strInputNodeCode = NodeCode.Rebuild_ReopenMemo_Input;
                strSendNodeCode = NodeCode.Rebuild_ReopenMemo_Send;
                strTypeCode = FlowCode.Rebuild_ReopenMemo;
            }
            else if (memo.ProjectId.ToLower().IndexOf("reimage") != -1)
            {
                strFlowCode = FlowCode.Reimage;
                strInputNodeCode = NodeCode.Reimage_ReopenMemo_Input;
                strSendNodeCode = NodeCode.Reimage_ReopenMemo_SendMemo;
                strTypeCode = FlowCode.Reimage_ReopenMemo;
            }
            using (var scope = new TransactionScope())
            {
                SaveReopenMemo(memo);
                if (memo.ProjectId.ToLower().IndexOf("rebuild") != -1
                    || memo.ProjectId.ToLower().IndexOf("reimage") != -1)
                {
                    var task = TaskWork.GetTaskWork(memo.ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish, strFlowCode, strTypeCode);
                    if (task != null)
                    {
                        task.Status = TaskWorkStatus.K2ProcessApproved;
                        task.ActivityName = "Finish";
                        task.FinishTime = DateTime.Now;
                        string taskUrl = "/" + strFlowCode + "/Main#/ReopenMemo/Process/View?projectId=" + memo.ProjectId;
                        task.Url = taskUrl;
                        TaskWork.Update(task);
                        ProjectInfo.FinishNode(memo.ProjectId, strTypeCode, strInputNodeCode);
                        ProjectInfo.FinishNode(memo.ProjectId, strTypeCode, strSendNodeCode);
                        ProjectInfo.CompleteMainIfEnable(memo.ProjectId);
                    }
                }
                else
                {
                    AttachmentsMemoProcessInfo.UpdateNotifyDate(memo.ProjectId, FlowCode.ReopenMemo);
                }
                scope.Complete();
            }
        }
    }
}
