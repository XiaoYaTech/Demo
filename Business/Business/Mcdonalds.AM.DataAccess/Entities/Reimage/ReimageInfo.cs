using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.DataModels;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimageInfo : BaseWFEntity<ReimageInfo>
    {

        public List<ProjectUsers> NecessaryNoticeUserList { get; set; }
        public List<ProjectUsers> NoticeUserList { get; set; }
        public ReimageConsInfo ConsInfo { get; set; }

        public bool IsSiteInfoSaveable { get; set; }

        public Guid SiteInfoId { get; set; }

        public void SendRemind()
        {
            Remind.SendRemind(ProjectId, FlowCode.Reimage, GetProjectUserList());
        }

        public int Year { get; set; }

        public EstimatedVsActualConstruction EstimatedVsActualConstruction { get; set; }

        public static ReimageInfo GetReimageInfo(string projectId)
        {
            return Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                var entity = Get(new Guid(id));
                if (entity != null)
                {
                    entity.EntityId = entity.Id;
                }
                return entity;
            }
            else
            {
                var entity = FirstOrDefault(e => e.ProjectId == projectId);
                if (entity != null)
                {
                    entity.EntityId = entity.Id;
                }
                return entity;
            }
        }

        private List<ProjectUsers> GetProjectUserList()
        {
            var usersList = new List<ProjectUsers>();
            var assetActor = ProjectUsers.Get(ProjectUserRoleCode.AssetActor, ProjectUserRoleCode.AssetActor, ProjectUserRoleCode.AssetActor, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, ProjectId);
            assetActor.CreateUserAccount = CreateUserAccount;
            assetActor.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assetActor);

            var assetRep = ProjectUsers.Get(ProjectUserRoleCode.AssetRep, ProjectUserRoleCode.AssetRep, ProjectUserRoleCode.AssetRep, AssetRepAccount, AssetRepNameENUS, AssetRepNameZHCN, ProjectId);
            assetRep.CreateUserAccount = CreateUserAccount;
            assetRep.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assetRep);

            var finance = ProjectUsers.Get(ProjectUserRoleCode.Finance, ProjectUserRoleCode.Finance, ProjectUserRoleCode.Finance, FinanceAccount, FinanceNameENUS, FinanceNameZHCN, ProjectId);
            finance.CreateUserAccount = CreateUserAccount;
            finance.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(finance);

            var legal = ProjectUsers.Get(ProjectUserRoleCode.Legal, ProjectUserRoleCode.Legal, ProjectUserRoleCode.Legal, LegalAccount, LegalNameENUS, LegalNameZHCN, ProjectId);
            legal.CreateUserAccount = CreateUserAccount;
            legal.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(legal);

            var pm = ProjectUsers.Get(ProjectUserRoleCode.PM, ProjectUserRoleCode.PM, ProjectUserRoleCode.PM, PMAccount, PMNameZHCN, PMNameENUS, ProjectId);
            pm.CreateUserAccount = CreateUserAccount;
            pm.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(pm);

            var assertMgr = ProjectUsers.Get(SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ENUS), SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ZHCN), ProjectUserRoleCode.AssetManager, AssetManagerAccount, AssetManagerNameENUS, AssetManagerNameZHCN, ProjectId);
            assertMgr.CreateUserAccount = CreateUserAccount;
            assertMgr.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(assertMgr);

            var cm = ProjectUsers.Get(SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ENUS), SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ZHCN), ProjectUserRoleCode.CM, CMAccount, CMNameENUS, CMNameZHCN, ProjectId);
            cm.CreateUserAccount = CreateUserAccount;
            cm.CreateUserName = CreateUserNameZHCN + "(" + CreateUserNameENUS + ")";
            usersList.Add(cm);

            if (NecessaryNoticeUserList != null && NecessaryNoticeUserList.Count > 0)
            {
                usersList.AddRange(NecessaryNoticeUserList.Select(
                    user => ProjectUsers.Get(ProjectUserRoleCode.View, ProjectUserRoleCode.View, ProjectUserRoleCode.View, user.UserAccount, user.UserNameENUS, user.UserNameZHCN, ProjectId)));
            }
            if (NoticeUserList != null && NoticeUserList.Count > 0)
            {
                usersList.AddRange(NoticeUserList.Select(
                    user => ProjectUsers.Get(ProjectUserRoleCode.View, ProjectUserRoleCode.View, ProjectUserRoleCode.View, user.UserAccount, user.UserNameENUS, user.UserNameZHCN, ProjectId)));
            }
            return usersList;
        }

        public void AddProjectUsers()
        {
            ProjectUsers.Add(GetProjectUserList().ToArray());
        }

        public void CreateAttachmentsMemo()
        {
            //AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.ClosureMemo, FlowCode.Reimage, USCode);
        }
        public void SendWorkTask()
        {
            var listTaskWork = new List<TaskWork>
            {
                GenerateTaskWork(
                    FlowCode.Reimage_ConsInfo,
                    FlowCode.Reimage_ConsInfo,
                    FlowCode.Reimage_ConsInfo,
                    string.Format(@"/Reimage/Main#/ConsInfo?projectId={0}", ProjectId)),
                //GenerateTaskWork(
                //    FlowCode.Reimage_Summary,
                //    FlowCode.Reimage_Summary,
                //    FlowCode.Reimage_Summary,
                //    string.Format(@"/Reimage/Main#/Summary?projectId={0}", ProjectId)),
            };

            TaskWork.Add(listTaskWork.ToArray());
        }

        public TaskWork GenerateTaskWork(string strFlowCode, string strFlowENName, string strFlowCNName, string strUrl)
        {
            var taskWork = new TaskWork
            {
                SourceCode = FlowCode.Reimage,
                SourceNameZHCN = FlowCode.Reimage,
                SourceNameENUS = FlowCode.Reimage,
                Status = 0,
                StatusNameZHCN = "任务",
                StatusNameENUS = "任务"
            };
            var title = TaskWork.BuildTitle(ProjectId, StoreNameZHCN, StoreNameENUS);

            taskWork.Title = title;
            taskWork.RefID = ProjectId;
            taskWork.StoreCode = USCode;

            taskWork.TypeCode = strFlowCode;
            taskWork.TypeNameENUS = strFlowENName;
            taskWork.TypeNameZHCN = strFlowCNName;

            if (strFlowCode == FlowCode.Reimage_ConsInfo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_Summary)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_Package)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_ConsInvtChecking)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_GBMemo)
            {
                taskWork.ReceiverAccount = PMAccount;
                taskWork.ReceiverNameENUS = PMNameENUS;
                taskWork.ReceiverNameZHCN = PMNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_ReopenMemo)
            {
                taskWork.ReceiverAccount = AssetActorAccount;
                taskWork.ReceiverNameENUS = AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = AssetActorNameZHCN;
            }
            else if (strFlowCode == FlowCode.Reimage_TempClosureMemo)
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
            taskWork.ActivityName = NodeCode.Start;

            return taskWork;
        }

        public void CreateSubProject()
        {
            var proj = new ProjectInfo();
            var listProj = new List<ProjectInfo>
            {
                proj.GenerateSubProject(FlowCode.Reimage_ConsInfo,
                    ProjectId, USCode,
                    NodeCode.Start,
                    PMAccount),
                proj.GenerateSubProject(FlowCode.Reimage_Summary,
                    ProjectId, USCode,
                    NodeCode.Start,
                    CreateUserAccount),
                proj.GenerateSubProject(FlowCode.Reimage_Package,
                    ProjectId, USCode,
                    NodeCode.Start,
                    CreateUserAccount),
                proj.GenerateSubProject(FlowCode.Reimage_SiteInfo,
                    ProjectId, USCode,
                    NodeCode.Start,
                    PMAccount),
                proj.GenerateSubProject(FlowCode.Reimage_ConsInvtChecking,
                    ProjectId, USCode,
                    NodeCode.Start,
                    PMAccount),
                proj.GenerateSubProject(FlowCode.Reimage_GBMemo,
                    ProjectId, USCode,
                    NodeCode.Start,
                    PMAccount),
                proj.GenerateSubProject(FlowCode.Reimage_ReopenMemo,
                    ProjectId, USCode,
                    NodeCode.Start,
                    CreateUserAccount),
                proj.GenerateSubProject(FlowCode.Reimage_TempClosureMemo,
                    ProjectId, USCode,
                    NodeCode.Start,
                    CreateUserAccount)
            };

            proj.CreateSubProject(listProj);

            var flowCodeList = new List<string>()
            {
                FlowCode.Reimage_ConsInfo,
                FlowCode.Reimage_Summary,
                FlowCode.Reimage_Package
            };

            foreach (var flowCode in flowCodeList)
            {
                var wfEntity = GetEmptyWorkflowEntity(flowCode);
                wfEntity.GenerateDefaultWorkflowInfo(ProjectId);
            }

        }

        protected override void UpdateProjectTaskUser(TaskWork taskWork, ProjectTeamMembers projectTeamMembers)
        {
            switch (taskWork.TypeCode)
            {
                case FlowCode.Reimage_Summary:
                case FlowCode.Reimage_Package:
                    if (projectTeamMembers.AssetActor != null)
                    {
                        taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                        taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                        taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                    }
                    break;
                case FlowCode.Reimage_ConsInvtChecking:
                case FlowCode.Reimage_ConsInfo:
                    taskWork.ReceiverAccount = projectTeamMembers.PM.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.PM.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.PM.UserNameZHCN;
                    break;

            }
        }

        protected override void ChangeProjectApprover(List<TaskWork> taskWorks, ProjectTeamMembers projectTeamMembers)
        {

            //foreach (var taskWork in taskWorks)
            //{
            List<ProcessDataField> dataFields = null;
            //switch (taskWork.TypeCode)
            //{
            //        case FlowCode.MajorLease_LegalReview:
            //            var legalReview = new MajorLeaseLegalReview();
            //            legalReview.AppUsers = new ApproveUsers()
            //            {
            //                Legal = new SimpleEmployee()
            //                {
            //                    Code = projectTeamMembers.Legal.UserAccount,
            //                    NameENUS = projectTeamMembers.Legal.UserNameENUS,
            //                    NameZHCN = projectTeamMembers.Legal.UserNameZHCN
            //                }
            //            };

            //            dataFields = legalReview.SetWorkflowDataFields(null);

            //            if (projectTeamMembers.Legal.UserAccount != LegalAccount
            //                && taskWork.ReceiverAccount == LegalAccount)
            //            {
            //                RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, projectTeamMembers.Legal.UserAccount,
            //                    dataFields);

            //                taskWork.ReceiverAccount = projectTeamMembers.Legal.UserAccount;
            //                taskWork.ReceiverNameENUS = projectTeamMembers.Legal.UserNameENUS;
            //                taskWork.ReceiverNameZHCN = projectTeamMembers.Legal.UserNameZHCN;

            //            }
            //            else
            //            {
            //                if (taskWork.ProcInstID.HasValue)
            //                {
            //                    UpdateWorkflowDataField(taskWork.ProcInstID.Value, dataFields);
            //                }
            //            }
            //            break;
            //    }
            //}

            //LegalAccount = projectTeamMembers.Legal.UserAccount;
            //LegalNameENUS = projectTeamMembers.Legal.UserNameENUS;
            //LegalNameZHCN = projectTeamMembers.Legal.UserNameZHCN;

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

            //}
        }

    }
}
