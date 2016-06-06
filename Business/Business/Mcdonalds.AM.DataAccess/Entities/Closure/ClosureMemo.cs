using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/26/2014 2:13:50 PM
 * FileName     :   ClosureMemo
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
    public partial class ClosureMemo : BaseEntity<ClosureMemo>
    {
        public static ClosureMemo Get(string projectId)
        {
            var closureMemo = FirstOrDefault(cm => cm.ProjectId == projectId && cm.Status);
            if (closureMemo == null)
            {
                var info = ProcedureManager.Proc_PrepareClosureMemo(projectId);
                closureMemo = new ClosureMemo();
                closureMemo.ProjectId = projectId;
                closureMemo.RegionCode = info.RegionCode;
                closureMemo.RegionNameENUS = info.RegionNameENUS;
                closureMemo.RegionNameZHCN = info.RegionNameZHCN;
                closureMemo.MarketCode = info.MarketCode;
                closureMemo.MarketNameENUS = info.MarketENUS;
                closureMemo.MarketNameZHCN = info.MarketZHCN;
                closureMemo.ProvinceCode = info.ProvinceNameENUS;
                closureMemo.ProvinceNameENUS = info.ProvinceNameENUS;
                closureMemo.ProvinceNameZHCN = info.ProvinceNameZHCN;
                closureMemo.CityCode = info.CityCode;
                closureMemo.CityNameENUS = info.CityNameENUS;
                closureMemo.CityNameZHCN = info.CityNameZHCN;
                closureMemo.USCode = info.USCode;
                closureMemo.StoreNameENUS = info.StoreNameENUS;
                closureMemo.StoreNameZHCN = info.StoreNameZHCN;
                closureMemo.StoreAddressENUS = info.StoreAddressENUS;
                closureMemo.StoreAddressZHCN = info.StoreAddressZHCN;
                closureMemo.OpenDate = info.OpenDate;
                closureMemo.ClosureDate = info.ClosureDate;
                closureMemo.ClosureNature = ClosureNatureType.Permanent;
                closureMemo.Status = true;
                closureMemo.HasRelocationPlan = info.IsRelocation.HasValue ? info.IsRelocation.Value : false;
                closureMemo.PipelineId = info.PipelineID;
                closureMemo.PipelineName = info.PipelineName;
            }
            return closureMemo;
        }

        public void GenerateClourseMemoTask(string projectId)
        {
            if (!TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Closure_Memo))
            {
                var taskWork = new TaskWork();
                taskWork.SourceCode = FlowCode.Closure;
                taskWork.SourceNameENUS = taskWork.SourceCode;
                taskWork.SourceNameZHCN = "关店流程";
                taskWork.Status = TaskWorkStatus.UnFinish;
                taskWork.StatusNameZHCN = "任务";
                taskWork.StatusNameENUS = "任务";
                taskWork.RefID = projectId;
                taskWork.Id = Guid.NewGuid();
                taskWork.CreateTime = DateTime.Now;

                var closureInfo = ClosureInfo.GetByProjectId(projectId);

                taskWork.Title = TaskWork.BuildTitle(projectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
                taskWork.TypeCode = FlowCode.Closure_Memo;
                taskWork.TypeNameENUS = "Closure Memo";
                taskWork.TypeNameZHCN = "Closure Memo";
                taskWork.ReceiverAccount = closureInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = closureInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = closureInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/Closure/Main#/ClosureMemo?projectId={0}", projectId);
                taskWork.ActivityName = "Start";
                taskWork.StoreCode = closureInfo.USCode;

                //TaskWork.Add(taskWork);
                //关店前7天发起Memo流程
                if (closureInfo.ActualCloseDate.HasValue)
                    ScheduleLog.GenerateTaskSchedule(closureInfo.ActualCloseDate.Value.AddDays(-7), taskWork, ClientCookie.UserCode, projectId, FlowCode.Closure_Memo, closureInfo.USCode);
            }
        }
    }

}
