using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System.IO;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.DataAccess.Common;
using System.Transactions;

namespace Mcdonalds.AM.Services.Controllers.Closure
{
    public class ClosureExecutiveSummaryController : ApiController
    {

        [Route("api/ClosureExecutiveSummary/GetByProjectId/{projectID}")]
        [HttpGet]
        public IHttpActionResult GetByProjectId(string projectID)
        {
            var entity = ClosureExecutiveSummary.FirstOrDefault(e => e.ProjectId == projectID && e.IsHistory == false);
            if (entity == null)
            {
                entity = new ClosureExecutiveSummary();
                entity.ProjectId = projectID;
                entity.IsHistory = false;
            }
            bool enableEdit = false;
            bool enableRecall = false;

            var closureInfo = ClosureInfo.FirstOrDefault(e => e.ProjectId == projectID);
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == projectID && e.FlowCode == FlowCode.Closure_ExecutiveSummary);

            if (projectInfo != null)
            {
                if (ClientCookie.UserCode.Equals(closureInfo.AssetActorAccount))
                {
                    var isExistTask = TaskWork.Any(e => e.RefID == projectID && e.TypeCode == FlowCode.Closure_ExecutiveSummary && e.Status == TaskWorkStatus.UnFinish && e.ReceiverAccount == ClientCookie.UserCode && e.ActivityName == "Originator");
                    var isPackgeStarted = ProjectInfo.IsFlowStarted(projectID, FlowCode.Closure_ClosurePackage);
                    var isESStarted = ProjectInfo.IsFlowStarted(projectID, FlowCode.Closure_ExecutiveSummary);
                    enableEdit = ProjectInfo.IsFlowEditable(projectID, FlowCode.Closure_ExecutiveSummary);
                    enableRecall = ProjectInfo.IsFlowRecallable(projectID, FlowCode.Closure_ExecutiveSummary);
                }
            }
            return Ok(new
            {
                entity = entity,
                enableEdit = enableEdit,
                enableReCall = enableRecall
            });
        }

