using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class MajorLeaseInfo : BaseWFEntity<MajorLeaseInfo>
    {
        public int ReopenedDays { get; set; }
        public DateTime? ConstCompletionDate { get; set; }
        public List<ProjectUsers> NecessaryNoticeUserList { get; set; }
        public List<ProjectUsers> NoticeUserList { get; set; }

        public ProjectContractRevision ProjectContractRevision { get; set; }

        public bool IsContractInfoSaveable { get; set; }
        public bool IsSiteInfoSaveable { get; set; }

        public override string GetDesignStypleForSiteInfo()
        {
            var consInfo = new MajorLeaseConsInfo();
            consInfo = consInfo.GetConsInfo(ProjectId);
            return consInfo.ReinBasicInfo != null ? consInfo.ReinBasicInfo.NewDesignType : null;
        }

        public ProjectContractRevision MappingProjectContractRevision(ProjectContractRevision projectContractRevision)
        {
            if (projectContractRevision != null)
            {
                projectContractRevision.LandlordNew = NewLandlord;
                projectContractRevision.LeaseChangeExpiryNew = NewChangeLeaseTermExpiraryDate;
                projectContractRevision.RedlineAreaNew = NewChangeRedLineRedLineArea.ToString();
                projectContractRevision.RentStructureNew = NewRentalStructure;

                projectContractRevision.Others = Others;
                projectContractRevision.OthersDescription = LeaseChangeDescription;
            }

            return projectContractRevision;
        }
        public void SendWorkTask()
        {
            var listTaskWork = new List<TaskWork>();
            var strURI = "/MajorLease/Main#/LegalReview?projectId=" + ProjectId;
            listTaskWork.Add(GenerateTaskWork(FlowCode.MajorLease_LegalReview, "LegalReview", "LegalReview", strURI));

            strURI = "/MajorLease/Main#/FinanceAnalysis?projectId=" + ProjectId;
            listTaskWork.Add(GenerateTaskWork(FlowCode.MajorLease_FinanceAnalysis, "FinanceAnalysis", "Finance Analysis", strURI));

            if (ChangeRedLineType.HasValue
                && ChangeRedLineType.Value)
            {
                strURI = "/MajorLease/Main#/ConsInfo?projectId=" + ProjectId;
                listTaskWork.Add(GenerateTaskWork(FlowCode.MajorLease_ConsInfo, "ConsInfo", "Cons Info", strURI));
            }

            TaskWork.Add(listTaskWork.ToArray());
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetMajorLeaseInfo(projectId);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public MajorLeaseInfo GetMajorLeaseInfo(string projectId)
        {
            return Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();
        }

        public TaskWork GenerateTaskWork(string strFlowCode, string strFlowENName, string strFlowCNName, string strUrl)
        {
            TaskWork taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.MajorLease;
            taskWork.SourceNameZHCN = FlowCode.MajorLease;
            taskWork.SourceNameENUS = FlowCode.MajorLease;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            var title = TaskWork.BuildTitle(ProjectId, StoreNameZHCN, StoreNameENUS);

            taskWork.Title = title;
            taskWork.RefID = ProjectId;
            taskWork.StoreCode = USCode;

            taskWork.TypeCode = strFlowCode;
            taskWork.TypeNameENUS = strFlowENName;
            taskWork.TypeNameZHCN = strFlowCNName;
            if (strFlowCode == FlowCode.MajorLease_LegalReview)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.MajorLease_FinanceAnalysis)
            {
                taskWork.ReceiverAccount = FinanceAccount;
                taskWork.ReceiverNameENUS = FinanceNameENUS;
                taskWork.ReceiverNameZHCN = FinanceNameZHCN;
            }
            else if (strFlowCode == FlowCode.MajorLease_ConsInfo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.MajorLease_Package)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.MajorLease_ConsInvtChecking)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.MajorLease_GBMemo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            taskWork.Id = Guid.NewGuid();
            taskWork.Url = strUrl;
            taskWork.CreateTime = DateTime.Now;
            taskWork.CreateUserAccount = CreateUserAccount;
            taskWork.Sequence = 0;
            taskWork.ActivityName = "Start";
            return taskWork;
        }

        public void SendRemind()
        {
            Remind.SendRemind(ProjectId, FlowCode.MajorLease, GetProjectUserList());
        }

        public void AddProjectUsers()
        {
            ProjectUsers.Add(GetProjectUserList().ToArray());
        }

        public void Update()
        {
            Update(this);
        }

        public List<ProjectUsers> GetProjectUserList()
        {
            var usersList = new List<ProjectUsers>();
            var assetActor = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ZHCN),
                ProjectUserRoleCode.AssetActor, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, ProjectId);
            assetActor.CreateUserAccount = CreateUserAccount;
            assetActor.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assetActor);

            var assetRep = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ZHCN),
                ProjectUserRoleCode.AssetRep, AssetRepAccount, AssetRepNameENUS, AssetRepNameZHCN, ProjectId);
            assetRep.CreateUserAccount = CreateUserAccount;
            assetRep.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assetRep);

            var finance = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ZHCN),
                ProjectUserRoleCode.Finance, FinanceAccount, FinanceNameENUS, FinanceNameZHCN, ProjectId);
            finance.CreateUserAccount = CreateUserAccount;
            finance.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(finance);

            var legal = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ZHCN),
                ProjectUserRoleCode.Legal, LegalAccount, LegalNameENUS, LegalNameZHCN, ProjectId);
            legal.CreateUserAccount = CreateUserAccount;
            legal.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(legal);

            var pm = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ZHCN),
                ProjectUserRoleCode.PM, PMAccount, PMNameZHCN, PMNameENUS, ProjectId);
            pm.CreateUserAccount = CreateUserAccount;
            pm.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(pm);

            var assertMgr = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ZHCN),
                ProjectUserRoleCode.AssetManager, AssetManagerAccount, AssetManagerNameENUS, AssetManagerNameZHCN, ProjectId);
            assertMgr.CreateUserAccount = CreateUserAccount;
            assertMgr.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assertMgr);

            var cm = ProjectUsers.Get(
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ENUS),
                SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ZHCN),
                ProjectUserRoleCode.CM, CMAccount, CMNameENUS, CMNameZHCN, ProjectId);
            cm.CreateUserAccount = CreateUserAccount;
            cm.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(cm);

            if (NecessaryNoticeUserList != null && NecessaryNoticeUserList.Count > 0)
            {
                usersList.AddRange(NecessaryNoticeUserList.Select(
                    user => ProjectUsers.Get(
                        SystemCode.Instance.GetCodeName(ProjectUserRoleCode.View, SystemLanguage.ENUS),
                        SystemCode.Instance.GetCodeName(ProjectUserRoleCode.View, SystemLanguage.ZHCN),
                        ProjectUserRoleCode.View, user.UserAccount, user.UserNameENUS, user.UserNameZHCN, ProjectId)));
            }
            if (NoticeUserList != null && NoticeUserList.Count > 0)
            {
                usersList.AddRange(NoticeUserList.Select(
                    user => ProjectUsers.Get(
                        SystemCode.Instance.GetCodeName(ProjectUserRoleCode.View, SystemLanguage.ENUS),
                        SystemCode.Instance.GetCodeName(ProjectUserRoleCode.View, SystemLanguage.ZHCN),
                        ProjectUserRoleCode.View, user.UserAccount, user.UserNameENUS, user.UserNameZHCN, ProjectId)));
            }
            return usersList;
        }
        public void CreateSubProject()
        {
            var status = ProjectStatus.UnKnown;
            var proj = new ProjectInfo();

            var listProj = new List<ProjectInfo>();

            if (!(ChangeRedLineType.HasValue && ChangeRedLineType.Value))
            {
                status = ProjectStatus.Finished;
            }

            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_LegalReview, ProjectId, USCode, NodeCode.Start, AssetActorAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_FinanceAnalysis, ProjectId, USCode, NodeCode.Start, FinanceAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_ConsInfo, ProjectId, USCode, NodeCode.Start, PMAccount, status));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_Package, ProjectId, USCode, NodeCode.Start, AssetActorAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_ContractInfo, ProjectId, USCode, NodeCode.Start, CreateUserAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_SiteInfo, ProjectId, USCode, NodeCode.Start, PMAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_ConsInvtChecking, ProjectId, USCode, NodeCode.Start, PMAccount, status));
            listProj.Add(proj.GenerateSubProject(FlowCode.MajorLease_GBMemo, ProjectId, USCode, NodeCode.Start, PMAccount));

            ProjectInfo.Add(listProj.ToArray());


            var flowCodeList = new List<string>()
            {
                FlowCode.MajorLease_LegalReview,
                FlowCode.MajorLease_FinanceAnalysis,
                FlowCode.MajorLease_ConsInfo
            };
            foreach (var flowCode in flowCodeList)
            {
                var wfEntity = GetEmptyWorkflowEntity(flowCode);
                wfEntity.GenerateDefaultWorkflowInfo(ProjectId);
            }

            var projectContractInfo = ProjectContractInfo.GetContractWithHistory(ProjectId).Current;
            projectContractInfo.Add();
        }

        public void CreateAttachmentsMemo()
        {
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.ClosureMemo, FlowCode.MajorLease, USCode);
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.GBMemo, FlowCode.MajorLease, USCode);
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.ReopenMemo, FlowCode.MajorLease, USCode);
        }

        protected override void UpdateProjectTaskUser(TaskWork taskWork, ProjectTeamMembers projectTeamMembers)
        {
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.FlowCode == taskWork.TypeCode
                                                              && e.ProjectId == taskWork.RefID);

            if (projectInfo != null)
            {
                switch (taskWork.TypeCode)
                {
                    case FlowCode.MajorLease_Package:
                    case FlowCode.MajorLease_LegalReview:
                        if (projectTeamMembers.AssetActor != null)
                        {
                            taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                            taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                            taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;

                            projectInfo.CreateUserAccount = projectTeamMembers.AssetActor.UserAccount;
                        }
                        break;
                    case FlowCode.MajorLease_FinanceAnalysis:
                        taskWork.ReceiverAccount = projectTeamMembers.Finance.UserAccount;
                        taskWork.ReceiverNameENUS = projectTeamMembers.Finance.UserNameENUS;
                        taskWork.ReceiverNameZHCN = projectTeamMembers.Finance.UserNameZHCN;

                        projectInfo.CreateUserAccount = projectTeamMembers.Finance.UserAccount;
                        break;
                    case FlowCode.MajorLease_ConsInvtChecking:
                    case FlowCode.MajorLease_ConsInfo:
                        taskWork.ReceiverAccount = projectTeamMembers.PM.UserAccount;
                        taskWork.ReceiverNameENUS = projectTeamMembers.PM.UserNameENUS;
                        taskWork.ReceiverNameZHCN = projectTeamMembers.PM.UserNameZHCN;

                        projectInfo.CreateUserAccount = projectTeamMembers.PM.UserAccount;
                        break;

                }

                ProjectInfo.Update(projectInfo);
            }

        }

        protected override void ChangeProjectApprover(List<TaskWork> taskWorks, ProjectTeamMembers projectTeamMembers)
        {

            foreach (var taskWork in taskWorks)
            {
                var work = taskWork;
                var approveDialogUser =
                    ApproveDialogUser.FirstOrDefault(
                        e => e.RefTableID == work.RefTableId.ToString() && e.FlowCode == work.TypeCode);

                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.MajorLease_LegalReview:
                        var legalReview = new MajorLeaseLegalReview();
                        legalReview.AppUsers = new ApproveUsers()
                        {
                            Legal = new SimpleEmployee()
                        {
                            Code = projectTeamMembers.Legal.UserAccount,
                            NameENUS = projectTeamMembers.Legal.UserNameENUS,
                            NameZHCN = projectTeamMembers.Legal.UserNameZHCN
                        }
                        };

                        dataFields = legalReview.SetWorkflowDataFields(null);

                        if (projectTeamMembers.Legal.UserAccount != LegalAccount
                            && taskWork.ReceiverAccount == LegalAccount)
                        {
                            RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, projectTeamMembers.Legal.UserAccount,
                                dataFields);

                            taskWork.ReceiverAccount = projectTeamMembers.Legal.UserAccount;
                            taskWork.ReceiverNameENUS = projectTeamMembers.Legal.UserNameENUS;
                            taskWork.ReceiverNameZHCN = projectTeamMembers.Legal.UserNameZHCN;

                        }
                        else
                        {
                            if (taskWork.ProcInstID.HasValue)
                            {
                                UpdateWorkflowDataField(taskWork.ProcInstID.Value, dataFields);
                            }
                        }
                        if (approveDialogUser != null)
                        {
                            approveDialogUser.LegalCode = projectTeamMembers.Legal.UserAccount;
                            ApproveDialogUser.Update(approveDialogUser);
                        }
                        break;
                }
            }

            LegalAccount = projectTeamMembers.Legal.UserAccount;
            LegalNameENUS = projectTeamMembers.Legal.UserNameENUS;
            LegalNameZHCN = projectTeamMembers.Legal.UserNameZHCN;

            if (projectTeamMembers.AssetActor != null)
            {
                AssetActorAccount = projectTeamMembers.AssetActor.UserAccount;
                AssetActorNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                AssetActorNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
            }

            AssetRepAccount = projectTeamMembers.AssetRep.UserAccount;
            AssetRepNameENUS = projectTeamMembers.AssetRep.UserNameENUS;
            AssetRepNameZHCN = projectTeamMembers.AssetRep.UserNameZHCN;

            FinanceAccount = projectTeamMembers.Finance.UserAccount;
            FinanceNameENUS = projectTeamMembers.Finance.UserNameENUS;
            FinanceNameZHCN = projectTeamMembers.Finance.UserNameZHCN;

            PMAccount = projectTeamMembers.PM.UserAccount;
            PMNameENUS = projectTeamMembers.PM.UserNameENUS;
            PMNameZHCN = projectTeamMembers.PM.UserNameZHCN;

            Update(this);

        }

        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {

            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.MajorLease_Package:
                        var package = new MajorLeaseChangePackage();
                        package.AppUsers = currApprover;
                        dataFields = package.SetWorkflowDataFields(null);
                        var packageApprovers =
                            prevApprovers.FirstOrDefault(e => e.FlowCode == FlowCode.MajorLease_Package);
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
                                RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, receiver.Code, dataFields);

                                taskWork.ReceiverAccount = receiver.Code;
                                taskWork.ReceiverNameENUS = receiver.NameENUS;
                                taskWork.ReceiverNameZHCN = receiver.NameZHCN;
                            }

                            packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                            packageApprovers.MarketMgrCode = currApprover.RegionalMgr.Code;
                            packageApprovers.MarketMgrCode = currApprover.DD.Code;
                            packageApprovers.MarketMgrCode = currApprover.GM.Code;
                            packageApprovers.MarketMgrCode = currApprover.FC.Code;
                            packageApprovers.MarketMgrCode = currApprover.RDD.Code;
                            packageApprovers.MarketMgrCode = currApprover.VPGM.Code;
                            packageApprovers.MarketMgrCode = currApprover.MCCLAssetDtr.Code;
                            packageApprovers.MarketMgrCode = currApprover.CDO.Code;
                            packageApprovers.MarketMgrCode = currApprover.CFO.Code;
                        }


                        break;
                }
            }
        }
    }
}
