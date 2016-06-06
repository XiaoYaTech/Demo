using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosureLegalReview : BaseWFEntity<ClosureLegalReview>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_LegalReview; }
        }

        private ObjectCopy objCopy = new ObjectCopy();

        public const string TableName = "ClosureLegalReview";
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
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

        public static ClosureLegalReview Get(string projectId, string id = "")
        {
            ClosureLegalReview entity = null;
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

        public static ClosureLegalReview GetById(Guid id)
        {
            ClosureLegalReview entity = null;

            entity = FirstOrDefault(e => e.Id == id);

            return entity;
        }

        public ClosureLegalReview GetByProcInstID(int ProcInstID)
        {
            ClosureLegalReview entity = null;
            try
            {
                entity = FirstOrDefault(e => e.ProcInstID == ProcInstID && e.IsHistory == false);
            }
            catch (Exception e)
            {

            }
            return entity;
        }


        public int Save()
        {
            var result = 0;

            if (!Any(l => l.Id == this.Id))
            {
                this.IsHistory = false;
                this.CreateUserAccount = ClientCookie.UserCode;
                this.CreateTime = DateTime.Now;
                result = Add(this);
            }
            else
            {
                this.LastUpdateTime = DateTime.Now;
                this.LastUpdateUserAccount = ClientCookie.UserCode;
                this.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
                this.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
                result = Update(this);
            }
            return result;
        }

        public string USCode
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }

        public string Comments
        {
            get;
            set;
        }

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

        public string UserNameZHCN
        {
            get;
            set;
        }

        public string UserNameENUS
        {
            get;
            set;
        }

        public string LegalAccount
        {
            get;
            set;
        }




        public override string Edit()
        {
            if (!PreEdit(this.ProjectId))
                return "";
            var taskWork = TaskWork.Search(e => e.ReceiverAccount == ClientCookie.UserCode
                        && e.SourceCode == FlowCode.Closure
                        && e.TypeCode == FlowCode.Closure_LegalReview && e.RefID == this.ProjectId
    ).FirstOrDefault();

            TaskWork.Cancel(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == this.WorkflowCode);//取消老的流程实例的所有未完成任务
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";

            var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            taskWork.ReceiverAccount = closureEntity.AssetActorAccount;
            taskWork.ReceiverNameENUS = closureEntity.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = closureEntity.AssetActorNameENUS;
            taskWork.Id = Guid.NewGuid();
            taskWork.ProcInstID = null;
            taskWork.CreateTime = DateTime.Now;
            taskWork.Url = "/closure/Main#/LegalReview?projectId=" + this.ProjectId;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            TaskWork.Add(taskWork);


            this.IsHistory = true;
            //TaskWork.SetTaskHistory(this.Id, this.ProcInstID);

            this.Save();
            var _db = GetDb();
            _db.Entry(this).State = EntityState.Modified;

            var objectCopy = new ObjectCopy();
            var newEntity = objectCopy.AutoCopy(this);
            newEntity.Id = Guid.NewGuid();
            newEntity.ProcInstID = 0;
            newEntity.LegalCommers = "";
            newEntity.Save();
            ProjectInfo.Reset(this.ProjectId, FlowCode.Closure_LegalReview);
            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString()
                                   && e.RefTableName == ClosureLegalReview.TableName).AsNoTracking().ToList();

            var newList = new List<Attachment>();
            foreach (var att in attList)
            {
                var newAtt = objCopy.AutoCopy(att);
                newAtt.RefTableID = newEntity.Id.ToString();
                newAtt.ID = Guid.NewGuid();
                newList.Add(newAtt);
            }
            Attachment.AddList(newList);

            _db.SaveChanges();
            return taskWork.Url;
        }
        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    ProjectInfo.FinishNode(this.ProjectId, FlowCode.Closure_LegalReview, NodeCode.Closure_LegalReview_LegalConfirm, ProjectStatus.Finished);
                    //ProjectInfo.UpdateProjectStatus(this.ProjectId, FlowCode.Closure_LegalReview, ProjectStatus.Finished);
                    if (ProjectInfo.Any(e => e.ProjectId == ProjectId
                                               && e.Status == ProjectStatus.Finished
                                               && e.FlowCode == FlowCode.Closure_ExecutiveSummary))
                    {
                        var package = new ClosurePackage();
                        package.GeneratePackageTask(ProjectId);
                        package.GeneratePackage(ProjectId);
                    }

                    break;
            }
        }
        /// <summary>
        /// Edit准备
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public bool PreEdit(string projectId)
        {
            //如果流程已经到了ClosurePackage审批环节就不能Edit
            if (ProjectInfo.IsFlowStarted(projectId, FlowCode.Closure_ClosurePackage))
            {
                return false;
            }
            else
            {
                #region Package撤回
                var needWidthDraw = TaskWork.Count(i => i.TypeCode == FlowCode.Closure_ClosurePackage && i.RefID == projectId && i.Status == TaskWorkStatus.UnFinish) > 0;
                if (needWidthDraw)
                {
                    //任务取消
                    var taskList = TaskWork.Search(i => i.TypeCode == FlowCode.Closure_ClosurePackage && i.RefID == projectId && i.Status != TaskWorkStatus.Cancel).ToArray();
                    foreach (var taskItem in taskList)
                    {
                        taskItem.Status = TaskWorkStatus.Cancel;
                    }
                    if (taskList.Length > 0)
                        TaskWork.Update(taskList);

                    //Package数据isHistory置成true
                    var package = ClosurePackage.Search(i => i.ProjectId == projectId && i.IsHistory == false).ToArray();
                    foreach (var pacItem in package)
                    {
                        pacItem.IsHistory = true;
                        pacItem.LastUpdateTime = DateTime.Now;
                        pacItem.LastUpdateUserAccount = ClientCookie.UserCode;
                    }
                    if (package.Length > 0)
                        ClosurePackage.Update(package);
                }
                #endregion

                return true;
            }
        }

        public string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Closure_LegalReview_Input;
                    break;
                case ProjectAction.ReSubmit:
                    nodeCode = NodeCode.Closure_LegalReview_UploadAgreement;
                    break;
                default:
                    nodeCode = NodeCode.Closure_LegalReview_LegalConfirm;
                    break;
            }
            return nodeCode;
        }
    }

}
