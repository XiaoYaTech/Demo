using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosureConsInvtChecking : BaseWFEntity<ClosureConsInvtChecking>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_ConsInvtChecking; }
        }

        public static ClosureConsInvtChecking Get(string projectId, string id = "")
        {
            ClosureConsInvtChecking entity = null;
            if (!string.IsNullOrEmpty(id))
            {
                entity = FirstOrDefault(e => e.Id == new Guid(id));
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                entity = FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            }
            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            }
            return entity;
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = Get(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public int Save()
        {
            var result = 0;
            var _db = GetDb();

            if (!Any(c => c.Id == this.Id))
            {
                this.IsHistory = false;
                this.CreateUserAccount = ClientCookie.UserCode;
                this.CreateTime = DateTime.Now;
                result = Add(this);
            }
            else
            {
                this.LastUpdateTime = DateTime.Now;
                result = Update(this);
            }
            return result;
        }

        public override string Edit()
        {
            var taskWork = TaskWork.Search(e => e.ReceiverAccount == ClientCookie.UserCode
                                    && e.SourceCode == FlowCode.Closure
                                    && e.TypeCode == FlowCode.Closure_ConsInvtChecking && e.RefID == this.ProjectId
                ).AsNoTracking().FirstOrDefault();


            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";

            var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            taskWork.ReceiverAccount = closureEntity.PMAccount;
            taskWork.ReceiverNameENUS = closureEntity.PMNameENUS;
            taskWork.ReceiverNameZHCN = closureEntity.PMNameZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.ProcInstID = null;
            taskWork.CreateTime = DateTime.Now;
            taskWork.Url = "/Closure/Main#/ConsInvtChecking?projectId=" + this.ProjectId;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            taskWork.ProcInstID = null;
            taskWork.FinishTime = null;
            TaskWork.Add(taskWork);


            this.IsHistory = true;
            this.Save();
            var objectCopy = new ObjectCopy();
            var newEntity = objectCopy.AutoCopy(this);
            newEntity.Id = Guid.NewGuid();
            newEntity.ProcInstID = 0;
            newEntity.Save();

            //var projectEntity = ProjectInfo.Get(this.ProjectId, FlowCode.Closure_ConsInvtChecking);
            //projectEntity.Status = ProjectStatus.UnFinish;

            //ProjectInfo.Update(projectEntity);
            //ProjectInfo.UpdateProjectNode(this.ProjectId, FlowCode.Closure_ConsInvtChecking,
            //    NodeCode.Closure_ConsInvtChecking_Approve);

            //ProjectInfo.Reset(ProjectId, FlowCode.Closure);
            ProjectInfo.Reset(ProjectId, FlowCode.Closure_ConsInvtChecking);
            ProjectInfo.UnFinishNode(this.ProjectId, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_Approve, ProjectStatus.UnFinish);
            
            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString()
                                   && e.RefTableName == ClosureConsInvtChecking.TableName).AsNoTracking();

            var newList = new List<Attachment>();
            foreach (var att in attList)
            {
                var newAtt = objectCopy.AutoCopy(att);
                newAtt.RefTableID = newEntity.Id.ToString();
                newAtt.ID = Guid.NewGuid();
                newList.Add(newAtt);
            }
            Attachment.Add(newList.ToArray());
            return taskWork.Url;
        }

        public string UserAccount
        {
            get;
            set;
        }

        public const string TableName = "ClosureConsInvtChecking";

        public string UserNameENUS { get; set; }

        public string UserNameZHCN { get; set; }

        public string USCode { get; set; }
        public string SN { get; set; }

        public string Action { get; set; }

        public string Comments { get; set; }

        public string PMSupervisor
        {
            get;
            set;
        }
        public string FinanceAccount
        {
            get;
            set;
        }
        public string FinControllerAccount
        {
            get;
            set;
        }
        public string VPGMAccount
        {
            get;
            set;
        }

        public void GenerateConsInvtCheckingTask(string projectId)
        {
            //if (!TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Closure_ConsInvtChecking && t.Status == TaskWorkStatus.UnFinish))
            //{
            //    var taskWork = new TaskWork();
            //    taskWork.SourceCode = FlowCode.Closure;
            //    taskWork.SourceNameENUS = taskWork.SourceCode;
            //    taskWork.SourceNameZHCN = "对帐流程";
            //    taskWork.Status = TaskWorkStatus.UnFinish;
            //    taskWork.StatusNameZHCN = "任务";
            //    taskWork.StatusNameENUS = "任务";
            //    taskWork.RefID = projectId;
            //    taskWork.Id = Guid.NewGuid();
            //    taskWork.CreateTime = DateTime.Now;

            //    var closureInfo = ClosureInfo.GetByProjectId(projectId);

            //    taskWork.Title = TaskWork.BuildTitle(projectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
            //    taskWork.TypeCode = WorkflowCode;
            //    taskWork.TypeNameENUS = WorkflowCode;
            //    taskWork.TypeNameZHCN = WorkflowCode;
            //    taskWork.ReceiverAccount = closureInfo.PMAccount;
            //    taskWork.ReceiverNameENUS = closureInfo.PMNameENUS;
            //    taskWork.ReceiverNameZHCN = closureInfo.PMNameZHCN;
            //    taskWork.Url = string.Format(@"/Closure/Main#/ConsInvtChecking?projectId={0}", projectId);
            //    taskWork.StoreCode = closureInfo.USCode;
            //    taskWork.ActivityName = "Start";

            //    TaskWork.Add(taskWork);
            //}

            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Closure;
            taskWork.SourceNameENUS = taskWork.SourceCode;
            taskWork.SourceNameZHCN = "对帐流程";
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = projectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;

            var closureInfo = ClosureInfo.GetByProjectId(projectId);

            taskWork.Title = TaskWork.BuildTitle(projectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
            taskWork.TypeCode = WorkflowCode;
            taskWork.TypeNameENUS = "ConsInvtChecking";
            taskWork.TypeNameZHCN = "ConsInvtChecking";
            taskWork.ReceiverAccount = closureInfo.PMAccount;
            taskWork.ReceiverNameENUS = closureInfo.PMNameENUS;
            taskWork.ReceiverNameZHCN = closureInfo.PMNameZHCN;
            taskWork.Url = string.Format(@"/Closure/Main#/ConsInvtChecking?projectId={0}", projectId);
            taskWork.StoreCode = closureInfo.USCode;
            taskWork.ActivityName = "Start";

            //关店后60天发起对账流程
            if (closureInfo.ActualCloseDate.HasValue)
                ScheduleLog.GenerateTaskSchedule(closureInfo.ActualCloseDate.Value.AddDays(60), taskWork, ClientCookie.UserCode, projectId, WorkflowCode, closureInfo.USCode);
        }

        public string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Closure_ConsInvtChecking_ResultUpload;
                    break;
                case ProjectAction.ReSubmit:
                    nodeCode = NodeCode.Closure_ConsInvtChecking_Approve;
                    break;
                default:
                    nodeCode = NodeCode.Closure_ConsInvtChecking_Approve;
                    break;
            }
            return nodeCode;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Closure_ConsInvtChecking_Approve, ProjectStatus.Finished);
                        ProjectInfo.CompleteMainIfEnable(ProjectId);
                        break;
                }

                scope.Complete();
            }
        }
    }

}


