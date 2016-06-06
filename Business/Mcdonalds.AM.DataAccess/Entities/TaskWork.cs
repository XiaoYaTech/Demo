using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels.Condition;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Linq.Expressions;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TaskWork : BaseEntity<TaskWork>
    {

        public string OperateMsg { get; set; }
        public string TypeName
        {
            get
            {
                return I18N.GetValue(this, "TypeName");
            }
        }

        public string SourceName
        {
            get
            {
                return I18N.GetValue(this, "SourceName");
            }
        }

        public TaskWorkType TaskType
        {
            get
            {
                TaskWorkType taskType;
                if (!this.Status.HasValue || this.Status == TaskWorkStatus.UnFinish)
                {
                    switch (ActivityName)
                    {
                        case "Start":
                        case "Start_MaterTrack_Rep":
                        case "Start_MaterTrack_Feedback":
                        case "Asset Actor":
                        case "Actor Upload":
                        case "Asset Rep Upload":
                        case "AssetActor":
                            taskType = TaskWorkType.Task;
                            break;
                        case "Originator":
                            var projectInfo =
                                ProjectInfo.Search(e => e.ProjectId == RefID && e.FlowCode == TypeCode).FirstOrDefault();
                            if (projectInfo != null)
                            {
                                if (projectInfo.Status == ProjectStatus.Recalled)
                                {
                                    taskType = TaskWorkType.Recall;
                                }
                                else if (projectInfo.Status == ProjectStatus.Finished)
                                {
                                    taskType = TaskWorkType.Task;
                                }
                                else
                                {
                                    taskType = TaskWorkType.Return;
                                }
                            }
                            else
                                taskType = TaskWorkType.Task;
                            break;
                        default:
                            taskType = TaskWorkType.Approve;
                            break;
                    }
                }
                else//已办任务按照当前操作来显示
                {
                    switch (ActionName)
                    {
                        case "Approve":
                            taskType = TaskWorkType.Approve;
                            break;
                        case "Return":
                            taskType = TaskWorkType.Return;
                            break;
                        case "Decline":
                            taskType = TaskWorkType.Decline;
                            break;
                        default:
                            taskType = TaskWorkType.Task;
                            break;
                    }
                }
                return taskType;
            }
        }


        public string ProjectDetailUrl
        {
            get { return string.Format("/Home/Main#/project/detail/{0}?flowCode={1}", RefID, TypeCode); }
        }

        public string GetViewUrl()
        {
            string[] flowCodes = TypeCode.Split('_');
            return string.Format("/{0}/Main#/{1}/Process/View?projectId={2}&procInstID={3}", flowCodes[0], flowCodes[1], RefID, ProcInstID);
        }

        public static bool IsExistsTask(string projectId, string typeCode)
        {
            return Any(e => e.RefID == projectId && e.TypeCode == typeCode && e.Status != TaskWorkStatus.Cancel);
        }

        /// <summary>
        /// 判断当前子流程的K2审批是否完成
        /// </summary>
        /// <param name="procInstId">The process instance identifier.</param>
        /// <param name="typeCode">The type code.</param>
        /// <param name="currentUserAccount">The current user account.</param>
        /// <returns><c>true</c> if [is k2 finished] [the specified proc inst identifier]; otherwise, <c>false</c>.</returns>
        public static bool IsK2Finished(int procInstId, string typeCode, string currentUserAccount)
        {
            bool result = false;

            if (procInstId > 0)
            {
                result = Count(e => e.ProcInstID == procInstId && e.TypeCode == typeCode && e.Status == TaskWorkStatus.K2ProcessApproved) > 0;
            }
            return result;
        }

        public static TaskWork CreateTask()
        {
            var task = new TaskWork();
            task.Status = TaskWorkStatus.UnFinish;
            task.StatusNameZHCN = "任务";
            task.StatusNameENUS = "任务";
            task.Id = Guid.NewGuid();
            task.CreateTime = DateTime.Now;
            task.CreateUserAccount = ClientCookie.UserCode;
            task.Sequence = 0;
            task.ActivityName = "Start";
            return task;
        }

        public static int SendTask(string refId, string title, string storeCode, string url, ProjectUsers receiver, string sourceCode, string typeCode, string activityName, int? procInstId = null)
        {
            var task = new TaskWork();
            var source = FlowInfo.Get(sourceCode);
            var taskType = FlowInfo.Get(typeCode);
            task.SourceCode = source.Code;
            task.SourceNameENUS = source.NameENUS;
            task.SourceNameZHCN = source.NameZHCN;
            task.Status = TaskWorkStatus.UnFinish;
            task.StatusNameZHCN = "任务";
            task.StatusNameENUS = "任务";
            task.Title = title;
            task.RefID = refId;
            task.StoreCode = storeCode;
            task.ReceiverAccount = receiver.UserAccount;
            task.ReceiverNameENUS = receiver.UserNameENUS;
            task.ReceiverNameZHCN = receiver.UserNameZHCN;
            task.TypeCode = taskType.Code;
            task.TypeNameENUS = taskType.NameENUS;
            task.TypeNameZHCN = taskType.NameZHCN;
            task.ProcInstID = procInstId;
            task.Id = Guid.NewGuid();
            task.Url = url;
            task.CreateTime = DateTime.Now;
            task.CreateUserAccount = ClientCookie.UserCode;
            task.Sequence = 0;
            task.ActivityName = activityName;
            return Add(task);
        }

        public static string BuildTitle(string projectId, string StoreNameZHCN, string StoreNameENUS)
        {
            return string.Format("{0} {1} {2}", projectId, StoreNameENUS, StoreNameZHCN);
        }
        public static string BuildUrl(string flowCode, string projectId, string taskSection = null)
        {
            var codeParts = flowCode.Split('_');
            return string.Format("/{0}/Main#/{1}{2}?projectId={3}", codeParts[0], codeParts[1], taskSection, projectId);
        }

        public IEnumerable<TaskWork> GetFinishedTasks(string refID)
        {
            string sql = string.Format(@"	SELECT * FROM dbo.TaskWork
			WHERE ISNULL(ProcInstID,-1) IN(SELECT ISNULL(MAX(ProcInstID),-1) FROM taskwork
			WHERE RefID = '{0}' GROUP BY TypeCode) 
			And RefID = '{0}'
			AND Status = {1} ", refID, (int)TaskWorkStatus.Finished);
            var list = SqlQuery<TaskWork>(sql, null).ToList();
            //var list = new TaskWork().Search(c => c.Status == TaskWorkStatus.K2Finished && c.SourceCode == sourceCode && c.RefID == refID);
            return list;
        }


        public static string ConvertToJson(TaskWork entity)
        {

            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string serialStr = JsonConvert.SerializeObject(entity, Formatting.Indented, timeConverter);

            return serialStr;
        }

        public static List<TaskWorkOperator> GetOperators(string typeCode, string refId)
        {
            string sqlStr = @"SELECT DISTINCT temp.TemplateENUS AS TemplateENUS,
                        temp.TemplateZHCN AS TemplateZHCN,
                        t.ReceiverAccount AS Code,
                        t.ReceiverNameZHCN AS NameZHCN,
                        t.ReceiverNameENUS AS NameENUS
                FROM dbo.TaskWork t OUTER APPLY
                (	
	                SELECT TOP 1 pn.*,n.TemplateENUS,n.TemplateZHCN FROM dbo.ProjectNode pn INNER JOIN NodeInfo n
	                ON n.Code = pn.NodeCode  WHERE pn.Status = {0} AND pn.ProjectId = t.RefID AND pn.FlowCode = t.TypeCode  ORDER BY n.Sequence
                ) temp
                WHERE t.RefID = @RefId AND t.TypeCode = @TypeCode
                 AND t.Status = 0";
            var tsk = FirstOrDefault(e => e.RefID == refId && e.TypeCode == typeCode && e.Status == TaskWorkStatus.UnFinish);
            if (tsk != null && (tsk.ActivityName == "Originator" || tsk.ActivityName == "Start") && !typeCode.Contains("Closure"))
            {
                if (tsk.ActivityName == "Start")
                {
                    var comptask = FirstOrDefault(e => e.RefID == refId && e.TypeCode == typeCode && e.Status == TaskWorkStatus.K2ProcessApproved);
                    if (comptask != null)
                        sqlStr = string.Format(sqlStr, "3");
                    else//not edit
                        sqlStr = string.Format(sqlStr, "1");
                }
                else
                    sqlStr = string.Format(sqlStr, "3");
            }
            else
                sqlStr = string.Format(sqlStr, "1");
            return SqlQuery<TaskWorkOperator>(sqlStr, new
            {
                RefId = refId,
                TypeCode = typeCode
            }).Distinct().ToList();
        }

        public static void Finish(Expression<Func<TaskWork, bool>> predicate)
        {
            var tasks = Search(predicate).ToList();
            tasks.ForEach(t =>
            {
                t.Url = t.GetViewUrl();
                t.FinishTime = DateTime.Now;
                t.Status = TaskWorkStatus.Finished;
            });
            Update(tasks.ToArray());
        }


        public static void Cancel(Expression<Func<TaskWork, bool>> predicate)
        {
            var tasks = Search(predicate).ToList();
            tasks.ForEach(t => t.Status = TaskWorkStatus.Cancel);
            Update(tasks.ToArray());
        }

        public void Finish()
        {
            Status = TaskWorkStatus.Finished;
            FinishTime = DateTime.Now;
            Url = GetViewUrl();
            Update(this);
        }

        public void Cancel()
        {
            Status = TaskWorkStatus.Cancel;
            Update(this);
        }

        public static TaskWork GetTaskWork(string projectId, string userAccount, TaskWorkStatus status, string sourceCode, string typeCode)
        {
            return FirstOrDefault(e => e.ReceiverAccount.Equals(userAccount)
                && e.Status == status && e.SourceCode.Equals(sourceCode)
                && e.TypeCode.Equals(typeCode)
                && e.RefID.Equals(projectId));
        }

        /// <summary>
        /// 获取处理过流程的用户的EID列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="flowCode">流程的Code</param>
        /// <returns></returns>
        public static List<string> GetProcessedUsers(string projectId, string flowCode)
        {
            List<string> userCodeList = new List<string>();
            userCodeList = Search(t => t.RefID.Equals(projectId) && t.TypeCode.Equals(flowCode) && (t.Status.Value != TaskWorkStatus.UnFinish && t.Status != TaskWorkStatus.K2TaskExpired && t.Status != TaskWorkStatus.None)).Select(t => t.ReceiverAccount).ToList();
            return userCodeList;
        }

        /// <summary>
        /// 获取处理过Package流程的用户的EID列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="flowCode"></param>
        /// <returns></returns>
        public static List<string> GetPackageProcessedUsers(string projectId, string flowCode)
        {
            var doneTaskList = Search(t => t.RefID.Equals(projectId) && t.TypeCode.Equals(flowCode) && (t.Status.Value != TaskWorkStatus.UnFinish && t.Status != TaskWorkStatus.K2TaskExpired && t.Status != TaskWorkStatus.None));
            var lastStartTaskList = doneTaskList.Where(i => i.ActivityName == "Originator" || i.ActivityName == "Start").OrderByDescending(i => i.CreateTime);
            if (lastStartTaskList.Count() > 0)
            {
                var lastStartTime = lastStartTaskList.ToArray()[0].CreateTime;
                doneTaskList = doneTaskList.Where(i => i.CreateTime >= lastStartTime);
            }
            return doneTaskList.Select(t => t.ReceiverAccount).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="flowCode"></param>
        /// <returns></returns>
        public static List<string> GetPackageProcessingUsers(string projectId, string flowCode)
        {
            var doingTaskList = Search(t => t.RefID.Equals(projectId) && t.TypeCode.Equals(flowCode) && t.Status.Value == TaskWorkStatus.UnFinish);
            return doingTaskList.Select(t => t.ReceiverAccount).ToList();
        }

        /// <summary>
        /// 根据K2流程的SN获取当前的Task对象
        /// </summary>
        /// <param name="sn">K2 SN</param>
        /// <returns>Task对象</returns>
        public static TaskWork GetTaskBySN(string sn)
        {
            TaskWork taskWork = Search(t => t.K2SN.Equals(sn)).SingleOrDefault();
            return taskWork;
        }

        public static IQueryable<TaskWork> GetUseableTaskWork()
        {
            var context = PrepareDb();
            var query = (from task in context.TaskWork
                         join projectInfo in context.ProjectInfo
                             on new { ProjectId = task.RefID, FlowCode = task.TypeCode } equals new { projectInfo.ProjectId, projectInfo.FlowCode }
                         where task.ActionName != ProjectAction.Pending
                         select task);

            return query;
        }

        public static void Complete(string projectId, string flowCode)
        {
            var taskWorks = Search(e => e.RefID == projectId
                                       && e.TypeCode == flowCode
                                       && e.ReceiverAccount == ClientCookie.UserCode).ToArray();
            foreach (var taskWork in taskWorks)
            {
                taskWork.Url = taskWork.GetViewUrl();
                taskWork.Status = TaskWorkStatus.Finished;
                taskWork.FinishTime = DateTime.Now;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.MajorLease_ContractInfo:
                    case FlowCode.MajorLease_SiteInfo:
                    case FlowCode.Reimage_SiteInfo:
                    case FlowCode.Rebuild_SiteInfo:
                        ProjectInfo.FinishProject(taskWork.RefID, taskWork.TypeCode);
                        break;
                }
            }

            Update(taskWorks);
        }

        /// <summary>
        /// Edit之后，给涉及到的TaskWork的Url加上isHistory=true
        /// </summary>
        /// <param name="procInstID"></param>
        public static void SetTaskHistory(Guid Id, int? procInstID)
        {
            var taskWorks = Search(e => e.ProcInstID == procInstID).ToArray();

            foreach (var taskWork in taskWorks)
            {
                if (!taskWork.Url.Contains("isHistory"))
                {
                    if (taskWork.Url.IndexOf('?') > 0)
                        taskWork.Url += "&";
                    else
                        taskWork.Url += "?";
                    taskWork.Url += "isHistory=true&Id=" + Id;
                }
            }

            Update(taskWorks);
        }

        public static IQueryable<TaskWork> Query(TaskWorkCondition searchCondition, out int totalSize)
        {
            var predicate = PredicateBuilder.True<TaskWork>();

            if (!string.IsNullOrEmpty(searchCondition.Title))
            {
                predicate = predicate.And(e => e.Title.Contains(searchCondition.Title));
            }

            if (!string.IsNullOrEmpty(searchCondition.StoreCode))
            {
                predicate = predicate.And(e => e.StoreCode.Contains(searchCondition.StoreCode));
            }

            if (!string.IsNullOrEmpty(searchCondition.StoreNameZHCN))
            {
                var storeList = StoreBasicInfo.Search(e => e.NameZHCN.Contains(searchCondition.StoreNameZHCN));
                var storeCodeList = storeList.Select(e => e.StoreCode).ToList();

                predicate = predicate.And(e => storeCodeList.Contains(e.StoreCode));
            }

            if (searchCondition.Status.HasValue)
            {
                predicate = predicate.And(e => e.Status == searchCondition.Status.Value);
            }

            if (!string.IsNullOrEmpty(searchCondition.TypeCode))
            {
                //var employeeList = Employee.Search(e => e.NameZHCN.Contains(searchCondition.SenderZHCN));
                //var employeeCodeList = employeeList.Select(e => e.Code).ToList();

                predicate = predicate.And(e => e.TypeCode.Contains(searchCondition.TypeCode));
            }

            if (!string.IsNullOrEmpty(searchCondition.ReceiverAccount))
            {
                predicate = predicate.And(e => e.ReceiverAccount == searchCondition.ReceiverAccount);
            }

            //从多少页开始取数据
            var list = Search(predicate, e => e.Num, searchCondition.PageIndex, searchCondition.PageSize,
                out totalSize, true);

            return list;
        }

        public static void Release(params TaskWork[] tasks)
        {
            foreach (var taskWork in tasks)
            {
                taskWork.Status = TaskWorkStatus.UnFinish;
            }

            if (tasks.Any())
            {
                Update(tasks.ToArray());
            }
        }

        public static void Holding(params TaskWork[] tasks)
        {
            foreach (var taskWork in tasks)
            {
                taskWork.Status = TaskWorkStatus.Holding;
            }

            if (tasks.Any())
            {
                Update(tasks.ToArray());
            }

        }
    }

    public enum TaskWorkType
    {
        Unknown = 0,
        Task,
        Approve,
        Return,
        Recall,
        Decline
    }
}
