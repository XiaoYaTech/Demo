using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using FlowCodeRef = Mcdonalds.AM.DataAccess.Constants.FlowCode;
using NodeCodeRef = Mcdonalds.AM.DataAccess.Constants.NodeCode;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectInfo : BaseWFEntity<ProjectInfo>
    {
        public string NameDisp
        {
            get
            {
                if (ClientCookie.Language == Infrastructure.SystemLanguage.ENUS)
                    return FlowNameENUS;
                else
                    return FlowNameZHCN;
            }
        }

        public static bool CheckIfExistStore(string usCode, string flowCode)
        {
            return
                !Any(p => p.USCode == usCode
                        && p.FlowCode == flowCode
                        && (p.Status == ProjectStatus.UnFinish || p.Status == ProjectStatus.Pending));
        }

        public static bool CheckIfStoreValid(string usCode, string flowCode)
        {
            return Count(p => p.USCode == usCode
                              && p.FlowCode == flowCode
                              && (p.Status == ProjectStatus.UnFinish || p.Status == ProjectStatus.Pending)) < 2;
        }

        public static string GetUSCode(string projectId)
        {
            return Search(p => p.ProjectId == projectId).Select(p => p.USCode).FirstOrDefault();
        }

        public static ProjectInfo Get(string projectId, string flowCode)
        {
            return FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode == flowCode);
        }
        public static void Reset(string projectId, string flowCode, ProjectStatus? status = null)
        {
            var nodes = ProjectNode.Search(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status != ProjectNodeStatus.UnFinish).ToList();
            nodes.ForEach(n =>
            {
                var nodeInfo = NodeInfo.FirstOrDefault(e => e.Code == n.NodeCode);
                if (nodeInfo.ClearStatus)
                {
                    n.Status = ProjectNodeStatus.UnFinish;
                }
                else
                {
                    n.Status = ProjectNodeStatus.Pending;
                }
            });
            ProjectNode.Update(nodes.ToArray());
            var projectInfo = FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode == flowCode);
            projectInfo.NodeCode = NodeCodeRef.Start;
            projectInfo.NodeNameENUS = "Start";
            projectInfo.NodeNameZHCN = "开始";
            if (status.HasValue)
            {
                projectInfo.Status = status.Value;
            }
            else
            {
                projectInfo.Status = ProjectStatus.UnFinish;
            }
            projectInfo.Update();
            UpdateProjectStatus(projectId, GetMainProjectFlowCode(flowCode), ProjectStatus.UnFinish);
        }
        public static void Reject(string projectId, string flowCode, ProjectStatus? status = null)
        {
            //var nodes = ProjectNode.Search(e => e.ProjectId == projectId && e.FlowCode == flowCode).ToList();
            //nodes.ForEach(n =>
            //{
            //    n.Status = ProjectNodeStatus.Pending;
            //});
            //ProjectNode.Update(nodes.ToArray());
            var projectInfo = FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode == flowCode);
            //projectInfo.NodeCode = NodeCodeRef.Start;
            //projectInfo.NodeNameENUS = "Start";
            //projectInfo.NodeNameZHCN = "开始";
            if (status.HasValue)
            {
                projectInfo.Status = status.Value;
            }
            else
            {
                projectInfo.Status = ProjectStatus.Rejected;
            }
            projectInfo.Update();
            UpdateProjectStatus(projectId, GetMainProjectFlowCode(flowCode), ProjectStatus.Rejected);
        }
        public static void FinishNode(string projectId, string flowCode, string nodeCode, ProjectStatus? status = null)
        {
            var currentProjectNode = ProjectNode.FinishProjectNode(projectId, flowCode, nodeCode);
            var node = NodeInfo.FirstOrDefault(e => e.Code == currentProjectNode.NodeCode);
            var projectInfo = FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode == flowCode);
            projectInfo.NodeCode = node.Code;
            projectInfo.NodeNameENUS = node.NameENUS;
            projectInfo.NodeNameZHCN = node.NameZHCN;
            if (status.HasValue)
            {
                projectInfo.Status = status.Value;
            }
            projectInfo.Update();
        }

        public static void UnFinishNode(string projectId, string flowCode, string nodeCode, ProjectStatus? status = null)
        {
            var currentProjectNode = ProjectNode.UnFinishProjectNode(projectId, flowCode, nodeCode);
            var node = NodeInfo.FirstOrDefault(e => e.Code == currentProjectNode.NodeCode);
            var projectInfo = FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode == flowCode);
            projectInfo.NodeCode = node.Code;
            projectInfo.NodeNameENUS = node.NameENUS;
            projectInfo.NodeNameZHCN = node.NameZHCN;
            if (status.HasValue)
            {
                projectInfo.Status = status.Value;
            }
            projectInfo.Update();
        }

        /// <summary>
        /// 创建子项目
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="projectId"></param>
        /// <param name="nodeCode"></param>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public static int CreateSubProject(string flowCode, string projectId, string usCode, string nodeCode, string userAccount)
        {
            var db = PrepareDb();
            var flowInfo = db.FlowInfo.FirstOrDefault(e => e.Code == flowCode);
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Id = Guid.NewGuid();
            projectInfo.FlowCode = flowCode;
            if (flowInfo != null)
            {
                projectInfo.FlowNameENUS = flowInfo.NameENUS;
                projectInfo.FlowNameZHCN = flowInfo.NameZHCN;
            }

            var nodeInfo = db.NodeInfo.FirstOrDefault(e => e.Code == nodeCode);
            if (nodeInfo != null)
            {
                projectInfo.NodeCode = nodeInfo.Code;
                projectInfo.NodeNameENUS = nodeInfo.NameENUS;
                projectInfo.NodeNameZHCN = nodeInfo.NameZHCN;
            }

            projectInfo.ProjectId = projectId;
            projectInfo.USCode = usCode;
            projectInfo.CreateUserAccount = userAccount;
            projectInfo.CreateTime = DateTime.Now;
            if (flowCode == "Renewal_GBMemo")
            {
                projectInfo.Status = ProjectStatus.Finished;
                projectInfo.FlowNameENUS = "Renewal GBMemo";
                projectInfo.FlowNameZHCN = "Renewal GBMemo";
            }
            else
                projectInfo.Status = ProjectStatus.UnFinish;
            return Add(projectInfo);
        }

        public void CreateSubProject(List<ProjectInfo> listProj)
        {
            try
            {
                Add(listProj.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProjectInfo GenerateSubProject(string flowCode, string projectId, string usCode, string nodeCode, string userAccount, ProjectStatus status = ProjectStatus.UnKnown)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            try
            {
                var db = GetDb();
                if (projectId.ToLower().IndexOf("majorlease") != -1 && flowCode.ToLower().IndexOf("gbmemo") != -1)
                {
                    projectInfo.FlowNameENUS = "MajorLease GB Memo";
                    projectInfo.FlowNameZHCN = "MajorLease GB Memo";
                    projectInfo.Status = ProjectStatus.Finished;
                }
                else if (projectId.ToLower().IndexOf("renewal") != -1 && flowCode.ToLower().IndexOf("gbmemo") != -1)
                {
                    projectInfo.FlowNameENUS = "Renewal GB Memo";
                    projectInfo.FlowNameZHCN = "Renewal GB Memo";
                    projectInfo.Status = ProjectStatus.Finished;
                }
                else
                {
                    var flowInfo = db.FlowInfo.First(e => e.Code == flowCode);
                    projectInfo.FlowNameENUS = (flowInfo == null ? "" : flowInfo.NameENUS);
                    projectInfo.FlowNameZHCN = (flowInfo == null ? "" : flowInfo.NameZHCN);
                    projectInfo.Status = ProjectStatus.UnFinish;
                }

                if (status != ProjectStatus.UnKnown)
                {
                    projectInfo.Status = status;
                }

                projectInfo.Id = Guid.NewGuid();
                projectInfo.FlowCode = flowCode;

                var nodeInfo = db.NodeInfo.First(e => e.Code == nodeCode);
                projectInfo.NodeCode = nodeCode;
                projectInfo.NodeNameENUS = (nodeInfo == null ? "" : nodeInfo.NameENUS);
                projectInfo.NodeNameZHCN = (nodeInfo == null ? "" : nodeInfo.NameZHCN);
                projectInfo.ProjectId = projectId;
                projectInfo.USCode = usCode;

                projectInfo.CreateUserAccount = userAccount;
                projectInfo.CreateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return projectInfo;
        }

        public static string GetProjectIDByProcInstId(int procInstId)
        {
            string sql = @"SELECT TOP 1 RefID FROM dbo.TaskWork
            WHERE ProcInstID = " + procInstId;
            string projectId = SqlQuery<string>(sql, null).FirstOrDefault();
            return projectId;
        }

        /// <summary>
        /// 生成主项目，返回项目编号
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="userCondition"></param>
        /// <returns>项目编号</returns>
        public static string CreateMainProject(string flowCode, string usCode, string nodeCode, string userAccount, string Id = "")
        {
            var db = PrepareDb();

            //获取项目编号的前缀
            var flowInfo = db.FlowInfo.First(e => e.Code == flowCode);
            var flowCodePrefix = flowInfo.FlowCodePrefix;
            int flowCodePrefixLength = flowCodePrefix.Length;
            //获取最大的项目编号
            // var projectInfo =  db.ProjectInfo.Max(e => e.FlowCode == flowCode);
            //           string sql = string.Format(@"SELECT Max(CAST(RIGHT(p.projectId,LEN(fi.FlowCodePrefix)) AS INT))
            //            from ProjectInfo p INNER JOIN dbo.FlowInfo fi ON fi.Code = p.FlowCode
            //             Where flowCode =  '{0}'", flowCode);
            var currentDateStr = DateTime.Now.ToString("yyMMdd");
            string sql = string.Format(@"SELECT Cast(Max(CAST(RIGHT(projectId,2) AS INT)) AS nvarchar(10)) Id from ProjectInfo
            Where ProjectId LIKE'%{0}%'
            ", flowCodePrefix + currentDateStr);
            var maxProjectId = SqlQuery<string>(sql, null).FirstOrDefault();


            //获取项目编号流水号部分
            var projectNum = string.Empty;

            if (string.IsNullOrEmpty(maxProjectId))
            {
                projectNum = "01";
            }
            else
            {
                int currentNum = int.Parse(maxProjectId) + 1;
                if (currentNum < 10)
                {
                    projectNum = "0" + currentNum;
                }
                else
                {
                    projectNum = currentNum.ToString();
                }
            }

            var projectId = flowCodePrefix + currentDateStr + projectNum;

            var nodeInfo = db.NodeInfo.First(e => e.Code == nodeCode);

            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Id = Guid.NewGuid();
            if (!string.IsNullOrEmpty(Id))
                projectInfo.Id = new Guid(Id);
            projectInfo.FlowCode = flowCode;
            projectInfo.FlowNameENUS = flowInfo.NameENUS;
            projectInfo.FlowNameZHCN = flowInfo.NameZHCN;
            projectInfo.NodeCode = nodeInfo.Code;
            projectInfo.NodeNameENUS = nodeInfo.NameENUS;
            projectInfo.NodeNameZHCN = nodeInfo.NameZHCN;
            projectInfo.ProjectId = projectId;
            projectInfo.USCode = usCode;
            projectInfo.CreateUserAccount = userAccount;
            projectInfo.CreateTime = DateTime.Now;
            Add(projectInfo);
            return projectId;
        }

        /// <summary>
        /// 生成DL项目，返回项目编号
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="userCondition"></param>
        /// <returns>项目编号</returns>
        public static string CreateDLProject(Guid Id, string flowCode, string usCode, string nodeCode, string userAccount, bool pushOrNot)
        {
            var db = PrepareDb();

            //获取项目编号的前缀
            var flowInfo = db.FlowInfo.First(e => e.Code == flowCode);
            var flowCodePrefix = flowInfo.FlowCodePrefix;
            int flowCodePrefixLength = flowCodePrefix.Length;
            var currentDateStr = DateTime.Now.ToString("yyMMdd");
            string sql = string.Format(@"SELECT Cast(Max(CAST(RIGHT(projectId,2) AS INT)) AS nvarchar(10)) Id from ProjectInfo
            Where ProjectId LIKE'%{0}%'
            ", flowCodePrefix + currentDateStr);
            var maxProjectId = SqlQuery<string>(sql, null).FirstOrDefault();


            //获取项目编号流水号部分
            var projectNum = string.Empty;

            if (string.IsNullOrEmpty(maxProjectId))
            {
                projectNum = "01";
            }
            else
            {
                int currentNum = int.Parse(maxProjectId) + 1;
                if (currentNum < 10)
                {
                    projectNum = "0" + currentNum;
                }
                else
                {
                    projectNum = currentNum.ToString();
                }
            }

            var projectId = flowCodePrefix + currentDateStr + projectNum;

            var nodeInfo = db.NodeInfo.First(e => e.Code == nodeCode);

            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Id = Id;
            projectInfo.FlowCode = flowCode;
            projectInfo.FlowNameENUS = flowInfo.NameENUS;
            projectInfo.FlowNameZHCN = flowInfo.NameZHCN;
            projectInfo.NodeCode = nodeInfo.Code;
            projectInfo.NodeNameENUS = nodeInfo.NameENUS;
            projectInfo.NodeNameZHCN = nodeInfo.NameZHCN;
            projectInfo.ProjectId = projectId;
            projectInfo.USCode = usCode;
            projectInfo.CreateUserAccount = userAccount;
            projectInfo.CreateTime = DateTime.Now;
            projectInfo.IsPushed = pushOrNot;
            Add(projectInfo);
            return projectId;
        }

        public static bool IsOriginator(string projectId, string flowCode, string userCode)
        {
            return Any(p => p.ProjectId == projectId && p.FlowCode == flowCode && p.CreateUserAccount == userCode);
        }

        public static List<ProjectHistory> GetHistory(string projectId, string tableName)
        {
            string sql = "SELECT * FROM " + tableName + " WHERE projectId ='" + projectId + "' and IsAvailable = 0 ";
            var list = SqlQuery<ProjectHistory>(sql, null).ToList();
            return list;
        }

        public static bool EnableRecall(string tableName, Guid id)
        {
            bool enableRecall = false;
            var list = ProjectComment.Search(e => e.RefTableName == tableName.Trim() && e.RefTableId == id)
                .OrderByDescending(e => e.CreateTime).AsNoTracking().ToList();
            if (list.Count > 0 && list[0].Action != ProjectAction.Recall)
            {
                enableRecall = true;
            }
            return enableRecall;
        }

        public static void FinishProject(string projectId, string flowCode)
        {
            FinishNode(projectId, flowCode, NodeCodeRef.Finish, ProjectStatus.Finished);
        }

        public static bool IsFlowHasStarted(string strProjectId, string strFlowCode)
        {
            return Any(p => p.ProjectId == strProjectId && p.FlowCode == strFlowCode && p.NodeCode != Constants.NodeCode.Start)
                && TaskWork.Any(e => e.ActivityName != "Start" && e.RefID == strProjectId && e.TypeCode == strFlowCode);
        }

        public static bool IsNextFlowStarted(string projectId, string flowCode)
        {
            var flow = FlowInfo.Get(flowCode);
            var nextFlowCodes = FlowInfo.Search(e => e.ParentCode == flow.ParentCode && e.ExecuteSequence == flow.ExecuteSequence + 1).Select(e => e.Code).ToList();
            return Any(p => p.ProjectId == projectId && nextFlowCodes.Contains(p.FlowCode) && p.NodeCode != Constants.NodeCode.Start && p.NodeCode != Constants.NodeCode.Finish);
        }

        public static bool IsFlowFinished(string strProjectId, string strFlowCode)
        {
            return Any(p => p.ProjectId == strProjectId && p.FlowCode == strFlowCode && p.Status == ProjectStatus.Finished);
        }

        public static List<ProjectInfo> GetFinishedProject(string strProjectId)
        {
            return Search(e => e.ProjectId.Equals(strProjectId) && e.Status == ProjectStatus.Finished).ToList();
        }

        public static void UpdateProjectStatus(string projectId, string flowCode, ProjectStatus status)
        {
            var projectInfo = Search(e => e.ProjectId == projectId && e.FlowCode == flowCode).FirstOrDefault();
            if (projectInfo != null)
            {
                projectInfo.Status = status;
            }
            Update(projectInfo);
        }

        public static bool CompleteMainIfEnable(string projectId)
        {
            if (!Any(p => p.ProjectId == projectId && p.Status != ProjectStatus.Finished && p.FlowCode.IndexOf("_") > 0))
            {
                var project = FirstOrDefault(p => p.ProjectId == projectId && p.FlowCode.IndexOf("_") < 0);
                project.NodeCode = NodeCodeRef.Finish;
                project.NodeNameENUS = "Finish";
                project.NodeNameZHCN = "完成";
                project.Status = ProjectStatus.Completed;
                project.Update();
                return true;
            }
            return false;
        }
        public static bool IsFlowSavable(string projectId, string flowCode)
        {
            switch (flowCode)
            {
                /*客户需求以及不进流程的Renwal子项目在只读页面是否能保存*/
                case FlowCodeRef.Renewal_LLNegotiation://客户需求
                    //case FlowCodeRef.Renewal_Analysis:
                    //case FlowCodeRef.Renewal_ClearanceReport:
                    //case FlowCodeRef.Renewal_ConfirmLetter:
                    return !ProjectInfo.IsFlowStarted(projectId, FlowCodeRef.Renewal_Package)
                        && ProjectInfo.IsOriginator(projectId, flowCode, ClientCookie.UserCode);
                /*进流程的子项目在只读页面是否能保存*/
                default:
                    return !TaskWork.Any(t => t.RefID == projectId && t.TypeCode == flowCode)
                        && !ProjectInfo.IsFlowFinished(projectId, flowCode)
                        && ProjectInfo.IsOriginator(projectId, flowCode, ClientCookie.UserCode);
            }

        }

        public static bool IsFlowEditable(string projectId, string flowCode)
        {
            string usCode = null;
            var projectInfo = FirstOrDefault(e => e.ProjectId == projectId
                                                  && e.FlowCode == flowCode);
            if (projectInfo != null)
            {
                usCode = projectInfo.USCode;
            }

            var adminRoleUsers = Employee.GetEmployeesByRole(RoleCode.System_Admin).Select(e => e.Code);
            var isOfAdminRole = adminRoleUsers.Contains(ClientCookie.UserCode);
            if (projectId.ToLower().IndexOf("renewal") != -1)
                return (IsOriginator(projectId, flowCode, ClientCookie.UserCode) || isOfAdminRole)
                    //&& Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && (e.Status == ProjectStatus.Finished || e.Status == ProjectStatus.Rejected))
                    && (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Finished)
                            || (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Rejected) && isOfAdminRole))
                    && !IsNextFlowStarted(projectId, flowCode)
                    && CheckIfStoreValid(usCode, GetMainProjectFlowCode(flowCode));
            else if (projectId.ToLower().IndexOf("closure") != -1)
            {
                var b = (IsOriginator(projectId, flowCode, ClientCookie.UserCode) || isOfAdminRole)
                    //&& Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && (e.Status == ProjectStatus.Finished || e.Status == ProjectStatus.Rejected))
                        && (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Finished)
                                || (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Rejected) && isOfAdminRole))
                        && CheckIfStoreValid(usCode, GetMainProjectFlowCode(flowCode));
                if (flowCode == "Closure_LegalReview")
                {
                    var projPackage =
                        ProjectInfo.Search(e => e.ProjectId == projectId && e.FlowCode == "Closure_ClosurePackage")
                            .FirstOrDefault();
                    if (projPackage != null && projPackage.Status == ProjectStatus.Finished)
                    {
                        b = false;
                    }
                    else
                        b = !IsFlowStarted(projectId, "Closure_ClosurePackage") && b;
                }
                
                return b;
            }
            else if (projectId.ToUpper().IndexOf("TPCLS") != -1)
            {
                var b = (IsOriginator(projectId, flowCode, ClientCookie.UserCode) || isOfAdminRole)
                    //&& Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && (e.Status == ProjectStatus.Finished || e.Status == ProjectStatus.Rejected))
                            && (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Finished)
                                    || (Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Rejected) && isOfAdminRole))
                            && CheckIfStoreValid(usCode, GetMainProjectFlowCode(flowCode));
                if (flowCode == "TempClosure_LegalReview")
                {
                    var projPackage =
                        ProjectInfo.Search(e => e.ProjectId == projectId && e.FlowCode == "TempClosure_ClosurePackage")
                            .FirstOrDefault();
                    if (projPackage != null && projPackage.Status == ProjectStatus.Finished)
                    {
                        b = false;
                    }
                    else
                        b = !IsFlowStarted(projectId, "TempClosure_ClosurePackage") && b;
                }
                return b;
            }
            else
                return (IsOriginator(projectId, flowCode, ClientCookie.UserCode) || isOfAdminRole)
                    //&& Any(e => e.ProjectId == projectId && e.FlowCode == flowCode && (e.Status == ProjectStatus.Finished || e.Status == ProjectStatus.Rejected))
                       &&
                       (Any(
                           e => e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Finished)
                        ||
                        (Any(
                            e =>
                                e.ProjectId == projectId && e.FlowCode == flowCode && e.Status == ProjectStatus.Rejected) &&
                         isOfAdminRole))
                       && CheckIfStoreValid(usCode, GetMainProjectFlowCode(flowCode));

        }

        public static bool IsFlowRecallable(string projectId, string flowCode)
        {
            return Any(p => p.ProjectId == projectId && p.FlowCode == flowCode && p.NodeCode != Constants.NodeCode.Start && p.Status != ProjectStatus.Rejected && p.Status != ProjectStatus.Finished)
                && TaskWork.Any(e => e.ActivityName != "Start" && e.RefID == projectId && e.TypeCode == flowCode)
                && ProjectInfo.IsOriginator(projectId, flowCode, ClientCookie.UserCode);
        }

        public static string CreateSummaryProject(string flowCode, string usCode, string userAccount)
        {
            var db = PrepareDb();

            //获取项目编号的前缀
            var flowInfo = db.FlowInfo.First(e => e.Code == flowCode);


            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.Id = Guid.NewGuid();
            projectInfo.FlowCode = flowCode;
            projectInfo.FlowNameENUS = flowInfo.NameENUS;
            projectInfo.FlowNameZHCN = flowInfo.NameZHCN;
            //projectInfo.NodeCode = nodeInfo.Code;
            //projectInfo.NodeNameENUS = nodeInfo.NameENUS;
            //projectInfo.NodeNameZHCN = nodeInfo.NameZHCN;
            //projectInfo.ProjectId = projectId;
            projectInfo.USCode = usCode;
            projectInfo.CreateUserAccount = userAccount;
            projectInfo.CreateTime = DateTime.Now;
            Add(projectInfo);
            return projectInfo.Id.ToString();
        }

    }

    public class ProjectHistory
    {

        public string CreateUserNameZHCN { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
