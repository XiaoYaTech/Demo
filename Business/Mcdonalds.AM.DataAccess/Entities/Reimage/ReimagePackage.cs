using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;
using Mcdonalds.AM.Services.Common;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimagePackage : BaseWFEntity<ReimagePackage>
    {
        public static string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Reimage_P"; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string WorkflowCode
        {
            get { return FlowCode.Reimage_Package; }
        }

        public Nullable<decimal> NetWriteOff { get; set; }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowReject { get; set; }
        public string LastUpdateUserAccount { get; set; }

        public string USCode { get; set; }
        public string SerialNumber { get; set; }
        public static string TableName { get { return "ReimagePackage"; } }

        public FinancialPreanalysis FinancialPreanalysis { get; set; }

        public ReimageInfo ReimInfo { get; set; }

        public bool IsShowSave { get; set; }

        public ReinvestmentBasicInfo ReinvestmentBasicInfo { get; set; }

        public static ReimagePackage Get(string projectId, string id = "")
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
        public static List<Attachment> GetPackageAgreementList(string projectId, string packageId, string uploadFilesURL)
        {
            string typeCode = "";
            string refTableId = "";
            var listReturn = new List<Attachment>();

            //var legal = ReimageLegalReview.GetLegalReviewInfo(projectId);
            //if (legal != null)
            //{
            //    refTableId = legal.Id.ToString();
            //    var listLegal = new List<Attachment>();
            //    var listTemp = Attachment.GetList("ReimageLegalReview", refTableId, typeCode);
            //    foreach (var item in listTemp)
            //    {
            //        if (item.TypeCode != "Contract")
            //        {
            //            item.FileURL = uploadFilesURL + "/" + item.InternalName;
            //            if (item.TypeCode == "LegalClearanceReport")
            //                item.Name = "Legal clearance report";
            //            else if (item.TypeCode == "Agreement")
            //            {
            //                item.Name = "Lease agreement";
            //            }
            //            else if (item.TypeCode == "Others")
            //            {
            //                item.Name = "Others（如往来函件，业主证明)";
            //            }
            //            listLegal.Add(item);
            //        }
            //    }
            //    listReturn.AddRange(listLegal);
            //}

            //var finance = new RebuildFinancAnalysis();
            //finance = finance.GetFinanceInfo(projectId, "");
            //if (finance != null)
            //{
            //    refTableId = finance.Id.ToString();
            //    var listFinance = new List<Attachment>();
            //    var listTemp = Attachment.GetList("RebuildFinancAnalysis", refTableId, "FinAgreement");
            //    foreach (var item in listTemp)
            //    {
            //        if (item.TypeCode != "Attachment")
            //        {
            //            item.FileURL = uploadFilesURL + "/" + item.InternalName;
            //            item.Name = "Finance Analysts";
            //            listFinance.Add(item);
            //        }
            //    }
            //    listReturn.AddRange(listFinance);
            //}

            var consinfo = new ReimageConsInfo();
            if (consinfo != null)
            {
                var listConsInfo = new List<Attachment>();
                refTableId = consinfo.Id.ToString();
                var listTemp = Attachment.GetList("ReimageConsInfo", refTableId, typeCode);
                foreach (var item in listTemp)
                {
                    if (item.TypeCode != "Attachment")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        if (item.TypeCode == "ConsInfoAgreement")
                            item.Name = "Store Layout";
                        else if (item.TypeCode == "ReinCost")
                        {
                            item.Name = "Additional investment list";
                        }
                        else if (item.TypeCode == "WriteOff")
                        {
                            item.Name = "Write-off details";
                        }
                        listConsInfo.Add(item);
                    }
                }
                listReturn.AddRange(listConsInfo);
            }

            var listPackage = new List<Attachment>();
            var listTempPackage = Attachment.GetList("ReimagePackage", packageId, typeCode);
            if (listTempPackage != null)
            {
                foreach (var item in listTempPackage)
                {
                    item.FileURL = uploadFilesURL + "/" + item.InternalName;
                    if (item.TypeCode == "SignedApproval")
                    {
                        item.Name = "Signed Approval";
                    }
                    else if (item.TypeCode == "SignedAgreement")
                    {
                        item.Name = "Signed Agreement";
                    }
                    else if (item.TypeCode == ExcelDataInputType.ReimageCover.ToString())
                    {
                        item.Name = "Cover";
                    }
                    listPackage.Add(item);
                }
                listReturn.AddRange(listPackage);
            }

            listReturn.Add(new Attachment()
            {
                Name = "Right sizing",
                TypeCode = "RightSizing",
                FileURL = "#",
            });
            listReturn.Add(new Attachment()
            {
                Name = "Facade, signage rendering",
                TypeCode = "FacadeSignageRendering",
                FileURL = "#",
            });
            if (!listReturn.Where(e => e.TypeCode == ExcelDataInputType.ReimageCover.ToString()).Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Cover",
                    TypeCode = "Cover",
                    FileURL = "#",
                });
            }
            if (!listReturn.Where(e => e.TypeCode == "SignedApproval").Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Signed Approval",
                    TypeCode = "SignedApproval",
                    FileURL = "#",
                });
            }
            if (!listReturn.Where(e => e.TypeCode == "SignedAgreement").Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Signed Agreement",
                    TypeCode = "SignedAgreement",
                    FileURL = "#",
                });
            }
            return listReturn;
        }
        public void ResubmitPackage(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/Reimage/Main#/Package?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (reimageInfo == null)
                {
                    throw new Exception("Could not find the Reimage Info, please check it!");
                }
                var task = reimageInfo.GenerateTaskWork(FlowCode.Reimage_Package,
                    "Reimage_Package",
                    "Reimage_Package",
                    taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);


                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == TableName).ToList();

                ProjectInfo.Reset(ProjectId, FlowCode.Reimage_Package);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Package_Input);
                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage_Package, ProjectStatus.UnFinish);
                var form = Duplicator.AutoCopy(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.CreateTime = DateTime.Now;
                form.Save("edit");

                List<Attachment> listAttachment = new List<Attachment>();
                foreach (var attachment in attachments)
                {
                    var newAttachment = Duplicator.AutoCopy(attachment);
                    newAttachment.RefTableID = form.Id.ToString();
                    newAttachment.ID = Guid.NewGuid();
                    listAttachment.Add(newAttachment);
                }
                Attachment.Add(listAttachment.ToArray());
                IsHistory = true;
                Update(this);
                scope.Complete();
            }

            return taskUrl;
        }
        public void RejectPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Decline, Comments);
        }

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            Log4netHelper.WriteInfo(JsonConvert.SerializeObject(new { desc = "finishtask", actionName, ProjectId, WorkflowCode }));
            if (actionName == ProjectAction.Return)
            {
                TaskWork.Finish(e => e.RefID == ProjectId
                    && e.TypeCode == WorkflowCode
                    && e.Status == TaskWorkStatus.UnFinish);
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, ParseActionName(actionName), comment, dataField);
            using (var scope = new TransactionScope())
            {
                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Package_Input);
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Package_Input, GetProjectStatus(actionName));
                        break;
                }


                scope.Complete();
            }

        }
        private string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                case ProjectAction.Decline:
                    nodeCode = NodeCode.Finish;
                    break;
                default:
                    nodeCode = NodeCode.Reimage_Package_Input;
                    break;
            }
            return nodeCode;
        }
        public void ApprovePackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }
        public void ReturnPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }

            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
                WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Package_Input);
                scope.Complete();
            }
        }

        public void Submit()
        {
            Save();
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Reimage, FlowCode.Reimage_Package);
            task.RefTableId = Id;
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Reimage/Main#/Package/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);
                Save("Submit");

                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_Package, NodeCode.Reimage_Package_Input);
                scope.Complete();
            }
        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var reimageInfo = ReimageInfo.GetReimageInfo(this.ProjectId);
            var storeInfo = StoreBasicInfo.GetStore(reimageInfo.USCode);
            StoreBasicInfo store = storeInfo.StoreBasicInfo;

            //生成Print文件
            var printDic = new Dictionary<string, string>();
            printDic.Add("WorkflowName", FlowCode.Reimage);
            printDic.Add("ProjectID", this.ProjectId);
            printDic.Add("USCode", reimageInfo.USCode);
            printDic.Add("StoreNameEN", store.NameENUS);
            printDic.Add("StoreNameCN", store.NameZHCN);
            printDic.Add("Region", store.RegionZHCN);
            printDic.Add("Market", store.MarketZHCN);
            printDic.Add("City", store.CityZHCN);
            printDic.Add("AddressZHCN", store.AddressZHCN);
            printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            printDic.Add("AssetsRep", storeInfo.StoreDevelop.AssetRepName);
            printDic.Add("AssetsActor", reimageInfo.AssetActorNameENUS);
            printDic.Add("StoreAge", (DateTime.Now.Year - store.OpenDate.Year).ToString());

            if (storeInfo.StoreContractInfo != null)
                printDic.Add("CurrentLeaseENDYear", storeInfo.StoreContractInfo.EndDate.HasValue ? storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd") : "");
            else
                printDic.Add("CurrentLeaseENDYear", "");

            var RmgSummaryentity = ReimageSummary.FirstOrDefault(e => e.ProjectId.Equals(this.ProjectId) && e.IsHistory == false);
            var financialPreanalysis = FinancialPreanalysis.FirstOrDefault(e => e.RefId.Equals(RmgSummaryentity.Id));
            var reimageConsInfo = ReimageConsInfo.GetConsInfo(ProjectId, "");
            var reinvestment = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
            var storePL = StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == RmgSummaryentity.Id);
            var reinCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
            var writeoff = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);

            printDic.Add("GBDate", reinvestment.GBDate.HasValue ? reinvestment.GBDate.Value.ToString("yyyy-MM-dd") : "");
            printDic.Add("ConsCompletionDate", reinvestment.ConsCompletionDate.HasValue ? reinvestment.ConsCompletionDate.Value.ToString("yyyy-MM-dd") : "");
            printDic.Add("ReopenDate", reinvestment.ReopenDate.HasValue ? reinvestment.ReopenDate.Value.ToString("yyyy-MM-dd") : "");
            printDic.Add("TotalSeatsNo", storeInfo.StoreSTLocation.TotalSeatsNo);
            printDic.Add("EstimatedSeatNo", reinvestment.EstimatedSeatNo);
            printDic.Add("RightSizingSeatNo", reinvestment.RightSizingSeatNo);
            printDic.Add("ProducingArea", storeInfo.StoreSTLocation.TotalArea);
            printDic.Add("KitchenArea", storeInfo.StoreSTLocation.KitchenArea);

            printDic.Add("NewAttachedKiosk", storeInfo.AttachedBeCount.ToString());
            printDic.Add("NewRemoteKiosk", storeInfo.RemoteBeCount.ToString());
            printDic.Add("NewMcCafe", storeInfo.MCCafeCount.ToString());
            printDic.Add("NewMDS", storeInfo.MDSBeCount.ToString());

            printDic.Add("DesignStyle", storeInfo.StoreSTLocation.DesignStyleDisplay);
            printDic.Add("NewDesignType", reinvestment.NewDesignTypeDisplay);

            printDic.Add("TTMSales", financialPreanalysis.TTMSales.HasValue ? DataConverter.ToMoney(financialPreanalysis.TTMSales.Value) : "");
            printDic.Add("TTMSOIPercent", storePL.TTMSOIPercent.HasValue ? DataConverter.ToPercentage((storePL.TTMSOIPercent / 100).ToString()) : "");

            if (financialPreanalysis.IsMcCafe.HasValue)
                printDic.Add("IsMcCafe", financialPreanalysis.IsMcCafe.Value ? "Yes" : "No");
            else
                printDic.Add("IsMcCafe", "No");
            printDic.Add("McCafe", DataConverter.ToPercentage(financialPreanalysis.McCafe));
            printDic.Add("TotalSalBldingInvstBudget", DataConverter.ToMoney(reinCost.TotalSalBldingInvstBudget));

            if (financialPreanalysis.IsKiosk.HasValue)
                printDic.Add("IsKiosk", financialPreanalysis.IsKiosk.Value ? "Yes" : "No");
            else
                printDic.Add("IsKiosk", "No");
            printDic.Add("Kiosk", DataConverter.ToPercentage(financialPreanalysis.Kiosk));
            printDic.Add("TotalNonSalBldingInvstBudget", DataConverter.ToMoney(reinCost.TotalNonSalBldingInvstBudget));

            if (financialPreanalysis.IsMDS.HasValue)
                printDic.Add("IsMDS", financialPreanalysis.IsMDS.Value ? "Yes" : "No");
            else
                printDic.Add("IsMDS", "No");
            printDic.Add("MDS", DataConverter.ToPercentage(financialPreanalysis.MDS));
            printDic.Add("TotalII", DataConverter.ToMoney(reinCost.TotalReinvestmentBudget));

            if (financialPreanalysis.IsTwientyFourHour.HasValue)
                printDic.Add("IsTwientyFourHour", financialPreanalysis.IsTwientyFourHour.Value ? "Yes" : "No");
            else
                printDic.Add("IsTwientyFourHour", "No");
            printDic.Add("TwientyFourHour", DataConverter.ToPercentage(financialPreanalysis.TwientyFourHour));
            printDic.Add("TotalWriteOff", DataConverter.ToMoney(writeoff.TotalWriteOff));

            printDic.Add("ReimagePrice", DataConverter.ToPercentage(financialPreanalysis.ReimagePrice));
            printDic.Add("ROI", DataConverter.ToPercentage(financialPreanalysis.ROI));

            printDic.Add("TotalSalesInc", DataConverter.ToPercentage(financialPreanalysis.TotalSalesInc));
            var paybackYears = financialPreanalysis.PaybackYears == null ? 0 : financialPreanalysis.PaybackYears.As<decimal>();
            printDic.Add("PaybackYears", Math.Round(paybackYears, 1).ToString("N1"));
            printDic.Add("CurrentPriceTier", financialPreanalysis.CurrentPriceTier);
            printDic.Add("PriceTierafterReimage", financialPreanalysis.PriceTierafterReimage);

            return printDic;
        }


        private int? StartProcess(TaskWork task)
        {
            CreateUserAccount = ClientCookie.UserCode;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount,
                processDataFields);
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            string dest_MarketMgr = "";

            if (AppUsers.MarketMgr != null)
                dest_MarketMgr += AppUsers.MarketMgr.Code;

            if (AppUsers.RegionalMgr != null)
                dest_MarketMgr += ";" + AppUsers.RegionalMgr.Code;
            var mdd_fc_do = new List<string>();
            mdd_fc_do.Add(AppUsers.MDD.Code);
            mdd_fc_do.Add(AppUsers.FC.Code);
            mdd_fc_do.Add(AppUsers.DO.Code);
            mdd_fc_do.Add(AppUsers.GM.Code);
            if (AppUsers.RDD != null)
            {
                mdd_fc_do.Add(AppUsers.RDD.Code);
            }


            bool isNeedCheckCDO = true;
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MarketMgr", dest_MarketMgr),
                new ProcessDataField("dest_VPGM",string.Join(";",AppUsers.VPGM.Code)),
                new ProcessDataField("dest_MDD_FAController_DO",string.Join(";",string.Join(";",mdd_fc_do.ToArray()))),
                new ProcessDataField("dest_CDO",string.Join(";",AppUsers.CDO.Code)),
                new ProcessDataField("dest_CFO",string.Join(";",AppUsers.CFO.Code)),
                new ProcessDataField("dest_MngDirector",string.Join(";",AppUsers.ManagingDirector.Code)),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
                new ProcessDataField("IsNeedCDOApproval",isNeedCheckCDO.ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        private bool CheckIfNeedCDOApproval()
        {
            if (NetWriteOff.HasValue)
            {
                if (NetWriteOff.Value >= Convert.ToDecimal(61.1 * 10000))
                    return true;
                return false;
            }
            return false;
        }

        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    CreateTime = DateTime.Now;
                    LastUpdateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;
                    IsHistory = false;
                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                var rmgInfo = ReimageInfo.GetReimageInfo(ProjectId);
                rmgInfo.Update();
                SaveApproveUsers(action);
                if (string.Compare(action, "edit", true) != 0)
                {
                    SaveComments(action);
                }
                scope.Complete();
            }
        }
        private void SaveApproveUsers(string actionName)
        {
            switch (actionName)
            {
                case ProjectAction.Submit:
                case ProjectAction.ReSubmit:
                    var approveUser = ApproveDialogUser.GetApproveDialogUser(Id.ToString());
                    if (approveUser == null)
                    {
                        approveUser = new ApproveDialogUser();
                        approveUser.Id = Guid.Empty;
                        approveUser.RefTableID = Id.ToString();
                        approveUser.ProjectId = ProjectId;
                        approveUser.FlowCode = FlowCode.Reimage_Package;
                    }
                    approveUser.MarketMgrCode = AppUsers.MarketMgr.Code;

                    if (AppUsers.RegionalMgr != null)
                    {
                        approveUser.RegionalMgrCode = AppUsers.RegionalMgr.Code;
                    }
                    approveUser.MDDCode = AppUsers.MDD.Code;
                    approveUser.GMCode = AppUsers.GM.Code;
                    approveUser.FCCode = AppUsers.FC.Code;
                    if (AppUsers.RDD != null)
                    {
                        approveUser.RDDCode = AppUsers.RDD.Code;
                    }

                    approveUser.VPGMCode = AppUsers.VPGM.Code;

                    if (AppUsers.CDO != null)
                    {
                        approveUser.CDOCode = AppUsers.CDO.Code;
                    }

                    if (AppUsers.CFO != null)
                    {
                        approveUser.CFOCode = AppUsers.CFO.Code;
                    }
                    if (AppUsers.ManagingDirector != null)
                    {
                        approveUser.MngDirectorCode = AppUsers.ManagingDirector.Code;
                    }

                    if (AppUsers.MCCLAssetMgr != null)
                    {
                        approveUser.MCCLAssetMgrCode = AppUsers.MCCLAssetMgr.Code;
                    }
                    if (AppUsers.MCCLAssetDtr != null)
                    {
                        approveUser.MCCLAssetDtrCode = AppUsers.MCCLAssetDtr.Code;
                    }
                    if (AppUsers.NecessaryNoticeUsers != null)
                        approveUser.NecessaryNoticeUsers = string.Join(";", AppUsers.NecessaryNoticeUsers.Select(u => u.Code).ToArray());
                    if (AppUsers.NoticeUsers != null)
                        approveUser.NoticeUsers = string.Join(";", AppUsers.NoticeUsers.Select(u => u.Code).ToArray());

                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }

        }

        public void GeneratePackageTask(string projectId)
        {

            if (CheckIfUnfreezePackageWorkflow(projectId))
            {
                if (!TaskWork.Any(e => e.RefID == projectId
                        && e.TypeCode == WorkflowCode
                        && (e.Status == TaskWorkStatus.UnFinish || e.Status == TaskWorkStatus.Holding)))
                {
                    var taskWork = new TaskWork();
                    taskWork.SourceCode = FlowCode.Reimage;
                    taskWork.SourceNameENUS = FlowCode.Reimage;
                    taskWork.SourceNameZHCN = FlowCode.Reimage;
                    taskWork.Status = TaskWorkStatus.Holding;
                    taskWork.StatusNameZHCN = "任务";
                    taskWork.StatusNameENUS = "任务";
                    taskWork.RefID = projectId;
                    taskWork.Id = Guid.NewGuid();
                    taskWork.CreateTime = DateTime.Now;
                    var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId);

                    if (reimageInfo == null)
                    {
                        throw new Exception("Cannot find the relative major lease info!");
                    }

                    taskWork.Title = TaskWork.BuildTitle(projectId, reimageInfo.StoreNameZHCN, reimageInfo.StoreNameENUS);
                    taskWork.TypeCode = WorkflowCode;
                    taskWork.TypeNameENUS = WorkflowCode;
                    taskWork.TypeNameZHCN = WorkflowCode;
                    taskWork.ReceiverAccount = reimageInfo.AssetActorAccount;
                    taskWork.ReceiverNameENUS = reimageInfo.AssetActorNameENUS;
                    taskWork.ReceiverNameZHCN = reimageInfo.AssetActorNameZHCN;
                    taskWork.Url = string.Format(@"/Reimage/Main#/Package?projectId={0}", projectId);
                    taskWork.StoreCode = reimageInfo.USCode;
                    taskWork.ActivityName = NodeCode.Start;
                    taskWork.ActionName = SetTaskActionName(projectId);
                    TaskWork.Add(taskWork);

                    //GeneratePackageHoldingTask(projectId, reimageInfo);
                }
            }

        }

        public override HoldingStatus GetPackageHoldingStatus()
        {
            var status = HoldingStatus.No;

            var changeLog = HoldingStatusChangeLog.FirstOrDefault(e => e.RefId == Id);
            if (changeLog != null)
            {
                status = changeLog.AfterStatus;
            }

            return status;
        }

        private void GeneratePackageHoldingTask(string projectId, ReimageInfo reimageInfo)
        {
            const string title = "有待呈递的Reimage Package需要处理";

            var users = GetPackageHoldingRoleUsers();

            foreach (var user in users)
            {
                Log4netHelper.WriteInfo("[[Holding]]:" + JsonConvert.SerializeObject(user));
                SimpleEmployee userLocal = user;
                var holdingTask = TaskWork.FirstOrDefault(e => e.Title == title
                                                    && e.TypeCode == WorkflowCode
                                                    && e.ReceiverAccount == userLocal.Code
                                                    && e.Status == TaskWorkStatus.UnFinish);
                if (holdingTask == null)
                {
                    var taskWork = new TaskWork();
                    taskWork.SourceCode = FlowCode.Reimage;
                    taskWork.SourceNameENUS = FlowCode.Reimage;
                    taskWork.SourceNameZHCN = FlowCode.Reimage;
                    taskWork.Status = TaskWorkStatus.UnFinish;
                    taskWork.StatusNameZHCN = "任务";
                    taskWork.StatusNameENUS = "任务";
                    taskWork.RefID = projectId;
                    taskWork.Id = Guid.NewGuid();
                    taskWork.CreateTime = DateTime.Now;

                    taskWork.Title = title;
                    taskWork.TypeCode = WorkflowCode;
                    taskWork.TypeNameENUS = WorkflowCode;
                    taskWork.TypeNameZHCN = WorkflowCode;
                    taskWork.ReceiverAccount = user.Code;
                    taskWork.ReceiverNameENUS = user.NameENUS;
                    taskWork.ReceiverNameZHCN = user.NameZHCN;
                    taskWork.Url = @"/Reimage/Main#/PackageList";
                    taskWork.StoreCode = reimageInfo.USCode;
                    taskWork.ActivityName = NodeCode.Start;
                    taskWork.ActionName = SetTaskActionName(projectId);
                    TaskWork.Add(taskWork);
                }
                else
                {
                    holdingTask.CreateTime = DateTime.Now;
                    TaskWork.Update(holdingTask);
                }

            }

        }

        public bool CheckIfUnfreezePackageWorkflow(string projectId)
        {
            var isUnfreeze = false;
            var flowCodeList = new List<string>()
            {
                FlowCode.Reimage_ConsInfo,
                FlowCode.Reimage_Summary
            };
            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId
                                                        && flowCodeList.Contains(e.FlowCode)).AsNoTracking().ToList();
            Log4netHelper.WriteInfo(JsonConvert.SerializeObject(projectInfos));

            if (projectInfos.Count == flowCodeList.Count
                && projectInfos.All(e => e.Status == ProjectStatus.Finished))
            {
                isUnfreeze = true;
            }

            return isUnfreeze;
        }
        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "ReimagePackage", ClientCookie.UserCode);
            if (comment != null)
            {
                comment.Content = Comments;
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.Update();
            }
            else
            {
                comment = new ProjectComment();
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.ActivityName = "";
                comment.Content = Comments;
                comment.CreateTime = DateTime.Now;
                comment.CreateUserAccount = ClientCookie.UserCode;
                comment.CreateUserNameENUS = ClientCookie.UserNameENUS;
                comment.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                comment.UserAccount = ClientCookie.UserCode;
                comment.UserNameENUS = ClientCookie.UserNameENUS;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "ReimagePackage";
                comment.SourceCode = FlowCode.Reimage;
                comment.SourceNameZHCN = FlowCode.Reimage;
                comment.SourceNameENUS = FlowCode.Reimage;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstID;
                comment.Add();
            }
        }
        public static ReimagePackage GetReimagePackageInfo(string strProjectId, string entityId = "")
        {
            ReimagePackage entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity == null)
            {
                entity = new ReimagePackage();
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            }
            else
            {
                //entity.ReinvestmentBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(entity.Id);


                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == strProjectId
                                                               && e.FlowCode == FlowCode.Reimage_Package);

                if (projectInfo != null)
                {
                    var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == strProjectId);

                    if (reimageInfo != null)
                    {
                        if (ClientCookie.UserCode.Equals(reimageInfo.AssetActorAccount))
                        {
                            var task = TaskWork.FirstOrDefault(e => e.RefID == strProjectId && e.TypeCode == FlowCode.Reimage_Package);
                            var isExistTask = TaskWork.Any(e => e.RefID == strProjectId
                                                                  && e.TypeCode == FlowCode.Reimage_Package
                                                                  && e.Status == TaskWorkStatus.UnFinish
                                                                  && e.ReceiverAccount == ClientCookie.UserCode
                                                                  && e.ActivityName == WorkflowActOriginator);

                            if (task != null)
                                entity.IsShowEdit = (projectInfo.Status == ProjectStatus.Finished || projectInfo.Status == ProjectStatus.Rejected) && !isExistTask && task.Status != TaskWorkStatus.Holding;
                            else
                                entity.IsShowEdit = false;
                            if (task != null)
                                entity.IsShowRecall = (projectInfo.Status != ProjectStatus.Finished && projectInfo.Status != ProjectStatus.Rejected) && !isExistTask && !isExistTask && task.Status != TaskWorkStatus.Holding;
                            else
                                entity.IsShowRecall = false;


                        }
                        var includeRejctActivityList = new List<string>()
                        {
                            "VPGM",
                            "Dev VP",
                            "CDO",
                            "CFO",
                            "Managing Director"
                        };

                        entity.IsShowReject = TaskWork.Any(
                                e => e.RefID == strProjectId
                                    && e.TypeCode == FlowCode.Reimage_Package
                                    && e.K2SN != null && e.Status == TaskWorkStatus.UnFinish
                                    && includeRejctActivityList.Contains(e.ActivityName));

                    }
                }
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Reimage;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = TableName;

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.ProjectComments = comments;

                    var saveComment =
                        comments.OrderByDescending(e => e.CreateTime)
                            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                    if (saveComment != null)
                    {
                        entity.Comments = saveComment.Content;
                    }
                }
            }
            var reimInfo = ReimageInfo.GetReimageInfo(strProjectId);
            entity.ReimInfo = reimInfo;
            //entity.TempClosureDate = rbdInfo.TempClosureDate;
            //entity.ReopenDate = rbdInfo.ReopenDate;
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            entity.USCode = reimInfo.USCode;
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Reimage_Package);

            var consInfo = ReimageConsInfo.CheckIfConsInfoFinished(strProjectId) ? ReimageConsInfo.GetConsInfo(strProjectId) : null;
            if (consInfo != null)
            {
                entity.ReinvestmentBasicInfo = consInfo.ReinBasicInfo;
            }


            var summary = ReimageSummary.GetReimageSummaryInfo(strProjectId);
            if (summary.FinancialPreanalysis != null)
            {
                entity.FinancialPreanalysis = summary.FinancialPreanalysis;
                if (consInfo != null
                    && consInfo.ReinCost != null)
                {
                    entity.FinancialPreanalysis.TotalReinvestmentNorm = consInfo.ReinCost.TotalReinvestmentNorm;
                }
            }

            PopulateAppUsers(entity);
            return entity;
        }

        private static void PopulateAppUsers(ReimagePackage entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.MarketMgrCode
                };
                entity.AppUsers.MarketMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RegionalMgrCode
                };
                entity.AppUsers.RegionalMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MDDCode
                };
                entity.AppUsers.MDD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.GMCode
                };
                entity.AppUsers.GM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.FCCode
                };
                entity.AppUsers.FC = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RDDCode
                };
                entity.AppUsers.RDD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.VPGMCode
                };
                entity.AppUsers.VPGM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CDOCode
                };
                entity.AppUsers.CDO = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CFOCode
                };
                entity.AppUsers.CFO = simp;
                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MDDCode
                };
                entity.AppUsers.ManagingDirector = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetMgrCode
                };
                entity.AppUsers.MCCLAssetMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetDtrCode
                };
                entity.AppUsers.MCCLAssetDtr = simp;

                if (!string.IsNullOrEmpty(approvedUsers.NecessaryNoticeUsers))
                {
                    var temp = approvedUsers.NecessaryNoticeUsers.Split(';');
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
                else
                {
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Reimage_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
                }

                if (!string.IsNullOrEmpty(approvedUsers.NoticeUsers))
                {
                    var temp = approvedUsers.NoticeUsers.Split(';');
                    entity.AppUsers.NoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
            }
            else
            {
                entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Reimage_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessDeclined:
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage, ProjectStatus.Rejected);
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage_Package, ProjectStatus.Rejected);
                        break;
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_Package, NodeCode.Reimage_Package_Examine, ProjectStatus.Finished);

                        var gbMemoTask = GenerateTaskWork(FlowCode.Reimage_GBMemo);
                        var rmgInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                        if (rmgInfo != null && rmgInfo.GBDate.HasValue)
                        {
                            ScheduleLog.GenerateTaskSchedule(rmgInfo.GBDate.Value.AddDays(-3), gbMemoTask, ClientCookie.UserCode, ProjectId, FlowCode.Reimage_GBMemo, rmgInfo.USCode);
                        }
                        //var reimageConsInvtChecking = new ReimageConsInvtChecking();
                        //TaskWork taskWork = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Reimage_ConsInvtChecking);
                        //if (taskWork == null)
                        //{
                        //    reimageConsInvtChecking.GenerateConsInvertTask(ProjectId);
                        //    var listTask = new List<TaskWork>
                        //    {
                        //        //GenerateTaskWork(FlowCode.Rebuild_ContractInfo),
                        //        //GenerateTaskWork(FlowCode.Rebuild_SiteInfo),
                        //        //GenerateTaskWork(FlowCode.Reimage_ReopenMemo),
                        //        GenerateTaskWork(FlowCode.Reimage_GBMemo),
                        //        //GenerateTaskWork(FlowCode.Rebuild_TempClosureMemo)
                        //    };
                        //    TaskWork.Add(listTask.ToArray());
                        //    //GenerateSiteInfoTask();
                        //}
                        break;
                }

                scope.Complete();
            }

        }

        public TaskWork GenerateTaskWork(string strTypeCode)
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Reimage;
            taskWork.SourceNameENUS = FlowCode.Reimage;
            taskWork.SourceNameZHCN = FlowCode.Reimage;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = ProjectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            var rmgInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);

            if (rmgInfo == null)
            {
                throw new Exception("Cannot find the relative Reimage info!");
            }

            taskWork.Title = TaskWork.BuildTitle(ProjectId, rmgInfo.StoreNameZHCN, rmgInfo.StoreNameENUS);
            taskWork.TypeCode = strTypeCode;
            taskWork.TypeNameENUS = strTypeCode;
            taskWork.TypeNameZHCN = strTypeCode;

            //if (strTypeCode == FlowCode.Rebuild_ContractInfo)
            //{
            //    taskWork.ReceiverAccount = rbdInfo.AssetActorAccount;
            //    taskWork.ReceiverNameENUS = rbdInfo.AssetActorNameENUS;
            //    taskWork.ReceiverNameZHCN = rbdInfo.AssetActorNameZHCN;
            //    taskWork.Url = string.Format(@"/Rebuild/Main#/ContractInfo/Process/Approval?action={0}&projectId={1}",
            //        "Confirm", ProjectId);
            //}
            //if (strTypeCode == FlowCode.Rebuild_SiteInfo)
            //{
            //    taskWork.ReceiverAccount = rmgInfo.PMAccount;
            //    taskWork.ReceiverNameENUS = rmgInfo.PMNameENUS;
            //    taskWork.ReceiverNameZHCN = rmgInfo.PMNameZHCN;
            //    taskWork.Url = string.Format(@"/Reimage/Main#/SiteInfo/Process/Approval?action={0}&projectId={1}",
            //        "Confirm", ProjectId);
            //}
            if (strTypeCode == FlowCode.Reimage_ReopenMemo)
            {
                taskWork.ReceiverAccount = rmgInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = rmgInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = rmgInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/Reimage/Main#/ReopenMemo?action={0}&projectId={1}",
                    "Confirm", ProjectId);
            }
            else if (strTypeCode == FlowCode.Reimage_GBMemo)
            {
                taskWork.ReceiverAccount = rmgInfo.PMAccount;
                taskWork.ReceiverNameENUS = rmgInfo.PMNameENUS;
                taskWork.ReceiverNameZHCN = rmgInfo.PMNameZHCN;
                taskWork.Url = string.Format(@"/Reimage/Main#/GBMemo?action={0}&projectId={1}",
                    "Confirm", ProjectId);
            }
            //else if (strTypeCode == FlowCode.Rebuild_TempClosureMemo)
            //{
            //    taskWork.ReceiverAccount = rbdInfo.AssetActorAccount;
            //    taskWork.ReceiverNameENUS = rbdInfo.AssetActorNameENUS;
            //    taskWork.ReceiverNameZHCN = rbdInfo.AssetActorNameZHCN;
            //    taskWork.Url = string.Format(@"/Rebuild/Main#/TempClosureMemo/Process/Approval?action={0}&projectId={1}",
            //        "Confirm", ProjectId);
            //}
            taskWork.StoreCode = rmgInfo.USCode;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            return taskWork;
        }

        public void GenerateSiteInfoTask()
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Reimage;
            taskWork.SourceNameENUS = FlowCode.Reimage;
            taskWork.SourceNameZHCN = FlowCode.Reimage;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = ProjectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;

            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (reimageInfo == null)
            {
                throw new Exception("Cannot find the relative reimage info!");
            }

            taskWork.Title = TaskWork.BuildTitle(ProjectId, reimageInfo.StoreNameZHCN, reimageInfo.StoreNameENUS);
            taskWork.TypeCode = FlowCode.Reimage_SiteInfo;
            taskWork.TypeNameENUS = FlowCode.Reimage_SiteInfo;
            taskWork.TypeNameZHCN = FlowCode.Reimage_SiteInfo;
            taskWork.ReceiverAccount = reimageInfo.PMAccount;
            taskWork.ReceiverNameENUS = reimageInfo.PMNameENUS;
            taskWork.ReceiverNameZHCN = reimageInfo.PMNameZHCN;
            taskWork.Url = string.Format(@"/Reimage/Main#/SiteInfo/Process/Approval?projectId={0}&action={1}", ProjectId, "Confirm");
            taskWork.StoreCode = reimageInfo.USCode;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            TaskWork.Add(taskWork);
        }

        public void CompleteActorPackageTask(string actor)
        {
            var task = TaskWork.FirstOrDefault(e => e.RefID == ProjectId
                                                    && e.TypeCode == WorkflowCode
                                                    && e.ReceiverAccount == actor
                                                    && e.Status == TaskWorkStatus.UnFinish);
            if (task != null)
            {
                task.Status = TaskWorkStatus.Finished;
                TaskWork.Update(task);
            }
        }

        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var package = new ReimagePackage();
            package.Id = Guid.NewGuid();
            package.IsHistory = false;
            package.ProjectId = projectId;
            package.CreateTime = DateTime.Now;
            package.Add();
        }

        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            var packageApprovers =
                                prevApprovers.FirstOrDefault(e => e.FlowCode == WorkflowCode
                                                                && e.RefTableID == Id.ToString());


            foreach (var taskWork in taskWorks.Where(e => e.TypeCode == WorkflowCode).ToList())
            {
                List<ProcessDataField> dataFields;
                AppUsers = currApprover;
                dataFields = SetWorkflowDataFields(null);

                if (packageApprovers != null)
                {
                    SimpleEmployee receiver = null;
                    if (taskWork.ReceiverAccount == packageApprovers.MarketMgrCode
                        && currApprover.MarketMgr != null
                        && packageApprovers.MarketMgrCode != currApprover.MarketMgr.Code)
                    {
                        receiver = currApprover.MarketMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RegionalMgrCode
                        && currApprover.RegionalMgr != null
                        && packageApprovers.RegionalMgrCode != currApprover.RegionalMgr.Code)
                    {
                        receiver = currApprover.RegionalMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.MDDCode
                        && currApprover.MDD != null
                         && packageApprovers.MDDCode != currApprover.MDD.Code)
                    {
                        receiver = currApprover.MDD;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.GMCode
                        && currApprover.GM != null
                         && packageApprovers.GMCode != currApprover.GM.Code)
                    {
                        receiver = currApprover.GM;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.FCCode
                        && currApprover.FC != null
                         && packageApprovers.FCCode != currApprover.FC.Code)
                    {
                        receiver = currApprover.FC;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RDDCode
                        && currApprover.RDD != null
                         && packageApprovers.RDDCode != currApprover.RDD.Code)
                    {
                        receiver = currApprover.RDD;
                    } if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
                        && currApprover.VPGM != null
                         && packageApprovers.VPGMCode != currApprover.VPGM.Code)
                    {
                        receiver = currApprover.VPGM;

                    } if (taskWork.ReceiverAccount == packageApprovers.MCCLAssetDtrCode
                        && currApprover.MCCLAssetDtr != null
                         && packageApprovers.MCCLAssetDtrCode != currApprover.MCCLAssetDtr.Code)
                    {
                        receiver = currApprover.MCCLAssetDtr;
                    }

                    if (taskWork.ReceiverAccount == packageApprovers.CDOCode
                        && currApprover.CDO != null
                         && packageApprovers.CDOCode != currApprover.CDO.Code)
                    {
                        receiver = currApprover.CDO;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.CFOCode
                        && currApprover.CFO != null
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
                        finally
                        {
                            taskWork.ReceiverAccount = receiver.Code;
                            taskWork.ReceiverNameENUS = receiver.NameENUS;
                            taskWork.ReceiverNameZHCN = receiver.NameZHCN;
                        }

                    }


                }
            }

            if (packageApprovers != null)
            {
                if (currApprover.MarketMgr != null)
                {
                    packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                }
                if (currApprover.RegionalMgr != null)
                {
                    packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                }
                if (currApprover.DD != null)
                {
                    packageApprovers.DDCode = currApprover.DD.Code;
                }

                if (currApprover.GM != null)
                {
                    packageApprovers.GMCode = currApprover.GM.Code;
                }

                if (currApprover.FC != null)
                {
                    packageApprovers.FCCode = currApprover.FC.Code;
                }

                if (currApprover.VPGM != null)
                {
                    packageApprovers.VPGMCode = currApprover.VPGM.Code;
                }

                if (currApprover.RDD != null)
                {
                    packageApprovers.RDDCode = currApprover.RDD.Code;
                }


                if (currApprover.MCCLAssetDtr != null)
                {
                    packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
                }

                if (currApprover.CDO != null)
                {
                    packageApprovers.CDOCode = currApprover.CDO.Code;
                }

                if (currApprover.CFO != null)
                {
                    packageApprovers.CFOCode = currApprover.CFO.Code;
                }

            }

        }


        public override void ChangePackageHoldingStatus(HoldingStatus status)
        {
            using (var scope = new TransactionScope())
            {

                var changeLog = HoldingStatusChangeLog.FirstOrDefault(e => e.RefId == Id);

                if (changeLog == null)
                {
                    changeLog = new HoldingStatusChangeLog()
                    {
                        AfterStatus = status,
                        ProjectId = ProjectId,
                        FlowCode = WorkflowCode,
                        RefId = Id
                    };
                }
                else
                {
                    changeLog.AfterStatus = status;
                }

                changeLog.Save();

                switch (status)
                {
                    case HoldingStatus.Yes:
                        {
                            var tasks = TaskWork.Search(e => e.RefID == ProjectId
                                                                && e.TypeCode == WorkflowCode
                                                                    && e.Status == TaskWorkStatus.Holding
                                                                    && e.ActivityName == NodeCode.Start);
                            TaskWork.Release(tasks.ToArray());
                        }

                        break;
                    case HoldingStatus.No:
                        {
                            var tasks = TaskWork.Search(e => e.RefID == ProjectId
                                                                && e.TypeCode == WorkflowCode
                                                             && e.Status == TaskWorkStatus.UnFinish
                                                             && e.ActivityName == NodeCode.Start);
                            if (tasks.Any())
                            {
                                TaskWork.Holding(tasks.ToArray());
                            }
                            else
                            {
                                //throw new Exception("Could not find any Reimage Package pending task, maybe it has been submited!");
                            }
                        }
                        break;
                }

                scope.Complete();
            }

        }
    }
}
