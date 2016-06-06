/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:27:18 PM
 * FileName     :   RenewalPackage
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using System.Web;
using System.IO;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.Services.Common;
using System.Configuration;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalPackage : BaseWFEntity<RenewalPackage>
    {

        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Renewal\RenewPackage"; } }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_P"; }
        }

        public override string TableName
        {
            get
            {
                return "RenewalPackage";
            }
        }
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_Package;
            }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string WorkflowActOriginator
        {
            get
            {
                return "Originator";
            }
        }

        public override string[] WorkflowNormalActors
        {
            get
            {
                return new[] { this.WorkflowActOriginator, "Market Manager", "DD_GM_FC" };
            }
        }
        public ApproveUsers AppUsers { get; set; }
        public static RenewalPackage Get(string projectId, string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(new Guid(id));
            }
            else
            {
                return FirstOrDefault(l => l.ProjectId == projectId && l.IsHistory == false);
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

        public static RenewalPackage Create(string projectId, string createUserAccount, Guid analisysId, Guid toolId)
        {
            RenewalPackage entity = new RenewalPackage();
            entity.Id = Guid.NewGuid();
            entity.ProjectId = projectId;
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = createUserAccount;
            entity.AnalysisId = analisysId;
            entity.ToolId = toolId;
            entity.Add();
            return entity;
        }

        public static RenewalPackageDTO InitPage(string projectId, string id = null)
        {
            RenewalPackageDTO dto = new RenewalPackageDTO();
            var entity = RenewalPackage.Get(projectId, id);
            var project = ProjectInfo.Get(projectId, FlowCode.Renewal_Package);
            var info = RenewalInfo.Get(projectId);
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            entity.GenerateCover();
            var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Renewal_Package && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(entity.Id, "RenewalPackage", ClientCookie.UserCode);
            var projectNode = NodeInfo.GetNodeInfo(project.NodeCode);
            dto.Info = info;
            dto.Entity = entity;
            dto.Analysis = RenewalAnalysis.Get(entity.AnalysisId.Value);
            dto.FinMeasureOutput = RenewalToolFinMeasureOutput.GetByToolId(entity.ToolId.Value);
            dto.Uploadable = projectNode.Sequence >= 4 && ClientCookie.UserCode == dto.Info.AssetActorAccount;
            var editStatuses = new[] { ProjectStatus.Finished, ProjectStatus.Rejected };
            dto.ProjectComment = projectComment != null ? projectComment.Content : "";
            dto.Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_Package);
            dto.Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_Package);
            dto.Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_Package) && string.IsNullOrEmpty(id);
            if (entity.ProcInstId.HasValue)
            {
                var currentActivityName = K2FxContext.Current.GetCurrentActivityName(entity.ProcInstId.Value);
                dto.Rejectable = !entity.WorkflowNormalActors.Contains(currentActivityName);
            }
            dto.IsLindaLu = string.Compare(ClientCookie.UserCode, ConfigurationManager.AppSettings["AssetMgrCode"], true) == 0;
            return dto;
        }

        public void Save(string comment, Action onExecuting = null)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Status = ProjectCommentStatus.Submit;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                        ProjectCommentAction.Submit,
                        comment,
                        this.Id,
                        this.TableName,
                        FlowCode.Renewal,
                        null,
                        ProjectCommentStatus.Save
                    );
                }
                this.Update();
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            string destMgrs = AppUsers.MarketMgr.Code;
            if (AppUsers.RegionalMgr != null)
            {
                destMgrs += ";" + AppUsers.RegionalMgr.Code;
            }
            string dest_GMApprovers = "";

            if (AppUsers.MDD != null)
                dest_GMApprovers += AppUsers.MDD.Code + ";";

            if (AppUsers.GM != null)
                dest_GMApprovers += AppUsers.GM.Code + ";";

            if (AppUsers.FC != null)
                dest_GMApprovers += AppUsers.FC.Code + ";";

            if (AppUsers.RDD != null)
                dest_GMApprovers += AppUsers.RDD.Code + ";";

            if (dest_GMApprovers.Length > 1)
                dest_GMApprovers = dest_GMApprovers.Substring(0, dest_GMApprovers.Length - 1);

            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MarketMgr", destMgrs),
                new ProcessDataField("dest_GMApprovers",dest_GMApprovers),
                new ProcessDataField("dest_VPGM",AppUsers.VPGM.Code),
                new ProcessDataField("dest_MCCLAssetDir",AppUsers.MCCLAssetDtr.Code),
                new ProcessDataField("dest_CDO",AppUsers.CDO!= null?AppUsers.CDO.Code:""),
                new ProcessDataField("dest_MngDirector",AppUsers.ManagingDirector.Code),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
                new ProcessDataField("IsNeedCDOApproval",CheckIfNeedCDOApproval().ToString(),"BOOLEAN"),
                new ProcessDataField("IsNeedUpload",CheckIfNeedUpload().ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                /*Linda Lu*/
                processDataFields.Add(new ProcessDataField("dest_AssetRep", ConfigurationManager.AppSettings["AssetMgrCode"]));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        private bool CheckIfNeedUpload()
        {
            var analysis = RenewalAnalysis.Get(this.AnalysisId.Value);
            return analysis.RentDeviation.HasValue ? Math.Abs(analysis.RentDeviation.Value * 100) > 10 : false;
        }

        private bool CheckIfNeedCDOApproval()
        {
            var analysis = RenewalAnalysis.Get(this.AnalysisId.Value);
            return analysis.RentDeviation.HasValue ? Math.Abs(analysis.RentDeviation.Value * 100) > 10 : false;
        }

        public void Submit(string comment, Action onExecuting = null)
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Renewal, WorkflowCode);
            var dataFields = SetWorkflowDataFields(task);
            var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
            if (procInstId > 0)
            {
                using (var tranScope = new TransactionScope())
                {
                    task.Finish();
                    this.ProcInstId = procInstId;
                    this.CreateTime = DateTime.Now;
                    this.CreateUserAccount = ClientCookie.UserCode;
                    this.Update();
                    SaveApprovers();
                    var project = ProjectInfo.Get(this.ProjectId, this.WorkflowCode);
                    project.CreateUserAccount = ClientCookie.UserCode;
                    project.Update();
                    var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                    if (SavedComment != null)
                    {
                        SavedComment.Status = ProjectCommentStatus.Submit;
                        SavedComment.Content = comment;
                        SavedComment.CreateTime = DateTime.Now;
                        SavedComment.Update();
                    }
                    else
                    {
                        ProjectComment.AddComment(
                            ProjectCommentAction.Submit,
                            comment,
                            this.Id,
                            this.TableName,
                            FlowCode.Renewal,
                            procInstId,
                            ProjectCommentStatus.Submit
                        );
                    }
                    ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_Package_Input);
                    if (onExecuting != null)
                    {
                        onExecuting();
                    }
                    tranScope.Complete();
                }
            }
        }

        public void Approve(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Approve,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }

        public void Return(string comment, string SerialNumber)
        {

            TaskWork.Finish(e => e.RefID == ProjectId
                && e.TypeCode == WorkflowCode
                && e.Status == TaskWorkStatus.UnFinish);

            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Return", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Return,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }

        public void Reject(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, ProjectAction.Decline, comment);
            using (var tranScope = new TransactionScope())
            {
                ProjectInfo.Reject(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Decline,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }
        public void Resubmit(string comment, string SerialNumber, Action onExecuting = null)
        {
            var task = TaskWork.FirstOrDefault(t => t.RefID == this.ProjectId && t.TypeCode == this.WorkflowCode && t.ReceiverAccount == ClientCookie.UserCode);
            var dataFields = SetWorkflowDataFields(task);
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment, dataFields);
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                SaveApprovers();
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_Package_Input);
                ProjectComment.AddComment(
                    ProjectCommentAction.ReSubmit,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }
        public void Confirm(string serialNumber)
        {
            K2FxContext.Current.ApprovalProcess(serialNumber, ClientCookie.UserCode, "Approve", "upload Signed Renewal Contract");
        }
        public override void Recall(string comment)
        {
            K2FxContext.Current.GoToActivityAndRecord(
                this.ProcInstId.Value,
                this.WorkflowActOriginator,
                ClientCookie.UserCode,
                ProjectAction.Recall,
                comment
            );
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Recall,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode, ProjectStatus.Recalled);
                tranScope.Complete();
            }
        }
        public override string Edit()
        {
            using (var tranScope = new TransactionScope())
            {
                var info = RenewalInfo.Get(this.ProjectId);
                var entity = Duplicator.AutoCopy(this);
                entity.Id = Guid.NewGuid();
                entity.IsHistory = false;
                entity.CreateTime = DateTime.Now;
                entity.CreateUserAccount = ClientCookie.UserCode;
                entity.Add();
                this.IsHistory = true;
                this.Update();
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = entity.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());

                //var TypeCodes = new[] { FlowCode.Renewal_ContractInfo, FlowCode.Renewal_SiteInfo };
                //foreach (var typeCode in TypeCodes)
                //{
                //    var proj = ProjectInfo.Search(e => e.ProjectId == ProjectId && e.FlowCode == typeCode).FirstOrDefault();
                //    if (proj != null && proj.Status != ProjectStatus.Finished)
                //    {
                //        var oldTask = TaskWork.Search(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == typeCode).FirstOrDefault();
                //        if (oldTask != null)
                //        {
                //            oldTask.Status = TaskWorkStatus.Cancel;
                //            TaskWork.Update(oldTask);
                //        }
                //    }
                //}
                var task = info.GenerateSubmitTask(this.WorkflowCode);
                tranScope.Complete();
                return task.Url;
            }
        }

        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.Renewal_Package:
                        var package = RenewalPackage.Get(taskWork.RefID);
                        package.AppUsers = currApprover;
                        dataFields = package.SetWorkflowDataFields(null);
                        var packageApprovers =
                            prevApprovers.FirstOrDefault(e => e.FlowCode == FlowCode.Renewal_Package);
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
                            if (taskWork.ReceiverAccount == packageApprovers.MngDirectorCode
                                && packageApprovers.MngDirectorCode != currApprover.ManagingDirector.Code)
                            {
                                receiver = currApprover.ManagingDirector;
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
                                try
                                {
                                    RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, receiver.Code, dataFields);
                                }
                                catch (Exception ex)
                                {
                                    Log4netHelper.WriteError(string.Format("【Redirect Workflow Task Error】：{0}",
                                        JsonConvert.SerializeObject(taskWork)));
                                }
                                taskWork.ReceiverAccount = receiver.Code;
                                taskWork.ReceiverNameENUS = receiver.NameENUS;
                                taskWork.ReceiverNameZHCN = receiver.NameZHCN;
                            }
                            packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                            if (currApprover.RegionalMgr != null)
                            {
                                packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                            }
                            packageApprovers.GMCode = currApprover.GM.Code;
                            packageApprovers.FCCode = currApprover.FC.Code;
                            packageApprovers.MDDCode = currApprover.MDD.Code;
                            packageApprovers.VPGMCode = currApprover.VPGM.Code;
                            packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
                            if (currApprover.CDO != null)
                            {
                                packageApprovers.CDOCode = currApprover.CDO.Code;
                            }
                            packageApprovers.MngDirectorCode = currApprover.ManagingDirector.Code;
                            packageApprovers.Update();
                        }
                        break;
                }
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            var info = RenewalInfo.Get(this.ProjectId);
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    {
                        ProjectProgress.SetProgress(ProjectId, "90%");
                        var taskcontract = TaskWork.Search(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Renewal_ContractInfo).FirstOrDefault();
                        if (taskcontract == null)
                        {
                            info.GenerateSubmitTask(FlowCode.Renewal_ContractInfo);
                        }
                        var taskSiteinfo = TaskWork.Search(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Renewal_SiteInfo).FirstOrDefault();
                        if (taskSiteinfo == null)
                        {
                            info.GenerateSubmitTask(FlowCode.Renewal_SiteInfo);
                        }

                        ProjectInfo.CompleteMainIfEnable(ProjectId);
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_Package_Approval);
                    }
                    break;
                case TaskWorkStatus.K2ProcessDeclined:
                    {
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Renewal, ProjectStatus.Rejected);
                        ProjectInfo.Reject(ProjectId, FlowCode.Renewal_Package);
                    }
                    break;
            }
        }
        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var project = ProjectInfo.Get(this.ProjectId, FlowCode.Renewal_Package);
            var storeBasic = StoreBasicInfo.GetStorInfo(project.USCode);
            var storeContract = StoreContractInfo.Search(c => c.StoreCode == project.USCode).OrderByDescending(c => c.CreatedTime).FirstOrDefault();
            var info = RenewalInfo.Get(ProjectId);
            var flowInfo = FlowInfo.Get(FlowCode.Renewal);
            var analysis = RenewalAnalysis.Get(this.AnalysisId.Value) ?? new RenewalAnalysis();
            var finOutput = RenewalToolFinMeasureOutput.GetByToolId(this.ToolId.Value);
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            templateFileds.Add("WorkflowName", flowInfo.NameENUS);
            templateFileds.Add("ProjectID", ProjectId);
            templateFileds.Add("USCode", storeBasic.StoreCode);
            templateFileds.Add("Region", storeBasic.Region);
            templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
            templateFileds.Add("Market", storeBasic.Market);
            templateFileds.Add("City", storeBasic.CityZHCN);
            templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
            templateFileds.Add("StoreAge", Math.Floor((DateTime.Now - storeBasic.OpenDate).TotalDays / 365D).ToString());
            templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
            templateFileds.Add("CloseDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");

            if (storeContract != null)
                templateFileds.Add("CurrentLeaseENDYear", storeContract.EndYear);
            else
                templateFileds.Add("CurrentLeaseENDYear", "");

            templateFileds.Add("AssetsManager", info.AssetManagerNameENUS);
            templateFileds.Add("AssetsActor", info.AssetActorNameENUS);
            templateFileds.Add("AssetsRep", info.AssetRepNameENUS);
            templateFileds.Add("NewLeaseENDYear", info.NewLeaseEndDate.HasValue ? info.NewLeaseEndDate.Value.ToString("yyyy") : "");
            templateFileds.Add("ClosureDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");
            templateFileds.Add("FairMarketRentPerAppraisal", analysis.FairMarketRentAmount.HasValue ? DataConverter.ToMoney(analysis.FairMarketRentAmount.Value) : "");
            templateFileds.Add("LeaseTenureAndTerm", analysis.LeaseTenureAndTerm);
            templateFileds.Add("DR1stTYAmount", analysis.DR1stTYAmount.HasValue ? DataConverter.ToMoney(analysis.DR1stTYAmount.Value) : "");
            templateFileds.Add("FreeRentalPeriod", analysis.FreeRentalPeriod);
            templateFileds.Add("RentDeviation", analysis.RentDeviation.HasValue ? DataConverter.ToPercentage(analysis.RentDeviation.Value.ToString()) : "");
            templateFileds.Add("RentStructureUR", analysis.RentStructure);
            templateFileds.Add("MFStructureUR", analysis.MFStructureNew);
            if (finOutput != null)
            {
                templateFileds.Add("AnnualRentExpenseLY", DataConverter.ToMoney(finOutput.AnnualRentExpenseLY));
                templateFileds.Add("AnnualRentExpenseYr1", DataConverter.ToMoney(finOutput.AnnualRentExpenseLY));
                templateFileds.Add("AnnualRentExpenseAvg", DataConverter.ToMoney(finOutput.AnnualRentExpenseLY));
                templateFileds.Add("RentAsProdSalesLY", DataConverter.ToPercentage(finOutput.RentAsProdSalesLY));
                templateFileds.Add("RentAsProdSalesYr1", DataConverter.ToPercentage(finOutput.RentAsProdSalesYr1));
                templateFileds.Add("RentAsProdSalesAvg", DataConverter.ToPercentage(finOutput.RentAsProdSalesAvg));
                templateFileds.Add("OccupancyProdSalesLY", DataConverter.ToPercentage(finOutput.OccupancyProdSalesLY));
                templateFileds.Add("OccupancyProdSalesYr1", DataConverter.ToPercentage(finOutput.OccupancyProdSalesYr1));
                templateFileds.Add("OccupancyProdSalesAvg", DataConverter.ToPercentage(finOutput.OccupancyProdSalesAvg));
                templateFileds.Add("SOIProdSalesLY", DataConverter.ToPercentage(finOutput.SOIProdSalesLY));
                templateFileds.Add("SOIProdSalesYr1", DataConverter.ToPercentage(finOutput.SOIProdSalesYr1));
                templateFileds.Add("SOIProdSalesAvg", DataConverter.ToPercentage(finOutput.SOIProdSalesAvg));
                templateFileds.Add("CashROILY", DataConverter.ToPercentage(finOutput.CashROILY));
                templateFileds.Add("CashROIYr1", DataConverter.ToPercentage(finOutput.CashROIYr1));
                templateFileds.Add("CashROIAvg", DataConverter.ToPercentage(finOutput.CashROIAvg));
            }
            else
            {
                templateFileds.Add("AnnualRentExpenseLY", "");
                templateFileds.Add("AnnualRentExpenseYr1", "");
                templateFileds.Add("AnnualRentExpenseAvg", "");
                templateFileds.Add("RentAsProdSalesLY", "");
                templateFileds.Add("RentAsProdSalesYr1", "");
                templateFileds.Add("RentAsProdSalesAvg", "");
                templateFileds.Add("OccupancyProdSalesLY", "");
                templateFileds.Add("OccupancyProdSalesYr1", "");
                templateFileds.Add("OccupancyProdSalesAvg", "");
                templateFileds.Add("SOIProdSalesLY", "");
                templateFileds.Add("SOIProdSalesYr1", "");
                templateFileds.Add("SOIProdSalesAvg", "");
                templateFileds.Add("CashROILY", "");
                templateFileds.Add("CashROIYr1", "");
                templateFileds.Add("CashROIAvg", "");
            }

            return templateFileds;
        }
        public void GenerateCover()
        {
            if (!Attachment.Any(att => att.RefTableID == this.Id.ToString() && att.RequirementId == new Guid("9D1E247C-AAE0-44F6-869E-F50B3E633C1C")))
            {
                var info = RenewalInfo.Get(this.ProjectId);
                var store = StoreBasicInfo.GetStorInfo(info.USCode);
                var contract = StoreContractInfo.Get(info.USCode);
                var context = HttpContext.Current;
                var templateFileName = context.Server.MapPath("~/Template/RenewalCover_Template_v20130922.xlsx");
                var fileName = context.Server.MapPath(string.Format("~/UploadFiles/{0}.xlsx", Guid.NewGuid()));
                File.Copy(templateFileName, fileName);
                var fileInfo = new FileInfo(fileName);
                ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.RenewalCover);
                ExcelInputDTO excelInput = new ExcelInputDTO();
                excelInput.Region = store.RegionENUS;
                excelInput.Province = store.ProvinceZHCN;
                excelInput.City = store.CityENUS;
                excelInput.Market = store.MarketENUS;
                excelInput.StoreName = store.NameZHCN;
                excelInput.USCode = store.StoreCode;
                excelInput.OpenDate = store.OpenDate;
                excelInput.LeaseExpirationDate = contract.EndDate.Value;
                excelInput.Priority = info.Priority;
                excelDirector.Input(excelInput);
                var att = new Attachment();
                att.ID = Guid.NewGuid();
                att.RefTableID = this.Id.ToString();
                att.RefTableName = this.TableName;
                att.RelativePath = "/";
                att.TypeCode = "Cover";
                att.RequirementId = Guid.Parse("9D1E247C-AAE0-44F6-869E-F50B3E633C1C");
                att.Name = "Cover";
                att.Extension = fileInfo.Extension;
                att.InternalName = fileInfo.Name;
                att.ContentType = "application/vnd.ms-excel";
                att.Length = (int)fileInfo.Length;
                att.CreatorID = ClientCookie.UserCode;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreateTime = DateTime.Now;
                att.Add();
            }
        }
        private void SaveApprovers()
        {
            bool isNew = false;
            ApproveDialogUser approvers = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
            if (approvers == null)
            {
                approvers = new ApproveDialogUser();
                approvers.Id = Guid.NewGuid();
                approvers.ProjectId = this.ProjectId;
                approvers.FlowCode = this.WorkflowCode;
                approvers.RefTableID = this.Id.ToString();
                isNew = true;
            }
            approvers.MarketMgrCode = AppUsers.MarketMgr.Code;
            if (AppUsers.RegionalMgr != null)
            {
                approvers.RegionalMgrCode = AppUsers.RegionalMgr.Code;
            }
            approvers.MDDCode = AppUsers.MDD.Code;
            if (AppUsers.GM != null)
            {
                approvers.GMCode = AppUsers.GM.Code;
            }
            approvers.FCCode = AppUsers.FC.Code;
            approvers.VPGMCode = AppUsers.VPGM.Code;
            approvers.MCCLAssetDtrCode = AppUsers.MCCLAssetDtr.Code;
            if (AppUsers.CDO != null)
            {
                approvers.CDOCode = AppUsers.CDO.Code;
            }
            approvers.MngDirectorCode = AppUsers.ManagingDirector.Code;
            if (AppUsers.MCCLAssetMgr != null)
            {
                approvers.MCCLAssetMgrCode = AppUsers.MCCLAssetMgr.Code;
            }
            if (AppUsers.NecessaryNoticeUsers != null)
                approvers.NecessaryNoticeUsers = string.Join(";", AppUsers.NecessaryNoticeUsers.Select(u => u.Code).ToArray());
            if (AppUsers.NoticeUsers != null)
                approvers.NoticeUsers = string.Join(";", AppUsers.NoticeUsers.Select(u => u.Code).ToArray());
            if (isNew)
            {
                approvers.Add();
            }
            else
            {
                approvers.Update();
            }
        }

        public override void PrepareTask(TaskWork taskWork)
        {
            switch (taskWork.ActivityName)
            {
                case "Asset Rep Upload":
                    ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_Package_Approval);
                    break;
            }
        }
        
    }
}
