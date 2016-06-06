using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Interfaces;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;

namespace Mcdonalds.AM.DataAccess.Entities
{
    public abstract class BaseWFEntity : BaseAbstractEntity, IWorkflowExtend
    {
        public Guid EntityId { get; set; }

        public bool IsProjectFreezed { get; set; }

        public bool IsMainProject { get; set; }

        public string WFProjectId { get; set; }

        public string WFCode { get; set; }

        #region ---- [Workflow Config Varables ] ----

        public virtual string WorkflowProcessName { get { return null; } }
        public virtual string WorkflowProcessCode { get { return null; } }
        public virtual string TableName { get { return "MajorLeaseLegalReview"; } }
        public virtual string WorkflowActOriginator { get { return null; } } // 退回至发起人节点名

        public virtual string WorkflowCode { get { return null; } }

        // 一般审批人节点，不能取消流程
        public virtual string[] WorkflowNormalActors { get { return new string[0]; } }

        #endregion
        protected static BaseWFEntity GetEmptyWorkflowEntity(string flowCode)
        {
            BaseWFEntity wfEntity = null;
            switch (flowCode)
            {
                case FlowCode.MajorLease:
                    wfEntity = new MajorLeaseInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.MajorLease_LegalReview:
                    wfEntity = new MajorLeaseLegalReview();
                    break;
                case FlowCode.MajorLease_FinanceAnalysis:
                    wfEntity = new MajorLeaseFinancAnalysis();
                    break;
                case FlowCode.MajorLease_ConsInfo:
                    wfEntity = new MajorLeaseConsInfo();
                    break;
                case FlowCode.MajorLease_Package:
                    wfEntity = new MajorLeaseChangePackage();
                    break;
                case FlowCode.MajorLease_ConsInvtChecking:
                    wfEntity = new MajorLeaseConsInvtChecking();
                    break;
                case FlowCode.MajorLease_GBMemo:
                    wfEntity = new MajorLeaseGBMemo();
                    break;
                case FlowCode.Closure:
                    wfEntity = new ClosureInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.Closure_ClosurePackage:
                    wfEntity = new ClosurePackage();
                    break;
                case FlowCode.Closure_ClosureTool:
                    wfEntity = new ClosureTool();
                    break;
                case FlowCode.Closure_ConsInvtChecking:
                    wfEntity = new ClosureConsInvtChecking();
                    break;
                case FlowCode.Closure_LegalReview:
                    wfEntity = new ClosureLegalReview();
                    break;
                case FlowCode.Closure_WOCheckList:
                    wfEntity = new ClosureWOCheckList();
                    break;
                case FlowCode.Closure_ExecutiveSummary:
                    wfEntity = new ClosureExecutiveSummary();
                    break;
                case FlowCode.TempClosure:
                    wfEntity = new TempClosureInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.TempClosure_LegalReview:
                    wfEntity = new TempClosureLegalReview();
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    wfEntity = new TempClosurePackage();
                    break;
                case FlowCode.Rebuild:
                    wfEntity = new RebuildInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.Rebuild_LegalReview:
                    wfEntity = new RebuildLegalReview();
                    break;
                case FlowCode.Rebuild_FinanceAnalysis:
                    wfEntity = new RebuildFinancAnalysis();
                    break;
                case FlowCode.Rebuild_ConsInfo:
                    wfEntity = new RebuildConsInfo();
                    break;
                case FlowCode.Rebuild_Package:
                    wfEntity = new RebuildPackage();
                    break;
                case FlowCode.Rebuild_ConsInvtChecking:
                    wfEntity = new RebuildConsInvtChecking();
                    break;
                case FlowCode.Rebuild_GBMemo:
                    wfEntity = new GBMemo();
                    break;
                case FlowCode.Renewal:
                    wfEntity = new RenewalInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.Renewal_Letter:
                    wfEntity = new RenewalLetter();
                    break;
                case FlowCode.Renewal_LLNegotiation:
                    wfEntity = new RenewalLLNegotiation();
                    break;
                case FlowCode.Renewal_ConsInfo:
                    wfEntity = new RenewalConsInfo();
                    break;
                case FlowCode.Renewal_Tool:
                    wfEntity = new RenewalTool();
                    break;
                case FlowCode.Renewal_Analysis:
                    wfEntity = new RenewalAnalysis();
                    break;
                case FlowCode.Renewal_ClearanceReport:
                    wfEntity = new RenewalClearanceReport();
                    break;
                case FlowCode.Renewal_ConfirmLetter:
                    wfEntity = new RenewalConfirmLetter();
                    break;
                case FlowCode.Renewal_LegalApproval:
                    wfEntity = new RenewalLegalApproval();
                    break;
                case FlowCode.Renewal_Package:
                    wfEntity = new RenewalPackage();
                    break;
                case FlowCode.Renewal_GBMemo:
                    wfEntity = new RenewalGBMemo();
                    break;
                case FlowCode.Reimage:
                    wfEntity = new ReimageInfo();
                    wfEntity.IsMainProject = true;
                    break;
                case FlowCode.Reimage_ConsInfo:
                    wfEntity = new ReimageConsInfo();
                    break;
                case FlowCode.Reimage_Summary:
                    wfEntity = new ReimageSummary();
                    break;
                case FlowCode.Reimage_Package:
                    wfEntity = new ReimagePackage();
                    break;
                case FlowCode.Reimage_ConsInvtChecking:
                    wfEntity = new ReimageConsInvtChecking();
                    break;
                case FlowCode.Reimage_GBMemo:
                    wfEntity = new ReimageGBMemo();
                    break;
            }
            return wfEntity;
        }