        [Route("api/ClosureExecutiveSummary/GetByProcInstID/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetByProcInstID(int procInstID)
        {
            var entity = ClosureExecutiveSummary.GetByProcInstID(procInstID);
            return Ok(entity);
        }

        [Route("api/ClosureExecutiveSummary/GetTemplates/{id}")]
        [HttpGet]
        public IHttpActionResult GetTemplates(Guid id)
        {
            var list = Attachment.GetList("ClosureExecutiveSummary", id.ToString(), "Template");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            return Ok(list);
        }

        [Route("api/ClosureExecutiveSummary/GetAttachments/{projectId}")]
        [HttpGet]
        public IHttpActionResult GetAttachments(string projectId)
        {
            var entity = ClosureExecutiveSummary.FirstOrDefault(i => i.ProjectId == projectId && i.IsHistory == false);
            if (entity == null)
                return Ok();

            var list = Attachment.GetList("ClosureExecutiveSummary", entity.Id.ToString(), "");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            return Ok(list);
        }

        [Route("api/ClosureExecutiveSummary/UploadTemplate/{projectid}")]
        [HttpPost]
        public IHttpActionResult UploadTemplate(string projectid)
        {


            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            string internalName = string.Empty;
            if (FileCollect.Count > 0) //如果集合的数量大于0
            {
                ClosureWOCheckList entity = new ClosureWOCheckList();



                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = FileCollect[0];
                string fileName = Path.GetFileName(fileSave.FileName);
                string fileExtension = Path.GetExtension(fileSave.FileName);


                internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + internalName;
                //通过此对象获取文件名

                fileSave.SaveAs(absolutePath);


                var woEntity = ClosureExecutiveSummary.FirstOrDefault(e => e.ProjectId == projectid && e.IsHistory == false);
                if (woEntity == null)
                {
                    woEntity = new ClosureExecutiveSummary();
                    woEntity.ProjectId = projectid;
                    woEntity.Id = Guid.NewGuid();
                    woEntity.CreateTime = DateTime.Now;
                    woEntity.CreatorAccount = ClientCookie.UserCode;

                    ClosureExecutiveSummary.Add(woEntity);
                }

                Attachment att = Attachment.GetAttachment("ClosureExecutiveSummary", woEntity.Id.ToString(), "Template");
                if (att == null)
                {
                    att = new Attachment();
                    att.RefTableName = "ClosureExecutiveSummary";
                    att.RefTableID = woEntity.Id.ToString();
                    att.CreateTime = DateTime.Now;
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "Template";
                    att.CreatorID = ClientCookie.UserCode;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.Name = fileName;
                    att.InternalName = internalName;
                    att.Name = fileName;
                    att.Extension = fileExtension;
                    att.Length = FileCollect[0].ContentLength;
                    Attachment.Add(att);
                }
                else
                {
                    att.CreateTime = DateTime.Now;
                    att.CreatorID = ClientCookie.UserCode;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.Name = fileName;
                    att.InternalName = internalName;
                    att.Extension = fileExtension;
                    att.Length = FileCollect[0].ContentLength;
                    Attachment.Update(att);
                }


                //var closureInfo = closureHandler.GetByProjectId(projectid);
                //Store store = new Store();
                //var storeInfo = store.GetStoreEntity(closureInfo.USCode);

                //string filePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + internalName;
                //Excel excel = new Excel();
                //excel.Open(filePath);
                //var sheet = excel.Sheets["PMT"];

                //sheet.Cells[1, 1].StrValue = closureInfo.StoreNameZHCN;
                //sheet.Cells[2, 1].StrValue = closureInfo.USCode;

                //if (storeInfo != null)
                //{
                //    sheet.Cells[3, 1].StrValue = storeInfo.CityName;
                //    sheet.Cells[4, 1].StrValue = storeInfo.MarketName;
                //    if (storeInfo.OpenDate != null)
                //    {
                //        sheet.Cells[5, 1].StrValue = storeInfo.OpenDate.Value.ToString("yyyy-MM-dd");
                //    }
                //    sheet.Cells[7, 1].StrValue = storeInfo.TotalLeasedArea;
                //    sheet.Cells[8, 1].StrValue = storeInfo.TotalSeatsNo;
                //    sheet.Cells[10, 1].StrValue = storeInfo.LeasePurchaseTerm;
                //    if (storeInfo.RentCommencementDate != null)
                //    {

                //        sheet.Cells[11, 1].StrValue = storeInfo.RentCommencementDate.Value.ToString("yyyy-MM-dd");
                //    }
                //    if (storeInfo.EndDate != null)
                //    {
                //        sheet.Cells[12, 1].StrValue = storeInfo.EndDate.Value.ToString("yyyy-MM-dd");
                //    }
                //    sheet.Cells[13, 1].StrValue = storeInfo.RentType;
                //    sheet.Cells[13, 1].StrValue = storeInfo.RentStructure;
                //}

                //excel.Save();


            }
            //return Ok(result);

            ProjectInfo.FinishNode(projectid, FlowCode.Closure_ExecutiveSummary,
                   NodeCode.Finish);


            var resultStr = SiteInfo.WebUrl + "UploadFiles/" + internalName;
            return Ok(resultStr);
        }

        private bool IsMeetVersion(string actualFilePath)
        {
            var siteFilePath = SiteFilePath.Executive_Summary_Template;
            var templatePath = string.Format(@"{0}\{1}", SiteFilePath.Template_DIRECTORY, siteFilePath);
            if (ExcelHelper.GetExcelVersionNumber(templatePath) == ExcelHelper.GetExcelVersionNumber(actualFilePath))
                return true;
            return false;
        }

        [Route("api/ClosureExecutiveSummary/UploadAttachement/{projectId}")]
        [HttpPost]
        public IHttpActionResult UploadAttachement(string projectId)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;

            if (FileCollect.Count > 0) //如果集合的数量大于0
            {
                var entity = ClosureExecutiveSummary.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);

                if (entity == null)
                {
                    entity = new ClosureExecutiveSummary();
                    entity.Id = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreatorAccount = ClientCookie.UserCode;
                    entity.IsHistory = false;
                    entity.ProjectId = projectId;
                    ClosureExecutiveSummary.Add(entity);
                }

                for (int i = 0; i < FileCollect.Count; i++)
                {
                    var fileSave = FileCollect[i];
                    //用key获取单个文件对象HttpPostedFile

                    string fileName = Path.GetFileName(fileSave.FileName);
                    string fileExtension = Path.GetExtension(fileSave.FileName);
                    var current = System.Web.HttpContext.Current;
                    string internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                    fileSave.SaveAs(absolutePath);

                    Attachment att = new Attachment();
                    att.InternalName = internalName;
                    att.RefTableName = "ClosureExecutiveSummary";
                    att.RefTableID = entity.Id.ToString();
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = fileExtension;
                    att.Length = FileCollect[0].ContentLength;
                    att.CreateTime = DateTime.Now;
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "Attachment";
                    att.CreatorID = ClientCookie.UserCode;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    Attachment.Add(att);

                }

            }
            return Ok();

        }

