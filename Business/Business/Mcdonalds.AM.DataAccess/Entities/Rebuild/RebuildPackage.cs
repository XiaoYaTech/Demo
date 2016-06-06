using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;
using Mcdonalds.AM.Services.Common;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RebuildPackage : BaseWFEntity<RebuildPackage>
    {
        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string SerialNumber { get; set; }
        public string USCode { get; set; }
        public RebuildInfo RbdInfo { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }
        public bool IsShowReject { get; set; }

        public bool IsAssetMgr
        {
            get { return ConfigurationManager.AppSettings["AssetMgrCode"] == ClientCookie.UserCode; }
        }

        public static string AssetMgrCode
        {
            get
            {
                return ConfigurationManager.AppSettings["AssetMgrCode"];
            }
        }

        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Rebuild\Package"; } }
        public override string WorkflowProcessCode { get { return @"MCD_AM_Rebuild_P"; } }
        public static string TableName { get { return "RebuildPackage"; } }
        public static string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override string WorkflowCode
        {
            get { return FlowCode.Rebuild_Package; }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetRebuildPackageInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
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

            if (AppUsers.RDD != null)
                dest_GMApprovers += AppUsers.RDD.Code + ";";

            if (dest_GMApprovers.Length > 1)
                dest_GMApprovers = dest_GMApprovers.Substring(0, dest_GMApprovers.Length - 1);
            bool isNeedCheckCDO = CheckIfNeedCDOApproval();
            bool isNeedAssetMgr = CheckIfNeedAssetMgrUpload();

            string strCDOCode = "";
            string strCFOCode = "";
            string strMDCode = "";
            string strAssetMgrCode = "";
            if (isNeedCheckCDO)
            {
                strCDOCode = AppUsers.CDO.Code;
                strCFOCode = AppUsers.CFO.Code;
                strMDCode = AppUsers.ManagingDirector.Code;
            }

            strAssetMgrCode = AssetMgrCode;

            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MarketMgr", dest_MarketMgr),
                new ProcessDataField("dest_GMApprovers",dest_GMApprovers),
                new ProcessDataField("dest_VPGM",string.Join(";",AppUsers.VPGM.Code)),
                new ProcessDataField("dest_DevVP",string.Join(";",AppUsers.MCCLAssetDtr.Code)),
                new ProcessDataField("dest_CDO",string.Join(";",strCDOCode)),
                new ProcessDataField("dest_CFO",string.Join(";",strCFOCode)),
                new ProcessDataField("dest_MngDirector",string.Join(";",strMDCode)),
                new ProcessDataField("dest_AssetMgr",string.Join(";",strAssetMgrCode)),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
                new ProcessDataField("IsNeedCDOApproval",isNeedCheckCDO.ToString(),"BOOLEAN"),
                new ProcessDataField("IsNeedAssetMgrUpload",isNeedAssetMgr.ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        private Boolean CheckIfNeedAssetMgrUpload()
        {
            if (NetWriteOff.HasValue)
            {
                if (NetWriteOff.Value >= Convert.ToDecimal(152.75 * 10000))
                    return true;
                return false;
            }
            return false;
        }

        public ProjectContractRevision PCR { get; set; }

        private bool CheckIfNeedCDOApproval()
        {
            if (NetWriteOff.HasValue)
            {
                if (NetWriteOff.Value >= Convert.ToDecimal(61.1 * 10000))
                    return true;
                return false;
            }
            return false;
        }

        public override string Edit()
        {
            var taskUrl = string.Format("/Rebuild/Main#/RebuildPackage?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                //CompleteNotFinishTask();
                GeneratePackageTask();
                IsHistory = true;
                Update(this);

                ProjectInfo.Reset(ProjectId, FlowCode.Rebuild_Package);
                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Rebuild_Package, ProjectStatus.UnFinish);

                Mapper.CreateMap<RebuildPackage, RebuildPackage>();
                var form = Mapper.Map<RebuildPackage>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

                CopyAttachment(Id.ToString(), form.Id.ToString());
                CopyAppUsers(Id.ToString(), form.Id.ToString());
                scope.Complete();
            }

            return taskUrl;
        }
        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessDeclined:
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Rebuild, ProjectStatus.Rejected);
                        ProjectInfo.UpdateProjectStatus(ProjectId, WorkflowCode, ProjectStatus.Rejected);
                        break;
                    case TaskWorkStatus.K2ProcessApproved:
                        {
                            if (CheckIfNeedAssetMgrUpload())
                                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Finish);
                            else
                            {
                                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Examine);
                                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Upload);
                                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Finish);
                            }

                            //var consCheckingTask = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Rebuild_ConsInvtChecking && e.Status == TaskWorkStatus.UnFinish);
                            //var checkingProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId && e.FlowCode == FlowCode.Rebuild_ConsInvtChecking);
                            //if (consCheckingTask == null && checkingProj != null &&
                            //    checkingProj.Status != ProjectStatus.Finished)
                            //{
                            //    var consInvtChecking = new RebuildConsInvtChecking();
                            //    consInvtChecking.ProjectId = task.RefID;
                            //    consInvtChecking.GenerateConsInvtCheckingTask();
                            //}

                            var listTask = new List<TaskWork>();
                            var contractTask = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Rebuild_ContractInfo && e.Status == TaskWorkStatus.UnFinish);
                            var contractProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId && e.FlowCode == FlowCode.Rebuild_ContractInfo);
                            if (contractTask == null && contractProj != null &&
                                contractProj.Status != ProjectStatus.Finished)
                            {
                                listTask.Add(GenerateTaskWork(FlowCode.Rebuild_ContractInfo));
                            }

                            var siteInfoTask = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Rebuild_SiteInfo && e.Status == TaskWorkStatus.UnFinish);
                            var siteInfoProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId && e.FlowCode == FlowCode.Rebuild_SiteInfo);
                            if (siteInfoTask == null && siteInfoProj != null &&
                                siteInfoProj.Status != ProjectStatus.Finished)
                            {
                                listTask.Add(GenerateTaskWork(FlowCode.Rebuild_SiteInfo));
                            }

                            var gbMemoTask = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Rebuild_GBMemo && e.Status == TaskWorkStatus.UnFinish);
                            var gbMemoProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId && e.FlowCode == FlowCode.Rebuild_SiteInfo);
                            if (gbMemoTask == null && gbMemoProj != null && gbMemoProj.Status != ProjectStatus.Finished)
                            {
                                //listTask.Add(GenerateTaskWork(FlowCode.Rebuild_GBMemo));
                                var tsk = GenerateTaskWork(FlowCode.Rebuild_GBMemo);
                                var rbdInfo = new RebuildInfo();
                                rbdInfo = rbdInfo.GetRebuildInfo(ProjectId);
                                if (rbdInfo != null && rbdInfo.GBDate.HasValue)
                                    ScheduleLog.GenerateTaskSchedule(rbdInfo.GBDate.Value.AddDays(-3), tsk, ClientCookie.UserCode,
                                        ProjectId, FlowCode.Rebuild_GBMemo, rbdInfo.USCode);
                            }

                            if (listTask.Count > 0)
                                TaskWork.Add(listTask.ToArray());
                        }
                        break;
                }

                scope.Complete();
            }
        }
        public static RebuildPackage GetRebuildPackageInfo(string strProjectId, string entityId = "")
        {
            var entity = string.IsNullOrEmpty(entityId) ?
                Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault()
                : Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();
            var rbdInfo = new RebuildInfo();
            rbdInfo = rbdInfo.GetRebuildInfo(strProjectId);

            if (entity == null)
            {
                entity = new RebuildPackage();
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
                entity.PCR = StoreContractInfo.MappingProjectContractRevision(rbdInfo.USCode);
                if (entity.PCR != null)
                {
                    entity.PCR.ProjectId = strProjectId;
                    entity.OldChangeLeaseTermExpiraryDate = entity.PCR.LeaseChangeExpiryOld;
                    if (!string.IsNullOrEmpty(entity.PCR.RedlineAreaOld))
                    {
                        entity.OldChangeRedLineRedLineArea = Convert.ToDecimal(entity.PCR.RedlineAreaOld);
                    }
                    entity.OldLandlord = entity.PCR.LandlordOld;
                    entity.OldRentalStructure = entity.PCR.RentStructureOld;
                }
            }
            else
            {
                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == strProjectId
                                                               && e.FlowCode == FlowCode.Rebuild_Package);

                if (projectInfo != null)
                {

                    if (rbdInfo != null)
                    {
                        entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.Rebuild_Package);
                        entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.Rebuild_Package);

                        var includeRejctActivityList = new List<string>()
                        {
                            "VPGM",
                            "Dev VP",
                            "CDO",
                            "CFO",
                            "Managing Director"
                        };
                        entity.IsShowReject = TaskWork.Any(
                                e => e.RefID == strProjectId
                                    && e.TypeCode == FlowCode.Rebuild_Package
                                    && e.K2SN != null
                                    && includeRejctActivityList.Contains(e.ActivityName)
                                    && e.Status == TaskWorkStatus.UnFinish);
                    }
                }
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Rebuild;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = TableName;

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.ProjectComments = comments;

                    var saveComment =
                        comments.OrderByDescending(e => e.CreateTime)
                            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                    if (saveComment != null)
                    {
                        entity.Comments = saveComment.Content;
                    }
                }
                entity.PCR = ProjectContractRevision.Get(rbdInfo.ProjectId);
            }
            if (entity.WriteOff == null || entity.NetWriteOff == null)
            {
                var consInfo = new RebuildConsInfo().GetConsInfo(strProjectId);
                if (consInfo != null)
                {
                    var writeOff = WriteOffAmount.GetByConsInfoId(consInfo.Id);
                    var recos = ReinvestmentCost.GetByConsInfoId(consInfo.Id);
                    if (writeOff != null && !string.IsNullOrEmpty(writeOff.TotalWriteOff))
                    {
                        entity.WriteOff = Convert.ToDecimal(Convert.ToDecimal(writeOff.TotalWriteOff).ToString("F2"));
                        entity.NewInvestment = Convert.ToDecimal(Convert.ToDecimal(recos.TotalReinvestmentBudget).ToString("F2"));
                    }
                    else
                    {
                        entity.WriteOff = null;
                        entity.NewInvestment = null;
                    }
                }
            }

            entity.RbdInfo = rbdInfo;
            entity.TempClosureDate = rbdInfo.TempClosureDate;
            entity.ReopenDate = rbdInfo.ReopenDate;
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            entity.USCode = rbdInfo.USCode;
            if (ClientCookie.UserCode.Equals(rbdInfo.AssetActorAccount))
                entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Rebuild_Package);
            PopulateAppUsers(entity);
            return entity;
        }

        public static void PopulateAppUsers(RebuildPackage entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.MarketMgrCode
                };
                entity.AppUsers.MarketMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RegionalMgrCode
                };
                entity.AppUsers.RegionalMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MDDCode
                };
                entity.AppUsers.MDD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.GMCode
                };
                entity.AppUsers.GM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.FCCode
                };
                entity.AppUsers.FC = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RDDCode
                };
                entity.AppUsers.RDD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.VPGMCode
                };
                entity.AppUsers.VPGM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CDOCode
                };
                entity.AppUsers.CDO = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CFOCode
                };
                entity.AppUsers.CFO = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetMgrCode
                };
                entity.AppUsers.MCCLAssetMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetDtrCode
                };
                entity.AppUsers.MCCLAssetDtr = simp;

                if (!string.IsNullOrEmpty(approvedUsers.NecessaryNoticeUsers))
                {
                    var temp = approvedUsers.NecessaryNoticeUsers.Split(';');
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
                else
                {
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Rebuild_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
                }

                if (!string.IsNullOrEmpty(approvedUsers.NoticeUsers))
                {
                    var temp = approvedUsers.NoticeUsers.Split(';');
                    entity.AppUsers.NoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
            }
            else
            {
                entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Rebuild_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
            }
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish,
                FlowCode.Rebuild, FlowCode.Rebuild_Package);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Rebuild/Main#/RebuildPackage/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);
                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Rebuild_Package, NodeCode.Rebuild_Package_Input);
                scope.Complete();
            }
        }
        private int? StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount,
                processDataFields);
        }
        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
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
                    case "Confirm":
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Examine);
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Upload);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Input, GetProjectStatus(actionName));
                        break;
                }

                if (actionName == ProjectAction.Return)
                {
                    TaskWork.Finish(e => e.RefID == ProjectId
                                         && e.TypeCode == WorkflowCode
                                         && e.Status == TaskWorkStatus.UnFinish);
                }
                scope.Complete();
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, ParseActionName(actionName), comment, dataField);
        }
        private string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                case ProjectAction.ReSubmit:
                    nodeCode = NodeCode.Rebuild_Package_Examine;
                    break;
                default:
                    nodeCode = NodeCode.Rebuild_Package_Upload;
                    break;
            }
            return nodeCode;
        }
        public void ApprovePackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }
        public void RejectPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Decline, Comments);
        }
        public void ReturnPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }
        public void ResubmitPackage(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
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
                    CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    CreateUserNameENUS = ClientCookie.UserNameENUS;
                    IsHistory = false;
                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                var rbdInfo = new RebuildInfo();
                rbdInfo = rbdInfo.GetRebuildInfo(ProjectId);
                rbdInfo.ReopenDate = ReopenDate;
                rbdInfo.TempClosureDate = TempClosureDate;
                rbdInfo.Update();
                SaveApproveUsers(action);
                SaveComments(action);
                //SaveProjectCtractRevision();
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
                        approveUser.FlowCode = FlowCode.Rebuild_Package;
                    }
                    approveUser.MarketMgrCode = AppUsers.MarketMgr.Code;

                    if (AppUsers.RegionalMgr != null)
                    {
                        approveUser.RegionalMgrCode = AppUsers.RegionalMgr.Code;
                    }
                    approveUser.MDDCode = AppUsers.MDD.Code;
                    approveUser.GMCode = AppUsers.GM.Code;
                    approveUser.FCCode = AppUsers.FC.Code;
                    //approveUser.RDDCode = AppUsers.RDD.Code;
                    approveUser.VPGMCode = AppUsers.VPGM.Code;

                    if (AppUsers.CDO != null)
                    {
                        approveUser.CDOCode = AppUsers.CDO.Code;
                    }

                    if (AppUsers.CFO != null)
                    {
                        approveUser.CFOCode = AppUsers.CFO.Code;
                    }

                    if (AppUsers.MCCLAssetMgr != null)
                    {
                        approveUser.MCCLAssetMgrCode = AppUsers.MCCLAssetMgr.Code;
                    }
                    if (AppUsers.MCCLAssetDtr != null)
                    {
                        approveUser.MCCLAssetDtrCode = AppUsers.MCCLAssetDtr.Code;
                    }
                    if (AppUsers.NecessaryNoticeUsers != null)
                        approveUser.NecessaryNoticeUsers = string.Join(";", AppUsers.NecessaryNoticeUsers.Select(u => u.Code).ToArray());
                    if (AppUsers.NoticeUsers != null)
                        approveUser.NoticeUsers = string.Join(";", AppUsers.NoticeUsers.Select(u => u.Code).ToArray());

                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }

        }
        private void SaveComments(string action)
        {
            if (action == "Edit")
                return;
            var comment = ProjectComment.GetSavedComment(Id, "RebuildPackage", ClientCookie.UserCode);
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
                comment.RefTableName = "RebuildPackage";
                comment.SourceCode = FlowCode.Rebuild;
                comment.SourceNameZHCN = FlowCode.Rebuild;
                comment.SourceNameENUS = FlowCode.Rebuild;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstID;
                comment.Add();
            }
        }

        private void SaveProjectCtractRevision()
        {
            if (PCR != null)
            {
                if (!string.IsNullOrEmpty(NewLandlord))
                    PCR.LandlordNew = NewLandlord;
                if (NewChangeLeaseTermExpiraryDate != null && NewChangeLeaseTermExpiraryDate.HasValue)
                    PCR.LeaseChangeExpiryNew = NewChangeLeaseTermExpiraryDate;
                if (NewChangeRedLineRedLineArea != null && NewChangeRedLineRedLineArea.HasValue)
                    PCR.RedlineAreaNew = NewChangeRedLineRedLineArea.Value.ToString();
                if (!string.IsNullOrEmpty(NewRentalStructure))
                    PCR.RentStructureNew = NewRentalStructure;
                PCR.Append();
            }
        }

        public static List<Attachment> GetPackageAgreementList(string projectId, string packageId, string uploadFilesURL)
        {
            string typeCode = "";
            string refTableId = "";
            var listReturn = new List<Attachment>();

            var legal = RebuildLegalReview.GetLegalReviewInfo(projectId);
            if (legal != null)
            {
                refTableId = legal.Id.ToString();
                var listLegal = new List<Attachment>();
                var listTemp = Attachment.GetList("RebuildLegalReview", refTableId, typeCode);
                foreach (var item in listTemp)
                {
                    item.FileName = item.Name;
                    if (item.TypeCode != "Contract")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        if (item.TypeCode == "" && item.RequirementId != null)
                        {
                            var attRe = AttachmentRequirement.Get(item.RequirementId.Value);
                            item.Name = attRe.NameZHCN;
                        }
                        else
                        {
                            if (item.TypeCode == "LegalClearanceReport")
                                item.Name = "Legal clearance report";
                            else if (item.TypeCode == "Agreement")
                            {
                                item.Name = "Lease agreement";
                            }
                            else if (item.TypeCode == "Others")
                            {
                                item.Name = "Others";
                            }
                            else if (item.TypeCode == "OthersCN")
                            {
                                item.Name = "往来函件，业主证明";
                            }
                        }
                        listLegal.Add(item);
                    }
                }
                listReturn.AddRange(listLegal);
            }

            var finance = new RebuildFinancAnalysis();
            finance = finance.GetFinanceInfo(projectId);
            if (finance != null)
            {
                refTableId = finance.Id.ToString();
                var listFinance = new List<Attachment>();
                var listTemp = Attachment.GetList("RebuildFinancAnalysis", refTableId, "FinAgreement");
                foreach (var item in listTemp)
                {
                    item.FileName = item.Name;
                    if (item.TypeCode != "Attachment")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        item.Name = "Finance Analysts";
                        listFinance.Add(item);
                    }
                }
                listReturn.AddRange(listFinance);
            }

            var consinfo = new RebuildConsInfo();
            consinfo = consinfo.GetConsInfo(projectId);
            if (consinfo != null)
            {
                var listConsInfo = new List<Attachment>();
                refTableId = consinfo.Id.ToString();
                var listTemp = Attachment.GetList("RebuildConsInfo", refTableId, typeCode);
                foreach (var item in listTemp)
                {
                    item.FileName = item.Name;
                    if (item.TypeCode != "Attachment")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        if (item.TypeCode == "ConsInfoAgreement")
                            item.Name = "Store Layout";
                        else if (item.TypeCode == "ReinCost")
                        {
                            item.Name = "FA Investment Tool";
                        }
                        else if (item.TypeCode == "WriteOff")
                        {
                            item.Name = "FA Write - off Tool";
                        }
                        listConsInfo.Add(item);
                    }
                }
                listReturn.AddRange(listConsInfo);
            }
            RebuildPackage package = null;
            if (string.IsNullOrEmpty(packageId))
            {
                package = FirstOrDefault(e => e.ProjectId.Equals(projectId));
                if (package != null)
                    packageId = package.Id.ToString();
            }
            var listPackage = new List<Attachment>();
            var listTempPackage = Attachment.GetList("RebuildPackage", packageId, typeCode);
            if (listTempPackage != null)
            {
                foreach (var item in listTempPackage)
                {
                    item.FileName = item.Name;
                    item.FileURL = uploadFilesURL + "/" + item.InternalName;
                    if (item.TypeCode == "SignedApproval")
                    {
                        item.Name = "Signed Approval";
                    }
                    else if (item.TypeCode == "SignedAgreement")
                    {
                        item.Name = "Signed Agreement";
                    }
                    else if (item.TypeCode == ExcelDataInputType.RebuildCover.ToString())
                    {
                        item.Name = "Cover";
                    }
                    if ((ClientCookie.UserCode == AssetMgrCode && item.TypeCode == "SignedApproval")
                        || item.TypeCode != "SignedApproval")
                    {
                        listPackage.Add(item);
                    }
                }
                listReturn.AddRange(listPackage);
            }
            //listReturn.Add(new Attachment()
            //{
            //    Name = "Right sizing",
            //    TypeCode = "RightSizing",
            //    FileURL = "#",
            //});
            //listReturn.Add(new Attachment()
            //{
            //    Name = "Facade, signage rendering",
            //    TypeCode = "FacadeSignageRendering",
            //    FileURL = "#",
            //});
            //if (!listReturn.Where(e => e.TypeCode == ExcelDataInputType.RebuildCover.ToString()).Any())
            //{
            //    listReturn.Add(new Attachment()
            //    {
            //        Name = "Cover",
            //        TypeCode = "Cover",
            //        FileURL = "#",
            //    });
            //}
            if (!listReturn.Where(e => e.TypeCode == "SignedApproval").Any() && ClientCookie.UserCode == AssetMgrCode)
            {
                listReturn.Add(new Attachment()
                {
                    Required = true,
                    Name = "Signed Approval",
                    TypeCode = "SignedApproval",
                    FileURL = "#",
                });
            }
            //if (!listReturn.Where(e => e.TypeCode == "SignedAgreement").Any())
            //{
            //    listReturn.Add(new Attachment()
            //    {
            //        Name = "Signed Agreement",
            //        TypeCode = "SignedAgreement",
            //        FileURL = "#",
            //    });
            //}
            return listReturn;
        }
        public void GenerateCoverEexcel(string templatePath, string tempFilePath)
        {
            //生成Cover Excecl 后，往Attachment表里插入一条记录。
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (projectInfo == null)
            {
                throw new Exception("Cannot find the project info!");
            }

            File.Copy(templatePath, tempFilePath);
            var fileInfo = new FileInfo(tempFilePath);
            var excelOutputDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.RebuildCover);

            var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == projectInfo.USCode);
            if (store == null)
            {
                throw new Exception("Cannot find Store info!");
            }

            var inputInfo = new ExcelInputDTO
            {
                Region = store.RegionENUS,
                Market = store.MarketENUS,
                Province = store.ProvinceENUS,
                StoreName = store.NameZHCN,
                City = store.CityENUS,
                USCode = store.StoreCode,
                StoreType = store.StoreType,
                OpenDate = store.OpenDate
            };

            excelOutputDirector.Input(inputInfo);

            var att = new Attachment
            {
                InternalName = fileInfo.Name,
                RefTableName = TableName,
                RefTableID = Id.ToString(),
                RelativePath = "//",
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                Length = (int)fileInfo.Length,
                CreateTime = DateTime.Now,
                ID = Guid.NewGuid(),
                TypeCode = ExcelDataInputType.RebuildCover.ToString(),
                CreatorID = ClientCookie.UserCode,
                CreatorNameENUS = ClientCookie.UserNameENUS,
                CreatorNameZHCN = ClientCookie.UserNameZHCN,
                UpdateTime = DateTime.Now,
                IsDelete = 0
            };
            Attachment.SaveSigleFile(att);

        }
        public bool CheckIfUnfreezePackageWorkflow(string projectId)
        {
            var isUnfreeze = false;
            var flowCodeList = new List<string>()
            {
                FlowCode.Rebuild_ConsInfo,
                FlowCode.Rebuild_FinanceAnalysis,
                FlowCode.Rebuild_LegalReview
            };
            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId
                                                        && flowCodeList.Contains(e.FlowCode)).AsNoTracking().ToList();

            if (projectInfos.Count == flowCodeList.Count
                && projectInfos.All(e => e.Status == ProjectStatus.Finished))
            {
                isUnfreeze = true;
            }

            return isUnfreeze;
        }
        public void GeneratePackageTask()
        {
            if (CheckIfUnfreezePackageWorkflow(ProjectId))
            {
                if (!TaskWork.Any(e => e.RefID == ProjectId
                        && e.TypeCode == WorkflowCode
                        && e.Status == TaskWorkStatus.UnFinish))
                {
                    var taskUrl = string.Format("/Rebuild/Main#/RebuildPackage?projectId={0}", ProjectId);
                    var rebuildInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                    if (rebuildInfo == null)
                    {
                        throw new Exception("Could not find the Rebuild Lease Info, please check it!");
                    }
                    var task = rebuildInfo.GenerateTaskWork(FlowCode.Rebuild_Package, WorkflowCode, WorkflowCode, taskUrl);
                    task.ActivityName = NodeCode.Start;
                    task.ActionName = SetTaskActionName(ProjectId);
                    TaskWork.Add(task);
                }
            }
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }

            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
                WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }
        private TaskWork GenerateTaskWork(string strTypeCode)
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Rebuild;
            taskWork.SourceNameENUS = FlowCode.Rebuild;
            taskWork.SourceNameZHCN = FlowCode.Rebuild;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = ProjectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == ProjectId);

            if (rbdInfo == null)
            {
                throw new Exception("Cannot find the relative Rebuild info!");
            }

            taskWork.Title = TaskWork.BuildTitle(ProjectId, rbdInfo.StoreNameZHCN, rbdInfo.StoreNameENUS);
            taskWork.TypeCode = strTypeCode;
            taskWork.TypeNameENUS = strTypeCode;
            taskWork.TypeNameZHCN = strTypeCode;

            if (strTypeCode == FlowCode.Rebuild_ContractInfo)
            {
                taskWork.ReceiverAccount = rbdInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = rbdInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = rbdInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/ContractInfo?projectId={0}", ProjectId);
            }
            else if (strTypeCode == FlowCode.Rebuild_SiteInfo)
            {
                taskWork.ReceiverAccount = rbdInfo.PMAccount;
                taskWork.ReceiverNameENUS = rbdInfo.PMNameENUS;
                taskWork.ReceiverNameZHCN = rbdInfo.PMNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/SiteInfo?projectId={0}", ProjectId);
            }
            else if (strTypeCode == FlowCode.Rebuild_ReopenMemo)
            {
                taskWork.ReceiverAccount = rbdInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = rbdInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = rbdInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/ReopenMemo?projectId={0}", ProjectId);
            }
            else if (strTypeCode == FlowCode.Rebuild_GBMemo)
            {
                taskWork.ReceiverAccount = rbdInfo.PMAccount;
                taskWork.ReceiverNameENUS = rbdInfo.PMNameENUS;
                taskWork.ReceiverNameZHCN = rbdInfo.PMNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/GBMemo?projectId={0}", ProjectId);
            }
            else if (strTypeCode == FlowCode.Rebuild_TempClosureMemo)
            {
                taskWork.ReceiverAccount = rbdInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = rbdInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = rbdInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/TempClosureMemo?projectId={0}", ProjectId);
            }
            else if (strTypeCode == "PackageActorUpload")
            {
                taskWork.TypeCode = FlowCode.Rebuild_Package;
                taskWork.TypeNameENUS = FlowCode.Rebuild_Package;
                taskWork.TypeNameZHCN = FlowCode.Rebuild_Package;

                taskWork.ReceiverAccount = CreateUserAccount;
                taskWork.ReceiverNameENUS = CreateUserNameENUS;
                taskWork.ReceiverNameZHCN = CreateUserNameZHCN;
                taskWork.Url = string.Format(@"/Rebuild/Main#/RebuildPackage/Process/Upload?projectId={0}", ProjectId);
            }
            taskWork.StoreCode = rbdInfo.USCode;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            return taskWork;
        }

        public void Confrim()
        {
            //var task = TaskWork.FirstOrDefault(e => e.RefID == ProjectId
            //                                     && e.TypeCode == WorkflowCode
            //                                     && e.ReceiverAccount == ClientCookie.UserCode);
            //if (task != null)
            //{
            //    task.Status = TaskWorkStatus.Finished;
            //}
            //TaskWork.Update(task);
            //ProjectInfo.CompleteMainIfEnable(ProjectId);

            ExecuteProcess(ClientCookie.UserCode, SerialNumber, "Confirm", Comments);
        }
        public void CompleteActorPackageTask(string actor)
        {
            var task = TaskWork.FirstOrDefault(e => e.RefID == ProjectId
                                                    && e.TypeCode == WorkflowCode
                                                    && e.ReceiverAccount == actor
                                                    && e.Status == TaskWorkStatus.UnFinish);
            if (task != null)
            {
                task.Status = TaskWorkStatus.Finished;
                TaskWork.Update(task);
            }
        }

        public void CompleteNotFinishTask()
        {
            var tasks = TaskWork.Search(e => e.RefID == ProjectId && e.Status == TaskWorkStatus.UnFinish).ToList();
            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    task.Status = TaskWorkStatus.Finished;
                    string strTypeCode = task.TypeCode.Split('_')[1];
                    if (strTypeCode == "Package")
                        strTypeCode = "RebuildPackage";
                    task.Url = string.Format("/Rebuild/Main#/{0}/Process/View?projectId={1}", strTypeCode, ProjectId);
                }
                TaskWork.Update(tasks.ToArray());
            }
        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var rbdInfo = new RebuildInfo();
            rbdInfo = rbdInfo.GetRebuildInfo(this.ProjectId);
            var storeInfo = StoreBasicInfo.GetStore(rbdInfo.USCode);
            StoreBasicInfo store = storeInfo.StoreBasicInfo;
            StoreContractInfo storeContract = StoreContractInfo.Get(store.StoreCode);
            var assetMgr = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetManager);
            var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            var assetRep = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetRep);

            var printDic = new Dictionary<string, string>();
            printDic.Add("WorkflowName", FlowCode.Rebuild_Package);
            printDic.Add("ProjectID", this.ProjectId);
            printDic.Add("USCode", rbdInfo.USCode);
            printDic.Add("StoreNameEN", store.NameENUS);
            printDic.Add("Market", store.MarketENUS);
            printDic.Add("Region", store.RegionENUS);
            printDic.Add("StoreNameCN", store.NameZHCN);
            printDic.Add("City", store.CityENUS);
            printDic.Add("StoreAge", Math.Floor((DateTime.Now - store.OpenDate).TotalDays / 365D).ToString());
            printDic.Add("Address", store.AddressZHCN);
            printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            if (store.CloseDate.HasValue && store.CloseDate.Value.Year == 1900)
            {
                printDic.Add("CloseDate", string.Empty);
            }
            else
            {
                printDic.Add("CloseDate", store.CloseDate.HasValue ? store.CloseDate.Value.ToString("yyyy-MM-dd") : "");
            }
            if (storeContract != null)
            {
                printDic.Add("OriginalLeaseENDDate", storeContract.EndDate.HasValue ? storeContract.EndDate.Value.ToString("yyyy-MM-dd") : "");
            }
            else
            {
                printDic.Add("OriginalLeaseENDDate", "");
            }
            printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            printDic.Add("AssetsActor", assetActor.UserNameENUS);
            printDic.Add("AssetsRep", assetRep.UserNameENUS);//store.AssetRepName  TODO::Cary
            if (store.CloseDate.HasValue)
            {
                if(store.CloseDate.Value.ToString("yyyy-MM-dd").IndexOf("1900")!=-1)
                    printDic.Add("ClosureDate", store.CloseDate.Value.ToString("yyyy-MM-dd"));
                else
                    printDic.Add("ClosureDate","");
            }
            else
            {
                printDic.Add("ClosureDate","");
            }
            
            printDic.Add("WriteOff", WriteOff.HasValue ? DataConverter.ToMoney(WriteOff.Value) : "");
            printDic.Add("CashCompensation", CashCompensation.HasValue ? DataConverter.ToMoney(CashCompensation.Value) : "");
            printDic.Add("NetWriteOff", NetWriteOff.HasValue ? DataConverter.ToMoney(NetWriteOff.Value) : "");
            printDic.Add("NewInvestment", NewInvestment.HasValue ? DataConverter.ToMoney(NewInvestment.Value) : "");
            printDic.Add("CashFlowNVPCurrent", CashFlowNVPCurrent.HasValue ? DataConverter.ToMoney(CashFlowNVPCurrent.Value) : "");
            printDic.Add("CashFlowNVPAfterChange", CashFlowNVPAfterChange.HasValue ? DataConverter.ToMoney(CashFlowNVPAfterChange.Value) : "");
            printDic.Add("OtherCompensation", OtherCompensation.HasValue ? DataConverter.ToMoney(OtherCompensation.Value) : "");
            printDic.Add("NetGain", NetGain.HasValue ? DataConverter.ToMoney(NetGain.Value) : "");
            printDic.Add("ReasonDesc", ReasonDesc);
            printDic.Add("OtherCompensationDescription", OtherCompenDesc);
            printDic.Add("TempClosureDate", TempClosureDate.HasValue ? TempClosureDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
            printDic.Add("ReopenDate", ReopenDate.HasValue ? ReopenDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
            if (storeInfo.StoreContractInfo.EndDate != null && storeInfo.StoreContractInfo.EndDate.HasValue)
                printDic.Add("CurrentLeaseENDYear",
                    (storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd")));
            else
                printDic.Add("CurrentLeaseENDYear", "");
            printDic.Add("DecisionLogicRecomendation", string.IsNullOrEmpty(DecisionLogicRecomendation) ? "&nbsp;" : DecisionLogicRecomendation);

            bool changeFlag = false;
            if (ChangeRentalType.HasValue && ChangeRentalType.Value)
            {
                printDic.Add("TheChangeOfTheRental", "The Change of the rental");
                printDic.Add("OldRentalStructure", "Old Rental Structure:" + OldRentalStructure);
                printDic.Add("NewRentalStructure", "New Rental Structure:" + NewRentalStructure);
                changeFlag = true;
            }
            else
            {
                printDic.Add("TheChangeOfTheRental", "");
                printDic.Add("OldRentalStructure", "");
                printDic.Add("NewRentalStructure", "");
            }

            if (ChangeRedLineType.HasValue && ChangeRedLineType.Value)
            {
                printDic.Add("TheChangeOfRedLine", "The Change of red line");
                printDic.Add("OldChangeRedLineRedLineArea", OldChangeRedLineRedLineArea.HasValue ? "Old Redline Area (sqm):" + Math.Round(OldChangeRedLineRedLineArea.Value, 0).ToString() : "");
                printDic.Add("NewChangeRedLineRedLineArea", NewChangeRedLineRedLineArea.HasValue ? "New Redline Area (sqm):" + Math.Round(NewChangeRedLineRedLineArea.Value, 0).ToString() : "");
                changeFlag = true;
            }
            else
            {
                printDic.Add("TheChangeOfRedLine", "");
                printDic.Add("OldChangeRedLineRedLineArea", "");
                printDic.Add("NewChangeRedLineRedLineArea", "");
            }

            if (ChangeLeaseTermType.HasValue && ChangeLeaseTermType.Value)
            {
                printDic.Add("TheChangeOfLeaseTeam", "The Change of lease term");
                printDic.Add("OldChangeLeaseTermExpiraryDate", OldChangeLeaseTermExpiraryDate.HasValue ? "Old Lease Change Expiry (Date):" + OldChangeLeaseTermExpiraryDate.Value.ToString("yyyy-MM-dd") : "");
                printDic.Add("NewChangeLeaseTermExpiraryDate", NewChangeLeaseTermExpiraryDate.HasValue ? "New Lease Change Expiry (Date):" + NewChangeLeaseTermExpiraryDate.Value.ToString("yyyy-MM-dd") : "");
                changeFlag = true;
            }
            else
            {
                printDic.Add("TheChangeOfLeaseTeam", "");
                printDic.Add("OldChangeLeaseTermExpiraryDate", "");
                printDic.Add("NewChangeLeaseTermExpiraryDate", "");
            }

            if (ChangeLandlordType.HasValue && ChangeLandlordType.Value)
            {
                printDic.Add("TheChangeOfLandlord", "The change of Landlord");
                printDic.Add("OldLandlord", "Old Landlord:" + OldLandlord);
                printDic.Add("NewLandlord", "New Landlord:" + NewLandlord);
                changeFlag = true;
            }
            else
            {
                printDic.Add("TheChangeOfLandlord", "");
                printDic.Add("OldLandlord", "");
                printDic.Add("NewLandlord", "");
            }

            if (ChangeOtherType.HasValue && ChangeOtherType.Value)
            {
                printDic.Add("TheChangeOfOthers", "Others");
                printDic.Add("Others", Others);
                changeFlag = true;
            }
            else
            {
                printDic.Add("TheChangeOfOthers", "");
                printDic.Add("Others", "");
            }

            if (changeFlag)
            {
                printDic.Add("LeaseChangeDescription", "Description:" + LeaseChangeDescription);
            }
            else
            {
                printDic.Add("LeaseChangeDescription", "");
            }

            return printDic;
        }

        public override void PrepareTask(TaskWork taskWork)
        {
            switch (taskWork.ActivityName)
            {
                case"Actor Upload":
                    ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_Package_Examine);
                    break;
            }
        }
        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            var packageApprovers =
                                prevApprovers.FirstOrDefault(e => e.FlowCode == WorkflowCode
                                                                && e.RefTableID == Id.ToString());


            foreach (var taskWork in taskWorks.Where(e => e.TypeCode == WorkflowCode).ToList())
            {
                List<ProcessDataField> dataFields = null;
                AppUsers = currApprover;
                dataFields = SetWorkflowDataFields(null);

                if (packageApprovers != null)
                {
                    SimpleEmployee receiver = null;
                    if (taskWork.ReceiverAccount == packageApprovers.MarketMgrCode
                        && currApprover.MarketMgr != null
                        && packageApprovers.MarketMgrCode != currApprover.MarketMgr.Code)
                    {
                        receiver = currApprover.MarketMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RegionalMgrCode
                        && currApprover.RegionalMgr != null
                        && packageApprovers.RegionalMgrCode != currApprover.RegionalMgr.Code)
                    {
                        receiver = currApprover.RegionalMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.MDDCode
                        && currApprover.MDD != null
                         && packageApprovers.MDDCode != currApprover.MDD.Code)
                    {
                        receiver = currApprover.MDD;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.GMCode
                        && currApprover.GM != null
                         && packageApprovers.GMCode != currApprover.GM.Code)
                    {
                        receiver = currApprover.GM;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.FCCode
                        && currApprover.FC != null
                         && packageApprovers.FCCode != currApprover.FC.Code)
                    {
                        receiver = currApprover.FC;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RDDCode
                        && currApprover.RDD != null
                         && packageApprovers.RDDCode != currApprover.RDD.Code)
                    {
                        receiver = currApprover.RDD;
                    } if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
                        && currApprover.VPGM != null
                         && packageApprovers.VPGMCode != currApprover.VPGM.Code)
                    {
                        receiver = currApprover.VPGM;

                    } if (taskWork.ReceiverAccount == packageApprovers.MCCLAssetDtrCode
                        && currApprover.MCCLAssetDtr != null
                         && packageApprovers.MCCLAssetDtrCode != currApprover.MCCLAssetDtr.Code)
                    {
                        receiver = currApprover.MCCLAssetDtr;
                    }

                    if (taskWork.ReceiverAccount == packageApprovers.CDOCode
                        && currApprover.CDO != null
                         && packageApprovers.CDOCode != currApprover.CDO.Code)
                    {
                        receiver = currApprover.CDO;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.CFOCode
                        && currApprover.CFO != null
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
                        catch (Exception e)
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
                }
            }

            if (packageApprovers != null)
            {
                packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                if (packageApprovers.RegionalMgrCode != null && currApprover.RegionalMgr.Code != null)
                    packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                if (currApprover.MDD != null)
                    packageApprovers.MDDCode = currApprover.MDD.Code;
                if (currApprover.GM != null)
                    packageApprovers.GMCode = currApprover.GM.Code;
                if (currApprover.FC != null)
                    packageApprovers.FCCode = currApprover.FC.Code;
                //packageApprovers.RDDCode = currApprover.RDD.Code;
                if (currApprover.VPGM != null)
                    packageApprovers.VPGMCode = currApprover.VPGM.Code;
                if (packageApprovers.MCCLAssetDtrCode != null && currApprover.MCCLAssetDtr.Code != null)
                    packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
                if (packageApprovers.CDOCode != null && currApprover.CDO != null)
                    packageApprovers.CDOCode = currApprover.CDO.Code;
                if (packageApprovers.CFOCode != null && currApprover.CFO != null)
                    packageApprovers.CFOCode = currApprover.CFO.Code;
                if (packageApprovers.MCCLAssetDtrCode != null && currApprover.MCCLAssetDtr != null)
                    packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
            }

        }
    }
}
