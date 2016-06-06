using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Infrastructure;
using System.Transactions;


namespace Mcdonalds.AM.DataAccess
{

    public class ClosureWOCheckListHistory
    {
        public string ItemName { get; set; }

        public string ApproverZHCN { get; set; }

        public string ApproverENUS { get; set; }

        public DateTime? UploadDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }

    public partial class ClosureWOCheckList : BaseWFEntity<ClosureWOCheckList>
    {
        public new const string TableName = "ClosureWOCheckList";
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_WOCheckList; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public static ClosureWOCheckList Get(string projectId, string id = "")
        {
            ClosureWOCheckList entity = null;
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
            if (!Any(w => w.Id == this.Id))
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

        //        public WOCheckListHistory GetHistoryList(string projectId)
        //        {
        //            string sql = string.Format(@"SELECT TOP 1 tb_att.Name AS ItemName ,tb_att.CreateTime,tb_att.CreatorNameZHCN,tb_att.CreatorNameENUS,
        //            tb_task.ReceiverNameZHCN,tb_task.ReceiverNameENUS,tb_task.FinishTime
        //              FROM dbo.ClosureWOCheckList tb_wo
        //              INNER JOIN    dbo.TaskWork tb_task
        //              ON tb_wo.ProcInstID = tb_task.ProcInstID
        //              INNER JOIN dbo.Attachment tb_att
        //              ON tb_att.RefTableID = CAST(tb_wo.Id AS NVARCHAR(50))
        //              WHERE tb_wo.ProjectId = {0}
        //              ORDER BY tb_wo.ProcInstID  DESC, tb_task.CreateTime DESC", projectId);
        //            var list = this.SqlQuery<WOCheckListHistory>(sql, null).ToList();
        //            WOCheckListHistory result = null;
        //            if (list.Count > 0)
        //            {
        //                result = list[0];
        //            }
        //            return result;
        //        }

        //        public List<ClosureWOCheckListHistory> GetHistoryList(string projectId)
        //        {
        //            string sql = @"SELECT TOP 1 tb_att.Name AS itemName,tb_comment.UserNameZHCN AS ApproverZHCN
        //                ,tb_comment.UserNameENUS AS ApproverENUS,tb_wo.CreateTime AS UploadDate,
        //                tb_comment.CreateTime AS ApprovalDate
        //                 FROM dbo.ClosureWOCheckList tb_wo
        //                INNER JOIN dbo.Attachment tb_att
        //                ON tb_wo.Id  =tb_att.RefTableID
        //                INNER JOIN  dbo.ProjectComment tb_comment
        //                ON tb_comment.RefTableId = tb_wo.Id
        //                WHERE tb_att.RefTableName = 'ClosureWOCheckList'
        //                AND TypeCode = 'template'
        //                AND tb_wo.IsAvailable =0
        //                AND ProjectId = '" + projectId+"'";
        //            var list = this.SqlQuery<ClosureWOCheckListHistory>(sql,null)
        //        }

        public string Action
        {
            get;
            set;
        }

        public string USCode
        {
            get;
            set;
        }
        /// <summary>
        /// K2SN
        /// </summary>
        public string SN
        {
            get;
            set;
        }

        public string Comments
        {
            get;
            set;
        }


        public string UserAccount
        {
            get;
            set;
        }
        public string UserNameZHCN
        {
            get;
            set;
        }
        public string UserNameENUS
        {
            get;
            set;
        }
        public string PMSupervisor
        {
            get;
            set;
        }

        public string MCCLApprover
        {
            get;
            set;
        }

        public void GetFullInfo(ClosureWOCheckList entity)
        {

            if (entity != null)
            {
                ProjectCommentCondition condition = new ProjectCommentCondition();
                condition.RefTableId = entity.Id;
                condition.RefTableName = ClosureWOCheckList.TableName;
                condition.UserAccount = ClientCookie.UserCode;
                condition.Status = ProjectCommentStatus.Save;
                var commentList = ProjectComment.SearchList(condition);
                if (commentList != null && commentList.Count > 0)
                {
                    entity.Comments = commentList[0].Content;
                }
                var closureEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
                entity.USCode = closureEntity.USCode;
                entity.UserAccount = ClientCookie.UserCode;
            }
        }


        public int? ProcInstId
        {
            get;
            set;
        }

        public override string Edit()
        {
            if (!PreEdit(this.ProjectId))
                return "";
            var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            var store = StoreBasicInfo.GetStorInfo(closureEntity.USCode);
            var taskWork = new TaskWork();
            var source = FlowInfo.Get(FlowCode.Closure);
            var taskType = FlowInfo.Get(FlowCode.Closure_WOCheckList);
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
            taskWork.ReceiverAccount = closureEntity.PMAccount;
            taskWork.ReceiverNameENUS = closureEntity.PMNameENUS;
            taskWork.ReceiverNameZHCN = closureEntity.PMNameZHCN;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            taskWork.Url = TaskWork.BuildUrl(FlowCode.Closure_WOCheckList, this.ProjectId, "");
            taskWork.ActivityName = NodeCode.Start;
            TaskWork.Add(taskWork);

            this.IsHistory = true;
            this.RefreshClosureTool = false;
            //TaskWork.SetTaskHistory(this.Id, this.ProcInstID);

            this.Save();
            var objectCopy = new ObjectCopy();
            var newWo = objectCopy.AutoCopy(this);
            newWo.Id = Guid.NewGuid();
            newWo.ProcInstID = 0;
            newWo.Save();

            var projectEntity = ProjectInfo.Get(this.ProjectId, FlowCode.Closure_WOCheckList);

            ProjectInfo.UnFinishNode(this.ProjectId, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_Approve, ProjectStatus.UnFinish);
            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString()
                                   && e.RefTableName == ClosureWOCheckList.TableName);
            var objCopy = new ObjectCopy();
            var newList = new List<Attachment>();
            foreach (var att in attList)
            {
                var newAtt = objCopy.AutoCopy(att);
                newAtt.RefTableID = newWo.Id.ToString();
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

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(this.ProjectId, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_Approve, ProjectStatus.Finished);
                        //ProjectInfo.UpdateProjectStatus(this.ProjectId, FlowCode.Closure_WOCheckList, ProjectStatus.Finished);
                        //如果是从ClosurePackage Edit之后撤回Task的，添加ClosurePackage的Task
                        if (TaskWork.Count(i => i.RefID == task.RefID && i.TypeCode == FlowCode.Closure_ClosurePackage && i.Status == TaskWorkStatus.Cancel) > 0)
                        {
                            if (ProjectInfo.Any(e => e.ProjectId == task.RefID && e.Status == ProjectStatus.Finished && e.FlowCode == FlowCode.Closure_LegalReview))
                            {
                                var package = new ClosurePackage();
                                package.GeneratePackageTask(task.RefID);
                            }
                        }
                        //如果是从ExecutiveSummary Edit之后撤回Task的，添加ExecutiveSummary的Task
                        else if (TaskWork.Count(i => i.RefID == task.RefID && i.TypeCode == FlowCode.Closure_ExecutiveSummary && i.Status == TaskWorkStatus.Cancel) > 0)
                        {
                            var executiveSummary = new ClosureExecutiveSummary();
                            executiveSummary.GenerateExecutiveSummaryTask(task.RefID);
                        }
                        break;
                }

                scope.Complete();
            }

        }
    }
}
