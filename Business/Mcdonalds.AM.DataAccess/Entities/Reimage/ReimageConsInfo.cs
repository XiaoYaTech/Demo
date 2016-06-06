using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using Mcdonalds.AM.DataAccess.Constants;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using AutoMapper;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimageConsInfo : BaseWFEntity<ReimageConsInfo>
    {
        public override string WorkflowProcessCode
        {
            get
            {
                return @"MCD_AM_Reimage_CI";
            }
        }
        public override string WorkflowCode
        {
            get { return FlowCode.Reimage_ConsInfo; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == _refTableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string SerialNumber { get; set; }

        private string _refTableName = "ReimageConsInfo";

        private Employee _createUser;
        public string Comments { get; set; }
        public Nullable<int> ReinvenstmentType { get; set; }
        public List<ProjectComment> ProjectComments { get; set; }

        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public int Year { get; set; }

        public bool IsShowSave { get; set; }

        public Employee CreateUser
        {
            get { return _createUser ?? (_createUser = Employee.GetEmployeeByCode(CreateUserAccount)); }
        }

        public ReinvestmentBasicInfo ReinBasicInfo { get; set; }
        public ReinvestmentCost ReinCost { get; set; }
        public WriteOffAmount WriteOff { get; set; }

        public ApproveUsers AppUsers { get; set; }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetConsInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public ReinvestmentBasicInfo ReinvestmentBasicInfo { get; set; }

        public string Comment { get; set; }
        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new ReimageConsInfo();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.IsHistory = false;

            var rbdInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId);
            if (rbdInfo != null)
            {
                entity.ReinBasicInfo = new ReinvestmentBasicInfo
                {
                    GBDate = rbdInfo.GBDate,
                    ReopenDate = rbdInfo.ReopenDate,
                    // ConsCompletionDate = rbdInfo.ConstCompletionDate
                };
            }
            Add(entity);
        }

        public static bool CheckIfConsInfoFinished(string projectId)
        {
            return ProjectInfo.Any(e => e.ProjectId == projectId
                                                              && e.FlowCode == FlowCode.Reimage_ConsInfo
                                                              && e.Status == ProjectStatus.Finished);
        }

        public override string Edit()
        {
            var taskUrl = string.Format("/Reimage/Main#/ConsInfo?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (reimageInfo == null)
                {
                    throw new Exception("Could not find the Reimage Info, please check it!");
                }
                var task = reimageInfo.GenerateTaskWork(FlowCode.Reimage_ConsInfo,
                     "Reimage_ConsInfo",
                     "Reimage_ConsInfo",
                     taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);

                var package = ReimagePackage.GetReimagePackageInfo(ProjectId);
                if (package != null)
                {
                    package.CompleteActorPackageTask(reimageInfo.AssetActorAccount);
                }


                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == WFReimageConsInfo.TableName).AsNoTracking().ToList();



                ProjectInfo.Reset(ProjectId, FlowCode.Reimage_ConsInfo);
                var wfEntity = GetWorkflowEntity(ProjectId, FlowCode.Reimage_Package);
                if (wfEntity != null)
                {
                    wfEntity.ChangePackageHoldingStatus(HoldingStatus.No);
                }



                var form = Duplicator.AutoCopy(this);
                form.Id = Guid.Empty;
                form.ProcInstId = null;
                form.IsHistory = false;
                form.Comments = null;
                form.CreateTime = DateTime.Now;
                form.SaveEdit("edit");

                List<Attachment> listAttachment = new List<Attachment>();
                Mapper.CreateMap<Attachment, Attachment>();
                foreach (var attachment in attachments)
                {
                    var newAttachment = Duplicator.AutoCopy(attachment);
                    newAttachment.RefTableID = form.Id.ToString();
                    newAttachment.ID = Guid.NewGuid();
                    listAttachment.Add(newAttachment);
                }
                Attachment.Add(listAttachment.ToArray());
                IsHistory = true;
                Update(this);
                scope.Complete();
            }

            return taskUrl;
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Reimage, FlowCode.Reimage_ConsInfo);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Reimage/Main#/ConsInfo/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            task.CreateUserAccount = ClientCookie.UserCode;
            ProcInstId = StartProcess(task);
            task.ProcInstID = ProcInstId;
            using (TransactionScope scope = new TransactionScope())
            {

                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInfo, NodeCode.Reimage_ConsInfo_Input);

                //var majorLeasePackage = new MajorLeaseChangePackage();
                //majorLeasePackage.GeneratePackageTask(ProjectId);

                //var majorLeaseConsInvtChecking = new MajorLeaseConsInvtChecking();
                //majorLeaseConsInvtChecking.GenerateConsInvtCheckingTask(ProjectId);

                scope.Complete();
            }

            //var currTask = TaskWork.FirstOrDefault(e => e.ReceiverAccount == CreateUserAccount
            //                                            && e.Status == 0
            //                                            && e.SourceCode == FlowCode.Reimage
            //                                            && e.TypeCode == FlowCode.Reimage_ConsInfo
            //                                            && e.RefID == ProjectId);

            //if (currTask != null)
            //{
            //    using (var scope = new TransactionScope())
            //    {
            //        currTask.Url = string.Format("/Reimage/Main#/Reimage/ConsInfo/View/{0}", ProjectId);
            //        currTask.Finish();

            //        if (ProcInstId > 0)
            //        {
            //            SaveComment();
            //            Add();
            //        }
            //        else
            //        {
            //            throw new Exception("Could not get process instance id, please check it and try it again!");
            //        }

            //        scope.Complete();
            //    }

            //    ProcInstId = StartProcess(currTask);
            //    Update(this);
            //}
            //else
            //{
            //    throw new Exception("Can not find the task, please check it!");
            //}

        }



        private int StartProcess(TaskWork task)
        {
            CreateUserAccount = ClientCookie.UserCode;
            var reimageConsInfo = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (reimageConsInfo == null)
            {
                throw new Exception("Could not find the Major Lease Info, please check it!");
            }


            var processDataFields = SetWorkflowDataFields(task);
            //return K2FxContext.Current.StartProcess(WFMajorLeaseConsInfo.ProcessCode, CreateUserAccount,
            //    processDataFields);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode,
                processDataFields);

        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Creator", ClientCookie.UserCode),
                new ProcessDataField("dest_ConsMgr", AppUsers.ConstructionManager.Code),
                new ProcessDataField("dest_MCCLConsMgr", AppUsers.MCCLConsManager.Code),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
                
            };

            //return K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode,
            //    processDataFields);

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }


        private string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                default:
                    nodeCode = NodeCode.Reimage_ConsInfo_Confirm;
                    break;
            }
            return nodeCode;
        }

        public void ApproveConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }

        public void RejectConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Decline", Comments);
        }

        public void ReturnConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }

        private void SaveComment()
        {
            var projectComment = new ProjectComment()
            {
                RefTableId = Id,
                RefTableName = _refTableName,
                CreateTime = CreateTime,
                CreateUserAccount = CreateUserAccount,
                CreateUserNameZHCN = CreateUser.NameZHCN,
                UserAccount = CreateUserAccount,
                UserNameENUS = CreateUser.NameENUS,
                Id = Guid.NewGuid(),
                Action = ProjectCommentAction.Submit,
                Status = ProjectCommentStatus.Submit,
                ProcInstID = ProcInstId,
                SourceCode = FlowCode.Reimage,
                SourceNameENUS = FlowCode.Reimage,
                SourceNameZHCN = ""
            };
            if (!string.IsNullOrEmpty(Comment))
            {
                projectComment.Content = Comment.Trim();
            }
            var userInfo = ProjectUsers.Get(CreateUserAccount, ProjectId);
            if (userInfo != null)
            {
                projectComment.TitleCode = userInfo.RoleCode;
                projectComment.TitleNameENUS = userInfo.RoleNameENUS;
                projectComment.TitleNameZHCN = userInfo.RoleNameZHCN;
            }
            projectComment.Add();
        }

        public void ResubmitConsInfo(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataFields = null)
        {
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataFields);

            using (var scope = new TransactionScope())
            {

                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInfo_Input);
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInfo_Upload);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInfo_Upload, GetProjectStatus(actionName));
                        break;
                }

                scope.Complete();
            }

        }
        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    CreateTime = DateTime.Now;
                    LastUpdateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;

                    IsHistory = false;

                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                if (ReinBasicInfo != null)
                {

                    ReinBasicInfo.ConsInfoID = Id;
                    ReinBasicInfo.Save();

                    //同步更新GBDate与ReopenDate
                    var reimageInfo = ReimageInfo.GetReimageInfo(ProjectId);
                    reimageInfo.ReopenDate = ReinBasicInfo.ReopenDate;
                    reimageInfo.GBDate = ReinBasicInfo.GBDate;
                    reimageInfo.Update();
                }

                if (ReinCost != null)
                {
                    ReinCost.ConsInfoID = Id;
                    ReinCost.Save();
                }

                if (WriteOff != null)
                {
                    WriteOff.ConsInfoID = Id;
                    WriteOff.Save();
                }
                SaveApproveUsers(action);
                if (string.Compare(action, "edit", true) != 0)
                {
                    SaveComments(action);
                }
                scope.Complete();
            }
        }

        public void SaveEdit(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    CreateTime = DateTime.Now;
                    LastUpdateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;

                    IsHistory = false;

                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                if (ReinBasicInfo != null)
                {
                    var ReinBasicInfo_New = Duplicator.AutoCopy(ReinBasicInfo);

                    ReinBasicInfo_New.ConsInfoID = Id;
                    ReinBasicInfo_New.Add();
                }

                if (ReinCost != null)
                {
                    var ReinCost_New = Duplicator.AutoCopy(ReinCost);
                    ReinCost_New.Id = Guid.NewGuid();
                    ReinCost_New.ConsInfoID = Id;
                    ReinCost_New.Add();
                }

                if (WriteOff != null)
                {
                    var WriteOff_New = Duplicator.AutoCopy(WriteOff);
                    WriteOff_New.Id = Guid.NewGuid();
                    WriteOff_New.ConsInfoID = Id;
                    WriteOff_New.Add();
                }
                SaveApproveUsers(action);
                if (string.Compare(action, "edit", true) != 0)
                {
                    SaveComments(action);
                }
                scope.Complete();
            }
        }

        private void SaveApproveUsers(string actionName)
        {
            switch (actionName)
            {
                case ProjectAction.Submit:
                case ProjectAction.ReSubmit:
                    var approveUser = ApproveDialogUser.GetApproveDialogUser(Id.ToString());
                    if (approveUser == null)
                    {
                        approveUser = new ApproveDialogUser();
                        approveUser.Id = Guid.Empty;
                        approveUser.RefTableID = Id.ToString();
                        approveUser.ProjectId = ProjectId;
                        approveUser.FlowCode = FlowCode.Reimage_ConsInfo;
                    }
                    if (!CheckIfHasNoReinvestment())
                    {
                        approveUser.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
                        approveUser.MCCLConsManagerCode = AppUsers.MCCLConsManager.Code;
                    }
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }

        private bool CheckIfHasNoReinvestment()
        {
            var hasNoReinvestment = false;
            if (ReinvenstmentType.HasValue)
            {
                if (ReinvenstmentType.Value.Equals(1))
                {
                    hasNoReinvestment = true;
                }
            }

            return hasNoReinvestment;
        }

        public override void Recall(string comment)
        {

            if (!ProcInstId.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstId.Value,
                WFReimageConsInfo.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }


        public ReinvestmentBasicInfo GetReinvestmentBasicInfo(string projectId)
        {
            ReinvestmentBasicInfo reinvestmentBasicInfo;
            using (var context = new McdAMEntities())
            {
                var result = (from mlc in context.ReimageConsInfo
                              join rbi in context.ReinvestmentBasicInfo on mlc.Id equals rbi.ConsInfoID
                              where mlc.ProjectId == projectId && !mlc.IsHistory
                              select rbi).Distinct().AsEnumerable();
                reinvestmentBasicInfo = result.FirstOrDefault();
            }
            return reinvestmentBasicInfo;
        }
        public static ReimageConsInfo GetConsInfo(string strProjectId, string entityId = "")
        {
            ReimageConsInfo entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();
            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == strProjectId);

            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
                entity.ReinCost = ReinvestmentCost.GetByConsInfoId(entity.Id);
                entity.ReinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(entity.Id);
                if (entity.ReinBasicInfo != null)
                {
                    entity.ReinBasicInfo.GBDate = reimageInfo.GBDate;
                    entity.ReinBasicInfo.ReopenDate = reimageInfo.ReopenDate;
                }
                else
                {
                    entity.ReinBasicInfo =
                            new ReinvestmentBasicInfo
                            {
                                GBDate = reimageInfo.GBDate,
                                ReopenDate = reimageInfo.ReopenDate
                            };
                }
                entity.WriteOff = WriteOffAmount.GetByConsInfoId(entity.Id);
                var attachmentReinCost = Attachment.FirstOrDefault(e => e.RefTableID == entity.Id.ToString() && e.TypeCode == "ReinCost");
                if (attachmentReinCost != null && entity.ReinCost != null)
                {
                    entity.ReinCost.ReinCostUser = attachmentReinCost.CreatorNameENUS;
                    entity.ReinCost.ReinCostTime = attachmentReinCost.CreateTime;
                }
                var attachmentWriteOff = Attachment.FirstOrDefault(e => e.RefTableID == entity.Id.ToString() && e.TypeCode == "WriteOff");
                if (attachmentWriteOff != null)
                {
                    entity.WriteOff.WriteOffUser = attachmentWriteOff.CreatorNameENUS;
                    entity.WriteOff.WriteOffTime = attachmentWriteOff.CreateTime;
                }
                //entity.ProcInstID = entity.ProcInstId;
                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.Reimage_ConsInfo);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.Reimage_ConsInfo);
            }
            else
            {
                //var reimage = ReimageInfo.FirstOrDefault(e => e.ProjectId == strProjectId);
                //if (reimage != null)
                //{
                //    entity = new ReimageConsInfo
                //    {
                //        ReinBasicInfo =
                //            new ReinvestmentBasicInfo
                //            {
                //                GBDate = reimage.GBDate,
                //                ReopenDate = reimage.ReopenDate
                //            },
                //        IsProjectFreezed =entity.CheckIfFreezeProject(strProjectId),
                //        ReinvenstmentType = 1
                //    };
                //}
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
                entity.ReinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(entity.Id);
                if (entity.ReinBasicInfo != null)
                {
                    entity.ReinBasicInfo.GBDate = reimageInfo.GBDate;
                    entity.ReinBasicInfo.ReopenDate = reimageInfo.ReopenDate;
                }
                else
                {
                    entity.ReinBasicInfo =
                            new ReinvestmentBasicInfo
                            {
                                GBDate = reimageInfo.GBDate,
                                ReopenDate = reimageInfo.ReopenDate
                            };
                }
            }
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Reimage_ConsInfo);
            entity.PopulateAppUsers();
            return entity;
        }

        private void PopulateAppUsers()
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());

            this.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.ConstructionManagerCode
                };
                this.AppUsers.ConstructionManager = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLConsManagerCode
                };
                this.AppUsers.MCCLConsManager = simp;

                //simp = new SimpleEmployee
                //{
                //    Code = approvedUsers.MCCLAssetMgrCode
                //};
                //entity.AppUsers.MCCLAssetMgr = simp;

                //simp = new SimpleEmployee
                //{
                //    Code = approvedUsers.MCCLAssetDtrCode
                //};
                //entity.AppUsers.MCCLAssetDtr = simp;
            }
            else
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                if (reimageInfo != null)
                {
                    this.AppUsers.ConstructionManager = new SimpleEmployee
                    {
                        Code = reimageInfo.CMAccount
                    };
                }
            }
        }

        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "ReimageConsInfo", ClientCookie.UserCode);
            if (comment != null)
            {
                comment.Content = Comments;
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.Update();
            }
            else
            {
                comment = new ProjectComment();
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.ActivityName = "";
                comment.Content = Comments;
                comment.CreateTime = DateTime.Now;
                comment.CreateUserAccount = ClientCookie.UserCode;
                comment.CreateUserNameENUS = ClientCookie.UserNameENUS;
                comment.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                comment.UserAccount = ClientCookie.UserCode;
                comment.UserNameENUS = ClientCookie.UserNameENUS;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "ReimageConsInfo";
                comment.SourceCode = FlowCode.Reimage;
                comment.SourceNameZHCN = FlowCode.Reimage;
                comment.SourceNameENUS = FlowCode.Reimage;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstId;
                comment.Add();
            }
        }


        public class WFReimageConsInfo
        {
            #region ---- [ Constant Strings ] ----

            public const string ProcessName = @"MCDAMK2Project.Reimage\ConsInfo";
            public const string ProcessCode = @"MCD_AM_Reimage_CI";
            public const string TableName = "ReimageConsInfo";


            public const string Act_Originator = "Originator"; // 退回至发起人节点名

            #endregion
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInfo, NodeCode.Reimage_ConsInfo_Confirm, ProjectStatus.Finished);

                        var reimagePackage = ReimagePackage.Get(ProjectId);
                        reimagePackage.GeneratePackageTask(ProjectId);

                        var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);

                        if (reimageInfo != null)
                        {
                            if (!TaskWork.Any(e => e.RefID == ProjectId
                                                   && e.TypeCode == FlowCode.Reimage_Summary
                                                   && e.Status == TaskWorkStatus.UnFinish
                                                   && e.ActivityName == NodeCode.Start))
                            {
                                var reimageSummary = ReimageSummary.GetReimageSummaryInfo(ProjectId);
                                string filePath;
                                reimageSummary.GenerateExcel(out filePath);

                                if (!ProjectInfo.Any(e => e.ProjectId == ProjectId
                                                          && e.FlowCode == FlowCode.Reimage_Summary
                                                          && e.Status == ProjectStatus.Finished))
                                {
                                    var summaryTask = reimageInfo.GenerateTaskWork(
                                        FlowCode.Reimage_Summary,
                                        FlowCode.Reimage_Summary,
                                        FlowCode.Reimage_Summary,
                                        string.Format(@"/Reimage/Main#/Summary?projectId={0}", ProjectId));

                                    summaryTask.Add();
                                }
                            }

                        }
                        break;
                }

                scope.Complete();
            }

        }

    }
}
