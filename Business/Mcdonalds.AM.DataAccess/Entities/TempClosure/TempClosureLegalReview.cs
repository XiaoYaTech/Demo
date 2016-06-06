using AutoMapper;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/9/2014 2:41:18 PM
 * FileName     :   TempClosureLegalReview
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Linq;
using System.Data.Entity;
using System.Transactions;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TempClosureLegalReview : BaseWFEntity<TempClosureLegalReview>, IWorkflowEntity
    {
        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project\TempClosureLegalReview"; }
        }

        public override string WorkflowProcessCode
        {
            get { return @"MCD_AM_TempClosure_LR"; }
        }

        public override string TableName
        {
            get { return "TempClosureLegalReview"; }
        }

        public override string WorkflowActOriginator
        {
            get { return "Originator"; }
        }
        public string Comments { get; set; }

        public override string WorkflowCode
        {
            get { return FlowCode.TempClosure_LegalReview; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string[] WorkflowNormalActors
        {
            get { return new string[] { }; }
        }

        public static TempClosureLegalReview Get(string projectId,string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(new Guid(id));
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                return FirstOrDefault(e => e.ProjectId.Equals(projectId) && !e.IsHistory);
            }
            return null;
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

        public static void Create(string projectId)
        {
            TempClosureLegalReview legalReview = new TempClosureLegalReview();
            legalReview.Id = Guid.NewGuid();
            legalReview.ProjectId = projectId;
            legalReview.IsHistory = false;
            legalReview.CreateUserAccount = ClientCookie.UserCode;
            legalReview.CreateTime = DateTime.Now;
            Add(legalReview);
        }

        public void UpdateType(string projectId, Dictionary type)
        {
            var entity = TempClosureInfo.FirstOrDefault(p => p.ProjectId == projectId);
            using (TransactionScope tranScope = new TransactionScope())
            {
                entity.Type = type.Code;
                entity.TypeNameENUS = type.NameENUS;
                entity.TypeNameZHCN = type.NameZHCN;
                TempClosureInfo.Update(entity);
                tranScope.Complete();
            }
        }
        public void Save(string comment)
        {
            var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
            if (SavedComment != null)
            {
                //SavedComment.Status = ProjectCommentStatus.Submit;
                SavedComment.Content = comment;
                SavedComment.CreateTime = DateTime.Now;
                SavedComment.Update();
            }
            else
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Submit,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    null,
                    ProjectCommentStatus.Save
                );
            }
        }

        public void Submit(string comment,Action onExecuting = null)
        {
            
            var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.Legal);
            var task = TaskWork.GetTaskWork(this.ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.TempClosure, this.WorkflowCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            List<ProcessDataField> dataFields = new List<ProcessDataField>
            {
                new ProcessDataField("dest_Creator",this.CreateUserAccount),
                new ProcessDataField("dest_Legal",legal.UserAccount),
                new ProcessDataField("ProcessCode",this.WorkflowProcessCode),
                new ProcessDataField("ProjectTaskInfo",JsonConvert.SerializeObject(task))
            };
            var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
            if (procInstId > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    TaskWork.Update(task);
                    this.ProcInstId = procInstId;
                    this.CreateTime = DateTime.Now;
                    this.CreateUserAccount = ClientCookie.UserCode;
                    this.Update();
                    var project = ProjectInfo.Get(this.ProjectId, this.WorkflowCode);
                    project.CreateUserAccount = ClientCookie.UserCode;
                    project.Update();
                    var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                    if (SavedComment != null)
                    {
                        SavedComment.Status = ProjectCommentStatus.Submit;
                        SavedComment.Content = comment;
                        SavedComment.CreateTime = DateTime.Now;
                        SavedComment.Update();
                    }
                    else
                    {
                        ProjectComment.AddComment(
                            ProjectCommentAction.Submit,
                            comment,
                            this.Id,
                            this.TableName,
                            FlowCode.TempClosure,
                            procInstId,
                            ProjectCommentStatus.Submit
                        );
                    }
                    ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.TempClosure_LegalReview_Input);
                    if (onExecuting != null)
                    {
                        onExecuting();
                    }
                    tranScope.Complete();
                }
            }
        }

        public void Approve(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
            ProjectComment.AddComment(
                ProjectCommentAction.Approve,
                comment,
                this.Id,
                this.TableName,
                FlowCode.TempClosure,
                this.ProcInstId,
                ProjectCommentStatus.Submit
            );
            this.Update();
        }

        public void Return(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Return", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Return,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                this.Update();
                tranScope.Complete();
            }
        }

        public void Reject(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Reject", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reject(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Return,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }

        public void Resubmit(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.TempClosure_LegalReview_Input);
                ProjectComment.AddComment(
                    ProjectCommentAction.ReSubmit,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }

        public override void Recall(string comment)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode, ProjectStatus.Recalled);
                ProjectComment.AddComment(
                    ProjectCommentAction.Recall,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.Legal);
                var oldUnfinishTasks = TaskWork.Search(t => t.RefID == this.ProjectId && t.TypeCode == FlowCode.TempClosure_LegalReview && t.Status == TaskWorkStatus.UnFinish).ToList();
                oldUnfinishTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldUnfinishTasks.ToArray());
                tranScope.Complete();
            }
            string comments = ClientCookie.UserNameZHCN + "进行了流程撤回操作";
            K2FxContext.Current.GoToActivityAndRecord(
                this.ProcInstId.Value,
                this.WorkflowActOriginator,
                ClientCookie.UserCode,
                ProjectAction.Recall,
                comments
            );
            //ProjectInfo.Reset(this.ProjectId, WorkflowCode);
        }


        public override string Edit()
        {
            using (var tranScope = new TransactionScope())
            {
                Mapper.CreateMap<TempClosureLegalReview, TempClosureLegalReview>();
                var legalReview = Mapper.Map<TempClosureLegalReview>(this);
                legalReview.Id = Guid.NewGuid();
                legalReview.IsHistory = false;
                legalReview.CreateUserAccount = ClientCookie.UserCode;
                legalReview.CreateTime = DateTime.Now;
                Add(legalReview);
                IsHistory = true;
                Update(this);
                ProjectInfo.Reset(ProjectId, FlowCode.TempClosure_LegalReview);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                var NewAtts = new List<Attachment>();
                attachments.ForEach(att =>
                {
                    var newAttach = Duplicator.AutoCopy(att);
                    newAttach.RefTableID = legalReview.Id.ToString();
                    newAttach.ID = Guid.NewGuid();
                    NewAtts.Add(newAttach);
                });
                Attachment.Add(NewAtts.ToArray());
                var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                var oldTasks = TaskWork.Search(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish).AsNoTracking().ToList();
                oldTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldTasks.ToArray());
                var latestTask = TaskWork.FirstOrDefault(t => t.RefID == ProjectId && t.TypeCode == "TempClosure_LegalReview");
                string url = "/TempClosure/Main#/LegalReview?projectId=" + ProjectId;
                TaskWork.SendTask(ProjectId, latestTask.Title, latestTask.StoreCode, url, assetActor, FlowCode.TempClosure, FlowCode.TempClosure_LegalReview, "Start");
                tranScope.Complete();
                return url;
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    ProjectInfo.FinishNode(ProjectId, FlowCode.TempClosure_LegalReview, NodeCode.TempClosure_LegalReview_Approve, ProjectStatus.Finished);
                    break;
            }
        }
    }
}
