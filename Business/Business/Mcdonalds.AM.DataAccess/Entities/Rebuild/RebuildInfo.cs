using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.Infrastructure;


namespace Mcdonalds.AM.DataAccess
{
    public partial class RebuildInfo : BaseWFEntity<RebuildInfo>
    {
        public List<ProjectUsers> NecessaryNoticeUserList { get; set; }
        public List<ProjectUsers> NoticeUserList { get; set; }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetRebuildInfo(projectId);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        protected override void UpdateProjectTaskUser(TaskWork taskWork, ProjectTeamMembers projectTeamMembers)
        {
            switch (taskWork.TypeCode)
            {
                case FlowCode.Rebuild_Package:
                case FlowCode.Rebuild_LegalReview:
                    if (projectTeamMembers.AssetActor != null)
                    {
                        taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                        taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                        taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                    }
                    break;
                case FlowCode.Rebuild_FinanceAnalysis:
                    taskWork.ReceiverAccount = projectTeamMembers.Finance.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.Finance.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.Finance.UserNameZHCN;
                    break;
                case FlowCode.Rebuild_ConsInvtChecking:
                case FlowCode.Rebuild_ConsInfo:
                    taskWork.ReceiverAccount = projectTeamMembers.PM.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.PM.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.PM.UserNameZHCN;
                    break;

            }
        }
        protected override void ChangeProjectApprover(List<TaskWork> taskWorks, ProjectTeamMembers projectTeamMembers)
        {

            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.Rebuild_LegalReview:
                        var legalReview = new RebuildLegalReview();
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
            RebuildPackage packageInfo = null;
            if (taskWorks.Count > 0)
            {
                string refID = taskWorks[0].RefID;
                packageInfo = RebuildPackage.FirstOrDefault(i => i.ProjectId == refID && i.IsHistory == false);
            }
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.Rebuild_Package:
                        var package = new RebuildPackage();
                        package.AppUsers = currApprover;
                        dataFields = package.SetWorkflowDataFields(null);
                        ApproveDialogUser packageApprovers = null;
                        if (packageInfo == null)
                            packageApprovers = prevApprovers.FirstOrDefault(e => e.FlowCode == FlowCode.Rebuild_Package);
                        else
                            packageApprovers = prevApprovers.FirstOrDefault(e => e.RefTableID == packageInfo.Id.ToString());
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
                            //if (taskWork.ReceiverAccount == packageApprovers.RDDCode
                            //     && packageApprovers.RDDCode != currApprover.RDD.Code)
                            //{
                            //    receiver = currApprover.RDD;
                            //} 
                            if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
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

                            if (currApprover.MarketMgr != null)
                                packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                            if (currApprover.RegionalMgr != null)
                                packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                            if (currApprover.DD != null)
                                packageApprovers.DDCode = currApprover.DD.Code;
                            if (currApprover.MDD != null)
                                packageApprovers.MDDCode = currApprover.MDD.Code;
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
                            packageApprovers.NoticeUsers = "";
                            foreach (var noticeUser in currApprover.NoticeUsers)
                            {
                                if (string.IsNullOrEmpty(packageApprovers.NoticeUsers))
                                    packageApprovers.NoticeUsers = noticeUser.Code;
                                else
                                    packageApprovers.NoticeUsers += ";" + noticeUser.Code;
                            }
                            packageApprovers.NecessaryNoticeUsers = "";
                            foreach (var noticeUser in currApprover.NecessaryNoticeUsers)
                            {
                                if (string.IsNullOrEmpty(packageApprovers.NecessaryNoticeUsers))
                                    packageApprovers.NecessaryNoticeUsers = noticeUser.Code;
                                else
                                    packageApprovers.NecessaryNoticeUsers += ";" + noticeUser.Code;
                            }
                            packageApprovers.Update();
                        }


