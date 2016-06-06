using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Entities;
using System;
using System.Linq;
using System.Transactions;
using System.Data.Entity;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Workflow;
using AutoMapper;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Collections.Generic;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TempClosurePackage : BaseWFEntity<TempClosurePackage>, IWorkflowEntity
    {
        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project\TempClosurePackage"; }
        }

        public override string WorkflowProcessCode
        {
            get { return @"MCD_AM_TempClosure_Package"; }
        }

        public override string TableName
        {
            get { return "TempClosurePackage"; }
        }

        public override string WorkflowActOriginator
        {
            get { return "Originator"; }
        }
        public string Comments { get; set; }
        public override string WorkflowCode
        {
            get { return FlowCode.TempClosure_ClosurePackage; }
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
            get
            {
                return new string[]{
                    "M&R Manager",
                    "DD_GM_FC_RDD", //要支持在途老流程
                    "DD_GM_FC"
                };
            }
        }
        public ApproveUsers AppUsers { get; set; }
        public static TempClosurePackage Create(string projectId)
        {
            var package = new TempClosurePackage();
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (!Any(p => p.IsHistory == false && p.ProjectId == projectId))
                {
                    package.Id = Guid.NewGuid();
                    package.ProjectId = projectId;
                    package.IsHistory = false;
                    package.CreateTime = DateTime.Now;
                    Add(package);
                    tranScope.Complete();
                }
            }
            return package;
        }

        public static TempClosurePackage Get(string projectId, string id = "")
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

        public static ProjectDto GetApproverAndNoticeUsers(string projectId)
        {
            var approver = ApproveDialogUser.GetApproveDialogUser(projectId, FlowCode.TempClosure_ClosurePackage);
            if (approver != null)
            {
                return new ProjectDto
                {
                    ApproveUsers = new ApproveUsers
                    {
                        MarketMgr = Employee.GetSimpleEmployeeByCode(approver.MarketMgrCode),
                        RegionalMgr = Employee.GetSimpleEmployeeByCode(approver.RegionalMgrCode),
                        MDD = Employee.GetSimpleEmployeeByCode(approver.MDDCode),
                        GM = Employee.GetSimpleEmployeeByCode(approver.GMCode),
                        FC = Employee.GetSimpleEmployeeByCode(approver.FCCode),
                        VPGM = Employee.GetSimpleEmployeeByCode(approver.VPGMCode),
                        MCCLAssetMgr = Employee.GetSimpleEmployeeByCode(approver.MCCLAssetMgrCode),
                        MCCLAssetDtr = Employee.GetSimpleEmployeeByCode(approver.MCCLAssetDtrCode),
                        NoticeUsers = string.IsNullOrEmpty(approver.NoticeUsers.TrimEnd(';')) ? new List<SimpleEmployee>() : Employee.GetSimpleEmployeeByCodes(approver.NoticeUsers.TrimEnd(';').Split(';')),

                        NecessaryNoticeUsers = string.IsNullOrEmpty(approver.NecessaryNoticeUsers.TrimEnd(';')) ? new List<SimpleEmployee>() : Employee.GetSimpleEmployeeByCodes(approver.NecessaryNoticeUsers.TrimEnd(';').Split(';'))
                    },
                    FlowCode = FlowCode.TempClosure_ClosurePackage,
                    ProjectId = projectId
                };
            }
            else
            {
                return new ProjectDto
                {
                    ApproveUsers = new ApproveUsers
                    {
                        MarketMgr = null,
                        RegionalMgr = null,
                        MDD = null,
                        GM = null,
                        FC = null,
                        VPGM = null,
                        MCCLAssetMgr = null,
                        MCCLAssetDtr = null,
                        NoticeUsers = new List<SimpleEmployee>(),
                        NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.TempClosure_ClosurePackage && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray())
                    },
                    FlowCode = FlowCode.TempClosure_ClosurePackage,
                    ProjectId = projectId
                };
            }
        }

        public void Save(string comment)
        {
            if (Any(p => p.Id == this.Id))
                Update(this);
            else
                Add(this);
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

        public void Submit(string comment, ApproveUsers approvers, Action onExecuting = null)
        {
            var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.Legal);
            var task = TaskWork.GetTaskWork(this.ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.TempClosure, this.WorkflowCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string destMRMgrs = approvers.MarketMgr.Code;
            if (approvers.RegionalMgr != null)
            {
                destMRMgrs += ";" + approvers.RegionalMgr.Code;
            }
            List<ProcessDataField> dataFields = new List<ProcessDataField>
            {
                new ProcessDataField("dest_Creator",this.CreateUserAccount),
                new ProcessDataField("dest_MRMgrs",destMRMgrs),
                new ProcessDataField("dest_GMApprovers",string.Concat(approvers.MDD.Code, ";", approvers.GM.Code, ";", approvers.FC.Code)),
                new ProcessDataField("dest_VPGM",approvers.VPGM.Code),
                new ProcessDataField("ProcessCode",this.WorkflowProcessCode),
                new ProcessDataField("ProjectTaskInfo",JsonConvert.SerializeObject(task))
            };
            var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
            if (procInstId > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    TaskWork.Update(task);
                    var project = ProjectInfo.Get(this.ProjectId, FlowCode.TempClosure_ClosurePackage);
                    project.CreateUserAccount = ClientCookie.UserCode;
                    project.Update();
                    this.CreateUserAccount = ClientCookie.UserCode;
                    this.ProcInstId = procInstId;
                    Update(this);
                    var approver = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
                    if (approver == null)
                    {
                        approver = new ApproveDialogUser();
                    }
                    approver.ProjectId = this.ProjectId;
                    approver.RefTableID = this.Id.ToString();
                    approver.FlowCode = FlowCode.TempClosure_ClosurePackage;
                    approver.MarketMgrCode = approvers.MarketMgr.Code;
                    if (approvers.RegionalMgr != null)
                    {
                        approver.RegionalMgrCode = approvers.RegionalMgr.Code;
                    }
                    approver.MDDCode = approvers.MDD.Code;
                    approver.GMCode = approvers.GM.Code;
                    approver.FCCode = approvers.FC.Code;
                    approver.VPGMCode = approvers.VPGM.Code;
                    //approver.MCCLAssetMgrCode = approvers.MCCLAssetMgr.Code;
                    //approver.MCCLAssetDtrCode = approvers.MCCLAssetDtr.Code;
                    approver.NecessaryNoticeUsers = string.Join(";", approvers.NecessaryNoticeUsers.Select(u => u.Code).ToArray());
                    approver.NoticeUsers = string.Join(";", approvers.NoticeUsers.Select(u => u.Code).ToArray());
                    approver.Save();
                    ProjectInfo.FinishNode(this.ProjectId, FlowCode.TempClosure_ClosurePackage, NodeCode.TempClosure_ClosurePackage_Input);
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
                            "TempClosurePackage",
                            FlowCode.TempClosure,
                            this.ProcInstId,
                            ProjectCommentStatus.Submit
                        );
                    }
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
            var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
            if (SavedComment != null)
            {
                SavedComment.Status = ProjectCommentStatus.Submit;
                SavedComment.Action = ProjectCommentAction.Approve;
                SavedComment.Content = comment;
                SavedComment.CreateTime = DateTime.Now;
                SavedComment.Update();
            }
            else
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Approve,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.TempClosure,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
            }
        }

        public void Reject(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, ProjectAction.Decline, comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reject(this.ProjectId, this.WorkflowCode);
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Status = ProjectCommentStatus.Submit;
                    SavedComment.Action = ProjectCommentAction.Decline;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                        ProjectCommentAction.Decline,
                        comment,
                        this.Id,
                        this.TableName,
                        FlowCode.TempClosure,
                        this.ProcInstId,
                        ProjectCommentStatus.Submit
                    );
                }

                tranScope.Complete();
            }
        }

        public void Return(string comment, string SerialNumber)
        {
            TaskWork.Finish(e => e.RefID == ProjectId
                                     && e.TypeCode == WorkflowCode
                                     && e.Status == TaskWorkStatus.UnFinish);
            //&& e.K2SN != SerialNumber);

            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Return", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode, ProjectStatus.UnFinish);
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Status = ProjectCommentStatus.Submit;
                    SavedComment.Action = ProjectCommentAction.Return;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                        ProjectCommentAction.Return,
                        comment,
                        this.Id,
                        this.TableName,
                        FlowCode.TempClosure,
                        this.ProcInstId,
                        ProjectCommentStatus.Submit
                    );
                }


                tranScope.Complete();
            }
        }

        public void Resubmit(string comment, string SerialNumber, ApproveUsers newApprover, Action onExecuting = null)
        {
            var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            string destMRMgrs = newApprover.MarketMgr.Code;
            if (newApprover.RegionalMgr != null)
            {
                destMRMgrs += ";" + newApprover.RegionalMgr.Code;
            }
            List<ProcessDataField> dataFields = new List<ProcessDataField>
            {
                new ProcessDataField("dest_Creator",this.CreateUserAccount),
                new ProcessDataField("dest_MRMgrs",destMRMgrs),
                new ProcessDataField("dest_GMApprovers",string.Concat(newApprover.MDD.Code, ";", newApprover.GM.Code, ";", newApprover.FC.Code)),
                new ProcessDataField("dest_VPGM",newApprover.VPGM.Code),
                new ProcessDataField("ProcessCode",this.WorkflowProcessCode)
            };
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment, dataFields);
            using (TransactionScope tranScope = new TransactionScope())
            {
                var project = ProjectInfo.Get(this.ProjectId, FlowCode.TempClosure_ClosurePackage);
                project.CreateUserAccount = ClientCookie.UserCode;
                project.Update();
                this.CreateUserAccount = ClientCookie.UserCode;
                Update(this);
                var approver = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
                if (approver == null)
                {
                    approver = new ApproveDialogUser();
                }
                approver.ProjectId = this.ProjectId;
                approver.RefTableID = this.Id.ToString();
                approver.FlowCode = this.WorkflowCode;
                approver.MarketMgrCode = newApprover.MarketMgr.Code;
                if (newApprover.RegionalMgr != null)
                {
                    approver.RegionalMgrCode = newApprover.RegionalMgr.Code;
                }
                else
                {
                    approver.RegionalMgrCode = null;
                }
                approver.MDDCode = newApprover.MDD.Code;
                approver.GMCode = newApprover.GM.Code;
                approver.FCCode = newApprover.FC.Code;
                approver.VPGMCode = newApprover.VPGM.Code;
                approver.MCCLAssetMgrCode = newApprover.MCCLAssetMgr.Code;
                approver.MCCLAssetDtrCode = newApprover.MCCLAssetDtr.Code;
                approver.NoticeUsers = string.Join(";", newApprover.NoticeUsers.Select(u => u.Code).ToArray());
                approver.Save();
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.TempClosure_ClosurePackage_Input);
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Status = ProjectCommentStatus.Submit;
                    SavedComment.Action = ProjectCommentAction.ReSubmit;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                            ProjectCommentAction.ReSubmit,
                            comment,
                            this.Id,
                            this.TableName,
                            FlowCode.TempClosure,
                            this.ProcInstId,
                            ProjectCommentStatus.Submit
                        );
                }

                var task = TaskWork.FirstOrDefault(t => t.RefID == this.ProjectId && t.TypeCode == this.WorkflowCode && t.ReceiverAccount == assetActor.UserAccount);
                task.Finish();
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }

        public override void Recall(string comment)
        {
            string comments = ClientCookie.UserNameZHCN + "进行了流程撤回操作";
            K2FxContext.Current.GoToActivityAndRecord(
                this.ProcInstId.Value,
                this.WorkflowActOriginator,
                ClientCookie.UserCode,
                ProjectAction.Recall,
                comments
            );
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode, ProjectStatus.Recalled);
                var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == this.ProjectId && pu.RoleCode == ProjectUserRoleCode.Legal);
                var oldUnfinishTasks = TaskWork.Search(t => t.RefID == this.ProjectId && t.TypeCode == FlowCode.TempClosure_LegalReview && t.Status == TaskWorkStatus.UnFinish).ToList();
                oldUnfinishTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldUnfinishTasks.ToArray());
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Status = ProjectCommentStatus.Submit;
                    SavedComment.Action = ProjectCommentAction.Recall;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                        ProjectCommentAction.Recall,
                        comment,
                        this.Id,
                        this.TableName,
                        FlowCode.TempClosure,
                        null,
                        ProjectCommentStatus.Submit
                    );
                }
                tranScope.Complete();
            }
            //ProjectInfo.Reset(this.ProjectId, WorkflowCode);
        }

        public override string Edit()
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                Mapper.CreateMap<TempClosurePackage, TempClosurePackage>();
                var package = Mapper.Map<TempClosurePackage>(this);
                package.Id = Guid.NewGuid();
                package.IsHistory = false;
                package.CreateUserAccount = ClientCookie.UserCode;
                package.CreateTime = DateTime.Now;
                Add(package);
                IsHistory = true;
                Update(this);
                ProjectInfo.Reset(ProjectId, FlowCode.TempClosure_ClosurePackage);
                ProjectInfo.Reset(ProjectId, FlowCode.TempClosure);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), "");
                var NewAtts = new List<Attachment>();
                attachments.ForEach(att =>
                {
                    var newAttach = Duplicator.AutoCopy(att);
                    newAttach.RefTableID = package.Id.ToString();
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
                var latestTask = TaskWork.FirstOrDefault(t => t.RefID == ProjectId && t.TypeCode == FlowCode.TempClosure_ClosurePackage);
                string url = "/TempClosure/Main#/ClosurePackage?projectId=" + ProjectId;
                TaskWork.SendTask(ProjectId, latestTask.Title, latestTask.StoreCode, url, assetActor, FlowCode.TempClosure, FlowCode.TempClosure_ClosurePackage, "Start");
                tranScope.Complete();
                return url;
            }
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            string dest_MarketMgr = "";

            if (AppUsers.MarketMgr != null)
                dest_MarketMgr += AppUsers.MarketMgr.Code;

            if (AppUsers.RegionalMgr != null)
                dest_MarketMgr += ";" + AppUsers.RegionalMgr.Code;

            string dest_GMApprovers = "";

            if (AppUsers.MDD != null)
                dest_GMApprovers += AppUsers.MDD.Code + ";";

            if (AppUsers.GM != null)
                dest_GMApprovers += AppUsers.GM.Code + ";";

            if (AppUsers.FC != null)
                dest_GMApprovers += AppUsers.FC.Code + ";";

            if (dest_GMApprovers.Length > 1)
                dest_GMApprovers = dest_GMApprovers.Substring(0, dest_GMApprovers.Length - 1);

            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MRMgrs", dest_MarketMgr),
                new ProcessDataField("dest_GMApprovers",dest_GMApprovers),
                new ProcessDataField("dest_VPGM",string.Join(";",AppUsers.VPGM.Code)),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
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
            switch (status)
            {
                case TaskWorkStatus.K2ProcessDeclined:
                    ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.TempClosure, ProjectStatus.Rejected);
                    ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.TempClosure_ClosurePackage, ProjectStatus.Rejected);
                    break;
                case TaskWorkStatus.K2ProcessApproved:
                    ProjectInfo.FinishNode(ProjectId, FlowCode.TempClosure_ClosurePackage, NodeCode.TempClosure_ClosurePackage_Approve);
                    var closureMemo = new TempClosureMemo();
                    closureMemo.GenerateClosureMemoTask(ProjectId);

                    var tempClosureReopenMemo = new TempClosureReopenMemo();
                    tempClosureReopenMemo.GenerateReopenTask(ProjectId);
                    break;
            }
        }

        public static void GeneratePackageTask(string projectId)
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
            taskWork.TypeCode = FlowCode.TempClosure_ClosurePackage;
            taskWork.TypeNameENUS = "Closure Package";
            taskWork.TypeNameZHCN = "Closure Package";
            taskWork.ReceiverAccount = tempClosure.AssetActorAccount;
            taskWork.ReceiverNameENUS = tempClosure.AssetActorNameENUS;
            taskWork.ReceiverNameZHCN = tempClosure.AssetActorNameZHCN;
            taskWork.Url = string.Format(@"/TempClosure/Main#/ClosurePackage?projectId={0}", projectId);
            taskWork.StoreCode = tempClosure.USCode;
            taskWork.ActivityName = "Start";

            TaskWork.Add(taskWork);

        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var project = ProjectInfo.Get(this.ProjectId, FlowCode.TempClosure_ClosurePackage);
            var storeBasic = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
            var storeContract = StoreContractInfo.Get(storeBasic.StoreCode);
            var assetMgr = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetManager);
            var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            var assetRep = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetRep);
            var tempClosurePackage = TempClosurePackage.Get(ProjectId);
            var tempClosure = TempClosureInfo.Get(ProjectId);
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            templateFileds.Add("WorkflowName", SystemCode.Instance.GetCodeName(FlowCode.TempClosure, ClientCookie.Language));
            templateFileds.Add("ProjectID", ProjectId);
            templateFileds.Add("USCode", storeBasic.StoreCode);
            templateFileds.Add("Region", storeBasic.Region);
            templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
            templateFileds.Add("Market", storeBasic.Market);
            templateFileds.Add("City", storeBasic.CityZHCN);
            templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
            templateFileds.Add("StoreAge", Math.Floor((DateTime.Now - storeBasic.OpenDate).TotalDays / 365D).ToString());
            templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
            var storeInfo = StoreBasicInfo.GetStore(project.USCode);

            if (storeInfo.StoreContractInfo != null)
                templateFileds.Add("CurrentLeaseENDYear", storeContract.EndYear);
            else
                templateFileds.Add("CurrentLeaseENDYear", "");

            if (assetMgr != null)
                templateFileds.Add("AssetsManager", assetMgr.UserNameENUS);
            else
                templateFileds.Add("AssetsManager", "");

            templateFileds.Add("AssetsActor", assetActor.UserNameENUS);
            templateFileds.Add("AssetsRep", assetRep.UserNameENUS);
            templateFileds.Add("Address", storeBasic.AddressZHCN);
            templateFileds.Add("CloseDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");
            templateFileds.Add("ClosureDate", tempClosure.ActualTempClosureDate.ToString("yyyy-MM-dd"));
            templateFileds.Add("LeaseExpireDate", tempClosure.LeaseExpireDate.HasValue ? tempClosure.LeaseExpireDate.Value.ToString("yyyy-MM-dd") : "");
            templateFileds.Add("ReOpenDate", tempClosure.ActualReopenDate.ToString("yyyy-MM-dd"));
            templateFileds.Add("RentFreeTerm", string.IsNullOrEmpty(tempClosurePackage.RentReliefClause) ? "否" : "是");
            templateFileds.Add("RentFreeStartDate", tempClosurePackage.RentReliefStartDate.HasValue ? tempClosurePackage.RentReliefStartDate.Value.ToString("yyyy-MM-dd") : "");
            templateFileds.Add("RentFreeEndDate", tempClosurePackage.RentReliefEndDate.HasValue ? tempClosurePackage.RentReliefEndDate.Value.ToString("yyyy-MM-dd") : "");
            templateFileds.Add("FreeRentTerm", tempClosurePackage.RentReliefClause);
            templateFileds.Add("LandlordName", tempClosure.LandlordName);
            if (tempClosurePackage.RentRelief.HasValue)
                templateFileds.Add("ReliefRent", tempClosurePackage.RentRelief.Value ? "是" : "否");
            else
                templateFileds.Add("ReliefRent", "");

            return templateFileds;
        }

        public void Confirm()
        {
            var tempClosureInfo = TempClosureInfo.Get(this.ProjectId);
            var url = "/TempClosure/Main#/ClosureMemo?projectId=" + this.ProjectId;
            var title = TaskWork.BuildTitle(this.ProjectId, tempClosureInfo.StoreNameZHCN, tempClosureInfo.StoreNameENUS);
            TaskWork.Finish(t => t.ReceiverAccount == ClientCookie.UserCode && t.RefID == this.ProjectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == FlowCode.TempClosure_ClosurePackage);
            ProjectInfo.FinishNode(ProjectId, FlowCode.TempClosure_ClosurePackage, NodeCode.TempClosure_ClosurePackage_Upload, ProjectStatus.Finished);
        }


        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            var packageApprovers =
                                prevApprovers.FirstOrDefault(e => e.FlowCode == WorkflowCode
                                                                && e.RefTableID == Id.ToString());
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.TempClosure_ClosurePackage:
                        var package = new TempClosurePackage();
                        package.AppUsers = currApprover;
                        dataFields = package.SetWorkflowDataFields(null);
                        if (packageApprovers != null)
                        {
                            SimpleEmployee receiver = null;
                            if (taskWork.ReceiverAccount == packageApprovers.MarketMgrCode
                                && packageApprovers.MarketMgrCode != currApprover.MarketMgr.Code)
                            {
                                receiver = currApprover.MarketMgr;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.RegionalMgrCode
                                && packageApprovers.RegionalMgrCode != currApprover.RegionalMgr.Code)
                            {
                                receiver = currApprover.RegionalMgr;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.MDDCode
                                 && packageApprovers.MDDCode != currApprover.MDD.Code)
                            {
                                receiver = currApprover.MDD;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.GMCode
                                 && packageApprovers.GMCode != currApprover.GM.Code)
                            {
                                receiver = currApprover.GM;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.FCCode
                                 && packageApprovers.FCCode != currApprover.FC.Code)
                            {
                                receiver = currApprover.FC;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.RDDCode
                                 && packageApprovers.RDDCode != currApprover.RDD.Code)
                            {
                                receiver = currApprover.RDD;
                            } if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
                                 && packageApprovers.VPGMCode != currApprover.VPGM.Code)
                            {
                                receiver = currApprover.VPGM;

                            } if (taskWork.ReceiverAccount == packageApprovers.MCCLAssetDtrCode
                                 && packageApprovers.MCCLAssetDtrCode != currApprover.MCCLAssetDtr.Code)
                            {
                                receiver = currApprover.MCCLAssetDtr;
                            }

                            if (taskWork.ReceiverAccount == packageApprovers.CDOCode
                                 && packageApprovers.CDOCode != currApprover.CDO.Code)
                            {
                                receiver = currApprover.CDO;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.CFOCode
                                 && packageApprovers.CFOCode != currApprover.CFO.Code)
                            {
                                receiver = currApprover.CFO;
                            }
                            else
                            {
                                if (taskWork.ProcInstID.HasValue)
                                {
                                    UpdateWorkflowDataField(taskWork.ProcInstID.Value, dataFields);
                                }
                            }

                            if (receiver != null)
                            {
                                try
                                {
                                    RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, receiver.Code, dataFields);
                                }
                                catch (Exception ex)
                                {
                                    Log4netHelper.WriteError(string.Format("【Redirect Workflow Task Error】：{0}",
                                        JsonConvert.SerializeObject(taskWork)));
                                }
                                finally
                                {
                                    taskWork.ReceiverAccount = receiver.Code;
                                    taskWork.ReceiverNameENUS = receiver.NameENUS;
                                    taskWork.ReceiverNameZHCN = receiver.NameZHCN;
                                }
                            }
                            if (currApprover.MarketMgr != null)
                                packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                            if (currApprover.RegionalMgr != null)
                                packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                            if (currApprover.DD != null)
                                packageApprovers.DDCode = currApprover.DD.Code;
                            if (currApprover.GM != null)
                                packageApprovers.GMCode = currApprover.GM.Code;
                            if (currApprover.FC != null)
                                packageApprovers.FCCode = currApprover.FC.Code;
                            if (currApprover.RDD != null)
                                packageApprovers.RDDCode = currApprover.RDD.Code;
                            if (currApprover.VPGM != null)
                                packageApprovers.VPGMCode = currApprover.VPGM.Code;
                            if (currApprover.MCCLAssetDtr != null)
                                packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
                            if (currApprover.CDO != null)
                                packageApprovers.CDOCode = currApprover.CDO.Code;
                            if (currApprover.CFO != null)
                                packageApprovers.CFOCode = currApprover.CFO.Code;
                            if (currApprover.NoticeUsers != null)
                            {
                                packageApprovers.NoticeUsers = "";
                                foreach (var noticeUser in currApprover.NoticeUsers)
                                {
                                    if (string.IsNullOrEmpty(packageApprovers.NoticeUsers))
                                        packageApprovers.NoticeUsers = noticeUser.Code;
                                    else
                                        packageApprovers.NoticeUsers += ";" + noticeUser.Code;
                                }
                            }
                            if (currApprover.NecessaryNoticeUsers != null)
                            {
                                packageApprovers.NecessaryNoticeUsers = "";
                                foreach (var noticeUser in currApprover.NecessaryNoticeUsers)
                                {
                                    if (string.IsNullOrEmpty(packageApprovers.NecessaryNoticeUsers))
                                        packageApprovers.NecessaryNoticeUsers = noticeUser.Code;
                                    else
                                        packageApprovers.NecessaryNoticeUsers += ";" + noticeUser.Code;
                                }
                            }
                        }


                        break;
                }
            }
        }

    }
}