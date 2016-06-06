using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Transactions;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosureTool : BaseWFEntity<ClosureTool>
    {
        private ObjectCopy objCopy = new ObjectCopy();

        public List<ClosureToolImpactOtherStore> ImpactOtherStores { get; set; }

        public string Action
        {
            get;
            set;
        }
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_ClosureTool; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string Comments
        {
            get;
            set;
        }

        public string SN
        {
            get;
            set;
        }

        public string FinReportToAccount
        {
            get;
            set;
        }

        public string UserAccount
        {
            get;
            set;
        }

        public string UserNameENUS { get; set; }
        public string UserNameZHCN { get; set; }

        public const string TableName = "ClosureTool";
        public string UserName
        {
            get;
            set;
        }

        public static ClosureTool GetByProcInstID(int ProcInstID)
        {
            return FirstOrDefault(e => e.ProcInstID == ProcInstID);
        }


        public static int UpdateEntity(ClosureTool entity)
        {
            if (!entity.CreateTime.HasValue)
            {
                entity.CreateTime = DateTime.Now;
            }
            if (string.IsNullOrEmpty(entity.CreateUserAccount))
            {
                entity.CreateUserAccount = ClientCookie.UserCode;
            }

            entity.LastUpdateTime = DateTime.Now;
            entity.LastUpdateUserAccount = ClientCookie.UserCode;
            entity.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
            entity.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;

            if (entity.IsOptionOffered.HasValue && !entity.IsOptionOffered.Value)
            {
                entity.LeaseTerm = null;
                entity.Investment = null;
                entity.NPVRestaurantCashflows = null;
                entity.Yr1SOI = null;
                entity.IRR = null;
                entity.CompAssumption = null;
                entity.CashflowGrowth = null;
            }

            var result = Update(entity);

            var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);
            if (closureInfo != null)
                closureInfo.UpdateRelocation(entity.IsOptionOffered ?? false);

            var closurePackage = ClosurePackage.Get(entity.ProjectId);
            if (closurePackage != null)
                closurePackage.UpdateRelocation(entity.IsOptionOffered ?? false);

            return result;
        }

        public static ClosureTool GetById(Guid id)
        {
            return FirstOrDefault(e => e.Id == id);
        }

        public static ClosureTool Get(string projectId, string id = "")
        {
            ClosureTool entity = null;
            if (!string.IsNullOrEmpty(id))
            {
                entity = FirstOrDefault(e => e.Id == new Guid(id));
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                entity = FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            }
            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            }
            return entity;
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

        public int Save()
        {
            var result = 0;

            if (!Any(t => t.Id == this.Id))
            {
                this.IsHistory = false;
                this.CreateUserAccount = ClientCookie.UserCode;
                this.CreateTime = DateTime.Now;
                result = Add(this);
            }
            else
            {
                this.LastUpdateTime = DateTime.Now;
                this.LastUpdateUserAccount = ClientCookie.UserCode;
                this.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
                this.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
                result = Update(this);

            }
            return result;
        }

        public string EnableGenClosureTool(Guid id)
        {
            var _db = GetDb();

            var enable = true;
            string err = string.Empty;


            var closureTool = _db.ClosureTool.Find(id);
            var closureCurrentNode = NodeInfo.GetCurrentNode(closureTool.ProjectId, FlowCode.Closure_ClosureTool);
            var repInputNode = NodeInfo.GetNodeInfo(NodeCode.Closure_ClosureTool_RepInput);

            if (closureCurrentNode.Sequence < repInputNode.Sequence)
            {
                err = "Closure Tool 未审批！" + Environment.NewLine;
            }

            var wocheckCurrentNode = NodeInfo.GetCurrentNode(closureTool.ProjectId, FlowCode.Closure_WOCheckList);
            if (wocheckCurrentNode.Code != NodeCode.Finish)
            {
                err += "WOCheckList 未完成！" + Environment.NewLine;
            }

            return err;

        }

        public bool EnableReGenClosureTool()
        {
            if (this == null)
                return false;
            var projectInfo = ProjectInfo.Get(this.ProjectId, FlowCode.Closure_ClosureTool);
            if (projectInfo.Status != ProjectStatus.Finished)
                return false;
            return true;
        }

        public override string Edit()
        {
            if (!PreEdit(this.ProjectId))
                return "";
            var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            var store = StoreBasicInfo.GetStorInfo(closureEntity.USCode);
            var taskWork = new TaskWork();
            var source = FlowInfo.Get(FlowCode.Closure);
            var taskType = FlowInfo.Get(FlowCode.Closure_ClosureTool);
            taskWork.SourceCode = source.Code;
            taskWork.SourceNameZHCN = source.NameZHCN;
            taskWork.SourceNameENUS = source.NameENUS;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.Title = TaskWork.BuildTitle(this.ProjectId, store.NameZHCN, store.NameENUS);
            taskWork.RefID = this.ProjectId;
            taskWork.StoreCode = closureEntity.USCode;

            taskWork.TypeCode = taskType.Code;
            taskWork.TypeNameENUS = taskType.NameENUS;
            taskWork.TypeNameZHCN = taskType.NameZHCN;
            taskWork.ReceiverAccount = closureEntity.FinanceAccount;
            taskWork.ReceiverNameENUS = closureEntity.FinanceNameENUS;
            taskWork.ReceiverNameZHCN = closureEntity.FinanceNameZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            taskWork.Url = TaskWork.BuildUrl(FlowCode.Closure_ClosureTool, this.ProjectId, "");
            TaskWork.Add(taskWork);

            this.IsHistory = true;
            TaskWork.SetTaskHistory(this.Id, this.ProcInstID);
            this.Save();

            var objectCopy = new ObjectCopy();
            var newEntity = objectCopy.AutoCopy(this);
            newEntity.Id = Guid.NewGuid();
            newEntity.ProcInstID = 0;
            newEntity.IsHistory = false;
            newEntity.Save();

            //复制ImpactOtherStore信息
            var impactOtherStores = ClosureToolImpactOtherStore.Search(i => i.ClosureId == this.Id).AsNoTracking().ToArray();
            if (impactOtherStores.Length > 0)
            {
                foreach (var impactItem in impactOtherStores)
                {
                    impactItem.Id = Guid.NewGuid();
                    impactItem.ClosureId = newEntity.Id;
                }
                ClosureToolImpactOtherStore.Add(impactOtherStores);
            }

            ProjectInfo.Reset(this.ProjectId, FlowCode.Closure_ClosureTool);

            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString()
                                   && e.RefTableName == ClosureTool.TableName).AsNoTracking().ToList();

            var newList = new List<Attachment>();
            foreach (var att in attList)
            {
                var newAtt = objCopy.AutoCopy(att);
                newAtt.RefTableID = newEntity.Id.ToString();
                newAtt.ID = Guid.NewGuid();
                newList.Add(newAtt);
            }
            Attachment.AddList(newList);
            return taskWork.Url;
        }

        /// <summary>
        /// Edit准备
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public bool PreEdit(string projectId)
        {
            //如果流程已经到了ClosurePackage审批环节就不能Edit
            if (ProjectInfo.IsFlowStarted(projectId, FlowCode.Closure_ClosurePackage))
            {
                return false;
            }
            else
            {
                #region ExecutiveSummary撤回
                var needWidthDraw = TaskWork.Count(i => i.TypeCode == FlowCode.Closure_ExecutiveSummary && i.RefID == projectId && i.Status == TaskWorkStatus.UnFinish) > 0;
                if (needWidthDraw)
                {
                    //任务取消
                    var taskList = TaskWork.Search(i => i.TypeCode == FlowCode.Closure_ExecutiveSummary && i.RefID == projectId && i.Status != TaskWorkStatus.Cancel).ToArray();
                    foreach (var taskItem in taskList)
                    {
                        taskItem.Status = TaskWorkStatus.Cancel;
                    }
                    if (taskList.Length > 0)
                        TaskWork.Update(taskList);

                    //ExecutiveSummary数据isHistory置成true
                    var executiveSummary = ClosureExecutiveSummary.Search(i => i.ProjectId == projectId && i.IsHistory == false).ToArray();
                    foreach (var exItem in executiveSummary)
                    {
                        exItem.IsHistory = true;
                        exItem.LastUpdateTime = DateTime.Now;
                        exItem.LastUpdateUserAccount = ClientCookie.UserCode;
                        exItem.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
                        exItem.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
                    }
                    if (executiveSummary.Length > 0)
                        ClosureExecutiveSummary.Update(executiveSummary);
                }
                #endregion

                #region Package撤回
                needWidthDraw = TaskWork.Count(i => i.TypeCode == FlowCode.Closure_ClosurePackage && i.RefID == projectId && i.Status == TaskWorkStatus.UnFinish) > 0;
                if (needWidthDraw)
                {
                    //任务取消
                    var taskList = TaskWork.Search(i => i.TypeCode == FlowCode.Closure_ClosurePackage && i.RefID == projectId && i.Status != TaskWorkStatus.Cancel).ToArray();
                    foreach (var taskItem in taskList)
                    {
                        taskItem.Status = TaskWorkStatus.Cancel;
                    }
                    if (taskList.Length > 0)
                        TaskWork.Update(taskList);

                    //Package数据isHistory置成true
                    var package = ClosurePackage.Search(i => i.ProjectId == projectId && i.IsHistory == false).ToArray();
                    foreach (var pacItem in package)
                    {
                        pacItem.IsHistory = true;
                        pacItem.LastUpdateTime = DateTime.Now;
                        pacItem.LastUpdateUserAccount = ClientCookie.UserCode;
                    }
                    if (package.Length > 0)
                        ClosurePackage.Update(package);
                }
                #endregion

                return true;
            }
        }

        public string GetNodeName(string actionName)
        {
            string nodeCode = string.Empty;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Closure_ClosureTool_FinInput;
                    break;
                case ProjectAction.ReSubmit:
                    nodeCode = NodeCode.Closure_ClosureTool_FinApprove;
                    break;
                default:
                    nodeCode = NodeCode.Closure_ClosureTool_RepInput;
                    break;
            }
            return nodeCode;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:

                        //ClosureTool的最后一步在K2中，但不属于审批
                        task.ActionName = "";
                        task.Update();

                        ProjectInfo.FinishNode(this.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_Generate, ProjectStatus.Finished);
                        if (!ClosureExecutiveSummary.Any(i => i.ProjectId == this.ProjectId && i.IsHistory == false))
                        {
                            var esEntity = new ClosureExecutiveSummary();
                            esEntity.ProjectId = this.ProjectId;
                            esEntity.Id = Guid.NewGuid();
                            esEntity.CreateTime = DateTime.Now;
                            esEntity.CreatorAccount = ClientCookie.UserCode;
                            ClosureExecutiveSummary.Add(esEntity);
                        }
                        break;
                }

                scope.Complete();
            }

        }

        public static IQueryable<TaskWork> FilterTaskWork(IQueryable<TaskWork> taskWorks)
        {
            var exceptList = new List<Guid>();
            foreach (var taskWork in taskWorks)
            {
                TaskWork work = taskWork;
                if (work.TypeCode == FlowCode.Closure_ClosureTool
                    && work.ActivityName == "Asset Actor"
                    && !ProjectInfo.Any(e => e.ProjectId == work.RefID
                                                           && e.FlowCode == FlowCode.Closure_WOCheckList
                                                           && e.Status == ProjectStatus.Finished))
                {
                    exceptList.Add(work.Id);
                }
            }

            return taskWorks.Where(e => !exceptList.Contains(e.Id));
        }
    }
}
