using System;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Linq;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataModels.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using System.Collections.Generic;
using Mcdonalds.AM.Services.EmailServiceReference;
using System.Text;
using System.Configuration;
using Mcdonalds.AM.DataAccess.Common;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.Services.Controllers.Reimage
{
    public class ReimageController : ApiController
    {
        [Route("api/Reimage/CreateProject")]
        [HttpPost]
        public IHttpActionResult CreateProject(ReimageInfo entity)
        {

            using (var tran = new TransactionScope())
            {
                entity.Id = Guid.NewGuid();
                entity.ProjectId = ProjectInfo.CreateMainProject(FlowCode.Reimage, entity.USCode, NodeCode.Start, entity.CreateUserAccount);
                entity.CreateDate = DateTime.Now;
                entity.Add();
                entity.AddProjectUsers();
                entity.SendRemind();
                entity.SendWorkTask();
                entity.CreateSubProject();
                entity.CreateAttachmentsMemo();
                ProjectNode.GenerateOnCreate(FlowCode.Reimage, entity.ProjectId);
                tran.Complete();
            }

            return Ok(entity);


        }

        [Route("api/reimage/project/{projectId}")]
        public IHttpActionResult Get(string projectId)
        {
            return Ok(ReimageInfo.FirstOrDefault(c => c.ProjectId == projectId));
        }

        [HttpGet]
        [Route("api/Reimage/GetReimageInfo")]
        public IHttpActionResult GetReimageInfo(string projectId)
        {
            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId));
            if (reimageInfo != null)
            {
                reimageInfo.IsSiteInfoSaveable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Reimage_SiteInfo);

                var siteInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId)
                                                               && e.FlowCode == FlowCode.Reimage_SiteInfo);

                if (siteInfo != null)
                {
                    reimageInfo.SiteInfoId = siteInfo.Id;

                    var estimatedVsActualConstruction =
                        EstimatedVsActualConstruction.FirstOrDefault(e => e.RefId == siteInfo.Id);

                    if (estimatedVsActualConstruction == null)
                    {
                        var consInfo = ReimageConsInfo.GetConsInfo(projectId);
                        var reinBasicInfo = consInfo.ReinBasicInfo;
                        var gbMemo = ReimageGBMemo.GetGBMemo(projectId);
                        var storeInfo = StoreSTLocation.FirstOrDefault(e => e.StoreCode == siteInfo.USCode);

                        var summary = ReimageSummary.GetReimageSummaryInfo(projectId);
                        var afterReimagePriceTier = summary.FinancialPreanalysis != null
                            ? summary.FinancialPreanalysis.PriceTierafterReimage
                            : null;


                        estimatedVsActualConstruction = new EstimatedVsActualConstruction
                        {
                            RefId = siteInfo.Id,
                            GBDate = gbMemo.GBDate,
                            CompletionDate = gbMemo.ConstCompletionDate,
                            ARDC = reinBasicInfo.NewDesignType,
                            OriginalOperationSize = storeInfo.TotalArea,
                            OriginalSeatNumber = storeInfo.TotalSeatsNo,
                            ARPT = afterReimagePriceTier
                        };
                    }

                    reimageInfo.EstimatedVsActualConstruction = estimatedVsActualConstruction;
                }
            }

            return Ok(reimageInfo);
        }


        [HttpPost]
        [Route("api/Reimage/SubmitConsInfo")]
        public IHttpActionResult SubmitConsInfo(ReimageConsInfo entity)
        {
            try
            {
                entity.Submit();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/ApproveConsInfo")]
        public IHttpActionResult ApproveConsInfo(ReimageConsInfo entity)
        {
            // entity.ExecuteProcess(ClientCookie.UserCode, sn, "Approve", comment);
            entity.ApproveConsInfo(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/ReturnConsInfo")]
        public IHttpActionResult ReturnConsInfo(ReimageConsInfo entity)
        {
            entity.ReturnConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/RejectConsInfo")]
        public IHttpActionResult RejectConsInfo(ReimageConsInfo entity)
        {
            entity.RejectConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/UploadWriteOffTpl")]
        public IHttpActionResult UploadWriteOffTpl(string projectId)
        {
            var request = HttpContext.Current.Request;
            var fileCollection = request.Files;
            if (fileCollection.Count > 0)
            {

            }

            return Ok();
        }

        [HttpPost]
        [Route("api/Reimage/SaveConsInfo")]
        public IHttpActionResult SaveConsInfo(ReimageConsInfo consinfo)
        {
            try
            {
                consinfo.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/reimage/update")]
        [HttpPost]
        public IHttpActionResult UpdateReimageInfo(ReimageInfo entity)
        {
            entity.Update();
            return Ok();
        }

        [HttpPost]
        [Route("api/Reimage/ResubmitConsInfo")]
        public IHttpActionResult ResubmitConsInfo(ReimageConsInfo entity)
        {
            entity.ResubmitConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/RecallConsInfo")]
        public IHttpActionResult RecallConsInfo(ReimageConsInfo consinfo)
        {
            try
            {
                consinfo.Recall(consinfo.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/EditConsInfo")]
        public IHttpActionResult EditConsInfo(ReimageConsInfo consinfo)
        {
            try
            {
                var taskUrl = consinfo.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Reimage/GetConsInfo")]
        public IHttpActionResult GetConsInfo(string projectId, string entityId = "")
        {
            var entity = ReimageConsInfo.GetConsInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Reimage;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "ReimageConsInfo";

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

            return Ok(entity);

        }

        [HttpPost]
        [Route("api/Reimage/SubmitConsInvtChecking")]
        public IHttpActionResult SubmitConsInvtChecking(ReimageConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Reimage/ResubmitConsInvtChecking")]
        public IHttpActionResult ResubmitConsInvtChecking(ReimageConsInvtChecking entity)
        {
            entity.ResubmitConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }
        [HttpPost]
        [Route("api/Reimage/RecallConsInvtChecking")]
        public IHttpActionResult RecallConsInvtChecking(ReimageConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Recall(checkinfo.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Reimage/ReturnConsInvtChecking")]
        public IHttpActionResult ReturnConsInvtChecking(ReimageConsInvtChecking entity)
        {
            entity.ReturnConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }
        [HttpPost]
        [Route("api/Reimage/EditConsInvtChecking")]
        public IHttpActionResult EditConsInvtChecking(ReimageConsInvtChecking checkinfo)
        {
            try
            {
                var taskUrl = checkinfo.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Reimage/ApproveConsInvtChecking")]
        public IHttpActionResult ApproveConsInvtChecking(ReimageConsInvtChecking entity)
        {
            entity.ApproveConsInvtChecking(ClientCookie.UserCode);
            var app = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            if (app != null && app.FinanceControllerCode == ClientCookie.UserCode)
            {
                try
                {
                    SendEmail(entity.ProjectId, entity.SerialNumber, entity.ProcInstID, app.VPGMCode);
                }
                catch (Exception e)
                {
                }
            }
            return Ok(entity);
        }



        [HttpPost]
        [Route("api/Reimage/SaveConsInvtChecking")]
        public IHttpActionResult SaveConsInvtChecking(ReimageConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/Reimage/GetInvestCost")]
        public IHttpActionResult GetInvestCost(string refTableId)
        {
            ReinvestmentCost cost = null;
            if (!string.IsNullOrEmpty(refTableId))
                cost = ReinvestmentCost.GetByConsInfoId(new Guid(refTableId));
            return Ok(cost);
        }
        [HttpGet]
        [Route("api/Reimage/GetPackageInfo")]
        public IHttpActionResult GetPackageInfo(string projectId, string entityId = "")
        {
            //ReimagePackage
            var package = ReimagePackage.GetReimagePackageInfo(projectId, entityId);
            return Ok(package);
        }
        [HttpGet]
        [Route("api/Reimage/GetConsInvtChecking")]
        public IHttpActionResult GetConsInvtChecking(string projectId, string entityId = "")
        {
            var checking = new ReimageConsInvtChecking();
            if (!string.IsNullOrEmpty(projectId))
                checking = checking.GetConsInvtChecking(projectId, entityId);
            return Ok(checking);
        }
        //getReimageSummary
        [HttpGet]
        [Route("api/Reimage/GetReimageSummary")]
        public IHttpActionResult GetReimageSummary(string projectId)
        {
            var reimageSummary = new ReimageSummary();
            if (!string.IsNullOrEmpty(projectId))
                reimageSummary = ReimageSummary.Get(projectId);
            return Ok(reimageSummary);
        }
        [HttpPost]
        [Route("api/Reimage/SavePackage")]
        public IHttpActionResult SavePackage(ReimagePackage package)
        {
            try
            {
                package.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/ApprovePackage")]
        public IHttpActionResult ApprovePackage(ReimagePackage entity)
        {
            entity.ApprovePackage(ClientCookie.UserCode);
            var app = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            if (app != null && app.FinanceControllerCode == ClientCookie.UserCode)
            {
                try
                {
                    SendEmail(entity.ProjectId, entity.SerialNumber, entity.ProcInstID, app.VPGMCode);
                }
                catch (Exception e)
                {
                }
            }
            return Ok(entity);
        }
        [HttpPost]
        [Route("api/Reimage/ReturnPackage")]
        public IHttpActionResult ReturnPackage(ReimagePackage entity)
        {
            entity.ReturnPackage(ClientCookie.UserCode);

            return Ok(entity);
        }
        [HttpPost]
        [Route("api/Reimage/RecallPackage")]
        public IHttpActionResult RecallPackage(ReimagePackage package)
        {
            try
            {
                package.Recall(package.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private void SendEmail(string ProjectId, string SerialNumber, int? ProcInstID, string receiverUserCode)
        {
            var project = ProjectInfo.Get(ProjectId, FlowCode.Reimage_ConsInvtChecking);

            var storeBasic = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
            using (EmailServiceClient emailClient = new EmailServiceClient())
            {
                EmailMessage email = new EmailMessage();
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApproverName", ClientCookie.UserNameENUS);
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("WorkflowName", "Reimage");
                bodyValues.Add("StoreCode", storeBasic.StoreCode);
                bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
                bodyValues.Add("WorkflowName", Constants.Reimage_ConsInvtChecking);////--流程名称
                bodyValues.Add("ProjectName", Constants.Reimage);//项目名称
                var viewPage = string.Format("{0}/Reimage/Main#/ConsInvtChecking/Process/Approval?projectId={1}&SN={2}&ProcInstID={3}",
                    HttpContext.Current.Request.Url.AbsolutePath, ProjectId, SerialNumber, ProcInstID);
                bodyValues.Add("FormUrl", viewPage);
                email.EmailBodyValues = bodyValues;

                List<string> emailAddresses = Employee.Search(e => e.Code == receiverUserCode).Select(e => e.Mail).ToList();
                emailAddresses.Add("Stephen.Wang@nttdata.com");
                emailAddresses.Add("Poyet.chen@nttdata.com");
                emailAddresses.Add("Cary.chen@nttdata.com");
                email.To = string.Join(";", emailAddresses);
                emailClient.SendNotificationEmail(email);
            }
        }
        [HttpPost]
        [Route("api/Reimage/SubmitPackage")]
        public IHttpActionResult SubmitPackage(ReimagePackage package)
        {
            try
            {
                package.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteError(JsonConvert.SerializeObject(ex));
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Reimage/ResubmitPackage")]
        public IHttpActionResult ResubmitPackage(ReimagePackage entity)
        {
            entity.ResubmitPackage(ClientCookie.UserCode);

            return Ok(entity);
        }
        [HttpPost]
        [Route("api/Reimage/EditPackage")]
        public IHttpActionResult EditPackageInfo(ReimagePackage package)
        {
            try
            {
                var taskUrl = package.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Reimage/RejectPackage")]
        public IHttpActionResult RejectPackage(ReimagePackage entity)
        {
            entity.RejectPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpGet]
        [Route("api/Reimage/GetPackageAgreementList")]
        public IHttpActionResult GetPackageAgreementList(string projectId, string refTableId)
        {
            var list = ReimagePackage.GetPackageAgreementList(projectId, refTableId, SiteFilePath.UploadFiles_URL);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Reimage/GetKeyMeasuresInfo")]
        public IHttpActionResult GetKeyMeasuresInfo(string projectId)
        {
            var reimage = ReimageConsInfo.GetConsInfo(projectId, "");
            ReinvestmentCost reinCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == reimage.Id);
            var entity = ReimageSummary.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == false);
            var financialPreanalysis = new FinancialPreanalysis();
            if (entity != null)
                financialPreanalysis = FinancialPreanalysis.FirstOrDefault(e => e.RefId.Equals(entity.Id));
            if (reinCost != null)
                financialPreanalysis.TotalReinvestmentNorm = reinCost.TotalReinvestmentNorm;
            return Ok(financialPreanalysis);
        }

        [Route("api/SummaryReinvestmentCost/GetSummaryReinvestmentCost")]
        public IHttpActionResult GetSummaryReinvestmentCost(string projectId)
        {
            var reimageConsInfo = ReimageConsInfo.GetConsInfo(projectId, "");
            ReinvestmentCost rein = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
            WriteOffAmount writeoff = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);
            return Ok(new
            {
                Rein = rein,
                Writeoff = writeoff
            });
        }

        [Route("api/StoreProfitabilityAndLeaseInfo/GetStoreProfitabilityAndLeaseInfo")]
        public IHttpActionResult GetStoreProfitabilityAndLeaseInfo(string projectId, string pageType = "")
        {

            StoreProfitabilityAndLeaseInfo store = new StoreProfitabilityAndLeaseInfo();
            var reimageSummary = ReimageSummary.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
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
                var storeContractInfo = StoreContractInfo.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                int Year = 0;
                if (storeContractInfo != null)
                {

                    if (storeContractInfo.EndDate != null)
                    {
                        DateTime dtNow = DateTime.Now;
                        Year = int.Parse(storeContractInfo.EndDate.ToString().Split('/')[2].Substring(0, 4)) - dtNow.Year;
                    }
                }
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
                store.RemainingLeaseYears = Year;
            }
            return Ok(store);
        }


        [Route("api/StoreProfitabilityAndLeaseInfo/GetSelectYearMonth")]
        public IHttpActionResult GetSelectYearMonth(string projectId)
        {
            using (var amdb = new McdAMEntities())
            {
                var selectItemList = new List<SelectItem>();
                var uscode = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId).USCode;

                var reimageSummary = ReimageSummary.GetReimageSummaryInfo(projectId);
                if (reimageSummary != null)
                {
                    var refId = reimageSummary.Id;
                    var storeProfitabilityAndLeaseInfo =
                        StoreProfitabilityAndLeaseInfo.FirstOrDefault(e => e.RefId == refId);

                    var asOf = storeProfitabilityAndLeaseInfo != null ? storeProfitabilityAndLeaseInfo.AsOf : string.Empty;
                    var yearMonthList = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == uscode).Select(i => new { financeYearMonth = i.FinanceYear + "-" + i.FinanceMonth }).Distinct().OrderByDescending(i => i.financeYearMonth).Take(12).ToList();
                    var ldw_financeData = LDW_FinanceData.Get(projectId);
                    foreach (var _yearMonth in yearMonthList)
                    {
                        var selectItem = new SelectItem();
                        selectItem.name = _yearMonth.financeYearMonth;
                        selectItem.value = _yearMonth.financeYearMonth;
                        if (ldw_financeData != null)
                            selectItem.selected = _yearMonth.financeYearMonth == ldw_financeData.FinanceYear + "-" + ldw_financeData.FinanceMonth;
                        else
                            // selectItem.selected = false;
                            selectItem.selected = _yearMonth.financeYearMonth == asOf;
                        selectItemList.Add(selectItem);
                    }
                    if (string.IsNullOrEmpty(asOf) && selectItemList.Count > 0)
                        selectItemList[0].selected = true;
                }

                return Ok(new { data = selectItemList });
            }
        }

        private class SelectItem
        {
            public string name;
            public string value;
            public bool selected;
        }
        public decimal CovertToDecimalPercent(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }
            else
            {
                data = data.Trim().TrimEnd('%').Replace(",", "");
                decimal r = 0;
                decimal.TryParse(data, out r);
                return r * 100;
            }
        }

        [Route("api/financialPreAnalysis/GetFinancialPreAnalysis")]
        public IHttpActionResult GetFinancialPreAnalysis(string projectId, string usCode, string pageType = "")
        {
            var financial = new FinancialPreanalysis();
            var reimageSum = ReimageSummary.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            if (reimageSum != null)
            {
                financial = FinancialPreanalysis.FirstOrDefault(e => e.RefId == reimageSum.Id);
            }
            if (financial != null)
            {
                if (financial.Id != 0)
                    pageType = "save";
            }
            if (pageType == "")
            {
                //FinancialPreanalysis financial = reimageSummary.LoadFinancialPreanalysisInfo();

                var enti = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == true);
                if (enti == null)
                    enti = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == false);
                WriteOffAmount writeOff = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == enti.Id);
                if (financial == null)
                {
                    financial = new FinancialPreanalysis();
                }
                if (writeOff != null)
                {
                    financial.EstimatedWriteOffCost = writeOff.TotalII;

                }
                var ContributionMargin = StoreCM.Get(usCode).ContributionMargin.ToString();
                var reimageConsInfo = ReimageConsInfo.GetConsInfo(projectId, "");
                var reinvestmentBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == reimageConsInfo.Id);

                //ReinvestmentCost reinCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == enti.Id);
                financial.IsMcCafe = reinvestmentBasicInfo.NewMcCafe;

                financial.IsKiosk = reinvestmentBasicInfo.NewKiosk;
                financial.IsMDS = reinvestmentBasicInfo.NewMDS;
                financial.IsTwientyFourHour = reinvestmentBasicInfo.NewTwientyFourHour;
                financial.StoreCM = ContributionMargin;
            }
            return Ok(financial);

        }

        [Route("api/financialPreAnalysis/GetTTMAndRoI")]
        public IHttpActionResult GetTTMAndRoI(string projectId, string usCode, string dateTime, string pageType = "")
        {
            string year = string.Empty;
            string month = string.Empty;
            if (!string.IsNullOrEmpty(dateTime))
            {
                //string time=dateTime.Split(',')[0];
                //time = time.Split(':')[1];
                //year = time.Split('-')[0].Substring(1, 4);
                //month = time.Split('-')[1].Substring(0, 2);
                year = dateTime.Split('-')[0];
                month = dateTime.Split('-')[1];
            }

            var data = DataSync_LDW_AM_STFinanceData.FirstOrDefault(e => e.FinanceYear == year && e.FinanceMonth == month && e.UsCode == usCode);
            var data2 = DataSync_LDW_AM_STFinanceData2.FirstOrDefault(e => e.FinanceYear == year && e.FinanceMonth == month && e.UsCode == usCode);

            if (data != null)
            {
                data.SOI_TTM = Math.Round(CovertToDecimalPercent(data.SOIPct_TTM), 1).ToString();
            }


            var financialPreanalysisDto = new FinancialPreanalysisDto();
            financialPreanalysisDto.STFinanceData = data;

            var reimageSum = ReimageSummary.GetReimageSummaryInfo(projectId);
            if (reimageSum != null
                && reimageSum.FinancialPreanalysis != null)
            {
                financialPreanalysisDto.Id = reimageSum.FinancialPreanalysis.Id;
                if (!reimageSum.FinancialPreanalysis.TTMSales.HasValue)
                {
                    financialPreanalysisDto.TTMSales = data == null ? 0 : Convert.ToDecimal(data.Total_Sales_TTM);
                }
                else
                {
                    financialPreanalysisDto.TTMSales = reimageSum.FinancialPreanalysis.TTMSales;
                }
                financialPreanalysisDto.ROI = reimageSum.FinancialPreanalysis.ROI;
                financialPreanalysisDto.PaybackYears = reimageSum.FinancialPreanalysis.PaybackYears;
                if (string.IsNullOrEmpty(reimageSum.FinancialPreanalysis.PaybackYears))
                {
                    financialPreanalysisDto.CurrentPriceTier = data2 == null ? "" : data2.Price_Tier;
                }
                else
                {
                    financialPreanalysisDto.CurrentPriceTier = reimageSum.FinancialPreanalysis.CurrentPriceTier;
                }

            }


            return Ok(financialPreanalysisDto);

            //}
        }

        [HttpGet]
        [Route("api/Reimage/GetConsInfoAgreementList")]
        public IHttpActionResult GetConsInfoAgreementList(string projectId, string typeCode)
        {
            var entity = ReimageConsInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId) && e.IsHistory == false);
            var list = new List<Attachment>();
            if (entity != null)
            {
                list = Attachment.GetList("ReimageConsInfo", entity.Id.ToString(), typeCode);
                foreach (var item in list)
                {
                    if (item.InternalName.IndexOf(".") != -1)
                        item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
                    else
                        item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName + item.Extension;
                }
            }
            return Ok(list);
        }


        [HttpGet]
        [Route("api/Reimage/GetWriteOff")]
        public IHttpActionResult GetWriteOff(string refTableId)
        {
            WriteOffAmount writeoff = null;
            if (!string.IsNullOrEmpty(refTableId))
                writeoff = WriteOffAmount.GetByConsInfoId(new Guid(refTableId));
            return Ok(writeoff);
        }

        #region Reimage Summary
        [HttpGet]
        [Route("api/Reimage/InitReimageSummary")]
        public IHttpActionResult InitReimageSummary(string projectId, string entityId = "")
        {
            var entity = ReimageSummary.GetReimageSummaryInfo(projectId, entityId);

            return Ok(new
            {
                entity = entity,
                isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor)
            });
        }

        [HttpPost]
        [Route("api/Reimage/SaveReimageSummary")]
        public IHttpActionResult SaveReimageSummary(ReimageSummary entity)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    entity.Save();
                    string filePath;
                    entity.GenerateExcel(out filePath);

                    scope.Complete();
                }


                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("api/Reimage/SubmitReimageSummary")]
        public IHttpActionResult SubmitReimageSummary(ReimageSummary entity)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    entity.Save();
                    string filePath;
                    entity.GenerateExcel(out filePath);

                    scope.Complete();
                }

                entity.Submit();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteErrorLog(JsonConvert.SerializeObject(ex));
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/EditReimageSummary")]
        public IHttpActionResult EditReimageSummary(ReimageSummary summary)
        {
            try
            {
                var taskUrl = summary.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/RecallReimageSummary")]
        public IHttpActionResult RecallReimageSummary(ReimageSummary summary)
        {
            try
            {
                summary.Recall(summary.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Reimage/IsExistReimageSummaryAttachment")]
        public IHttpActionResult IsExistReimageSummaryAttachment(ReimageSummary entity)
        {
            try
            {
                var isExist = entity.IsExistReimageSummaryAttachment();
                return Ok(new { isExist });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Site Info

        public IHttpActionResult GetEstimatedVsActualConstruction(string projectId, Guid identifier)
        {

            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId.Equals(projectId));

            var estimatedVsActualConstruction = EstimatedVsActualConstruction.FirstOrDefault(e => e.RefId == identifier);
            if (estimatedVsActualConstruction == null)
            {
                var reimageConsInfo = ReimageConsInfo.GetConsInfo(projectId);
                var storeInfo = StoreSTLocation.FirstOrDefault(e => e.StoreCode == reimageInfo.USCode);
                estimatedVsActualConstruction = new EstimatedVsActualConstruction
                {
                    GBDate = reimageConsInfo.ReinBasicInfo != null ? reimageConsInfo.ReinBasicInfo.GBDate : null,
                    CompletionDate = reimageConsInfo.ReinBasicInfo != null ? reimageConsInfo.ReinBasicInfo.ConsCompletionDate : null,
                    ARDC = reimageConsInfo.ReinBasicInfo != null ? reimageConsInfo.ReinBasicInfo.NewDesignType : null,
                    OriginalOperationSize = storeInfo.TotalArea,
                    OriginalSeatNumber = storeInfo.TotalSeatsNo
                };
            }

            return Ok(estimatedVsActualConstruction);
        }
        #endregion

        #region GBMemo
        [HttpGet]
        [Route("api/Reimage/GetGBMemoInfo")]
        public IHttpActionResult GetGBMemoInfo(string projectId, string entityId = "")
        {
            var memo = ReimageGBMemo.GetGBMemo(projectId, entityId);
            //if (memo != null)
            //{
            //    var condition = new ProjectCommentCondition();
            //    condition.SourceCode = FlowCode.Reimage;
            //    condition.UserAccount = ClientCookie.UserCode;

            //    condition.RefTableId = memo.Id;
            //    condition.RefTableName = "ReimageGBMemo";

            //    var comments = ProjectComment.SearchList(condition);
            //    if (comments != null && comments.Count > 0)
            //    {
            //        memo.ProjectComments = comments;

            //        var saveComment =
            //            comments.OrderByDescending(e => e.CreateTime)
            //                .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
            //        if (saveComment != null)
            //        {
            //            memo.Comments = saveComment.Content;
            //        }
            //    }
            //}
            return Ok(memo);
        }
        [HttpPost]
        [Route("api/Reimage/SaveGBMemo")]
        public IHttpActionResult SaveGBMemo(ReimageGBMemo memo)
        {
            memo.Save();
            return Ok();
        }

        [HttpPost]
        [Route("api/Reimage/SubmitGBMemo")]
        public IHttpActionResult SubmitGBMemo(ReimageGBMemo memo)
        {
            memo.Submit();
            return Ok();
        }
        [HttpPost]
        [Route("api/Reimage/ResubmitGBMemo")]
        public IHttpActionResult ResubmitGBMemo(ReimageGBMemo entity)
        {
            entity.Resubmit(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/ApproveGBMemo")]
        public IHttpActionResult ApproveGBMemo(ReimageGBMemo entity)
        {
            entity.Approve(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Reimage/RecallGBMemo")]
        public IHttpActionResult RecallGBMemo(ReimageGBMemo entity)
        {
            entity.Recall(entity.Comments);
            return Ok();
        }

        [HttpPost]
        [Route("api/Reimage/EditGBMemo")]
        public IHttpActionResult EditGBMemo(ReimageGBMemo entity)
        {
            var taskUrl = entity.Edit();
            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }

        [HttpPost]
        [Route("api/Reimage/ReturnGBMemo")]
        public IHttpActionResult ReturnGBMemo(ReimageGBMemo entity)
        {
            entity.Return(ClientCookie.UserCode);
            return Ok(entity);
        }

        [Route("api/Reimage/NotifyGBMemo")]
        [HttpPost]
        public IHttpActionResult NotifyGBMemo(PostMemo<ReimageGBMemo> postData)
        {
            var actor = ProjectUsers.GetProjectUser(postData.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                pdfData.Add("WorkflowName", Constants.Reimage);
                pdfData.Add("ProjectID", postData.Entity.ProjectId);
                pdfData.Add("RegionNameENUS", postData.Entity.Store.StoreBasicInfo.RegionENUS);
                pdfData.Add("RegionNameZHCN", postData.Entity.Store.StoreBasicInfo.RegionZHCN);
                pdfData.Add("MarketNameENUS", postData.Entity.Store.StoreBasicInfo.MarketENUS);
                pdfData.Add("MarketNameZHCN", postData.Entity.Store.StoreBasicInfo.MarketZHCN);
                pdfData.Add("ProvinceNameENUS", postData.Entity.Store.StoreBasicInfo.ProvinceENUS);
                pdfData.Add("ProvinceNameZHCN", postData.Entity.Store.StoreBasicInfo.ProvinceZHCN);
                pdfData.Add("CityNameENUS", postData.Entity.Store.StoreBasicInfo.CityENUS);
                pdfData.Add("CityNameZHCN", postData.Entity.Store.StoreBasicInfo.CityZHCN);
                pdfData.Add("StoreNameENUS", postData.Entity.Store.StoreBasicInfo.NameENUS);
                pdfData.Add("StoreNameZHCN", postData.Entity.Store.StoreBasicInfo.NameZHCN);
                pdfData.Add("USCode", postData.Entity.Store.StoreBasicInfo.StoreCode);

                pdfData.Add("IsClosed", postData.Entity.IsClosed ? "Y" : "N");
                pdfData.Add("IsInOperation", postData.Entity.IsInOperation ? "Y" : "N");
                pdfData.Add("IsMcCafe", postData.Entity.IsMcCafe ? "Y" : "N");
                pdfData.Add("IsKiosk", postData.Entity.IsKiosk ? "Y" : "N");
                pdfData.Add("IsMDS", postData.Entity.IsMDS ? "Y" : "N");
                pdfData.Add("Is24Hour", postData.Entity.Is24Hour ? "Y" : "N");
                var consInfo = ReimageConsInfo.GetConsInfo(postData.Entity.ProjectId);
                postData.Entity.ReinvestInfo = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id);
                pdfData.Add("GBDate", postData.Entity.rmgInfo.GBDate.HasValue ? postData.Entity.rmgInfo.GBDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ConstCompletionDate", postData.Entity.ReinvestInfo.ConsCompletionDate.HasValue ? postData.Entity.ReinvestInfo.ConsCompletionDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ReopenDate", postData.Entity.rmgInfo.ReopenDate.HasValue ? postData.Entity.rmgInfo.ReopenDate.Value.ToString("yyyy-MM-dd") : "");

                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.GBMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.Store.StoreBasicInfo.StoreCode);
                bodyValues.Add("StoreName", postData.Entity.Store.StoreBasicInfo.NameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", Constants.Reimage_GBMemo); ////--流程名称
                bodyValues.Add("ProjectName", Constants.Reimage); //项目名称

                string viewPage = string.Format("{0}/Reimage/Main#/GBMemo/Process/View?projectId={1}",
                        ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    foreach (Employee emp in postData.Receivers)
                    {
                        if (sbTo.Length > 0)
                        {
                            sbTo.Append(";");
                        }
                        if (!string.IsNullOrEmpty(emp.Mail))
                        {
                            sbTo.Append(emp.Mail);
                        }
                    }
                    if (sbTo.Length > 0)
                    {
                        sbTo.Append(";");
                    }
                    message.EmailBodyValues = bodyValues;
                    string strTitle = FlowCode.Reimage_GBMemo;
                    attachments.Add(pdfPath, strTitle + "_" + postData.Entity.ProjectId + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendGBMemoNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }
                postData.Entity.CompleteNotifyTask(postData.Entity.ProjectId);
                tranScope.Complete();
            }
            return Ok();
        }
        #endregion


        #region Select Package Feature
        [HttpPost]
        [Route("api/Reimage/PackageList")]
        public IHttpActionResult PackageList(TaskWorkCondition condition)
        {
            condition.Status = TaskWorkStatus.Holding;
            condition.TypeCode = FlowCode.Reimage_Package;

            int totalSize;
            var data = TaskWork.Query(condition, out totalSize).ToList();

            return Ok(new { data, totalSize });
        }


        [HttpPost]
        [Route("api/Reimage/ReleasePackages")]
        public IHttpActionResult ReleasePackages(List<TaskWork> tasks)
        {
            TaskWork.Release(tasks.ToArray());

            return Ok(tasks);
        }
        #endregion
    }
}