        public static BaseWFEntity GetWorkflowEntity(string projectId, string flowCode, string id = "")
        {
            var wfEntity = GetEmptyWorkflowEntity(flowCode);
            if (wfEntity != null)
            {
                wfEntity.WFProjectId = projectId;
                wfEntity.WFCode = flowCode;
                wfEntity = wfEntity.GetWorkflowInfo(projectId, id);
            }

            return wfEntity;

        }

        public static void GoTo(int procInstId, string activityName)
        {
            K2FxContext.Current.GoToActivityAndRecord(procInstId, activityName, ClientCookie.UserCode);
        }

        public virtual ApproveDialogUser GetApproveDialogUser()
        {
            return null;
        }

        public virtual List<ProjectComment> GetEntityProjectComment()
        {
            return null;
        }

        public virtual void ChangePackageHoldingStatus(HoldingStatus status)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfFreezeProject(string projectId)
        {
            var isFreezed = ProjectInfo.Any(e => e.ProjectId == projectId
                && (e.Status == ProjectStatus.Pending || e.Status == ProjectStatus.Killed));

            return isFreezed;
        }

        public void PendingProject(string projectId)
        {
            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId).AsNoTracking().ToList();
            projectInfos.ForEach(e =>
            {
                if (e.Status != ProjectStatus.Finished)
                {
                    e.Status = ProjectStatus.Pending;
                }
            });

            ProjectInfo.Update(projectInfos.ToArray());
        }

