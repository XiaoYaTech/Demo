using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/30/2014 5:08:32 PM
 * FileName     :   TempClosureReopenMemo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TempClosureReopenMemo : BaseEntity<TempClosureReopenMemo>
    {
        public DateTime? TempCloseDate { get; set; }

        public static TempClosureReopenMemo Get(string projectId)
        {
            var db = PrepareDb();
            var usCode = db.ProjectInfo.FirstOrDefault(pi => pi.ProjectId == projectId).USCode;
            var reopenMemo = FirstOrDefault(cm => cm.ProjectId == projectId);
            if (reopenMemo == null)
            {
                reopenMemo = SqlQuery<TempClosureReopenMemo>(@"
                    SELECT CAST('00000000-0000-0000-0000-000000000000' AS uniqueidentifier) [Id]
                      ,@ProjectId [ProjectId]
                      ,@USCode [USCode]
                      ,s.RegionCode [RegionCode]
                      ,s.RegionENUS [RegionENUS]
                      ,s.RegionZHCN [RegionZHCN]
                      ,s.MarketCode [MarketCode]
                      ,s.MarketENUS [MarketENUS]
                      ,s.MarketZHCN [MarketZHCN]
                      ,s.ProvinceENUS [ProvinceENUS]
                      ,s.ProvinceZHCN [ProvinceZHCN]
                      ,s.CityCode [CityCode]
                      ,s.CityENUS [CityENUS]
                      ,s.CityZHCN [CityZHCN]
                      ,s.NameENUS [StoreENUS]
                      ,s.NameZHCN [StoreZHCN]
                      ,'' [PipelineId]
                      ,NULL MarketDesirability
                      ,NULL RERating
                      ,NULL [ActualConsFinishDate]
                      ,ci.ActualReopenDate [OpeningDate]
                      ,s.PortfolioType [PortfolioType]
                      ,s.PortfolioTypeName [ProtfolioTypeName]
                      ,s.TACode [TAClassification]
                      ,s.TAName [TAClassificationName]
                      ,cast(cast(isnull(sloc.Seats1,0) as decimal)+cast(isnull(sloc.Seats2,0) as decimal)
	                  +cast(isnull(sloc.Seats3,0) as decimal)+cast(isnull(sloc.Seats4,0) as decimal)
                      +cast(isnull(sloc.Seats5,0) as decimal) as nvarchar(10)) SeatingNum
                      ,sloc.TotalArea [BusinessArea]
                      ,sloc.KitchenFloor [KitchenFloor]
                      ,sloc.Floor [SeatingFloor]
                      ,'' [EarlyTerminationClause]
                      ,NULL CarParkTotal
                      ,sloc.KitchenFloor [ParkingNum]
                      ,'' [ContractType]
                      ,'' [Kiosk]
                      ,sd.AssetRepEid [RERep]
                      ,sd.PlannerEid [Planner]
                      ,sd.RERepName RERepName
                      ,sd.PlannerName PlannerName
                      ,@Creator Creator
                      ,getdate() CreateTime
                    FROM StoreBasicInfo s INNER JOIN StoreSTLocation sloc
                    ON sloc.StoreCode = s.StoreCode INNER JOIN StoreDevelop sd
                    ON sd.StoreCode = s.StoreCode INNER JOIN TempClosureInfo ci
                    ON ci.USCode = s.StoreCode
                    WHERE s.StoreCode = @USCode AND ci.ProjectId = @ProjectId
                ", new
                 {
                     ProjectId = projectId,
                     USCode = usCode,
                     Creator = ClientCookie.UserCode
                 }).FirstOrDefault();
                var contractType = StoreContractInfo.SearchByProject(projectId).OrderByDescending(c => c.CreatedTime).Select(c => c.LeasePurchase).FirstOrDefault();
                var taInfo = StoreMMInfo.Search(ta => ta.StoreCode == usCode).Select(ta => new
                {
                    Desirability = ta.Desirability,
                    LocationRatingPP = ta.LocationRatingPP
                }).FirstOrDefault();
                var dic_contractType = Dictionary.GetDictionary(contractType);
                reopenMemo.ContractType = dic_contractType == null ? "" : dic_contractType.NameZHCN;
                var dic_desirability = Dictionary.GetDictionary(taInfo.Desirability);
                reopenMemo.MarketDesirability = dic_desirability == null ? "" : dic_desirability.NameZHCN;
                var dic_locationRatingPP = Dictionary.GetDictionary(taInfo.LocationRatingPP);
                reopenMemo.RERating = dic_locationRatingPP == null ? "" : dic_locationRatingPP.NameZHCN;
                var tempMemo = TempClosureMemo.GetTempClosureMemo(projectId);
                if (tempMemo != null)
                    reopenMemo.TempCloseDate = tempMemo.ClosureDate;
            }
            return reopenMemo;
        }

        public int Save()
        {
            int result = 0;
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
                CreateTime = DateTime.Now;
                Creator = ClientCookie.UserCode;
                result = Add(this);
            }
            else
            {
                result = Update(this);
            }
            return result;
        }

        public void GenerateReopenTask(string projectId)
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.TempClosure;
            taskWork.SourceNameENUS = taskWork.SourceCode;
            taskWork.SourceNameZHCN = taskWork.SourceCode;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = projectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            taskWork.CreateUserAccount = ClientCookie.UserCode;

            var tempClosure = TempClosureInfo.FirstOrDefault(i => i.ProjectId == projectId);
            if (tempClosure == null)
                return;

            taskWork.Title = TaskWork.BuildTitle(projectId, tempClosure.StoreNameZHCN, tempClosure.StoreNameENUS);
            taskWork.TypeCode = FlowCode.TempClosure_ReopenMemo;
            taskWork.TypeNameENUS = "ReopenMemo";
            taskWork.TypeNameZHCN = "ReopenMemo";
            taskWork.ReceiverAccount = tempClosure.AssetActorAccount;
            taskWork.ReceiverNameENUS = tempClosure.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = tempClosure.AssetActorNameZHCN;
            taskWork.Url = string.Format(@"/TempClosure/Main#/ReopenMemo?projectId={0}", projectId);
            taskWork.StoreCode = tempClosure.USCode;
            taskWork.ActivityName = "Start";

            //Re-open Date 前7天发出任务
            ScheduleLog.GenerateTaskSchedule(tempClosure.ActualReopenDate.AddDays(-7), taskWork, ClientCookie.UserCode, projectId, FlowCode.TempClosure_ReopenMemo, tempClosure.USCode);
        }

        public void Submit()
        {
            string strFlowCode = FlowCode.TempClosure;
            string strNodeCode = NodeCode.Finish;
            string strTypeCode = FlowCode.TempClosure_ReopenMemo;

            using (var scope = new TransactionScope())
            {
                Save();
                var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish, strFlowCode, strTypeCode);
                if (task != null)
                {
                    task.Status = TaskWorkStatus.K2ProcessApproved;
                    task.ActivityName = "Finish";
                    string taskUrl = "/" + strFlowCode + "/Main#/ReopenMemo/Process/View?projectId=" + ProjectId;
                    task.Url = taskUrl;
                    task.FinishTime = DateTime.Now;
                    TaskWork.Update(task);
                    ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.TempClosure_ReopenMemo_Input);
                    ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.TempClosure_ReopenMemo_Send, ProjectStatus.Finished);
                    //ProjectInfo.FinishNode(ProjectId, strTypeCode, strNodeCode);
                    ProjectInfo.CompleteMainIfEnable(ProjectId);
                }
                scope.Complete();
            }
        }
    }
}