        [Route("api/ClosureExecutiveSummary/SaveExcutiveSummary")]
        [HttpPost]
        public IHttpActionResult SaveExcutiveSummary(ClosureExecutiveSummary entity)
        {
            if (!ClosureExecutiveSummary.Any(i => i.ProjectId == entity.ProjectId && i.IsHistory == false))
            {
                if (entity.Id == Guid.Empty || entity.Id == null)
                    entity.Id = Guid.NewGuid();
                entity.CreateTime = DateTime.Now;
                entity.CreatorAccount = ClientCookie.UserCode;
                ClosureExecutiveSummary.Add(entity);
            }
            else
                ClosureExecutiveSummary.Update(entity);

            ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ExecutiveSummary, NodeCode.Closure_ExecutiveSummary_Input);
            return Ok();
        }

        [Route("api/LegalReview/PostExcutiveSummary")]
        [HttpPost]
        public IHttpActionResult PostExcutiveSummary(ClosureExecutiveSummary entity)
        {


            var task = TaskWork.FirstOrDefault(
           e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                && e.TypeCode == FlowCode.Closure_ExecutiveSummary && e.RefID == entity.ProjectId);


            if (task != null)
            {
                task.Status = TaskWorkStatus.K2ProcessApproved;
                task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_ExecutiveSummary, task.RefID);
                task.FinishTime = DateTime.Now;
                var ProcInstID = entity.StartProcess(task);
                task.ProcInstID = ProcInstID;
                entity.ProcInstID = ProcInstID;

                entity.Update();
                TaskWork.Update(task);

                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ExecutiveSummary, NodeCode.Closure_ExecutiveSummary_Input);
            }
            return Ok();
        }

        [Route("api/ClosureExecutiveSummary/GetOnline/{projectID}")]
        [HttpGet]
        public IHttpActionResult GetOnline(string projectID)
        {
            var closure = ClosureInfo.GetByProjectId(projectID);
            var store = StoreBasicInfo.GetStore(closure.USCode);
            var toolEntity = ClosureTool.Get(closure.ProjectId);
            var woEntity = ClosureWOCheckList.Get(closure.ProjectId);

            List<StoreBEInfo> remoteBeList = new List<StoreBEInfo>();
            List<StoreBEInfo> attachedBeList = new List<StoreBEInfo>();
            StoreBEInfo mds = null;
            StoreBEInfo mcCafe = null;
            StoreBEInfo hour24 = null;
            if (store.StoreBEInfoList.Count > 0)
            {
                foreach (var beInfo in store.StoreBEInfoList)
                {
                    switch (beInfo.BETypeName)
                    {
                        case "Remote Kiosk":
                            remoteBeList.Add(beInfo);
                            break;
                        case "Attach Kiosk":
                            attachedBeList.Add(beInfo);
                            break;
                        case "MDS":
                            mds = beInfo;
                            break;
                        case "McCafe":
                            mcCafe = beInfo;
                            break;
                        case "24 Hour":
                            hour24 = beInfo;
                            break;

                    }
                }
            }
            return Ok(new
            {
                Store = store,
                ClosureTool = toolEntity,
                WOCheckList = woEntity,
                RemoteBeList = remoteBeList,
                AttachedBeList = attachedBeList,
                MDS = mds,
                McCafe = mcCafe,
                Hour24 = hour24
            });
        }

        [Route("api/ClosureExecutiveSummary/GenExecutiveSummaty")]
        [HttpPost]
        public IHttpActionResult GenExecutiveSummaty(ClosureExecutiveSummary entity)
        {
            if (!ClosureExecutiveSummary.Any(i => i.ProjectId == entity.ProjectId && i.IsHistory == false))
            {
                if (entity.Id == Guid.Empty || entity.Id == null)
                    entity.Id = Guid.NewGuid();
                entity.CreateTime = DateTime.Now;
                entity.CreatorAccount = ClientCookie.UserCode;
                ClosureExecutiveSummary.Add(entity);
            }
            else
                ClosureExecutiveSummary.Update(entity);

            var current = System.Web.HttpContext.Current;
            string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Executive_Summary_Template;
            string internalFileName = Guid.NewGuid() + ".xlsx";
            string filePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + internalFileName;
            File.Copy(path, filePath);
            var fileInfo = new FileInfo(filePath);
            var excelPMTDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.ClosureExecutiveSummary);

            var closure = ClosureInfo.GetByProjectId(entity.ProjectId);
            var store = StoreBasicInfo.GetStore(closure.USCode);
            var inputInfo = new ExcelInputDTO
            {
                StoreNameCN = store.StoreBasicInfo.NameZHCN,
                USCode = store.StoreBasicInfo.StoreCode,
                City = store.StoreBasicInfo.CityENUS,
                Market = store.StoreBasicInfo.MarketENUS,
                OpenDate = store.StoreBasicInfo.OpenDate,
                Floor = store.StoreSTLocation.Floor,
                TotalArea = store.StoreSTLocation.TotalArea,
                TotalSeatsNo = store.StoreSTLocation.TotalSeatsNo,
                BE = store.StoreBEInfoList.Count,
                RentType = store.StoreContractInfo.RentType,
                RentStructure = store.StoreContractInfo.RentStructure,
                MiniMarket = entity.MiniMarket,
                StoreLocation = entity.StoreLocation,
                CurrentSituation = entity.CurrentSituation,
                NegotiationHistory = entity.NegotiationHistory,
                ProposedSolution = entity.ProposedSolution,
                SalesTransfer = entity.SalesTransfer
            };


            var leaseRecapID = StoreContractInfo.SearchByProject(entity.ProjectId).FirstOrDefault().LeaseRecapID ?? 0;
            var storeAtt = StoreContractInfoAttached.Search(c => c.LeaseRecapID == leaseRecapID.ToString()).OrderByDescending(e => e.CreateDate).FirstOrDefault();
            if (storeAtt != null)
            {
                inputInfo.LeasingTerm = store.StoreContractInfo.LeasePurchaseTerm;
            }
            if (store.StoreContractInfo.RentCommencementDate.HasValue)
            {

                inputInfo.RentCommencementDate = store.StoreContractInfo.RentCommencementDate.Value;
            }

            if (store.StoreContractInfo.EndDate.HasValue)
            {

                inputInfo.LeaseExpirationDate = store.StoreContractInfo.EndDate.Value;
            }


            var toolEntity = ClosureTool.Get(closure.ProjectId);
            if (toolEntity != null)
            {
                if (toolEntity.TotalSales_Adjustment_RMB != null)
                {
                    inputInfo.TotalSales_TTM = string.Format("{0:f}", toolEntity.TotalSales_Adjustment_RMB);
                }
                if (toolEntity.CompSales_Adjustment != null)
                {
                    inputInfo.SalesComp_TTM = string.Format("{0:f}", toolEntity.CompSales_Adjustment);
                }
                if (toolEntity.CompCG_Adjustment != null)
                {
                    inputInfo.GCComp_TTM = string.Format("{0:f}", toolEntity.CompCG_Adjustment);
                }
                if (toolEntity.PAC_Adjustment != null)
                {
                    inputInfo.PAC_TTM = string.Format("{0:f}", toolEntity.PAC_Adjustment);
                }
                if (toolEntity.SOI_Adjustment != null)
                {
                    inputInfo.SOI_TTM = string.Format("{0:f}", toolEntity.SOI_Adjustment);
                }
                if (toolEntity.CashFlow_RMB_Adjustment != null)
                {
                    inputInfo.CASHFLOW_TTM = string.Format("{0:f}", toolEntity.CashFlow_RMB_Adjustment);
                }
                if (toolEntity.TotalSales_TTMY1 != null)
                {
                    inputInfo.TotalSales_TTMY1 = string.Format("{0:f}", toolEntity.TotalSales_TTMY1);
                }
                if (toolEntity.CompSales_TTMY1 != null)
                {
                    inputInfo.CompSales_TTMY1 = string.Format("{0:f}", toolEntity.CompSales_TTMY1);
                }
                if (toolEntity.CompGC_TTMY1 != null)
                {
                    inputInfo.CompGC_TTMY1 = string.Format("{0:f}", toolEntity.CompGC_TTMY1);
                }
                if (toolEntity.PAC_TTMY1 != null)
                {
                    inputInfo.PAC_TTMY1 = string.Format("{0:f}", toolEntity.PAC_TTMY1);
                }
                if (toolEntity.SOI_TTMY1 != null)
                {
                    inputInfo.SOI_TTMY1 = string.Format("{0:f}", toolEntity.SOI_TTMY1);
                }
                if (toolEntity.CashFlow_TTMY1 != null)
                {
                    inputInfo.CashFlow_TTMY1 = string.Format("{0:f}", toolEntity.CashFlow_TTMY1);
                }
                if (toolEntity.TotalSales_TTMY2 != null)
                {
                    inputInfo.TotalSales_TTMY2 = string.Format("{0:f}", toolEntity.TotalSales_TTMY2);
                }
                if (toolEntity.CompSales_TTMY2 != null)
                {
                    inputInfo.CompSales_TTMY2 = string.Format("{0:f}", toolEntity.CompSales_TTMY2);
                }
                if (toolEntity.CompGC_TTMY2 != null)
                {
                    inputInfo.CompGC_TTMY2 = string.Format("{0:f}", toolEntity.CompGC_TTMY2);
                }
                if (toolEntity.PAC_TTMY2 != null)
                {
                    inputInfo.PAC_TTMY2 = string.Format("{0:f}", toolEntity.PAC_TTMY2);
                }
                if (toolEntity.SOI_TTMY2 != null)
                {
                    inputInfo.SOI_TTMY2 = string.Format("{0:f}", toolEntity.SOI_TTMY2);
                }
                if (toolEntity.CashFlow_TTMY2 != null)
                {
                    inputInfo.CashFlow_TTMY2 = string.Format("{0:f}", toolEntity.CashFlow_TTMY2);
                }


                List<StoreBEInfo> remoteBeList = new List<StoreBEInfo>();
                List<StoreBEInfo> attachedBeList = new List<StoreBEInfo>();
                StoreBEInfo mds = null;
                StoreBEInfo mcCafe = null;
                StoreBEInfo hour24 = null;
                if (store.StoreBEInfoList.Count > 0)
                {
                    foreach (var beInfo in store.StoreBEInfoList)
                    {
                        switch (beInfo.BETypeName)
                        {
                            case "Remote Kiosk":
                                remoteBeList.Add(beInfo);
                                break;
                            case "Attach Kiosk":
                                attachedBeList.Add(beInfo);
                                break;
                            case "MDS":
                                mds = beInfo;
                                break;
                            case "McCafe":
                                mcCafe = beInfo;
                                break;
                            case "24 Hour":
                                hour24 = beInfo;
                                break;

                        }
                    }
                }
                inputInfo.RemoteKiosk1_Status = "No";
                inputInfo.RemoteKiosk2_Status = "No";
                inputInfo.RemoteKiosk3_Status = "No";
                inputInfo.AttachedKiosk1_Status = "No";
                inputInfo.AttachedKiosk2_Status = "No";
                inputInfo.AttachedKiosk3_Status = "No";
                inputInfo.MDS_Status = "No";
                inputInfo.McCafe_Status = "No";
                inputInfo.TwentyFourHour_Status = "No";

                if (remoteBeList.Count > 0)
                {

                    inputInfo.RemoteKiosk1_Status = "Yes";
                    inputInfo.RemoteKiosk1_OpenDate = remoteBeList[0].LaunchDate;
                    if (remoteBeList.Count > 1)
                    {
                        inputInfo.RemoteKiosk2_Status = "Yes";
                        inputInfo.RemoteKiosk2_OpenDate = remoteBeList[1].LaunchDate;

                        if (remoteBeList.Count > 2)
                        {
                            inputInfo.RemoteKiosk3_Status = "Yes";
                            inputInfo.RemoteKiosk3_OpenDate = remoteBeList[2].LaunchDate;
                        }
                    }
                }

                if (attachedBeList.Count > 0)
                {
                    inputInfo.AttachedKiosk1_Status = "Yes";
                    inputInfo.AttachedKiosk1_OpenDate = attachedBeList[0].LaunchDate;
                    if (attachedBeList.Count > 1)
                    {
                        inputInfo.AttachedKiosk2_Status = "Yes";
                        inputInfo.AttachedKiosk2_OpenDate = attachedBeList[1].LaunchDate;

                        if (attachedBeList.Count > 2)
                        {
                            inputInfo.AttachedKiosk3_Status = "Yes";
                            inputInfo.AttachedKiosk3_OpenDate = attachedBeList[2].LaunchDate;
                        }
                    }
                }
                if (mds != null)
                {
                    inputInfo.MDS_Status = "Yes";
                    inputInfo.MDS_OpenDate = mds.LaunchDate;
                }
                if (mcCafe != null)
                {
                    inputInfo.McCafe_Status = "Yes";
                    inputInfo.McCafe_OpenDate = mcCafe.LaunchDate;
                }
                if (hour24 != null)
                {
                    inputInfo.TwentyFourHour_Status = "Yes";
                    inputInfo.TwentyFourHour_OpenDate = hour24.LaunchDate;
                }


                var woEntity = ClosureWOCheckList.Get(closure.ProjectId);
                if (woEntity != null)
                {
                    if (woEntity.LHI_NBV != null)
                    {
                        inputInfo.LHI_NBV = string.Format("{0:f}", woEntity.LHI_NBV);
                    }
                    if (woEntity.ESSD_NBV != null)
                    {
                        inputInfo.ESSD_NBV = string.Format("{0:f}", woEntity.ESSD_NBV);
                    }
                    if (woEntity.TotalCost_NBV != null)
                    {
                        inputInfo.TotalCost_NBV = string.Format("{0:f}", woEntity.TotalCost_NBV);
                    }
                }
            }

            excelPMTDirector.Input(inputInfo);

            Attachment att = Attachment.GetAttachment("ClosureExecutiveSummary", entity.Id.ToString(), "Template");
            if (att == null)
            {
                att = new Attachment();
                att.RefTableName = "ClosureExecutiveSummary";
                att.RefTableID = entity.Id.ToString();
                att.CreateTime = DateTime.Now;
                att.ID = Guid.NewGuid();
                att.TypeCode = "Template";
                att.CreatorID = ClientCookie.UserCode;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.Name = SiteFilePath.Executive_Summary_Template;
                att.InternalName = internalFileName;
                att.Extension = fileInfo.Extension;
                att.Length = Convert.ToInt32(fileInfo.Length);
                att.RequirementId = new Guid("79258ffb-c2ef-4eff-897d-ba8376c90071");
                Attachment.Add(att);
            }
            else
            {
                att.CreateTime = DateTime.Now;
                att.CreatorID = ClientCookie.UserCode;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.Name = SiteFilePath.Executive_Summary_Template;
                att.InternalName = internalFileName;
                att.Extension = fileInfo.Extension;
                att.Length = Convert.ToInt32(fileInfo.Length);
                att.RequirementId = new Guid("79258ffb-c2ef-4eff-897d-ba8376c90071");
                Attachment.Update(att);
            }

            ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ExecutiveSummary, NodeCode.Closure_ExecutiveSummary_Generate);

            return Ok();
        }

        [Route("api/ClosureExecutiveSummary/DownLoadTemplate/{projectID}")]
        [HttpGet]
        public IHttpActionResult DownLoadTemplate(string projectID)
        {
            var current = System.Web.HttpContext.Current;
            string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Executive_Summary_Template;

            string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + Guid.NewGuid() + ".xlsx";
            File.Copy(path, tempFilePath);
            var fileInfo = new FileInfo(tempFilePath);
            var excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.ClosureExecutiveSummary);

            var closure = ClosureInfo.GetByProjectId(projectID);
            var store = StoreBasicInfo.GetStore(closure.USCode);
            var inputInfo = new ExcelInputDTO
            {
                StoreNameCN = store.StoreBasicInfo.NameZHCN,
                USCode = store.StoreBasicInfo.StoreCode,
                City = store.StoreBasicInfo.CityENUS,
                Market = store.StoreBasicInfo.MarketENUS,
                OpenDate = store.StoreBasicInfo.OpenDate,
                Floor = store.StoreSTLocation.Floor,
                TotalArea = store.StoreSTLocation.TotalArea,
                TotalSeatsNo = store.StoreSTLocation.TotalSeatsNo,
                BE = store.StoreBEInfoList.Count,
                RentType = store.StoreContractInfo.RentType,
                RentStructure = store.StoreContractInfo.RentStructure
            };


            var leaseRecapID = StoreContractInfo.SearchByProject(projectID).FirstOrDefault().LeaseRecapID ?? 0;
            var storeAtt = StoreContractInfoAttached.Search(c => c.LeaseRecapID == leaseRecapID.ToString()).OrderByDescending(e => e.CreateDate).FirstOrDefault();
            if (storeAtt != null)
            {
                inputInfo.LeasingTerm = store.StoreContractInfo.LeasePurchaseTerm;
            }
            if (store.StoreContractInfo.RentCommencementDate.HasValue)
            {

                inputInfo.RentCommencementDate = store.StoreContractInfo.RentCommencementDate.Value;
            }

            if (store.StoreContractInfo.EndDate.HasValue)
            {

                inputInfo.LeaseExpirationDate = store.StoreContractInfo.EndDate.Value;
            }


            var toolEntity = ClosureTool.Get(closure.ProjectId);
            if (toolEntity != null)
            {
                if (toolEntity.TotalSales_Adjustment_RMB != null)
                {
                    inputInfo.TotalSales_TTM = string.Format("{0:f}", toolEntity.TotalSales_Adjustment_RMB);
                }
                if (toolEntity.CompSales_Adjustment != null)
                {
                    inputInfo.SalesComp_TTM = string.Format("{0:f}", toolEntity.CompSales_Adjustment);
                }
                if (toolEntity.CompCG_Adjustment != null)
                {
                    inputInfo.GCComp_TTM = string.Format("{0:f}", toolEntity.CompCG_Adjustment);
                }
                if (toolEntity.PAC_RMB_Adjustment != null)
                {
                    inputInfo.PAC_TTM = string.Format("{0:f}", toolEntity.PAC_RMB_Adjustment);
                }
                if (toolEntity.SOI_Adjustment != null)
                {
                    inputInfo.SOI_TTM = string.Format("{0:f}", toolEntity.SOI_Adjustment);
                }
                if (toolEntity.CashFlow_RMB_Adjustment != null)
                {
                    inputInfo.CASHFLOW_TTM = string.Format("{0:f}", toolEntity.CashFlow_RMB_Adjustment);
                }
                if (toolEntity.TotalSales_TTMY1 != null)
                {
                    inputInfo.TotalSales_TTMY1 = string.Format("{0:f}", toolEntity.TotalSales_TTMY1);
                }
                if (toolEntity.CompSales_TTMY1 != null)
                {
                    inputInfo.CompSales_TTMY1 = string.Format("{0:f}", toolEntity.CompSales_TTMY1);
                }
                if (toolEntity.CompGC_TTMY1 != null)
                {
                    inputInfo.CompGC_TTMY1 = string.Format("{0:f}", toolEntity.CompGC_TTMY1);
                }
                if (toolEntity.PAC_TTMY1 != null)
                {
                    inputInfo.PAC_TTMY1 = string.Format("{0:f}", toolEntity.PAC_TTMY1);
                }
                if (toolEntity.SOI_TTMY1 != null)
                {
                    inputInfo.SOI_TTMY1 = string.Format("{0:f}", toolEntity.SOI_TTMY1);
                }
                if (toolEntity.CashFlow_TTMY1 != null)
                {
                    inputInfo.CashFlow_TTMY1 = string.Format("{0:f}", toolEntity.CashFlow_TTMY1);
                }
                if (toolEntity.TotalSales_TTMY2 != null)
                {
                    inputInfo.TotalSales_TTMY2 = string.Format("{0:f}", toolEntity.TotalSales_TTMY2);
                }
                if (toolEntity.CompSales_TTMY2 != null)
                {
                    inputInfo.CompSales_TTMY2 = string.Format("{0:f}", toolEntity.CompSales_TTMY2);
                }
                if (toolEntity.CompGC_TTMY2 != null)
                {
                    inputInfo.CompGC_TTMY2 = string.Format("{0:f}", toolEntity.CompGC_TTMY2);
                }
                if (toolEntity.PAC_TTMY2 != null)
                {
                    inputInfo.PAC_TTMY2 = string.Format("{0:f}", toolEntity.PAC_TTMY2);
                }
                if (toolEntity.SOI_TTMY2 != null)
                {
                    inputInfo.SOI_TTMY2 = string.Format("{0:f}", toolEntity.SOI_TTMY2);
                }
                if (toolEntity.CashFlow_TTMY2 != null)
                {
                    inputInfo.CashFlow_TTMY2 = string.Format("{0:f}", toolEntity.CashFlow_TTMY2);
                }


                List<StoreBEInfo> remoteBeList = new List<StoreBEInfo>();
                List<StoreBEInfo> attachedBeList = new List<StoreBEInfo>();
                StoreBEInfo mds = null;
                StoreBEInfo mcCafe = null;
                StoreBEInfo hour24 = null;
                if (store.StoreBEInfoList.Count > 0)
                {
                    foreach (var beInfo in store.StoreBEInfoList)
                    {
                        switch (beInfo.BETypeName)
                        {
                            case "Remote Kiosk":
                                remoteBeList.Add(beInfo);
                                break;
                            case "Attached Kiosk":
                                attachedBeList.Add(beInfo);
                                break;
                            case "MDS":
                                mds = beInfo;
                                break;
                            case "McCafe":
                                mcCafe = beInfo;
                                break;
                            case "24 Hour":
                                hour24 = beInfo;
                                break;

                        }
                    }
                }
                if (remoteBeList.Count > 0)
                {

                    inputInfo.RemoteKiosk1_Status = remoteBeList[0].IsSingleContract == 1 ? "Yes" : "No";
                    inputInfo.RemoteKiosk1_OpenDate = remoteBeList[0].LaunchDate;
                    if (remoteBeList.Count > 1)
                    {
                        inputInfo.RemoteKiosk2_Status = remoteBeList[1].IsSingleContract == 1 ? "Yes" : "No";
                        inputInfo.RemoteKiosk2_OpenDate = remoteBeList[1].LaunchDate;

                        if (remoteBeList.Count > 2)
                        {
                            inputInfo.RemoteKiosk3_Status = remoteBeList[2].IsSingleContract == 1 ? "Yes" : "No";
                            inputInfo.RemoteKiosk3_OpenDate = remoteBeList[2].LaunchDate;
                        }
                    }
                }

                if (attachedBeList.Count > 0)
                {
                    inputInfo.AttachedKiosk1_Status = attachedBeList[0].IsSingleContract == 1 ? "Yes" : "No";
                    inputInfo.AttachedKiosk1_OpenDate = attachedBeList[0].LaunchDate;
                    if (attachedBeList.Count > 1)
                    {
                        inputInfo.AttachedKiosk2_Status = attachedBeList[1].IsSingleContract == 1 ? "Yes" : "No";
                        inputInfo.AttachedKiosk2_OpenDate = attachedBeList[1].LaunchDate;

                        if (attachedBeList.Count > 2)
                        {
                            inputInfo.AttachedKiosk3_Status = attachedBeList[2].IsSingleContract == 1 ? "Yes" : "No";
                            inputInfo.AttachedKiosk3_OpenDate = attachedBeList[2].LaunchDate;
                        }
                    }
                }
                if (mds != null)
                {
                    inputInfo.MDS_Status = mds.IsSingleContract == 1 ? "Yes" : "No";
                    inputInfo.MDS_OpenDate = mds.LaunchDate;
                }
                if (mcCafe != null)
                {
                    inputInfo.McCafe_Status = mcCafe.IsSingleContract == 1 ? "Yes" : "No";
                    inputInfo.McCafe_OpenDate = mcCafe.LaunchDate;
                }
                if (hour24 != null)
                {
                    inputInfo.TwentyFourHour_Status = hour24.IsSingleContract == 1 ? "Yes" : "No";
                    inputInfo.TwentyFourHour_OpenDate = hour24.LaunchDate;
                }


                var woEntity = ClosureWOCheckList.Get(closure.ProjectId);
                if (woEntity != null)
                {
                    if (woEntity.LHI_NBV != null)
                    {
                        inputInfo.LHI_NBV = string.Format("{0:f}", woEntity.LHI_NBV);
                    }
                    if (woEntity.ESSD_NBV != null)
                    {
                        inputInfo.ESSD_NBV = string.Format("{0:f}", woEntity.ESSD_NBV);
                    }
                    if (woEntity.TotalCost_NBV != null)
                    {
                        inputInfo.TotalCost_NBV = string.Format("{0:f}", woEntity.TotalCost_NBV);
                    }
                }
            }

            excelDirector.Input(inputInfo);


            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DataConverter.ToHexString(SiteFilePath.Executive_Summary_Template));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();
            return Ok();
        }
        #region 不需要K2审批
        //[Route("api/ClosureExecutiveSummary/GetK2Status/{sn}/{procInstID}")]
        //[HttpGet]
        //public IHttpActionResult GetK2Status(string sn, string procInstID)
        //{
        //    // Load K2 Process
        //    bool result = false;
        //    var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, ClientCookie.UserCode);
        //    // result = !resultStr.Equals(Workflows.Closure.WFClosureWOCheckList.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

        //    if (result)
        //    {
        //        // 非发起人节点
        //        resultStr = "Process";
        //    }
        //    else
        //    {
        //        resultStr = "Edit";
        //    }

        //    object resultObj = new
        //    {
        //        Status = resultStr
        //    };
        //    return Ok(resultObj);

        //}

        ///// <summary>
        ///// 处理
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //[Route("api/ClosureExecutiveSummary/ProcessClosureExecutiveSummary")]
        //[HttpPost]
        //public IHttpActionResult ProcessClosureExecutiveSummary(ClosureExecutiveSummary entity)
        //{
        //    int procInstID = entity.ProcInstID.Value;

        //    string actionLower = entity.Action;
        //    string account = ClientCookie.UserCode;
        //    //评论信息
        //    string comments = entity.Comments;
        //    switch (actionLower)
        //    {
        //        case "resubmit":
        //            break;
        //        case "submit":
        //            break;
        //        case "decline":
        //            break;
        //        case "return":
        //            break;

        //    }

        //    return Ok();
        //}
        #endregion

        [Route("api/ClosureExecutiveSummary/Edit")]
        [HttpPost]
        public IHttpActionResult Edit(ClosureExecutiveSummary entity)
        {
            var result = string.Empty;
            using (TransactionScope tranScope = new TransactionScope())
            {
                result = entity.Edit();
                tranScope.Complete();
            }
            return Ok(new
            {
                TaskUrl = result
            });
        }

        //[Route("api/LegalReview/Recall")]
        //[HttpPost]
        //public IHttpActionResult Recall(ClosureExecutiveSummary entity)
        //{
        //    //bool _recallSuccess = false;
        //    //if (entity.ProcInstID != null)
        //    //{

        //    //    string comments = ClientCookie.UserNameZHCN + "进行了流程撤回操作";
        //    //    _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value,
        //    //        WF.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comments);
        //    //}
        //    //if (_recallSuccess)
        //    //{
        //    //    var result = ModifyProject(entity, ProjectAction.Recall);
        //    //}
        //    return Ok();
        //}
    }
}