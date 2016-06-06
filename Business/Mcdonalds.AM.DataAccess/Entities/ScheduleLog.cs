using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ScheduleLog : BaseEntity<ScheduleLog>
    {
        public void Save()
        {
            if (Any(i => i.Id == Id))
                Update();
            else
                Add();
        }

        /// <summary>
        /// 定时触发创建任务
        /// </summary>
        /// <param name="ExecuteDate">触发时间</param>
        /// <param name="task">创建的任务实例</param>
        /// <param name="userAccount"></param>
        /// <param name="projectId"></param>
        /// <param name="projectId"></param>
        /// <param name="flowCode">子流程FlowCode</param>
        public static void GenerateTaskSchedule(DateTime ExecuteDate, TaskWork task, string userAccount, string projectId, string flowCode, string uscode = "")
        {
            var info = ScheduleLog.FirstOrDefault(i => i.ProjectId == projectId && i.FlowCode == flowCode && i.Action == ScheduleAction.Generate && i.IsExecuted == false);
            if (info == null)
            {
                info = new ScheduleLog();
                info.Id = Guid.NewGuid();
            }
            info.Action = ScheduleAction.Generate;
            info.ProjectId = projectId;
            info.FlowCode = flowCode;
            info.USCode = uscode;
            info.CreateTime = DateTime.Now;
            info.CreatorUserAccount = userAccount;
            info.ExecuteDate = ExecuteDate;
            info.Info = TaskWork.ConvertToJson(task);
            info.IsExecuted = false;
            info.Save();
        }

        /// <summary>
        /// 定时触发改变店面状态
        /// </summary>
        /// <param name="uscode"></param>
        /// <param name="projectId"></param>
        /// <param name="ExecuteDate">触发时间</param>
        /// <param name="userAccount"></param>
        public static void UpdateStoreStatusSchedule(string uscode, string projectId, DateTime ExecuteDate, string userAccount)
        {
            var info = ScheduleLog.FirstOrDefault(i => i.Action == ScheduleAction.Update && i.IsExecuted == false && i.USCode == uscode && i.ProjectId == projectId);
            if (info == null)
            {
                info = new ScheduleLog();
                info.Id = Guid.NewGuid();
            }
            info.Action = ScheduleAction.Update;
            info.CreateTime = DateTime.Now;
            info.CreatorUserAccount = userAccount;
            info.ExecuteDate = ExecuteDate;
            info.IsExecuted = false;
            info.ProjectId = projectId;
            info.USCode = uscode;
            info.Save();
        }
    }
}