                        break;
                }
            }
        }
        public RebuildInfo GetRebuildInfo(string projectId)
        {
            return Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();
        }
        public TaskWork GenerateTaskWork(string strFlowCode, string strFlowENName, string strFlowCNName, string strUrl)
        {
            TaskWork taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Rebuild;
            taskWork.SourceNameZHCN = FlowCode.Rebuild;
            taskWork.SourceNameENUS = FlowCode.Rebuild;
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
            if (strFlowCode == FlowCode.Rebuild_LegalReview)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_FinanceAnalysis)
            {
                taskWork.ReceiverAccount = FinanceAccount;
                taskWork.ReceiverNameENUS = FinanceNameENUS;
                taskWork.ReceiverNameZHCN = FinanceNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_ConsInfo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_Package)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_ConsInvtChecking)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_GBMemo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_ReopenMemo)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Rebuild_TempClosureMemo)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
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
            Remind.SendRemind(ProjectId, FlowCode.Rebuild, GetProjectUserList());
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
            var proj = new ProjectInfo();

            var listProj = new List<ProjectInfo>();

            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_LegalReview, ProjectId, USCode, NodeCode.Start, AssetActorAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_FinanceAnalysis, ProjectId, USCode, NodeCode.Start, FinanceAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_ConsInfo, ProjectId, USCode, NodeCode.Start, PMAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_Package, ProjectId, USCode, NodeCode.Start, AssetActorAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_ContractInfo, ProjectId, USCode, NodeCode.Start, CreateUserAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_SiteInfo, ProjectId, USCode, NodeCode.Start, PMAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_ConsInvtChecking, ProjectId, USCode, NodeCode.Start, PMAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_TempClosureMemo, ProjectId, USCode, NodeCode.Start, CreateUserAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_GBMemo, ProjectId, USCode, NodeCode.Start, PMAccount));
            listProj.Add(proj.GenerateSubProject(FlowCode.Rebuild_ReopenMemo, ProjectId, USCode, NodeCode.Start, CreateUserAccount));

            ProjectInfo.Add(listProj.ToArray());


            var flowCodeList = new List<string>()
            {
                FlowCode.Rebuild_LegalReview,
                FlowCode.Rebuild_FinanceAnalysis,
                FlowCode.Rebuild_ConsInfo
            };
            foreach (var flowCode in flowCodeList)
            {
                var wfEntity = GetEmptyWorkflowEntity(flowCode);
                wfEntity.GenerateDefaultWorkflowInfo(ProjectId);
            }

            var projectContractInfo = ProjectContractInfo.GetContractWithHistory(ProjectId).Current;
            projectContractInfo.Add();
        }
        public void SendWorkTask()
        {
            List<TaskWork> listTaskWork = new List<TaskWork>();
            string strURI = "/Rebuild/Main#/LegalReview?projectId=" + ProjectId;
            listTaskWork.Add(GenerateTaskWork(FlowCode.Rebuild_LegalReview, "LegalReview", "Legal Review", strURI));

            strURI = "/Rebuild/Main#/FinanceAnalysis?projectId=" + ProjectId;
            listTaskWork.Add(GenerateTaskWork(FlowCode.Rebuild_FinanceAnalysis, "FinanceAnalysis", "Finance Analysis", strURI));

            strURI = "/Rebuild/Main#/ConsInfo?projectId=" + ProjectId;
            listTaskWork.Add(GenerateTaskWork(FlowCode.Rebuild_ConsInfo, "ConsInfo", "Cons Info", strURI));
            TaskWork.Add(listTaskWork.ToArray());
        }
        public void UpdateFromProjectList()
        {
            var rbdInfo = GetRebuildInfo(ProjectId);
            if (rbdInfo != null)
            {
                rbdInfo.TempClosureDate = TempClosureDate;
                rbdInfo.GBDate = GBDate;
                rbdInfo.ReopenDate = ReopenDate;
                rbdInfo.ConstCompletionDate = ConstCompletionDate;
                rbdInfo.Update();
            }
        }
    }
}