        public void ChangeProjectStatus(ProjectDto project)
        {
            using (var scope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == project.ProjectId
                                                                   && e.FlowCode == project.FlowCode);

                if (projectInfo != null)
                {
                    projectInfo.Status = project.CurrStatus;
                    projectInfo.Update();


                    var tasks = TaskWork.Search(e => e.RefID == project.ProjectId
                                                     && e.Status == TaskWorkStatus.UnFinish
                                                     && e.ActivityName == NodeCode.Start).ToList();

                    tasks.ForEach(e =>
                    {
                        e.ActionName = SetTaskActionName(project.CurrStatus);
                    });

                    TaskWork.Update(tasks.ToArray());

                    var log = new ProjectStatusChangeLog
                    {
                        ProjectId = project.ProjectId,
                        PrevStatus = project.PrevStatus,
                        CurrStatus = project.CurrStatus,
                        FlowCode = project.FlowCode,
                        Comment = project.Comment,
                        CreateBy = ClientCookie.UserCode,
                        CreateTime = DateTime.Now
                    };

                    log.Add();
                }

                scope.Complete();
            }
        }

        private string SetTaskActionName(ProjectStatus currStatus)
        {
            string actionName;
            switch (currStatus)
            {
                case ProjectStatus.Killed:
                case ProjectStatus.Pending:
                    actionName = ProjectAction.Pending;
                    break;
                default:
                    actionName = null;
                    break;
            }

            return actionName;

        }

        protected string SetTaskActionName(string projectId)
        {
            return CheckIfFreezeProject(projectId) ? ProjectAction.Pending : null;
        }

        public void ResumeProject(string projectId)
        {
            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId).AsNoTracking().ToList();
            projectInfos.ForEach(e =>
            {
                if (e.Status == ProjectStatus.Pending)
                {
                    e.Status = ProjectStatus.UnFinish;
                }
            });

            ProjectInfo.Update(projectInfos.ToArray());
        }

        protected virtual void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            throw new NotImplementedException();
        }

        public static string GetProcessOwnerCode(string flowCode, string projectId)
        {
            var code = string.Empty;
            var info = ProjectInfo.Get(projectId, flowCode);
            if (info != null)
            {
                code = info.CreateUserAccount;
            }
            return code;
        }


        public List<TaskWork> ChangeWorkflowApprovers(string projectId, ApproveUsers approver)
        {
            var workflowTasks = TaskWork.Search(e => e.RefID == projectId
                                                          && e.Status == TaskWorkStatus.UnFinish
                                                          && e.K2SN != null).ToList();
            var approveDiallogUsers = ApproveDialogUser.Search(e => e.ProjectId == projectId).ToList();

            ChangeWorkflowApprovers(workflowTasks, approveDiallogUsers, approver);

            ApproveDialogUser.Update(approveDiallogUsers.ToArray());
            TaskWork.Update(workflowTasks.ToArray());

            return workflowTasks;
        }

        protected virtual void ChangeProjectApprover(List<TaskWork> taskWorks, ProjectTeamMembers projectTeamMembers)
        {
            throw new NotImplementedException();
        }

        public void UpdateWorkflowDataField(int procInstId, List<ProcessDataField> dataFields)
        {
            K2FxContext.Current.SetProcessDataFields(procInstId, dataFields);
        }

        public virtual List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            throw new NotImplementedException();
        }

        public void RedirectWorkflowTask(string sn, string taskUserAccount, string redirectToUserAccount, List<ProcessDataField> dataFields = null)
        {
            Log4netHelper.WriteInfo(
                JsonConvert.SerializeObject(new { sn, taskUserAccount, redirectToUserAccount, dataFields }));
            var isSuccess = K2FxContext.Current.RedirectSign(sn, taskUserAccount, redirectToUserAccount, dataFields);
            if (!isSuccess)
            {
                throw new Exception("K2 redirect action fails!");
            }
        }

        public static bool IsFlowStarted(string projectId, string flowCode)
        {
            bool isStart = TaskWork.Any(e => e.RefID == projectId
                                             && e.TypeCode == flowCode
                                             && e.ActivityName != "Start"
                                             && e.ActivityName != "Originator"
                                             && e.Status == TaskWorkStatus.UnFinish);

            return isStart;
        }

        public static bool FlowHaveTask(string projectId, string flowCode)
        {
            return TaskWork.Any(e => e.RefID == projectId
                                             && e.TypeCode == flowCode);
        }

        public static string GetMainProjectFlowCode(string subFlowCode)
        {
            var flowCode = string.Empty;
            switch (subFlowCode)
            {
                case FlowCode.MajorLease:
                case FlowCode.MajorLease_LegalReview:
                case FlowCode.MajorLease_FinanceAnalysis:
                case FlowCode.MajorLease_ConsInfo:
                case FlowCode.MajorLease_Package:
                case FlowCode.MajorLease_ConsInvtChecking:
                case FlowCode.MajorLease_SiteInfo:
                    flowCode = FlowCode.MajorLease;
                    break;
                case FlowCode.Closure:
                case FlowCode.Closure_ClosurePackage:
                case FlowCode.Closure_ClosureTool:
                case FlowCode.Closure_ConsInvtChecking:
                case FlowCode.Closure_LegalReview:
                case FlowCode.Closure_WOCheckList:
                case FlowCode.Closure_ExecutiveSummary:
                    flowCode = FlowCode.Closure;
                    break;
                case FlowCode.TempClosure:
                case FlowCode.TempClosure_LegalReview:
                case FlowCode.TempClosure_ClosurePackage:
                    flowCode = FlowCode.TempClosure;
                    break;
                case FlowCode.Rebuild:
                case FlowCode.Rebuild_LegalReview:
                case FlowCode.Rebuild_FinanceAnalysis:
                case FlowCode.Rebuild_ConsInfo:
                case FlowCode.Rebuild_Package:
                case FlowCode.Rebuild_ConsInvtChecking:
                case FlowCode.Rebuild_GBMemo:
                case FlowCode.Rebuild_ReopenMemo:
                case FlowCode.Rebuild_TempClosureMemo:
                    flowCode = FlowCode.Rebuild;
                    break;
                case FlowCode.Renewal:
                case FlowCode.Renewal_Letter:
                case FlowCode.Renewal_LLNegotiation:
                case FlowCode.Renewal_ConsInfo:
                case FlowCode.Renewal_Tool:
                case FlowCode.Renewal_Analysis:
                case FlowCode.Renewal_ClearanceReport:
                case FlowCode.Renewal_ConfirmLetter:
                case FlowCode.Renewal_LegalApproval:
                case FlowCode.Renewal_Package:
                    flowCode = FlowCode.Renewal;
                    break;
                case FlowCode.Reimage:
                case FlowCode.Reimage_ConsInfo:
                case FlowCode.Reimage_Summary:
                case FlowCode.Reimage_Package:
                case FlowCode.Reimage_ConsInvtChecking:
                case FlowCode.Reimage_GBMemo:
                case FlowCode.Reimage_ReopenMemo:
                case FlowCode.Reimage_TempClosureMemo:
                    flowCode = FlowCode.Reimage;
                    break;
            }

            return flowCode;
        }
        public virtual string Edit()
        {
            throw new NotImplementedException();
        }

        public virtual BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            throw new NotImplementedException();
        }


        public void ChangeProjectTeamMembers(string projectId, ProjectTeamMembers projectTeamMembers)
        {
            using (var scope = new TransactionScope())
            {
                #region update ProjectUsers
                var projectUsers = ProjectUsers.Search(e => e.ProjectId == projectId).AsNoTracking().ToList();
                foreach (var projectUser in projectUsers)
                {
                    switch (projectUser.RoleCode)
                    {
                        case ProjectUserRoleCode.AssetActor:
                            if (projectTeamMembers.AssetActor != null)
                            {
                                projectUser.UserAccount = projectTeamMembers.AssetActor.UserAccount;
                                projectUser.UserNameENUS = projectTeamMembers.AssetActor.UserNameENUS;
                                projectUser.UserNameZHCN = projectTeamMembers.AssetActor.UserNameZHCN;
                            }
                            break;
                        case ProjectUserRoleCode.Finance:
                        case "Finance Team":
                            projectUser.UserAccount = projectTeamMembers.Finance.UserAccount;
                            projectUser.UserNameENUS = projectTeamMembers.Finance.UserNameENUS;
                            projectUser.UserNameZHCN = projectTeamMembers.Finance.UserNameZHCN;
                            break;
                        case ProjectUserRoleCode.Legal:
                            if (projectTeamMembers.Legal != null)
                            {
                                projectUser.UserAccount = projectTeamMembers.Legal.UserAccount;
                                projectUser.UserNameENUS = projectTeamMembers.Legal.UserNameENUS;
                                projectUser.UserNameZHCN = projectTeamMembers.Legal.UserNameZHCN;
                            }
                            break;
                        case ProjectUserRoleCode.PM:
                            projectUser.UserAccount = projectTeamMembers.PM.UserAccount;
                            projectUser.UserNameENUS = projectTeamMembers.PM.UserNameENUS;
                            projectUser.UserNameZHCN = projectTeamMembers.PM.UserNameZHCN;
                            break;
                        case ProjectUserRoleCode.AssetRep:
                            projectUser.UserAccount = projectTeamMembers.AssetRep.UserAccount;
                            projectUser.UserNameENUS = projectTeamMembers.AssetRep.UserNameENUS;
                            projectUser.UserNameZHCN = projectTeamMembers.AssetRep.UserNameZHCN;
                            break;
                        case ProjectUserRoleCode.AssetManager:
                            projectUser.UserAccount = projectTeamMembers.AssetMgr.UserAccount;
                            projectUser.UserNameENUS = projectTeamMembers.AssetMgr.UserNameENUS;
                            projectUser.UserNameZHCN = projectTeamMembers.AssetMgr.UserNameZHCN;
                            break;
                        case ProjectUserRoleCode.CM:
                            projectUser.UserAccount = projectTeamMembers.CM.UserAccount;
                            projectUser.UserNameENUS = projectTeamMembers.CM.UserNameENUS;
                            projectUser.UserNameZHCN = projectTeamMembers.CM.UserNameZHCN;
                            break;
                    }
                }

                ProjectUsers.Update(projectUsers.ToArray());


                #endregion

                #region update project tasks
                var projectTasks = TaskWork.Search(e => e.RefID == projectId
                                                    && e.Status == TaskWorkStatus.UnFinish
                                                    && e.ActivityName == "Start").AsNoTracking().ToList();

                foreach (var projectTask in projectTasks)
                {
                    UpdateProjectTaskUser(projectTask, projectTeamMembers);
                }

                UpdateProjectCreateUser(projectTeamMembers);

                TaskWork.Update(projectTasks.ToArray());
                #endregion

                #region update workflow tasks
                var workflowTasks = TaskWork.Search(e => e.RefID == projectId
                                                              && e.Status == TaskWorkStatus.UnFinish
                                                              && e.K2SN != null).ToList();

                ChangeProjectApprover(workflowTasks, projectTeamMembers);
                #endregion
                TaskWork.Update(workflowTasks.ToArray());

                scope.Complete();
            }


        }

        protected virtual void UpdateProjectCreateUser(ProjectTeamMembers projectTeamMembers)
        {

        }


        protected virtual void UpdateProjectTaskUser(TaskWork taskWork, ProjectTeamMembers projectTeamMembers)
        {
            throw new NotImplementedException();
        }

        public virtual void Finish(TaskWorkStatus status, TaskWork task)
        {

        }

        public ProjectStatus GetProjectStatus(string actionName)
        {
            ProjectStatus projectStatus;
            switch (actionName)
            {
                case ProjectAction.Recall:
                    projectStatus = ProjectStatus.Recalled;
                    break;
                case ProjectAction.Decline:
                    projectStatus = ProjectStatus.Rejected;
                    break;
                default:
                    projectStatus = ProjectStatus.UnFinish;
                    break;
            }
            return projectStatus;
        }

        public virtual void GenerateDefaultWorkflowInfo(string projectId)
        {

        }

        public virtual void PrepareTask(TaskWork taskWork)
        {
        }

        public static bool CheckIfShowRecallByPojectStatus(ProjectStatus status)
        {
            var isShowRecall = false;
            if (status != ProjectStatus.Finished
                && status != ProjectStatus.Rejected)
            {
                isShowRecall = true;
            }
            return isShowRecall;
        }

        public virtual void Recall(string comment)
        {
            throw new NotImplementedException();
        }

        public virtual HoldingStatus GetPackageHoldingStatus()
        {
            throw new NotImplementedException();
        }

        public void CompleteNotifyTask(string projectId)
        {
            var task = TaskWork.Search(e => e.RefID == projectId
                    && e.Status == TaskWorkStatus.UnFinish
                    && (e.TypeCode == WorkflowCode) && e.ActivityName == "Notify").FirstOrDefault();
            if (task != null)
            {
                task.Status = TaskWorkStatus.Finished;
                //string strTypeCode = task.TypeCode.Split('_')[1];
                //task.Url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", task.TypeCode.Split('_')[0], strTypeCode, projectId);
                TaskWork.Update(task);
            }
        }

        public static List<SimpleEmployee> GetPackageHoldingRoleUsers()
        {
            List<SimpleEmployee> roleUsers = null;
            var roleCodeStr = System.Configuration.ConfigurationManager.AppSettings["ReimagePackage_Hold_Role"];

            RoleCode roldCode;
            if (Enum.TryParse(roleCodeStr, out roldCode))
            {
                roleUsers = Employee.GetEmployeesByRole(roldCode);
            }

            return roleUsers;
        }

        public static string ParseActionName(string actionName)
        {
            string action;
            switch (actionName)
            {
                case "Confirm":
                    action = "Approve";
                    break;
                default:
                    action = actionName;
                    break;
            }

            return action;
        }

        public virtual string GetDesignStypleForSiteInfo()
        {
            return null;
        }
    }
    public abstract class BaseWFEntity<T> : BaseWFEntity where T : BaseWFEntity, new()
    {
        public virtual int Add()
        {
            var db = GetDb();
            try
            {
                db.Set(this.GetType()).Attach(this);
                db.Entry(this).State = EntityState.Added;
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteError(JsonConvert.SerializeObject(new { Desc = "BaseWFEntity Add Method Error", ex }));
                db.Set(this.GetType()).Add(this);
            }
            return db.SaveChanges();
        }

        public virtual int Update()
        {
            try
            {
                var db = GetDb();

                db.Set(this.GetType()).Attach(this);

                db.Entry(this).State = EntityState.Modified;
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteError(JsonConvert.SerializeObject(new { Desc = "BaseWFEntity Update Method Error", ex }));
                throw ex;
            }
        }

        public virtual int Delete()
        {
            var db = GetDb();
            try
            {
                db.Set(this.GetType()).Attach(this);
                db.Entry(this).State = EntityState.Deleted;
            }
            catch
            {
                db.Set(this.GetType()).Remove(this);
            }
            return db.SaveChanges();
        }
        /// <summary>
        /// 传入实体主键值，返回实体
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public static T Get(Guid key)
        {
            var db = PrepareDb();
            var entity = db.Set<T>().Find(key);
            return entity;
        }

        public static int Add(params T[] entities)
        {
            try
            {
                var db = PrepareDb();
                if (entities != null && entities.Any())
                {
                    db.Set<T>().AddRange(entities);
                    return db.SaveChanges();
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteError(JsonConvert.SerializeObject(new { Desc = "BaseWFEntity Add Method Error", ex }));
                throw ex;
            }
        }

        public static int Update(params T[] entities)
        {
            var db = PrepareDb();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                return db.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 传入实体主键值， 判断实体是否存在
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public static bool Any(Guid key)
        {
            var db = PrepareDb();
            return db.Set<T>().Find(key) != null;
        }

        /// <summary>
        /// 传入Lamda表达式， 判断数据库中是否存在
        /// </summary>
        /// <param name="keys">Lamda表达式</param>
        /// <returns></returns>
        public static bool Any(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            return db.Set<T>().Where(predicate).AsNoTracking().Any();
        }

        public static T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            var entity = db.Set<T>().AsNoTracking().FirstOrDefault(predicate);
            return entity;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalRecords">记录总数</param>
        /// <returns></returns>
        public static List<T> GetAll(int pageIndex, int pageSize, out int totalRecords)
        {
            var db = PrepareDb();
            totalRecords = db.Set<T>().Count();
            var entities = db.Set<T>().Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList();
            return entities;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entities">实体们</param>
        public static int Delete(params T[] entities)
        {
            var db = PrepareDb();
            if (entities != null && entities.Any())
            {
                db.Set<T>().RemoveRange(entities);
                return db.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids">实体主键们</param>
        public static int Delete(params Guid[] ids)
        {
            var db = PrepareDb();
            if (ids != null && ids.Count() > 0)
            {
                foreach (var entity in ids.Select(id => db.Set<T>().Find(id)))
                {
                    db.Set<T>().Remove(entity);
                }
                return db.SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public static int Count(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            return db.Set<T>().Count(predicate);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="Tkey">排序键</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderByPredicate">排序条件</param>
        /// <param name="IsDesc">是否倒序</param>
        /// <returns></returns>
        public static IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            var entities = db.Set<T>().Where(predicate);
            return entities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey">排序键</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderByPredicate">排序条件</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalRecords">记录总数</param>
        /// <param name="IsDesc">是否倒序</param>
        /// <returns></returns>
        public static IQueryable<T> Search<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageIndex, int pageSize, out int totalRecords, bool IsDesc = false) where Tkey : IComparable
        {
            var db = PrepareDb();
            var query = db.Set<T>();
            IOrderedQueryable<T> result;
            if (IsDesc)
            {
                result = query.Where(predicate).OrderByDescending(orderByPredicate);
            }
            else
            {
                result = query.Where(predicate).OrderBy(orderByPredicate);
            }
            totalRecords = query.Count(predicate);
            var entities = result.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return entities;
        }

        public static string GetRefTableId(string refTableName, string projectId)
        {
            var db = PrepareDb();
            return db.Database.SqlQuery<Guid>(string.Format(@"
                SELECT Id FROM {0} 
                WHERE ProjectId = '{1}'
                AND IsHistory = 0
            ", refTableName, projectId)).FirstOrDefault().ToString();
        }
        public void CopyAppUsers(string oldEntityId, string newEntityId)
        {
            var approveUser = ApproveDialogUser.GetApproveDialogUser(oldEntityId);
            Mapper.CreateMap<ApproveDialogUser, ApproveDialogUser>();
            var appUser = Mapper.Map<ApproveDialogUser>(approveUser);
            appUser.Id = Guid.Empty;
            appUser.RefTableID = newEntityId;
            appUser.LastUpdateDate = DateTime.Now;
            appUser.LastUpdateUserAccount = ClientCookie.UserCode;
            appUser.Save();
        }
        public void CopyAttachment(string oldEntityId, string newEntityId)
        {
            var attachments = Attachment.Search(e => e.RefTableID == oldEntityId
                                                              && e.RefTableName == TableName).AsNoTracking().ToList();


            var addAttachments = new List<Attachment>();
            foreach (var attachment in attachments)
            {
                var newAttachment = Duplicator.AutoCopy(attachment);
                newAttachment.RefTableID = newEntityId;
                newAttachment.ID = Guid.NewGuid();
                addAttachments.Add(newAttachment);
            }
            Attachment.Add(addAttachments.ToArray());
        }
        public void CopyReinvestmentCost(ReinvestmentCost reinCost, Guid newEntityId)
        {
            Mapper.CreateMap<ReinvestmentCost, ReinvestmentCost>();
            var entity = Mapper.Map<ReinvestmentCost>(reinCost);
            entity.Id = Guid.Empty;
            entity.ConsInfoID = newEntityId;
            entity.Save();
        }

        public void CopyReinvestmentBasicInfo(ReinvestmentBasicInfo info, Guid newEntityId)
        {
            Mapper.CreateMap<ReinvestmentBasicInfo, ReinvestmentBasicInfo>();
            var entity = Mapper.Map<ReinvestmentBasicInfo>(info);
            entity.Id = 0;
            entity.ConsInfoID = newEntityId;
            entity.Save();
        }

        public void CopyWriteOffAmount(WriteOffAmount writeOff, Guid newEntityId)
        {
            var entity = Mapper.Map<WriteOffAmount>(writeOff);
            entity.Id = Guid.Empty;
            entity.ConsInfoID = newEntityId;
            entity.Save();
        }
        public void CopyReinvestmentCost(Guid oldEntityId, Guid newEntityId)
        {
            var reinCost = ReinvestmentCost.GetByConsInfoId(oldEntityId);
            Mapper.CreateMap<ReinvestmentCost, ReinvestmentCost>();
            var entity = Mapper.Map<ReinvestmentCost>(reinCost);
            entity.Id = Guid.Empty;
            entity.ConsInfoID = newEntityId;
            entity.Save();
        }
        public void CopyWriteOffAmount(Guid oldEntityId, Guid newEntityId)
        {
            var writeOff = WriteOffAmount.GetByConsInfoId(oldEntityId);
            Mapper.CreateMap<WriteOffAmount, WriteOffAmount>();
            var entity = Mapper.Map<WriteOffAmount>(writeOff);
            entity.Id = Guid.Empty;
            entity.ConsInfoID = newEntityId;
            entity.Save();
        }
    }
}
