using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/7/2014 12:18:42 PM
 * FileName     :   TempClosureInfo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class TempClosureInfo : BaseWFEntity<TempClosureInfo>
    {

        public static void Create(PostCreateWorkflow<TempClosureInfo> tempClosure)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var store = StoreBasicInfo.GetStorInfo(tempClosure.Entity.USCode);
                tempClosure.Entity.Id = Guid.NewGuid();
                tempClosure.Entity.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Entity.CreateTime = DateTime.Now;

                tempClosure.Entity.StoreNameENUS = store.NameENUS;
                tempClosure.Entity.StoreNameZHCN = store.NameZHCN;

                tempClosure.Entity.AssetRepAccount = tempClosure.Team.AssetRep.UserAccount;
                tempClosure.Entity.AssetRepNameENUS = tempClosure.Team.AssetRep.UserNameENUS;
                tempClosure.Entity.AssetRepNameZHCN = tempClosure.Team.AssetRep.UserNameZHCN;

                tempClosure.Entity.AssetActorAccount = tempClosure.Team.AssetActor.UserAccount;
                tempClosure.Entity.AssetActorNameENUS = tempClosure.Team.AssetActor.UserNameENUS;
                tempClosure.Entity.AssetActorNameZHCN = tempClosure.Team.AssetActor.UserNameZHCN;

                tempClosure.Entity.FinanceAccount = tempClosure.Team.Finance.UserAccount;
                tempClosure.Entity.FinanceNameENUS = tempClosure.Team.Finance.UserNameENUS;
                tempClosure.Entity.FinanceNameZHCN = tempClosure.Team.Finance.UserNameZHCN;

                tempClosure.Entity.PMAccount = tempClosure.Team.PM.UserAccount;
                tempClosure.Entity.PMNameENUS = tempClosure.Team.PM.UserNameENUS;
                tempClosure.Entity.PMNameZHCN = tempClosure.Team.PM.UserNameZHCN;

                tempClosure.Entity.LegalAccount = tempClosure.Team.Legal.UserAccount;
                tempClosure.Entity.LegalNameENUS = tempClosure.Team.Legal.UserNameENUS;
                tempClosure.Entity.LegalNameZHCN = tempClosure.Team.Legal.UserNameZHCN;

                tempClosure.Entity.AssetManagerAccount = tempClosure.Team.AssetMgr.UserAccount;
                tempClosure.Entity.AssetManagerNameENUS = tempClosure.Team.AssetMgr.UserNameENUS;
                tempClosure.Entity.AssetManagerNameZHCN = tempClosure.Team.AssetMgr.UserNameZHCN;

                tempClosure.Entity.CMAccount = tempClosure.Team.CM.UserAccount;
                tempClosure.Entity.CMNameENUS = tempClosure.Team.CM.UserNameENUS;
                tempClosure.Entity.CMNameZHCN = tempClosure.Team.CM.UserNameZHCN;

                var projectId = ProjectInfo.CreateMainProject(FlowCode.TempClosure, tempClosure.Entity.USCode, NodeCode.Start, tempClosure.Entity.CreateUserAccount);
                tempClosure.Entity.ProjectId = projectId;
                Add(tempClosure.Entity);

                List<ProjectUsers> projectUsers = new List<ProjectUsers>();

                tempClosure.Team.AssetRep.Id = Guid.NewGuid();
                tempClosure.Team.AssetRep.ProjectId = projectId;
                tempClosure.Team.AssetRep.CreateDate = DateTime.Now;
                tempClosure.Team.AssetRep.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.AssetRep.RoleCode = ProjectUserRoleCode.AssetRep;
                tempClosure.Team.AssetRep.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ENUS);
                tempClosure.Team.AssetRep.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.AssetRep);

                tempClosure.Team.AssetActor.Id = Guid.NewGuid();
                tempClosure.Team.AssetActor.ProjectId = projectId;
                tempClosure.Team.AssetActor.CreateDate = DateTime.Now;
                tempClosure.Team.AssetActor.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.AssetActor.RoleCode = ProjectUserRoleCode.AssetActor;
                tempClosure.Team.AssetActor.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ENUS);
                tempClosure.Team.AssetActor.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.AssetActor);

                tempClosure.Team.Finance.Id = Guid.NewGuid();
                tempClosure.Team.Finance.ProjectId = projectId;
                tempClosure.Team.Finance.CreateDate = DateTime.Now;
                tempClosure.Team.Finance.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.Finance.RoleCode = ProjectUserRoleCode.Finance;
                tempClosure.Team.Finance.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ENUS);
                tempClosure.Team.Finance.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.Finance);

                tempClosure.Team.PM.Id = Guid.NewGuid();
                tempClosure.Team.PM.ProjectId = projectId;
                tempClosure.Team.PM.CreateDate = DateTime.Now;
                tempClosure.Team.PM.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.PM.RoleCode = ProjectUserRoleCode.PM;
                tempClosure.Team.PM.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ENUS);
                tempClosure.Team.PM.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.PM);

                tempClosure.Team.Legal.Id = Guid.NewGuid();
                tempClosure.Team.Legal.ProjectId = projectId;
                tempClosure.Team.Legal.CreateDate = DateTime.Now;
                tempClosure.Team.Legal.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.Legal.RoleCode = ProjectUserRoleCode.Legal;
                tempClosure.Team.Legal.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ENUS);
                tempClosure.Team.Legal.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.Legal);

                tempClosure.Team.AssetMgr.Id = Guid.NewGuid();
                tempClosure.Team.AssetMgr.ProjectId = projectId;
                tempClosure.Team.AssetMgr.CreateDate = DateTime.Now;
                tempClosure.Team.AssetMgr.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.AssetMgr.RoleCode = ProjectUserRoleCode.AssetManager;
                tempClosure.Team.AssetMgr.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ENUS);
                tempClosure.Team.AssetMgr.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.AssetMgr);

                tempClosure.Team.CM.Id = Guid.NewGuid();
                tempClosure.Team.CM.ProjectId = projectId;
                tempClosure.Team.CM.CreateDate = DateTime.Now;
                tempClosure.Team.CM.CreateUserAccount = ClientCookie.UserCode;
                tempClosure.Team.CM.RoleCode = ProjectUserRoleCode.CM;
                tempClosure.Team.CM.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ENUS);
                tempClosure.Team.CM.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ZHCN);
                projectUsers.Add(tempClosure.Team.CM);
                ProjectUsers.Add(projectUsers.ToArray());

                tempClosure.Viewers.ForEach(v =>
                {
                    v.Id = Guid.NewGuid();
                    v.ProjectId = projectId;
                    v.CreateDate = DateTime.Now;
                    v.CreateUserAccount = ClientCookie.UserCode;
                    v.RoleCode = ProjectUserRoleCode.View;
                });
                ProjectUsers.Add(tempClosure.Viewers.ToArray());
                //tempClosure.NecessaryViewers.ForEach(v =>
                //{
                //    v.Id = Guid.NewGuid();
                //    v.ProjectId = projectId;
                //    v.CreateDate = DateTime.Now;
                //    v.CreateUserAccount = ClientCookie.UserCode;
                //    v.RoleCode = ProjectUserRoleCode.View;
                //});
                //ProjectUsers.Add(tempClosure.NecessaryViewers.ToArray());

                Remind.SendRemind(projectId, FlowCode.TempClosure, projectUsers);
                ProjectInfo.CreateSubProject(FlowCode.TempClosure_LegalReview, projectId, tempClosure.Entity.USCode, NodeCode.Start, ClientCookie.UserCode);
                TempClosureLegalReview.Create(projectId);
                ProjectInfo.CreateSubProject(FlowCode.TempClosure_ClosurePackage, projectId, tempClosure.Entity.USCode, NodeCode.Start, ClientCookie.UserCode);
                TempClosurePackage.Create(projectId);
                ProjectInfo.CreateSubProject(FlowCode.TempClosure_ClosureMemo, projectId, tempClosure.Entity.USCode, NodeCode.Start, ClientCookie.UserCode);
                ProjectInfo.CreateSubProject(FlowCode.TempClosure_ReopenMemo, projectId, tempClosure.Entity.USCode, NodeCode.Start, ClientCookie.UserCode);

                string taskUrl = TaskWork.BuildUrl(FlowCode.TempClosure_LegalReview, projectId, "");
                string title = TaskWork.BuildTitle(projectId, store.NameZHCN, store.NameENUS);
                TaskWork.SendTask(projectId, title, store.StoreCode, taskUrl, tempClosure.Team.AssetActor, FlowCode.TempClosure, FlowCode.TempClosure_LegalReview, "Start");
                ProjectNode.GenerateOnCreate(FlowCode.TempClosure, projectId);
                tranScope.Complete();
            }
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

        public static TempClosureInfo Get(string projectId, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(new Guid(id));
            }
            else
            {
                var contraInfo = ProjectContractInfo.Get(projectId);
                var tempClosureInfo = FirstOrDefault(r => r.ProjectId == projectId);
                if (tempClosureInfo != null)
                {
                    tempClosureInfo.LandlordName = contraInfo.PartyAFullName;
                    if (contraInfo.EndDate.HasValue)
                    {
                        tempClosureInfo.LeaseExpireDate = contraInfo.EndDate.Value;
                    }
                }
                return tempClosureInfo;
            }
        }


        protected override void ChangeProjectApprover(List<TaskWork> taskWorks, ProjectTeamMembers projectTeamMembers)
        {
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.TempClosure_LegalReview:
                        dataFields = new List<ProcessDataField>
                        {
                            new ProcessDataField("dest_Legal", projectTeamMembers.Legal.UserAccount)
                        };
                        if (projectTeamMembers.Legal.UserAccount != LegalAccount
                            && taskWork.ReceiverAccount == LegalAccount)
                        {
                            RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, projectTeamMembers.Legal.UserAccount, dataFields);

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

        protected override void UpdateProjectTaskUser(TaskWork taskWork, ProjectTeamMembers projectTeamMembers)
        {
            switch (taskWork.TypeCode)
            {
                case FlowCode.TempClosure_LegalReview:
                case FlowCode.TempClosure_ClosurePackage:
                case FlowCode.TempClosure_ClosureMemo:
                case FlowCode.TempClosure_ReopenMemo:
                    taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                    break;
            }
        }


        protected override void UpdateProjectCreateUser(ProjectTeamMembers projectTeamMembers)
        {
            var projectInfo = ProjectInfo.Search(i => i.ProjectId == this.ProjectId && i.FlowCode.Contains(FlowCode.TempClosure));
            foreach (var projectInfoItem in projectInfo)
            {
                switch (projectInfoItem.FlowCode)
                {
                    case FlowCode.TempClosure:
                    case FlowCode.TempClosure_LegalReview:
                    case FlowCode.TempClosure_ClosurePackage:
                    case FlowCode.TempClosure_ClosureMemo:
                    case FlowCode.TempClosure_ReopenMemo:
                        projectInfoItem.CreateUserAccount = projectTeamMembers.AssetActor.UserAccount;
                        break;
                }
            }
            ProjectInfo.Update(projectInfo.ToArray());
        }
    }
}
