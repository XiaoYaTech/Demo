using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.Common.Excel;
using System.IO;
using System.Web;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimageSummary : BaseWFEntity<ReimageSummary>
    {
        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Reimage_RS"; }
        }

        public override string WorkflowActOriginator
        {
            get { return "Originator"; }
        }

        public override string WorkflowCode
        {
            get { return FlowCode.Reimage_Summary; }
        }

        public override string TableName
        {
            get { return "ReimageSummary"; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string PageType { get; set; }

        public string Comments { get; set; }

        public static ReimageSummary Get(string projectId, string id = "")
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

        public StoreProfitabilityAndLeaseInfo StoreProfitabilityAndLeaseInfo { get; set; }

        public FinancialPreanalysis FinancialPreanalysis { get; set; }

        public ReinvestmentBasicInfo ReinvestmentBasicInfo { get; set; }

        public ReimageConsInfo ReimageConsInfo { get; set; }


        public static ReimageSummary GetReimageSummaryInfo(string projectId, string id = "")
        {
            var entity = string.IsNullOrEmpty(id) ?
                Search(e => e.ProjectId.Equals(projectId) && !e.IsHistory).FirstOrDefault()
                : Search(e => e.Id.ToString().Equals(id)).FirstOrDefault();
            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId);

            if (entity != null)
            {
                //entity.FinancialPreanalysis = FinancialPreanalysis.FirstOrDefault(e => e.RefId == entity.Id);               
                //entity.StoreProfitabilityAndLeaseInfo =
                //    StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == entity.Id);
                //var reimageSummary = ReimageSummary.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
                //var store = ReinvestmentBasicInfo.GetByConsInfoId(reimageSummary.Id);
                //var storelease =
                //  StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == reimageSummary.Id);
                //var financial = FinancialPreanalysis.FirstOrDefault(e => e.RefId == reimageSummary.Id);
                //if (store != null || storelease != null || financial != null)
                //{
                //    entity.PageType = "save";
                //}




                entity.IsShowEdit = ProjectInfo.IsFlowEditable(projectId, FlowCode.Reimage_Summary);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Reimage_Summary);



                //var condition = new ProjectCommentCondition();
                //condition.SourceCode = FlowCode.Reimage;
                //condition.UserAccount = ClientCookie.UserCode;

                //condition.RefTableId = entity.Id;
                //condition.RefTableName = "ReimageSummary";

                //var comments = ProjectComment.SearchList(condition);
                //if (comments != null && comments.Count > 0)
                //{
                //    entity.ProjectComments = comments;

                //    var saveComment =
                //        comments.OrderByDescending(e => e.CreateTime)
                //            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                //    if (saveComment != null)
                //    {
                //        entity.Comments = saveComment.Content;
                //    }
                //}
            }
            else
            {
                entity = new ReimageSummary()
                {
                    ProjectId = projectId
                };


            }
            entity.FinancialPreanalysis = entity.getFinan(projectId, reimageInfo.USCode);
            entity.StoreProfitabilityAndLeaseInfo = entity.GetStore(projectId) == null ? new StoreProfitabilityAndLeaseInfo() : entity.GetStore(projectId);

            entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);

            if (ReimageConsInfo.CheckIfConsInfoFinished(projectId))
            {
                entity.ReimageConsInfo = ReimageConsInfo.GetConsInfo(projectId);

                entity.ReinvestmentBasicInfo = entity.ReimageConsInfo.ReinBasicInfo;
            }



            return entity;
        }



        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetReimageSummaryInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new ReimageSummary();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.IsHistory = false;

            entity.Add();
        }

        public override string Edit()
        {
            var taskUrl = string.Format("/Reimage/Main#/Summary?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (reimageInfo == null)
                {
                    throw new Exception("Could not find the Reimage Info, please check it!");
                }
                var task = reimageInfo.GenerateTaskWork(WorkflowCode,
                     "Reimage_Summary",
                     "Reimage_Summary",
                     taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);

                var package = ReimagePackage.GetReimagePackageInfo(ProjectId);
                if (package != null)
                {
                    package.CompleteActorPackageTask(reimageInfo.AssetActorAccount);
                }


                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == TableName).AsNoTracking().ToList();


                ProjectInfo.Reset(ProjectId, WorkflowCode);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Summary_Input);
                var wfEntity = GetWorkflowEntity(ProjectId, FlowCode.Reimage_Package);
                if (wfEntity != null)
                {
                    wfEntity.ChangePackageHoldingStatus(HoldingStatus.No);
                }

                var form = Duplicator.AutoCopy(this);
                form.Id = Guid.Empty;
                form.ProcInstId = null;
                form.IsHistory = false;
                form.CreateTime = DateTime.Now;
                form.Save("edit");

                var listAttachment = new List<Attachment>();
                Mapper.CreateMap<Attachment, Attachment>();
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

        public override void Recall(string comment)
        {
            if (!ProcInstId.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstId.Value,
                WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Summary_Input);
                scope.Complete();
            }
        }

        public Attachment GenerateExcel(out string filePath)
        {
            var att = GenerateAttachment(out filePath);

            var financialPreanalysis =
                    FinancialPreanalysis.FirstOrDefault(e => e.RefId.ToString().Equals(att.RefTableID));

            if (financialPreanalysis != null)
            {
                var excelHandler = new ExcelToolHandler(filePath, "PMT");

                financialPreanalysis.ROI = excelHandler.GetCellValue("E2").ToString();
                financialPreanalysis.PaybackYears = excelHandler.GetCellValue("E3").ToString();
                financialPreanalysis.MarginInc = excelHandler.GetCellValue("E4").ToString();

                financialPreanalysis.Update();

                FinancialPreanalysis = financialPreanalysis;

                excelHandler.Dispose();

            }

            return att;
        }

        public StoreProfitabilityAndLeaseInfo GetStore(string projectId)
        {
            string pageType = "";
            var store = new StoreProfitabilityAndLeaseInfo();
            var reimageSummary = FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            if (reimageSummary != null)
            {
                store =
                   StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == reimageSummary.Id);
            }
            if (store != null)
            {
                if (store.Id != 0)
                    pageType = "save";
            }
            if (pageType == "")
            {

                //var resultStoreAllInfo = storeBll.GetStoreDetailsByEID(eid, usCode);

                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId);
                var storeBasicInfo = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                //var storeContractInfo = StoreContractInfo.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                //int Year = 0;
                //if (storeContractInfo != null)
                //{

                //    if (storeContractInfo.EndDate != null)
                //    {
                //        DateTime dtNow = DateTime.Now;
                //        Year = int.Parse(storeContractInfo.EndDate.ToString().Split('/')[2].Substring(0, 4)) - dtNow.Year;
                //    }
                //}
                //var stFinanceData =
                //   Mcdonalds.AM.DataAccess.DataSync_LDW_AM_STFinanceData.OrderByDescending(e => e.FinanceYear == DateTime.Now.Year.ToString(),
                //       e => e.FinanceMonth).FirstOrDefault();
                //if (stFinanceData == null)
                //{
                //    throw new Exception("Could not get the latest finance month data!");
                //}
                //var latestMonth = stFinanceData.FinanceMonth;
                //var data = Mcdonalds.AM.DataAccess.DataSync_LDW_AM_STFinanceData.FirstOrDefault(e => e.FinanceYear == DateTime.Now.Year.ToString() && e.FinanceMonth == latestMonth && e.UsCode == stFinanceData.UsCode);

                //store.AsOf =DateTime.Parse(DateTime.Now.Year + "/" + latestMonth + "/01");
                //store.TTMSales = data.Total_Sales_TTM.As<decimal>();
                //store.TTMSOIPercent = data.SOIPct_TTM.As<decimal>();
                if (store == null)
                {
                    store = new StoreProfitabilityAndLeaseInfo();
                }
                DateTime? dt = storeBasicInfo.ReImageDate;
                if (dt != null)
                {
                    if (dt.ToString().Substring(0, 8) == "1/1/1900")
                    {
                        dt = null;
                    }
                }
                store.LastRemodelDate = dt;
                //store.RemainingLeaseYears = Year;
            }
            return store;
        }

        public FinancialPreanalysis getFinan(string projectId, string usCode)
        {
            var financialPreanalysis = FinancialPreanalysis.FirstOrDefault(e => e.RefId == Id);

            if (financialPreanalysis == null)
            {
                financialPreanalysis = new FinancialPreanalysis();
                //FinancialPreanalysis financial = reimageSummary.LoadFinancialPreanalysisInfo();

                var enti = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == true);
                if (enti == null)
                    enti = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == false);
                var writeOff = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == enti.Id);

                if (writeOff != null)
                {
                    financialPreanalysis.EstimatedWriteOffCost = writeOff.TotalII;
                }
                var contributionMargin = StoreCM.Get(usCode).ContributionMargin.ToString();
                var reimageConsInfo = ReimageConsInfo.GetConsInfo(projectId);
                var reinvestmentBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
                if (reinvestmentBasicInfo != null)
                {
                    //ReinvestmentCost reinCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == enti.Id);
                    financialPreanalysis.IsMcCafe = reinvestmentBasicInfo.NewMcCafe;

                    if ((reinvestmentBasicInfo.NewAttachedKiosk.HasValue && reinvestmentBasicInfo.NewAttachedKiosk.Value)
                        || (reinvestmentBasicInfo.NewRemoteKiosk.HasValue && reinvestmentBasicInfo.NewRemoteKiosk.Value))
                    {
                        financialPreanalysis.IsKiosk = true;
                    }
                    else
                    {
                        financialPreanalysis.IsKiosk = false;
                    }
                    financialPreanalysis.IsMDS = reinvestmentBasicInfo.NewMDS;
                    financialPreanalysis.IsTwientyFourHour = reinvestmentBasicInfo.NewTwientyFourHour;
                    financialPreanalysis.StoreCM = contributionMargin;

                    financialPreanalysis.GetNewAddBE();
                }

            }


            return financialPreanalysis;
        }
        public FinancialPreanalysis LoadFinancialPreanalysisInfo()
        {

            var stFinanceData =
                DataSync_LDW_AM_STFinanceData.OrderByDescending(e => e.FinanceYear == DateTime.Now.Year.ToString(),
                    e => e.FinanceMonth).FirstOrDefault();
            if (stFinanceData == null)
            {
                throw new Exception("Could not get the latest finance month data!");
            }
            var latestMonth = stFinanceData.FinanceMonth;
            using (var context = GetDb())
            {
                var query = from financeData in context.DataSync_LDW_AM_STFinanceData
                            join financeData2 in context.DataSync_LDW_AM_STFinanceData2
                                on new { financeData.UsCode, financeData.FinanceYear, financeData.FinanceMonth } equals
                                new { financeData2.UsCode, financeData2.FinanceYear, financeData2.FinanceMonth }
                            where financeData.FinanceMonth == latestMonth
                            select new
                            {
                                TTMSales = financeData == null ? "" : financeData.Total_Sales_TTM,
                                ROI = financeData2 == null ? "" : financeData2.C_ROI_TTM,
                                CurrentPriceTier = financeData2 == null ? "" : financeData2.Price_Tier
                            };

                var result = query.FirstOrDefault();

                return new FinancialPreanalysis()
                {
                    TTMSales = result.TTMSales.As<decimal>(),
                    ROI = result.ROI,
                    CurrentPriceTier = result.CurrentPriceTier
                };

            }
        }

        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (this.Id == Guid.Empty)
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
                    //Id = summary.Id;
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                if (ReinvestmentBasicInfo != null)
                {
                    ReinvestmentBasicInfo.ConsInfoID = Id;
                    ReinvestmentBasicInfo.Save();
                }

                if (StoreProfitabilityAndLeaseInfo != null)
                {
                    StoreProfitabilityAndLeaseInfo.RefId = Id;
                    StoreProfitabilityAndLeaseInfo.Save();
                }

                if (FinancialPreanalysis != null)
                {
                    FinancialPreanalysis.RefId = Id;
                    FinancialPreanalysis.Save();
                }

                scope.Complete();
            }
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Reimage, WorkflowCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            var taskUrl = "/Reimage/Main#/Summary/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstId = StartProcess(task);
            task.ProcInstID = ProcInstId;

            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);

                Save("Submit");

                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_Summary_Input);

                //var reimagePackage = new ReimagePackage();
                //reimagePackage.GeneratePackageTask(ProjectId);

                //var majorLeaseConsInvtChecking = new MajorLeaseConsInvtChecking();
                //majorLeaseConsInvtChecking.GenerateConsInvtCheckingTask(ProjectId);

                scope.Complete();
            }
        }

        private int StartProcess(TaskWork task)
        {
            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (reimageInfo == null)
            {
                throw new Exception("Could not find the Reimage Info, please check it!");
            }

            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode,
                processDataFields);

        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Creator", ClientCookie.UserCode),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        public bool IsExistReimageSummaryAttachment()
        {
            var reimageSummaryId = new Guid("5647BDBB-5B5F-47CD-9AD2-3E35A16DD303");
            return Attachment.Any(e => e.RefTableID == Id.ToString() &&
                e.RefTableName == TableName
                && e.RequirementId == reimageSummaryId);
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_Summary, NodeCode.Finish, ProjectStatus.Finished);

                        var reimagePackage = ReimagePackage.Get(ProjectId);
                        reimagePackage.GeneratePackageTask(ProjectId);
                        break;
                }

                scope.Complete();
            }

        }
        private Guid? _refId;
        public Attachment GenerateAttachment(out string filePath)
        {
            var info = ReimageInfo.FirstOrDefault(e => e.ProjectId == this.ProjectId);
            _refId = GetRefId(this.ProjectId);
            var storeInfo = StoreBasicInfo.GetStorInfo(info.USCode);
            string templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.Reimage_Summary_Template);
            string fileName = HttpContext.Current.Server.MapPath(string.Format("~/UploadFiles/{0}.xlsx", Guid.NewGuid()));
            var extetion = Path.GetExtension(fileName);
            var internalName = Path.GetFileName(fileName);
            File.Copy(templateFileName, fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.ReimageSummary);
            ExcelInputDTO excelDto = new ExcelInputDTO();
            excelDto.USCode = info.USCode;
            excelDto.ProjectId = info.ProjectId;
            excelDirector.Input(excelDto);
            var att = Attachment.FirstOrDefault(e => e.RefTableID == this.Id.ToString() && e.TypeCode == "ReimageSummary");
            bool hasAttach = true;
            if (att == null)
            {
                hasAttach = false;
                att = new Attachment();
                att.ID = Guid.NewGuid();
            }

            att.TypeCode = "ReimageSummary";
            att.RefTableID = this.Id.ToString();
            att.RefTableName = "ReimageSummary";
            att.Name = "Reimage Summary";
            att.Extension = extetion;
            att.RelativePath = "/";
            att.InternalName = internalName;
            att.ContentType = "application/vnd.ms-excel";
            att.Length = (int)fileInfo.Length;
            att.CreatorID = ClientCookie.UserCode;
            att.CreatorNameENUS = ClientCookie.UserNameENUS;
            att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
            att.CreateTime = DateTime.Now;
            att.RequirementId = new Guid("5647BDBB-5B5F-47CD-9AD2-3E35A16DD303");
            if (hasAttach)
            {
                att.Update();
            }
            else
            {
                att.Add();
            }
            //fileInfo = new FileInfo(fileName);
            //var importDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.FinancialPreAnalysis);
            //importDirector.FillEntityEvent += FillFinancialPreAnalysisEntity;
            //using (var scope = new TransactionScope())
            //{
            //    importDirector.ParseAndImport();
            //    scope.Complete();
            //}

            filePath = fileName;
            return att;
        }
        public Guid? GetRefId(string projectId)
        {
            Guid? refId = null;
            var reimageSummary = ReimageSummary.FirstOrDefault(e => e.ProjectId == projectId);
            if (reimageSummary != null && reimageSummary.Id != Guid.Empty)
            {
                refId = reimageSummary.Id;
            }
            return refId;
        }

        private void FillFinancialPreAnalysisEntity(BaseAbstractEntity entity)
        {

            var financialPreanalysis = entity as FinancialPreanalysis;
            if (financialPreanalysis != null)
            {
                var existFinancialPreanalysis =
                FinancialPreanalysis.FirstOrDefault(e => e.RefId.ToString().Equals(_refId.ToString()));
                if (existFinancialPreanalysis != null)
                {
                    financialPreanalysis.Id = existFinancialPreanalysis.Id;
                }
                else
                {
                    int id = FinancialPreanalysis.Search(e => true).Max(e => e.Id);
                    financialPreanalysis.Id = id + 1;
                }

                if (_refId == null)
                {
                    throw new Exception("ConsInfo Id is null, please check it!");
                }

                financialPreanalysis.RefId = _refId.Value;

            }
        }




    }
}
