using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/24/2014 11:15:10 AM
 * FileName     :   RenewalInfo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalInfo : BaseWFEntity<RenewalInfo>
    {
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = Get(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        public static RenewalInfo Get(string projectId, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(Guid.Parse(id));
            }
            else
            {
                return FirstOrDefault(r => r.ProjectId == projectId);
            }
        }
        public static void Create(PostCreateWorkflow<RenewalInfo> postCreateRenewal)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var usCode = postCreateRenewal.Entity.USCode;
                var store = StoreBasicInfo.GetStorInfo(usCode);
                var projectId = ProjectInfo.CreateMainProject(FlowCode.Renewal, usCode, NodeCode.Start, ClientCookie.UserCode);
                postCreateRenewal.Entity.Id = Guid.NewGuid();
                postCreateRenewal.Entity.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Entity.CreateTime = DateTime.Now;
                postCreateRenewal.Entity.StoreNameENUS = store.NameENUS;
                postCreateRenewal.Entity.StoreNameZHCN = store.NameZHCN;

                postCreateRenewal.Entity.AssetRepAccount = postCreateRenewal.Team.AssetRep.UserAccount;
                postCreateRenewal.Entity.AssetRepNameENUS = postCreateRenewal.Team.AssetRep.UserNameENUS;
                postCreateRenewal.Entity.AssetRepNameZHCN = postCreateRenewal.Team.AssetRep.UserNameZHCN;

                postCreateRenewal.Entity.AssetActorAccount = postCreateRenewal.Team.AssetActor.UserAccount;
                postCreateRenewal.Entity.AssetActorNameENUS = postCreateRenewal.Team.AssetActor.UserNameENUS;
                postCreateRenewal.Entity.AssetActorNameZHCN = postCreateRenewal.Team.AssetActor.UserNameZHCN;

                postCreateRenewal.Entity.FinanceAccount = postCreateRenewal.Team.Finance.UserAccount;
                postCreateRenewal.Entity.FinanceNameENUS = postCreateRenewal.Team.Finance.UserNameENUS;
                postCreateRenewal.Entity.FinanceNameZHCN = postCreateRenewal.Team.Finance.UserNameZHCN;

                postCreateRenewal.Entity.PMAccount = postCreateRenewal.Team.PM.UserAccount;
                postCreateRenewal.Entity.PMNameENUS = postCreateRenewal.Team.PM.UserNameENUS;
                postCreateRenewal.Entity.PMNameZHCN = postCreateRenewal.Team.PM.UserNameZHCN;

                postCreateRenewal.Entity.LegalAccount = postCreateRenewal.Team.Legal.UserAccount;
                postCreateRenewal.Entity.LegalNameENUS = postCreateRenewal.Team.Legal.UserNameENUS;
                postCreateRenewal.Entity.LegalNameZHCN = postCreateRenewal.Team.Legal.UserNameZHCN;

                postCreateRenewal.Entity.AssetManagerAccount = postCreateRenewal.Team.AssetMgr.UserAccount;
                postCreateRenewal.Entity.AssetManagerNameENUS = postCreateRenewal.Team.AssetMgr.UserNameENUS;
                postCreateRenewal.Entity.AssetManagerNameZHCN = postCreateRenewal.Team.AssetMgr.UserNameZHCN;

                postCreateRenewal.Entity.CMAccount = postCreateRenewal.Team.CM.UserAccount;
                postCreateRenewal.Entity.CMNameENUS = postCreateRenewal.Team.CM.UserNameENUS;
                postCreateRenewal.Entity.CMNameZHCN = postCreateRenewal.Team.CM.UserNameZHCN;

                postCreateRenewal.Entity.ProjectId = projectId;
                postCreateRenewal.Entity.Add();
                ProjectInfo.CreateSubProject(FlowCode.Renewal_Letter, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalLetter.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_LLNegotiation, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalLLNegotiation.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_ConsInfo, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.PM.UserAccount);
                RenewalConsInfo.Create(projectId, postCreateRenewal.Team.PM.UserAccount, postCreateRenewal.Entity.NeedProjectCostEst);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_Tool, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.Finance.UserAccount);
                var tool = RenewalTool.Create(projectId, postCreateRenewal.Team.Finance.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_ClearanceReport, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalClearanceReport.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_ConfirmLetter, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalConfirmLetter.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_Analysis, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                var analysis = RenewalAnalysis.Create(postCreateRenewal.Entity);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_LegalApproval, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalLegalApproval.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_Package, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalPackage.Create(projectId, postCreateRenewal.Team.AssetActor.UserAccount, analysis.Id, tool.Id);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_ContractInfo, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.AssetActor.UserAccount);
                RenewalContractInfo.Create(projectId);
                ProjectInfo.CreateSubProject(FlowCode.Renewal_SiteInfo, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.PM.UserAccount);
                RenewalSiteInfo.Create(projectId, postCreateRenewal.Team.PM.UserAccount);

                ProjectInfo.CreateSubProject(FlowCode.Renewal_GBMemo, projectId, usCode, NodeCode.Start, postCreateRenewal.Team.PM.UserAccount);

                List<ProjectUsers> projectUsers = new List<ProjectUsers>();

                postCreateRenewal.Team.AssetRep.Id = Guid.NewGuid();
                postCreateRenewal.Team.AssetRep.ProjectId = projectId;
                postCreateRenewal.Team.AssetRep.CreateDate = DateTime.Now;
                postCreateRenewal.Team.AssetRep.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.AssetRep.RoleCode = ProjectUserRoleCode.AssetRep;
                postCreateRenewal.Team.AssetRep.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ENUS);
                postCreateRenewal.Team.AssetRep.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetRep, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.AssetRep);

                postCreateRenewal.Team.AssetActor.Id = Guid.NewGuid();
                postCreateRenewal.Team.AssetActor.ProjectId = projectId;
                postCreateRenewal.Team.AssetActor.CreateDate = DateTime.Now;
                postCreateRenewal.Team.AssetActor.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.AssetActor.RoleCode = ProjectUserRoleCode.AssetActor;
                postCreateRenewal.Team.AssetActor.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ENUS);
                postCreateRenewal.Team.AssetActor.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetActor, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.AssetActor);

                postCreateRenewal.Team.Finance.Id = Guid.NewGuid();
                postCreateRenewal.Team.Finance.ProjectId = projectId;
                postCreateRenewal.Team.Finance.CreateDate = DateTime.Now;
                postCreateRenewal.Team.Finance.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.Finance.RoleCode = ProjectUserRoleCode.Finance;
                postCreateRenewal.Team.Finance.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ENUS);
                postCreateRenewal.Team.Finance.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Finance, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.Finance);

                postCreateRenewal.Team.PM.Id = Guid.NewGuid();
                postCreateRenewal.Team.PM.ProjectId = projectId;
                postCreateRenewal.Team.PM.CreateDate = DateTime.Now;
                postCreateRenewal.Team.PM.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.PM.RoleCode = ProjectUserRoleCode.PM;
                postCreateRenewal.Team.PM.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ENUS);
                postCreateRenewal.Team.PM.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.PM, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.PM);

                postCreateRenewal.Team.Legal.Id = Guid.NewGuid();
                postCreateRenewal.Team.Legal.ProjectId = projectId;
                postCreateRenewal.Team.Legal.CreateDate = DateTime.Now;
                postCreateRenewal.Team.Legal.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.Legal.RoleCode = ProjectUserRoleCode.Legal;
                postCreateRenewal.Team.Legal.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ENUS);
                postCreateRenewal.Team.Legal.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.Legal, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.Legal);

                postCreateRenewal.Team.AssetMgr.Id = Guid.NewGuid();
                postCreateRenewal.Team.AssetMgr.ProjectId = projectId;
                postCreateRenewal.Team.AssetMgr.CreateDate = DateTime.Now;
                postCreateRenewal.Team.AssetMgr.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.AssetMgr.RoleCode = ProjectUserRoleCode.AssetManager;
                postCreateRenewal.Team.AssetMgr.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ENUS);
                postCreateRenewal.Team.AssetMgr.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.AssetManager, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.AssetMgr);

                postCreateRenewal.Team.CM.Id = Guid.NewGuid();
                postCreateRenewal.Team.CM.ProjectId = projectId;
                postCreateRenewal.Team.CM.CreateDate = DateTime.Now;
                postCreateRenewal.Team.CM.CreateUserAccount = ClientCookie.UserCode;
                postCreateRenewal.Team.CM.RoleCode = ProjectUserRoleCode.CM;
                postCreateRenewal.Team.CM.RoleNameENUS = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ENUS);
                postCreateRenewal.Team.CM.RoleNameZHCN = SystemCode.Instance.GetCodeName(ProjectUserRoleCode.CM, SystemLanguage.ZHCN);
                projectUsers.Add(postCreateRenewal.Team.CM);

                ProjectUsers.Add(projectUsers.ToArray());

                postCreateRenewal.Viewers.ForEach(v =>
                {
                    v.Id = Guid.NewGuid();
                    v.ProjectId = projectId;
                    v.CreateDate = DateTime.Now;
                    v.CreateUserAccount = ClientCookie.UserCode;
                    v.RoleCode = ProjectUserRoleCode.View;
                });
                ProjectUsers.Add(postCreateRenewal.Viewers.ToArray());
                //postCreateRenewal.NecessaryViewers.ForEach(v =>
                //{
                //    v.Id = Guid.NewGuid();
                //    v.ProjectId = projectId;
                //    v.CreateDate = DateTime.Now;
                //    v.CreateUserAccount = ClientCookie.UserCode;
                //    v.RoleCode = ProjectUserRoleCode.View;
                //});
                //ProjectUsers.Add(postCreateRenewal.NecessaryViewers.ToArray());
                Remind.SendRemind(projectId, FlowCode.Renewal, projectUsers);
                Remind.SendRemind(projectId, FlowCode.Renewal, postCreateRenewal.Viewers);
                postCreateRenewal.Entity.GenerateSubmitTask(FlowCode.Renewal_Letter);
                postCreateRenewal.Entity.GenerateSubmitTask(FlowCode.Renewal_LLNegotiation);
                postCreateRenewal.Entity.CreateAttachmentsMemo();
                ProjectNode.GenerateOnCreate(FlowCode.Renewal, projectId);
                ProjectProgress.SetProgress(projectId, "10%");
                tranScope.Complete();
            }
        }
        public void CreateAttachmentsMemo()
        {
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.ClosureMemo, FlowCode.Renewal, USCode);
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.GBMemo, FlowCode.Renewal, USCode);
            AttachmentsMemoProcessInfo.CreateAttachmentsMemoProcessInfo(ProjectId, FlowCode.ReopenMemo, FlowCode.Renewal, USCode);
        }
        public TaskWork GenerateSubmitTask(string flowCode)
        {
            TaskWork.Cancel(t => t.TypeCode == flowCode && t.ActivityName == "Start" && t.RefID == this.ProjectId);
            TaskWork taskWork = new TaskWork();
            var url = TaskWork.BuildUrl(flowCode, this.ProjectId);
            switch (flowCode)
            {
                case FlowCode.Renewal_Letter:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_LLNegotiation:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_ConsInfo:
                    taskWork = GenerateTask(flowCode, url, PMAccount, PMNameENUS, PMNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_Tool:
                    taskWork = GenerateTask(flowCode, url, FinanceAccount, FinanceNameENUS, FinanceNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_Analysis:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_ClearanceReport:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_ConfirmLetter:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_LegalApproval:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_Package:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_ContractInfo:
                    taskWork = GenerateTask(flowCode, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
                    break;
                case FlowCode.Renewal_SiteInfo:
                    taskWork = GenerateTask(flowCode, url, PMAccount, PMNameENUS, PMNameZHCN, "Start");
                    break;
            }
            return taskWork;
        }

        public TaskWork GenerateToolUploadTask()
        {
            var url = TaskWork.BuildUrl(FlowCode.Renewal_Tool, this.ProjectId, "/Process/View");
            return GenerateTask(FlowCode.Renewal_Tool, url, AssetActorAccount, AssetActorNameENUS, AssetActorNameZHCN, "Start");
        }

        public TaskWork GenerateTask(string flowCode, string url, string receiver, string reveiverENUS, string receiverZHCN, string activityName)
        {
            TaskWork taskWork = new TaskWork();
            var source = FlowInfo.Get(FlowCode.Renewal);
            var taskType = FlowInfo.Get(flowCode);
            taskWork.SourceCode = source.Code;
            taskWork.SourceNameZHCN = source.NameZHCN;
            taskWork.SourceNameENUS = source.NameENUS;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            var title = TaskWork.BuildTitle(ProjectId, StoreNameZHCN, StoreNameENUS);

            taskWork.Title = title;
            taskWork.RefID = ProjectId;
            taskWork.StoreCode = USCode;

            taskWork.TypeCode = taskType.Code;
            taskWork.TypeNameENUS = taskType.NameENUS;
            taskWork.TypeNameZHCN = taskType.NameZHCN;
            taskWork.ReceiverAccount = receiver;
            taskWork.ReceiverNameENUS = reveiverENUS;
            taskWork.ReceiverNameZHCN = receiverZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.Url = url;
            taskWork.CreateTime = DateTime.Now;
            taskWork.CreateUserAccount = receiver;
            taskWork.Sequence = 0;
            taskWork.ActivityName = activityName;
            taskWork.Add();
            return taskWork;
        }

        protected override void UpdateProjectTaskUser(TaskWork taskWork, DataModels.ProjectTeamMembers projectTeamMembers)
        {
            switch (taskWork.TypeCode)
            {
                case FlowCode.Renewal_Letter:
                case FlowCode.Renewal_LLNegotiation:
                case FlowCode.Renewal_Analysis:
                case FlowCode.Renewal_ClearanceReport:
                case FlowCode.Renewal_ConfirmLetter:
                case FlowCode.Renewal_LegalApproval:
                case FlowCode.Renewal_Package:
                case FlowCode.Renewal_ContractInfo:
                    taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                    break;
                case FlowCode.Renewal_ConsInfo:
                case FlowCode.Renewal_SiteInfo:
                    taskWork.ReceiverAccount = projectTeamMembers.PM.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.PM.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.PM.UserNameZHCN;
                    break;
                case FlowCode.Renewal_Tool:
                    taskWork.ReceiverAccount = projectTeamMembers.Finance.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.Finance.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.Finance.UserNameZHCN;
                    break;
            }
        }
        protected override void ChangeProjectApprover(List<TaskWork> taskWorks, DataModels.ProjectTeamMembers projectTeamMembers)
        {
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.Renewal_Tool:
                        dataFields = new List<ProcessDataField>
                        {
                            new ProcessDataField("dest_AssetActor", projectTeamMembers.AssetActor.UserAccount)
                        };
                        if (projectTeamMembers.Legal.UserAccount != AssetActorAccount
                            && taskWork.ReceiverAccount == AssetActorAccount)
                        {
                            RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, projectTeamMembers.AssetActor.UserAccount, dataFields);

                            taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                            taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                            taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                        }
                        else
                        {
                            if (taskWork.ProcInstID.HasValue)
                            {
                                UpdateWorkflowDataField(taskWork.ProcInstID.Value, dataFields);
                            }
                        }
                        break;
                    case FlowCode.Renewal_LegalApproval:
                        dataFields = new List<ProcessDataField>()
                        {
                            new ProcessDataField("dest_LegalUser", projectTeamMembers.Legal.UserAccount)
                        };
                        if (projectTeamMembers.Legal != null
                            && projectTeamMembers.Legal.UserAccount != LegalAccount
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
            taskWork.ReceiverAccount = PMAccount;
            taskWork.ReceiverNameENUS = PMNameENUS;
            taskWork.ReceiverNameZHCN = PMNameZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.Url = strUrl;
            taskWork.CreateTime = DateTime.Now;
            taskWork.CreateUserAccount = CreateUserAccount;
            taskWork.Sequence = 0;
            taskWork.ActivityName = "Start";
            return taskWork;
        }
        public void UpdateFromProjectList()
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var oldInfo = RenewalInfo.Get(this.ProjectId);
                var consInfo = RenewalConsInfo.Get(this.ProjectId);
                var analysis = RenewalAnalysis.Get(this.ProjectId);
                analysis.LeaseTenureAndTerm = string.Format("{0} years,from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", this.RenewalYears, this.NewLeaseStartDate, this.NewLeaseEndDate);
                analysis.Update();
                if (oldInfo.NeedProjectCostEst != this.NeedProjectCostEst)
                {
                    consInfo.HasReinvenstment = this.NeedProjectCostEst;
                    consInfo.Update();
                    if (ProjectInfo.IsFlowFinished(this.ProjectId, FlowCode.Renewal_Letter) &&
                           ProjectInfo.IsFlowFinished(this.ProjectId, FlowCode.Renewal_LLNegotiation))
                    {
                        if (!this.NeedProjectCostEst)
                        {
                            TaskWork.Cancel(e => e.RefID == this.ProjectId && e.Status == TaskWorkStatus.UnFinish && e.TypeCode == FlowCode.Renewal_ConsInfo);
                            ProjectInfo.FinishProject(this.ProjectId, FlowCode.Renewal_ConsInfo);
                            if (!ProjectInfo.IsFlowFinished(this.ProjectId, FlowCode.Renewal_Tool))
                            {
                                if (!TaskWork.Any(e => e.RefID == this.ProjectId && e.Status == TaskWorkStatus.UnFinish && e.TypeCode == FlowCode.Renewal_Tool))
                                {
                                    GenerateSubmitTask(FlowCode.Renewal_Tool);
                                }
                            }
                        }
                        else
                        {
                            ProjectInfo.Reset(this.ProjectId, FlowCode.Renewal_ConsInfo);

                            GenerateSubmitTask(FlowCode.Renewal_ConsInfo);
                            if (!ProjectInfo.IsFlowFinished(this.ProjectId, FlowCode.Renewal_Tool))
                            {
                                TaskWork.Cancel(e => e.RefID == this.ProjectId && e.TypeCode == FlowCode.Renewal_Tool && e.Status == TaskWorkStatus.UnFinish);
                                ProjectInfo.Reset(this.ProjectId, FlowCode.Renewal_Tool);
                            }
                        }
                    }
                }

                if (oldInfo.RenewalYears != this.RenewalYears)
                {
                    if (ProjectInfo.IsFlowFinished(ProjectId, FlowCode.Renewal_Analysis))
                    {
                        if (this.RenewalYears <= 2)
                        {
                            TaskWork.Finish(e => e.RefID == this.ProjectId && e.Status == TaskWorkStatus.UnFinish && e.TypeCode == FlowCode.Renewal_ClearanceReport);
                            ProjectInfo.FinishProject(this.ProjectId, FlowCode.Renewal_ClearanceReport);
                        }
                        else
                        {
                            if (!TaskWork.Any(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Renewal_ClearanceReport && e.Status == TaskWorkStatus.UnFinish))
                                GenerateSubmitTask(FlowCode.Renewal_ClearanceReport);
                        }
                    }
                }
                this.Update();
                tranScope.Complete();
            }
        }
    }
}
