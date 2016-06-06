using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using NTTMNC.BPM.Fx.Core.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosureExecutiveSummary : BaseWFEntity<ClosureExecutiveSummary>
    {
        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Closure_ES"; }
        }

        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project\ClosureExecutiveSummary"; }
        }

        public string USCode { get; set; }
        public string Action { get; set; }
        public string Comments { get; set; }
        public new const string TableName = "ClosureExecutiveSummary";
        public string SN
        {
            get;
            set;
        }

        public string UserAccount
        {
            get;
            set;
        }

        public string UserNameENUS
        {
            get;
            set;
        }

        public string UserNameZHCN
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_ExecutiveSummary; }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            ClosureExecutiveSummary entity = null;
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
        public override string Edit()
        {
            var taskWork = TaskWork.FirstOrDefault(e => e.ReceiverAccount == ClientCookie.UserCode && e.SourceCode == FlowCode.Closure && e.TypeCode == FlowCode.Closure_ExecutiveSummary && e.RefID == this.ProjectId);


            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";

            var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            taskWork.ReceiverAccount = closureEntity.AssetActorAccount;
            taskWork.ReceiverNameENUS = closureEntity.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = closureEntity.AssetActorNameZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            taskWork.Url = TaskWork.BuildUrl(FlowCode.Closure_ExecutiveSummary, this.ProjectId, "");
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ProcInstID = null;
            taskWork.FinishTime = null;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            TaskWork.Add(taskWork);

            this.IsHistory = true;

            this.Save();

            var objectCopy = new ObjectCopy();
            var newEntity = objectCopy.AutoCopy(this);
            newEntity.Id = Guid.NewGuid();
            newEntity.ProcInstID = 0;
            newEntity.IsHistory = false;
            newEntity.Add();

            var projectEntity = ProjectInfo.Get(this.ProjectId, FlowCode.Closure_ExecutiveSummary);
            projectEntity.Status = ProjectStatus.UnFinish;

            ProjectInfo.Update(projectEntity);
            ProjectInfo.Reset(ProjectId, FlowCode.Closure_ExecutiveSummary);
            ProjectInfo.Reset(ProjectId, FlowCode.Closure);
            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString() && e.RefTableName == ClosureExecutiveSummary.TableName).AsNoTracking().ToList();

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

        private int Save()
        {
            var result = 0;

            if (!Any(p => p.Id == this.Id))
            {
                this.IsHistory = false;
                this.CreateTime = DateTime.Now;
                result = Add(this);
            }
            else
            {
                this.LastUpdateTime = DateTime.Now;
                this.LastUpdateUserAccount = ClientCookie.UserCode;
                result = Update(this);
            }
            return result;
        }

        public static ClosureExecutiveSummary GetByProcInstID(int ProcInstID)
        {
            return FirstOrDefault(e => e.ProcInstID == ProcInstID);
        }



        public void GenerateExecutiveSummaryTask(string projectId)
        {
            var taskWork = new TaskWork();
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            taskWork.CreateUserAccount = ClientCookie.UserCode;

            var closureInfo = ClosureInfo.GetByProjectId(projectId);
            taskWork.StoreCode = closureInfo.USCode;
            taskWork.SourceCode = FlowCode.Closure;
            taskWork.SourceNameZHCN = "关店流程";
            taskWork.SourceNameENUS = taskWork.SourceCode;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = projectId;
            taskWork.TypeCode = FlowCode.Closure_ExecutiveSummary;
            taskWork.TypeNameENUS = "ExecutiveSummary";
            taskWork.TypeNameZHCN = "ExecutiveSummary";
            taskWork.ReceiverAccount = closureInfo.AssetActorAccount;
            taskWork.ReceiverNameENUS = closureInfo.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = closureInfo.AssetActorNameZHCN;
            taskWork.Title = TaskWork.BuildTitle(projectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
            taskWork.Url = string.Format(@"/Closure/Main#/ExecutiveSummary?projectId={0}", projectId);
            taskWork.ActivityName = "Start";
            TaskWork.Add(taskWork);
        }

        public int StartProcess(TaskWork task)
        {
            var closureInfo = ClosureInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (closureInfo == null)
            {
                throw new Exception("Could not find the Closure Info, please check it!");
            }

            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode,
                processDataFields);

        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Creator", ClientCookie.UserCode),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(this.ProjectId, FlowCode.Closure_ExecutiveSummary, NodeCode.Closure_ExecutiveSummary_Generate, ProjectStatus.Finished);
                        if (ProjectInfo.Any(e => e.ProjectId == ProjectId
                                               && e.Status == ProjectStatus.Finished
                                               && e.FlowCode == FlowCode.Closure_LegalReview))
                        {
                            var package = new ClosurePackage();
                            package.GeneratePackageTask(ProjectId);
                            package.GeneratePackage(ProjectId);
                        }
                        break;
                }

                scope.Complete();
            }

        }
    }
}
