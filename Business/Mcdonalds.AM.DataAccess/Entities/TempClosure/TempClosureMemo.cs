using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/28/2014 11:20:48 AM
 * FileName     :   TempClosureMemo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TempClosureMemo : BaseEntity<TempClosureMemo>
    {
        public static TempClosureMemo Get(string projectId)
        {
            var db = PrepareDb();
            var closureMemo = FirstOrDefault(cm => cm.ProjectId == projectId);
            if (closureMemo == null)
            {
                closureMemo = SqlQuery<TempClosureMemo>(@"
                    SELECT CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) Id,p.ProjectID,s.RegionCode,s.RegionZHCN RegionNameZHCN,s.RegionENUS RegionNameENUS,
                        s.MarketCode MarketCode,s.MarketENUS MarketNameENUS,s.MarketZHCN MarketNameZHCN,
                        s.ProvinceZHCN ProvinceNameZHCN,s.ProvinceENUS ProvinceNameENUS,
                        s.CityCode,s.CityZHCN CityNameZHCN,s.CityENUS CityNameENUS,
                        s.StoreCode USCode,s.NameZHCN StoreNameZHCN,s.NameENUS StoreNameENUS,
                        s.AddressZHCN StoreAddressZHCN,s.AddressENUS StoreAddressENUS,s.OpenDate,
                        cl.ActualTempClosureDate ClosureDate,0 PipelineID,'' PipelineName,
                        2 ClosureNature,CAST(0 as BIT) PermanentCloseOpportunity,
                        CAST(0 as BIT) HasRelocationPlan,CAST(0 as BIT) BecauseOfReimaging,
                        CAST(0 as BIT) BecauseOfRemodel,cast(0 as BIT) BecauseOfDespute,
                        CAST(0 as BIT) BecauseOfRedevelopment,CAST(0 as BIT) BecauseOfPlanedClosure,
                        CAST(0 as BIT) BecauseOfRebuild,'' BecauseOfOthers,getdate() CreateTime,
                        CAST(0 as BIT) CompensationAwarded,NULL Compensation,CAST(0 as BIT) Status,NULL Creator
                    FROM ProjectInfo p INNER JOIN TempClosureInfo cl
                    ON cl.ProjectId = p.ProjectId LEFT JOIN dbo.StoreBasicInfo s
                    ON s.StoreCode = p.USCode
                    WHERE p.ProjectID = @ProjectId AND p.FlowCode = 'TempClosure_LegalReview'
                ", new
                {
                    ProjectId = projectId
                }).FirstOrDefault();
            }
            return closureMemo;
        }

        public static TempClosureMemo GetAttachClosureMemo(string projectId, string flowCode)
        {
            var db = PrepareDb();
            var closureMemo = FirstOrDefault(cm => cm.ProjectId == projectId);
            if (closureMemo == null)
            {
                closureMemo = SqlQuery<TempClosureMemo>(@"
                    SELECT DISTINCT CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER) Id,p.ProjectID,s.RegionCode,s.RegionZHCN RegionNameZHCN,s.RegionENUS RegionNameENUS,
                        s.MarketCode MarketCode,s.MarketENUS MarketNameENUS,s.MarketZHCN MarketNameZHCN,
                        s.ProvinceZHCN ProvinceNameZHCN,s.ProvinceENUS ProvinceNameENUS,
                        s.CityCode,s.CityZHCN CityNameZHCN,s.CityENUS CityNameENUS,
                        s.StoreCode USCode,s.NameZHCN StoreNameZHCN,s.NameENUS StoreNameENUS,
                        s.AddressZHCN StoreAddressZHCN,s.AddressENUS StoreAddressENUS,s.OpenDate,
                        cl.ActualTempClosureDate ClosureDate,0 PipelineID,'' PipelineName,
                        1 ClosureNature,CAST(0 as BIT) PermanentCloseOpportunity,
                        CAST(0 as BIT) HasRelocationPlan,CAST(0 as BIT) BecauseOfReimaging,
                        CAST(0 as BIT) BecauseOfRemodel,cast(0 as BIT) BecauseOfDespute,
                        CAST(0 as BIT) BecauseOfRedevelopment,CAST(0 as BIT) BecauseOfPlanedClosure,
                        CAST(0 as BIT) BecauseOfRebuild,'' BecauseOfOthers,getdate() CreateTime,
                        CAST(0 as BIT) CompensationAwarded,NULL Compensation,CAST(0 as BIT) Status,NULL Creator
                    FROM AttachmentsMemoProcessInfo p LEFT JOIN TempClosureInfo cl
                    ON cl.ProjectId = p.ProjectId LEFT JOIN dbo.StoreBasicInfo s
                    ON s.StoreCode = p.USCode
                    WHERE p.ProjectID = @ProjectId AND p.FlowCode = @FlowCode
                ", new
                 {
                     ProjectId = projectId,
                     FlowCode = flowCode
                 }).FirstOrDefault();
                closureMemo.Save();
            }
            return closureMemo;
        }

        public static TempClosureMemo GetTempClosureMemo(string projectId)
        {
            var memo = FirstOrDefault(e => e.ProjectId.Equals(projectId));
            if (memo == null)
            {
                memo = new TempClosureMemo();
                memo.ProjectId = projectId;
                var eac = new EstimatedVsActualConstruction();
                if (projectId.ToLower().IndexOf("rebuild") >= 0)
                {
                    var rbdInfo = new RebuildInfo();
                    rbdInfo = rbdInfo.GetRebuildInfo(projectId);
                    var store = StoreBasicInfo.GetStore(rbdInfo.USCode);
                    eac = eac.GetEAC(rbdInfo.Id);
                    if (rbdInfo != null)
                    {
                        memo.ClosureDate = rbdInfo.TempClosureDate;
                    }
                    if (store != null)
                    {
                        memo.RegionCode = store.StoreBasicInfo.RegionCode;
                        memo.RegionNameENUS = store.StoreBasicInfo.RegionENUS;
                        memo.RegionNameZHCN = store.StoreBasicInfo.RegionZHCN;

                        memo.MarketCode = store.StoreBasicInfo.MarketCode;
                        memo.MarketNameENUS = store.StoreBasicInfo.MarketENUS;
                        memo.MarketNameZHCN = store.StoreBasicInfo.MarketZHCN;

                        memo.ProvinceNameENUS = store.StoreBasicInfo.ProvinceENUS;
                        memo.ProvinceNameZHCN = store.StoreBasicInfo.ProvinceZHCN;

                        memo.CityCode = store.StoreBasicInfo.CityCode;
                        memo.CityNameENUS = store.StoreBasicInfo.CityENUS;
                        memo.CityNameZHCN = store.StoreBasicInfo.CityZHCN;

                        memo.StoreNameENUS = store.StoreBasicInfo.NameENUS;
                        memo.StoreNameZHCN = store.StoreBasicInfo.NameZHCN;

                        memo.StoreAddressENUS = store.StoreBasicInfo.AddressENUS;
                        memo.StoreAddressZHCN = store.StoreBasicInfo.AddressZHCN;

                        memo.USCode = store.StoreBasicInfo.StoreCode;
                        memo.OpenDate = store.StoreBasicInfo.OpenDate;
                    }
                }
                else if (projectId.ToLower().IndexOf("reimage") >= 0)
                {
                    var rmgInfo = ReimageInfo.GetReimageInfo(projectId);
                    var store = StoreBasicInfo.GetStore(rmgInfo.USCode);
                    eac = eac.GetEAC(rmgInfo.Id);
                    memo.ClosureDate = DateTime.Now;
                    if (store != null)
                    {
                        memo.RegionCode = store.StoreBasicInfo.RegionCode;
                        memo.RegionNameENUS = store.StoreBasicInfo.RegionENUS;
                        memo.RegionNameZHCN = store.StoreBasicInfo.RegionZHCN;

                        memo.MarketCode = store.StoreBasicInfo.MarketCode;
                        memo.MarketNameENUS = store.StoreBasicInfo.MarketENUS;
                        memo.MarketNameZHCN = store.StoreBasicInfo.MarketZHCN;

                        memo.ProvinceNameENUS = store.StoreBasicInfo.ProvinceENUS;
                        memo.ProvinceNameZHCN = store.StoreBasicInfo.ProvinceZHCN;

                        memo.CityCode = store.StoreBasicInfo.CityCode;
                        memo.CityNameENUS = store.StoreBasicInfo.CityENUS;
                        memo.CityNameZHCN = store.StoreBasicInfo.CityZHCN;

                        memo.StoreNameENUS = store.StoreBasicInfo.NameENUS;
                        memo.StoreNameZHCN = store.StoreBasicInfo.NameZHCN;

                        memo.StoreAddressENUS = store.StoreBasicInfo.AddressENUS;
                        memo.StoreAddressZHCN = store.StoreBasicInfo.AddressZHCN;

                        memo.USCode = store.StoreBasicInfo.StoreCode;
                        memo.OpenDate = store.StoreBasicInfo.OpenDate;
                    }
                }
                memo.Save();
            }
            return memo;
        }

        public TempClosureMemo GetClosureMemo(string projectId)
        {
            return FirstOrDefault(e => e.ProjectId.Equals(projectId));
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

        public void Submit()
        {
            string strFlowCode = FlowCode.TempClosure;
            string strNodeCode = NodeCode.Finish;
            string strTypeCode = FlowCode.TempClosure_ClosureMemo;
            if (ProjectId.ToLower().IndexOf("rebuild") != -1)
            {
                strFlowCode = FlowCode.Rebuild;
                //strNodeCode = NodeCode.Finish;
                strTypeCode = FlowCode.Rebuild_TempClosureMemo;
            }
            else if (ProjectId.ToLower().IndexOf("reimage") != -1)
            {
                strFlowCode = FlowCode.Reimage;
                //strNodeCode = NodeCode.Finish;
                strTypeCode = FlowCode.Reimage_TempClosureMemo;
            }
            using (var scope = new TransactionScope())
            {
                Save();
                if (ProjectId.ToLower().IndexOf("majorlease") != -1
                    || ProjectId.ToLower().IndexOf("renewal") != -1)
                {
                    AttachmentsMemoProcessInfo.UpdateNotifyDate(ProjectId, FlowCode.ClosureMemo);
                }
                else
                {
                    var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish, strFlowCode, strTypeCode);
                    if (task != null)
                    {
                        task.Status = TaskWorkStatus.K2ProcessApproved;
                        task.ActivityName = "Finish";
                        string taskUrl = "/" + strFlowCode + "/Main#/ClosureMemo/Process/View?projectId=" + ProjectId;
                        if (ProjectId.ToLower().IndexOf("rebuild") != -1
                            || ProjectId.ToLower().IndexOf("reimage") != -1)
                        {
                            taskUrl = "/" + strFlowCode + "/Main#/TempClosureMemo/Process/View?projectId=" + ProjectId;
                        }
                        task.Url = taskUrl;
                        task.FinishTime = DateTime.Now;
                        TaskWork.Update(task);
                        if (ProjectId.ToLower().IndexOf("rebuild") != -1)
                        {
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.Rebuild_TempClosureMemo_Input);
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.Rebuild_TempClosureMemo_Send, ProjectStatus.Finished);
                        }
                        else if (ProjectId.ToLower().IndexOf("reimage") != -1)
                        {
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.Reimage_TempClosureMemo_Input);
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.Reimage_TempClosureMemo_Send, ProjectStatus.Finished);
                        }
                        else
                        {
                            //TempClosure
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.TempClosure_ClosureMemo_Input);
                            ProjectInfo.FinishNode(ProjectId, strTypeCode, NodeCode.TempClosure_ClosureMemo_Send, ProjectStatus.Finished);
                        }
                        ProjectInfo.CompleteMainIfEnable(ProjectId);
                    }
                }
                scope.Complete();
            }
        }

        public void GenerateClosureMemoTask(string projectId)
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

            taskWork.Title = TaskWork.BuildTitle(projectId, tempClosure.StoreNameENUS, tempClosure.StoreNameZHCN);
            taskWork.TypeCode = FlowCode.TempClosure_ClosureMemo;
            taskWork.TypeNameENUS = "ClosureMemo";
            taskWork.TypeNameZHCN = "ClosureMemo";
            taskWork.ReceiverAccount = tempClosure.AssetActorAccount;
            taskWork.ReceiverNameENUS = tempClosure.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = tempClosure.AssetActorNameZHCN;
            taskWork.Url = string.Format(@"/TempClosure/Main#/ClosureMemo?projectId={0}", projectId);
            taskWork.StoreCode = tempClosure.USCode;
            taskWork.ActivityName = "Start";

            TaskWork.Add(taskWork);

        }
    }
}
