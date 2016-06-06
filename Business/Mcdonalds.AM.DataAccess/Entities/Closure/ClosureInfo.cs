using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels;
using Mcdonalds.AM.DataAccess.Entities;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.DataTransferObjects;

namespace Mcdonalds.AM.DataAccess
{

    public partial class ClosureInfo : BaseWFEntity<ClosureInfo>
    {

        public List<Employee> NoticeUserList { get; set; }
        public List<Employee> NecessaryNoticeUserList { get; set; }

        public DateTime LeaseExpireDate { get; set; }
        public string LandlordName { get; set; }

        public static ClosureInfo GetByProjectId(string projectId)
        {
            var contraInfo = ProjectContractInfo.Get(projectId);

            var clsInfo = Search(e => e.ProjectId == projectId).FirstOrDefault();
            if (clsInfo != null)
            {
                clsInfo.LandlordName = contraInfo.PartyAFullName;
                if (contraInfo.EndDate.HasValue)
                {
                    clsInfo.LeaseExpireDate = contraInfo.EndDate.Value;
                }
            }
            return clsInfo;

        }


        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetByProjectId(projectId);
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
                case FlowCode.Closure_LegalReview:
                    taskWork.ReceiverAccount = projectTeamMembers.AssetActor.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;

                    break;
                case FlowCode.Closure_WOCheckList:
                    taskWork.ReceiverAccount = projectTeamMembers.PM.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.PM.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.PM.UserNameZHCN;
                    break;
                case FlowCode.Closure_ClosureTool:
                    taskWork.ReceiverAccount = projectTeamMembers.Finance.UserAccount;
                    taskWork.ReceiverNameENUS = projectTeamMembers.Finance.UserNameENUS;
                    taskWork.ReceiverNameZHCN = projectTeamMembers.Finance.UserNameZHCN;
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
                    case FlowCode.MajorLease_LegalReview:
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
                    case FlowCode.Closure_ClosureTool:
                        dataFields = new List<ProcessDataField>()
                        {
                            new ProcessDataField("dest_Actor", AssetActorAccount)
                        };
                        if (projectTeamMembers.AssetActor != null
                            && projectTeamMembers.AssetActor.UserAccount != AssetActorAccount
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

        protected override void UpdateProjectCreateUser(ProjectTeamMembers projectTeamMembers)
        {
            var projectInfo = ProjectInfo.Search(i => i.ProjectId == this.ProjectId && i.FlowCode.Contains(FlowCode.Closure));
            foreach (var projectInfoItem in projectInfo)
            {
                switch (projectInfoItem.FlowCode)
                {
                    case FlowCode.Closure:
                    case FlowCode.Closure_LegalReview:
                    case FlowCode.Closure_ExecutiveSummary:
                    case FlowCode.Closure_ClosurePackage:
                    case FlowCode.Closure_Memo:
                    case FlowCode.Closure_ContractInfo:
                        projectInfoItem.CreateUserAccount = projectTeamMembers.AssetActor.UserAccount;
                        break;
                    case FlowCode.Closure_WOCheckList:
                    case FlowCode.Closure_ConsInvtChecking:
                        projectInfoItem.CreateUserAccount = projectTeamMembers.PM.UserAccount;
                        break;
                    case FlowCode.Closure_ClosureTool:
                        projectInfoItem.CreateUserAccount = projectTeamMembers.Finance.UserAccount;
                        break;
                }
            }
            ProjectInfo.Update(projectInfo.ToArray());
        }

        public string GetProjectId()
        {
            var db = GetDb();
            string projectId = string.Empty;
            string currentDateStr = DateTime.Now.ToString("yyyyMMdd").Substring(2);
            int dateNum = int.Parse(currentDateStr);
            var count = db.ClosureInfo.Count();
            if (count == 0)
            {
                projectId = currentDateStr + "01";
            }
            else
            {
                var maxProjectId = db.ClosureInfo.Max(e => e.ProjectId);
                if (!string.IsNullOrEmpty(maxProjectId))
                {
                    //前缀结束的索引
                    int prefixSplitIndex = FlowCode.Closure.Length;

                    int maxProjectIdVal = int.Parse(maxProjectId.Substring(prefixSplitIndex));
                    string maxDateStr = maxProjectId.Substring(prefixSplitIndex, prefixSplitIndex + 6);
                    if (maxDateStr != currentDateStr)
                    {
                        projectId = FlowCode.Closure + currentDateStr + "01";
                    }
                    else
                    {
                        projectId = FlowCode.Closure + "_" + maxProjectIdVal;
                    }
                }
            }

            return projectId;
        }

        #region Relocation
        /// <summary>
        /// 将Relocation的选项转换成bool
        /// </summary>
        /// <returns></returns>
        public bool IsRelocation()
        {
            if (!string.IsNullOrEmpty(this.RelocationCode))
            {
                var dic_Relocation = Dictionary.FirstOrDefault(i => i.Code == this.RelocationCode);
                if (dic_Relocation.Value.Equals("Y"))
                    return true;
                else
                    return false;
            }
            return false;
        }

        public void UpdateRelocation(bool IsRelocation)
        {
            if (IsRelocation)
            {
                this.RelocationCode = "Relocation1";
                this.RelocationNameENUS = "Y";
                this.RelocationNameZHCN = "Y";
            }
            else
            {
                this.RelocationCode = "Relocation2";
                this.RelocationNameENUS = "N";
                this.RelocationNameZHCN = "N";
            }
            this.Update();
        }
        #endregion
    }
}
