using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.Services.Workflows.Closure;
using Mcdonalds.AM.Services.Workflows;
using Newtonsoft.Json;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Enums;
using NTTMNC.BPM.Fx.Core;
using Mcdonalds.AM.DataAccess.Common.Excel;

namespace Mcdonalds.AM.Services.Controllers.Closure
{
    public class ClosurePackageController : ApiController
    {

        private McdAMEntities _db = new McdAMEntities();

        [Route("api/ClosurePackage/GetById/{id}")]
        [HttpGet]
        public IHttpActionResult GetById(Guid id)
        {
            var entity = ClosurePackage.Get(id);
            return Ok(entity);
        }

        [Route("api/ClosurePackage/GetByProjectId/{projectID}")]
        [HttpGet]
        public IHttpActionResult GetByProjectId(string projectId)
        {
            var entity = ClosurePackage.Get(projectId);
            if (entity == null || entity.CreateTime == null)
            {
                var usCode = ProjectInfo.GetUSCode(projectId);
                //var store = StoreBasicInfo.GetStorInfo(usCode);
                var closureTool = ClosureTool.Get(projectId);
                if (entity == null)
                    entity = new ClosurePackage();
                entity.IsRelocation = closureTool.IsOptionOffered;
                entity.RelocationPipelineID = closureTool.RelocationPipelineID;
                entity.PipelineName = closureTool.PipelineName;
                entity.NewSiteNetCFNPV = (double)(closureTool.NPVRestaurantCashflows.HasValue ? closureTool.NPVRestaurantCashflows.Value : 0);
                entity.OriginalCFNPV = (double)(closureTool.NPVSC.HasValue ? closureTool.NPVSC.Value : 0);
                entity.NetOperatingIncome = closureTool.OperatingIncome;
            }
            else
            {
                var currentComment = ProjectComment.FirstOrDefault(i => i.RefTableName == ClosurePackage.TableName && i.RefTableId == entity.Id && i.Status == ProjectCommentStatus.Save && i.SourceCode == FlowCode.Closure && i.CreateUserAccount == ClientCookie.UserCode);
                if (currentComment != null)
                    entity.Comments = currentComment.Content;
            }
            return Ok(entity);
        }

        [Route("api/ClosurePackage/GetByProcInstID/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetByProcInstID(int procInstID)
        {
            var entity = ClosurePackage.GetByProcInstID(procInstID);
            return Ok(entity);

        }

        [Route("api/ClosurePackage/Get")]
        [HttpGet]
        public IHttpActionResult Get(string projectId, Guid? id = null)
        {
            var closureTool = ClosureTool.Get(projectId);
            var closureInfo = ClosureInfo.GetByProjectId(projectId);
            var closureWoCheckList = ClosureWOCheckList.Get(projectId);
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            var project = ProjectInfo.Get(projectId, FlowCode.Closure_ClosurePackage);
            ClosurePackage package = null;
            if (id != null)
            {
                package = ClosurePackage.Get(id.Value);
                projectId = package.ProjectId;
            }
            else
            {
                package = ClosurePackage.Get(projectId);
            }
            if (package == null)
            {
                package = ClosurePackage.GetHistory(projectId);
                if (package == null)
                {
                    var usCode = ProjectInfo.GetUSCode(projectId);
                    var store = StoreBasicInfo.GetStorInfo(usCode);
                    package = new ClosurePackage();
                    package.Id = Guid.NewGuid();
                    package.ProjectId = projectId;
                    package.RelocationPipelineID = store.PipelineID;
                    package.PipelineName = store.PipelineNameENUS;
                }
            }
            if (closureTool != null)
            {
                package.IsRelocation = closureTool.IsOptionOffered;
                package.RelocationPipelineID = closureTool.RelocationPipelineID;
                package.PipelineName = closureTool.PipelineName;
                package.NewSiteNetCFNPV = (double)(closureTool.NPVRestaurantCashflows.HasValue ? closureTool.NPVRestaurantCashflows.Value : 0);
                package.OriginalCFNPV = (double)(closureTool.NPVSC.HasValue ? closureTool.NPVSC.Value : 0);
                package.NetOperatingIncome = closureTool.OperatingIncome;
            }

            var signedConfig = System.Configuration.ConfigurationManager.AppSettings["ClosurePackage_SignedPackage_Receiver"];
            var signedPackageSavable = project.Status == ProjectStatus.Finished && signedConfig.Split(':')[1] == ClientCookie.UserCode && package.NeedUploadSign();

            return Ok(new
            {
                ClosureInfo = closureInfo,
                Package = package,
                ClosureTool = closureTool,
                ClosureWoCheckList = closureWoCheckList,
                Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Closure_ClosurePackage),
                Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Closure_ClosurePackage),
                SignedPackageSavable = signedPackageSavable
            });
        }

        [Route("api/ClosurePackage/GetClosureCommers/{refTableName}/{refTableId}")]
        [HttpGet]
        public IHttpActionResult GetClosureCommers(string refTableName, Guid refTableId)
        {
            var list = _db.ProjectComment.Where(e => e.RefTableName == refTableName && e.RefTableId == refTableId
                  && string.IsNullOrEmpty(e.Content) && e.SourceCode == "Closure").OrderByDescending(e => e.CreateTime);
            return Ok(list);
        }

        [Route("api/ClosurePackage/LoadAttachment/{projectID}")]
        [HttpGet]
        public IHttpActionResult LoadAttachment(string projectId)
        {
            var list = ClosurePackage.QueryAttList(projectId);

            var package = ClosurePackage.FirstOrDefault(i => i.ProjectId == projectId && i.IsHistory == false);
            if (package != null)
            {
                //第一次打开package时，如果不存在cover文件，自动生成
                if (!list.Any(i => i.TypeCode == "Cover"))
                {
                    var closureInfoEntity = ClosureInfo.GetByProjectId(projectId);
                    //生成cover文件
                    string coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_Closure_Cover_Template;
                    StoreBasicInfo store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == closureInfoEntity.USCode);

                    string coverTempPath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + Guid.NewGuid() + ".xlsx";
                    File.Copy(coverPath, coverTempPath);
                    var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(coverTempPath), ExcelDataInputType.ClosureCover);
                    var inputInfo = new ExcelInputDTO
                    {
                        StoreNameEN = store.NameENUS,
                        USCode = closureInfoEntity.USCode,
                        City = store.CityENUS,
                        Market = store.MarketENUS,
                        ActualCloseDate = closureInfoEntity.ActualCloseDate.Value.ToString("yyyy-MM-dd")
                    };
                    excelOutputDirector.Input(inputInfo);

                    string coverName = "Cover";
                    string coverExtention = Path.GetExtension(coverTempPath);
                    var coverItem = _db.Attachment.FirstOrDefault(e => e.RefTableName == ClosurePackage.TableName && e.RefTableID == package.Id.ToString() && e.TypeCode == coverName);


                    coverItem = new Attachment();
                    coverItem.Name = coverName + coverExtention;
                    coverItem.InternalName = Path.GetFileName(coverTempPath);
                    coverItem.RefTableName = ClosurePackage.TableName;
                    coverItem.RefTableID = package.Id.ToString();
                    coverItem.RelativePath = "//";
                    coverItem.Extension = coverExtention;
                    coverItem.CreateTime = DateTime.Now;

                    coverItem.TypeCode = coverName;
                    coverItem.CreatorID = ClientCookie.UserCode;


                    using (FileStream stream = new FileStream(coverTempPath, FileMode.Open))
                    {
                        coverItem.Length = (int)stream.Length;
                    }
                    coverItem.Name = coverName;
                    Attachment.SaveSigleFile(coverItem);
                    var result = _db.SaveChanges();
                    list.Add(coverItem);
                }
            }
            return Ok(list);



        }

        [Route("api/ClosurePackage/DownLoadAttachment/{id}")]
        [HttpGet]
        public IHttpActionResult DownLoadAttachment(Guid id)
        {
            var current = HttpContext.Current;
            var att = _db.Attachment.Find(id);

            string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + att.InternalName;

            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DataConverter.ToHexString(att.Name + att.Extension));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + absolutePath + "");
            current.Response.End();
            return Ok();

        }

        [Route("api/ClosurePackage")]
        [HttpGet]
        public void PackageAttachment(string projectId, string Id = "")
        {
            var closureInfoEntity = ClosureInfo.GetByProjectId(projectId);

            var exEntity = ClosureExecutiveSummary.FirstOrDefault(e => e.ProjectId == projectId);
            var legalReviewEntity = ClosureLegalReview.Get(projectId);

            var packageInfo = ClosurePackage.Get(projectId);
            if (packageInfo == null)
            {
                packageInfo = new ClosurePackage();
                packageInfo.ProjectId = projectId;
                packageInfo.CreateTime = DateTime.Now;
                packageInfo.CreateUserAccount = ClientCookie.UserCode;
                if (!string.IsNullOrEmpty(Id))
                    packageInfo.Id = new Guid(Id);
                else
                    packageInfo.Id = Guid.NewGuid();
                packageInfo.IsHistory = false;
                ClosurePackage.Add(packageInfo);
            }

            //生成打印图片
            var printPath = GenPrintAttachment(closureInfoEntity, packageInfo, PrintFileType.Pdf);

            ////生成cover文件
            //string coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_Closure_Cover_Template;
            //StoreBasicInfo store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == closureInfoEntity.USCode);

            //Excel tempExcel = new Excel();
            //tempExcel.Open(coverPath);

            //var sheet = tempExcel.Sheets["PMT"];
            //sheet.Cells[1, 1].StrValue = store.NameENUS;
            //sheet.Cells[2, 1].StrValue = closureInfoEntity.USCode;
            //sheet.Cells[4, 1].StrValue = store.CityENUS;
            //sheet.Cells[5, 1].StrValue = store.MarketENUS;
            //if (closureInfoEntity.ActualCloseDate != null)
            //    sheet.Cells[6, 1].StrValue = closureInfoEntity.ActualCloseDate.Value.ToString("yyyy-MM-dd");
            //string coverTempPath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + Guid.NewGuid() + ".xls";
            //tempExcel.Save(coverTempPath);


            //string coverName = "Cover";
            //string coverExtention = Path.GetExtension(coverTempPath);
            //var coverItem = _db.Attachment.FirstOrDefault(e => e.RefTableName == ClosurePackage.TableName
            //                                                   && e.RefTableID == entity.Id.ToString() &&
            //                                                   e.TypeCode == coverName);


            //coverItem = new Attachment();
            //coverItem.Name = coverName + coverExtention;
            //coverItem.InternalName = Path.GetFileName(coverTempPath);
            //coverItem.RefTableName = ClosurePackage.TableName;
            //coverItem.RefTableID = entity.Id.ToString();
            //coverItem.RelativePath = "//";
            //coverItem.Extension = coverExtention;
            //coverItem.CreateTime = DateTime.Now;

            //coverItem.TypeCode = coverName;
            //coverItem.CreatorID = ClientCookie.UserCode;


            //using (FileStream stream = new FileStream(coverTempPath, FileMode.Open))
            //{
            //    coverItem.Length = (int)stream.Length;
            //}
            //coverItem.Name = coverName;

            //Attachment.SaveSigleFile(coverItem);

            //var result = _db.SaveChanges();

            var attList = ClosurePackage.QueryAttList(projectId);

            var current = System.Web.HttpContext.Current;
            //var urlList = atts.Select(at => SiteFilePath.UploadFiles_DIRECTORY + "\\" + at.InternalName);
            var printFileName = Path.GetFileName(printPath);
            var printExtention = Path.GetExtension(printPath);
            attList.Add(new Attachment() { InternalName = printFileName, Name = FlowCode.Closure + " Print", Extension = printExtention });



            string packageFileUrl = ZipHandle.ExeFiles(attList);
            string fileName = DateTime.Now.ToString("yyMMdd");
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DataConverter.ToHexString("Package" + fileName + ".zip"));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile(packageFileUrl);
            current.Response.End();


        }

        private string GenPrintAttachment(ClosureInfo closureInfoEntity, ClosurePackage entity, PrintFileType fileType)
        {
            var storeInfo = StoreBasicInfo.GetStore(closureInfoEntity.USCode);
            StoreBasicInfo store = storeInfo.StoreBasicInfo;

            //生成Print文件
            var printDic = new Dictionary<string, string>();
            printDic.Add("WorkflowName", FlowCode.Closure);
            printDic.Add("ProjectID", entity.ProjectId);
            printDic.Add("USCode", closureInfoEntity.USCode);
            printDic.Add("StoreNameEN", store.NameENUS);
            printDic.Add("Market", store.MarketENUS);
            printDic.Add("Region", store.RegionENUS);
            printDic.Add("StoreNameCN", store.NameZHCN);
            printDic.Add("City", store.CityENUS);
            printDic.Add("StoreAge", (DateTime.Now.Year - store.OpenDate.Year).ToString());
            printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            //var currentLeaseEndYear = storeInfo.CurrentYear - int.Parse(storeInfo.StoreContractInfo.EndYear);
            printDic.Add("CurrentLeaseENDYear", storeInfo.StoreContractInfo.EndYear);
            printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            printDic.Add("AssetsActor", storeInfo.StoreDevelop.AssetRepName);
            printDic.Add("AssetsRep", closureInfoEntity.AssetActorNameENUS);

            printDic.Add("ClosureType", closureInfoEntity.ClosureTypeNameENUS);
            printDic.Add("LandlordName", closureInfoEntity.LandlordName);
            //if (storeInfo.StoreBeContractInfo != null)
            //{
            //    printDic.Add("LandlordName", storeInfo.StoreBeContractInfo.LandlordName);
            //    printDic.Add("LeaseExpireDate", storeInfo.StoreBeContractInfo.LeaseEndDate);
            //}
            //else
            //{
            //    printDic.Add("LandlordName", "");
            //    printDic.Add("LeaseExpireDate", "");
            //}
            if (storeInfo.StoreContractInfo != null)
            {
                printDic.Add("LeaseExpireDate", storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                printDic.Add("LeaseExpireDate", "");
            }
            //Submission and Approval Records - 只显示通过意见
            List<SubmissionApprovalRecord> recordList = new List<SubmissionApprovalRecord>();
            //Submission and Approval Records Details — 所有意见
            List<SubmissionApprovalRecord> recordDetailList = new List<SubmissionApprovalRecord>();

            ProjectComment projectCommentBll = new ProjectComment();
            ProjectCommentCondition condition = new ProjectCommentCondition();
            var package = ClosurePackage.Get(entity.ProjectId);
            condition.RefTableName = ClosurePackage.TableName;
            condition.RefTableId = entity.Id;
            condition.SourceCode = FlowCode.Closure;
            condition.Status = ProjectCommentStatus.Submit;

            var commentList = VProjectComment.SearchVListForPDF(condition);
            var commentDetailList = VProjectComment.SearchVList(condition);

            SubmissionApprovalRecord record = null;
            foreach (var item in commentList)
            {
                record = new SubmissionApprovalRecord();
                record.ActionName = item.ActionDesc;
                if (item.CreateTime != null)
                {
                    record.OperationDate = item.CreateTime.Value;
                }
                record.OperatorID = item.UserAccount;
                record.OperatorName = item.UserNameENUS;
                record.OperatorTitle = item.PositionName;
                record.Comments = item.Content;
                recordList.Add(record);
            }
            recordList = recordList.OrderBy(i => i.OperationDate).ToList();

            foreach (var item in commentDetailList)
            {
                record = new SubmissionApprovalRecord();
                record.ActionName = item.ActionDesc;
                if (item.CreateTime != null)
                {
                    record.OperationDate = item.CreateTime.Value;
                }
                record.OperatorID = item.UserAccount;
                record.OperatorName = item.UserNameENUS;
                record.OperatorTitle = item.PositionName;
                record.Comments = item.Content;
                recordDetailList.Add(record);
            }
            recordDetailList = recordDetailList.OrderBy(i => i.OperationDate).ToList();

            var closureToolEntity = ClosureTool.Get(entity.ProjectId);
            if (closureToolEntity != null)
            {
                printDic.Add("TotalOneOffCosts", closureToolEntity.TotalOneOffCosts.HasValue ? DataConverter.ToMoney(closureToolEntity.TotalOneOffCosts.Value) : string.Empty);
                printDic.Add("Compensation", closureToolEntity.Compensation.HasValue ? DataConverter.ToMoney(closureToolEntity.Compensation.Value) : string.Empty);
            }


            if (closureInfoEntity.ActualCloseDate != null)
            {
                string closureDate = string.Empty;
                if (closureInfoEntity.ActualCloseDate.HasValue && closureInfoEntity.ActualCloseDate.Value.Year == 1900)
                {
                    closureDate = string.Empty;
                }
                else
                {
                    closureDate = closureInfoEntity.ActualCloseDate.Value.ToString("yyyy-MM-dd");
                }
                printDic.Add("ClosureDate", closureDate);
            }

            if (entity.OriginalCFNPV.HasValue)
            {
                printDic.Add("OriginalCFNPV", DataConverter.ToMoney(entity.OriginalCFNPV.Value));
            }

            printDic.Add("Relocation", closureInfoEntity.RelocationNameENUS);


            printDic.Add("NetOperatingIncome",
                entity.NetOperatingIncome.HasValue ? DataConverter.ToMoney(entity.NetOperatingIncome.Value) : string.Empty);

            printDic.Add("RelocatedPipelineID",
                entity.RelocationPipelineID.HasValue ? entity.RelocationPipelineID.ToString() : string.Empty);
            printDic.Add("PipelineName", entity.PipelineName);
            printDic.Add("NewSiteNetCFNPV", entity.NewSiteNetCFNPV.HasValue ? DataConverter.ToMoney(entity.NewSiteNetCFNPV.Value) : string.Empty);
            printDic.Add("OtherCFNPV", entity.OtherCFNPV.HasValue ? DataConverter.ToMoney(entity.OtherCFNPV.Value) : string.Empty);
            printDic.Add("NetGain", entity.NetGain.HasValue ? DataConverter.ToMoney(entity.NetGain.Value) : string.Empty);
            printDic.Add("ReasonDescription", entity.ReasonDescriptionForNegativeNetGain);

            string result = string.Empty;
            if (fileType == PrintFileType.Pdf)
            {
                result = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.Default, printDic, recordList, recordDetailList);
            }
            else
            {
                result = HtmlConversionUtility.ConvertToImage(HtmlTempalteType.Default, printDic, recordList, recordDetailList);
            }
            return result;
        }



        [Route("api/ClosurePackage/UploadSignedTerminationAgreement/{projectid}")]
        [HttpPost]
        public void UploadSignedTerminationAgreement(string projectid)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection fileCollect = request.Files;
            int result = 0;
            if (fileCollect.Count > 0) //如果集合的数量大于0
            {
                var woEntity = ClosurePackage.Get(projectid);

                if (woEntity == null)
                {
                    woEntity = new ClosurePackage();
                    woEntity.Id = Guid.NewGuid();
                    woEntity.CreateTime = DateTime.Now;
                    woEntity.CreateUserAccount = ClientCookie.UserCode;
                    woEntity.ProjectId = projectid;
                    woEntity.IsHistory = false;
                    _db.ClosurePackage.Add(woEntity);
                }

                //用key获取单个文件对象HttpPostedFile
                var fileSave = fileCollect[0];
                string fileName = Path.GetFileName(fileSave.FileName);
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                string internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                var att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosurePackage.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = fileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.CreatorID = ClientCookie.UserCode;
                att.TypeCode = "SignedTerminationAgreement";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                Attachment.SaveSigleFile(att);
                result = _db.SaveChanges();
            }
        }

        [Route("api/ClosurePackage/UploadSignedPackage/{projectid}")]
        [HttpPost]
        public void UploadSignedPackage(string projectid)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection fileCollect = request.Files;
            int result = 0;
            if (fileCollect.Count > 0) //如果集合的数量大于0
            {

                var woEntity = ClosurePackage.Get(projectid);

                if (woEntity == null)
                {
                    woEntity = new ClosurePackage();
                    woEntity.Id = Guid.NewGuid();
                    woEntity.CreateTime = DateTime.Now;
                    woEntity.ProjectId = projectid;
                    woEntity.CreateUserAccount = ClientCookie.UserCode;
                    woEntity.IsHistory = false;
                    _db.ClosurePackage.Add(woEntity);
                }

                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = fileCollect[0];
                string fileName = Path.GetFileName(fileSave.FileName);
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                string internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                var att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosurePackage.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = fileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;

                att.TypeCode = "SignedPackage";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorID = ClientCookie.UserCode;
                Attachment.SaveSigleFile(att);


                result = _db.SaveChanges();
            }
        }

        [Route("api/ClosurePackage/UploadOthers/{projectid}")]
        [HttpPost]
        public void UploadOthers(string projectid)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection fileCollect = request.Files;
            int result = 0;
            if (fileCollect.Count > 0) //如果集合的数量大于0
            {
                var woEntity = ClosurePackage.Get(projectid);

                if (woEntity == null)
                {
                    woEntity = new ClosurePackage();
                    woEntity.Id = Guid.NewGuid();
                    woEntity.CreateTime = DateTime.Now;
                    woEntity.CreateUserAccount = ClientCookie.UserCode;
                    woEntity.ProjectId = projectid;
                    woEntity.IsHistory = false;
                    _db.ClosurePackage.Add(woEntity);
                }

                //用key获取单个文件对象HttpPostedFile
                var fileSave = fileCollect[0];
                string fileName = Path.GetFileName(fileSave.FileName);
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                string internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                var att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosurePackage.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = fileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.CreatorID = ClientCookie.UserCode;
                att.TypeCode = "Others";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                Attachment.SaveSigleFile(att);
                result = _db.SaveChanges();
            }
        }

        [Route("api/ClosurePackage/ProcessClosurePackage")]
        [HttpPost]
        public IHttpActionResult ProcessClosurePackage(ClosurePackage entity)
        {

            string actionLower = entity.Action.ToLower();
            string account = ClientCookie.UserCode;
            ClosureInfo closureInfoEntity = null;
            var _listDataFields = new List<ProcessDataField>();
            switch (entity.Action)
            {
                case "Submit":

                    // projectInfoHandler.UpdateProjectNode(entity.ProjectId, FlowCode.Closure_ClosurePackage,
                    //NodeCode.Finish);
                    break;
                case "Return":
                    ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosurePackage);
                    break;
                case "Decline":
                    ProjectInfo.Reject(entity.ProjectId, FlowCode.Closure_ClosurePackage);
                    break;
                case "ReSubmit":
                    string _procCode = WFClosurePackage.ProcessCode;

                    _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode)); // 发起人

                    _listDataFields.Add(new ProcessDataField("dest_MarketMgr", entity.MarketMgr));
                    //entity.   Market Manager
                    _listDataFields.Add(new ProcessDataField("dest_GMApprovers", entity.DD_GM_FC_RDD));
                    //entity.   DD_GM_FC_RDD
                    _listDataFields.Add(new ProcessDataField("dest_VPGM", entity.VPGM)); //entity.   VPGM
                    //_listDataFields.Add(new ProcessDataField("dest_DevVP", entity.Dev_VP));//entity.   Dev VP
                    _listDataFields.Add(new ProcessDataField("dest_CDO", entity.CDO)); //entity.   CDO
                    _listDataFields.Add(new ProcessDataField("dest_CFO", entity.CFO)); //entity.   CFO
                    //_listDataFields.Add(new ProcessDataField("dest_Director", entity.Director));//entity.   CFO
                    _listDataFields.Add(new ProcessDataField("dest_MngDirector", entity.MngDirector));

                    string strReceiver = "";
                    //if (!string.IsNullOrEmpty(entity.MCCLAssetMgr))
                    //    strReceiver = entity.MCCLAssetMgr + ";";
                    if (!string.IsNullOrEmpty(entity.MCCLAssetDtr))
                        strReceiver += entity.MCCLAssetDtr + ";";

                    if (strReceiver != "")
                        strReceiver = strReceiver.Substring(0, strReceiver.Length - 1);

                    _listDataFields.Add(new ProcessDataField("dest_Receiver", strReceiver));
                    _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));

                    closureInfoEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
                    var printPath = GenPrintAttachment(closureInfoEntity, entity, PrintFileType.Image);
                    //var diskPath = SiteFilePath.ConvertToDiskPath(printPath);
                    _listDataFields.Add(new ProcessDataField("EmailAttachments", printPath));

                    break;
            }

            // To-Do K2 action
            var _action = BPMHelper.ConvertToProcAction(actionLower);

            if (actionLower.Equals(ProjectAction.Return, StringComparison.CurrentCultureIgnoreCase))
            {
                TaskWork.Finish(e => e.RefID == entity.ProjectId
                                     && e.TypeCode == FlowCode.Closure_ClosurePackage
                                     && e.Status == TaskWorkStatus.UnFinish);
                //&& e.K2SN != entity.SN);
            }

            var result = false;
            if (_listDataFields.Count > 0)
                result = K2FxContext.Current.ApprovalProcess(entity.SN, account, BaseWFEntity.ParseActionName(_action.ToString()), entity.Comments, _listDataFields);
            else
                result = K2FxContext.Current.ApprovalProcess(entity.SN, account, BaseWFEntity.ParseActionName(_action.ToString()), entity.Comments);



            if (result && actionLower == "resubmit")
            {
                entity.SaveApproveUsers(ProjectAction.ReSubmit);
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosurePackage, NodeCode.Closure_ClosurePackage_Input);
            }

            SaveCommers(entity, entity.Action, ProjectCommentStatus.Submit);
            return Ok();
        }

        /// <summary>
        /// 是否能够Reject
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [Route("api/ClosurePackage/EnableReject/{sn}")]
        [HttpGet]
        public IHttpActionResult EnableReject(string sn)
        {
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, ClientCookie.UserCode);
            var result = !WFClosurePackage.NormalActivities.Contains(resultStr);
            return Ok(result);
        }

        /// <summary>
        /// Package流转中Actor是否能够Submit
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [Route("api/ClosurePackage/EnableSubmit/{projectId}")]
        [HttpGet]
        public IHttpActionResult EnableSubmit(string projectId)
        {
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            var projectInfo = ProjectInfo.Get(projectId, FlowCode.Closure_ClosurePackage.ToString());
            var result = false;
            if (projectInfo.NodeCode != NodeCode.Finish && isActor)
            {
                if (TaskWork.Count(i => i.Status == TaskWorkStatus.UnFinish && i.RefID == projectId && i.ReceiverAccount == ClientCookie.UserCode && i.SourceCode == FlowCode.Closure && i.TypeCode == FlowCode.Closure_ClosurePackage) > 0)
                    result = true;
            }
            return Ok(result);
        }

        /// <summary>
        /// Package流转结束后Actor是否能够Submit
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [Route("api/ClosurePackage/EnableVirtualSubmit/{projectId}")]
        [HttpGet]
        public IHttpActionResult EnableVirtualSubmit(string projectId)
        {
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            var projectInfo = ProjectInfo.Get(projectId, FlowCode.Closure_ClosurePackage.ToString());
            var result = false;
            if (projectInfo.NodeCode == NodeCode.Finish && isActor)
            {
                if (TaskWork.Count(i => i.Status == TaskWorkStatus.UnFinish && i.RefID == projectId && i.ReceiverAccount == ClientCookie.UserCode && i.SourceCode == FlowCode.Closure && i.TypeCode == FlowCode.Closure_ClosurePackage) > 0)
                    result = true;
            }
            return Ok(result);
        }

        [Route("api/ClosurePackage/GetK2Status/{userAccount}/{sn}/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetK2Status(string userAccount, string sn, string procInstID)
        {
            // Load K2 Process
            bool result = false;
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, userAccount);
            result = !resultStr.Equals(WFClosurePackage.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

            if (result)
            {
                // 非发起人节点
                resultStr = "Process";
            }
            else
            {
                resultStr = "Edit";
            }

            object resultObj = new
            {
                Status = resultStr
            };
            return Ok(resultObj);

        }

        [Route("api/ClosurePackage/SaveClosurePackage")]
        [HttpPost]
        public IHttpActionResult SaveClosurePackage(ClosurePackage entity)
        {
            if (entity.Id == new Guid())
            {
                entity.Id = Guid.NewGuid();
                entity.IsHistory = false;
                ClosurePackage.Add(entity);
            }
            else
            {
                if (entity.CreateTime == null)
                    entity.CreateTime = DateTime.Now;
                if (string.IsNullOrEmpty(entity.CreateUserAccount))
                    entity.CreateUserAccount = ClientCookie.UserCode;
                if (!ClosurePackage.Any(p => p.Id == entity.Id))
                {
                    entity.IsHistory = false;
                    ClosurePackage.Add(entity);
                }
                else
                {
                    ClosurePackage.Update(entity);
                }
            }
            string action = string.IsNullOrEmpty(entity.Action) ?
                ProjectCommentAction.Submit : entity.Action;


            SaveCommers(entity, action, ProjectCommentStatus.Save);
            return Ok(entity.Id);
        }

        [Route("api/ClosurePackage/FinishClosurePackage")]
        [HttpPost]
        public IHttpActionResult FinishClosurePackage(ClosurePackage entity)
        {
            if (entity.Id == new Guid())
            {
                entity.Id = Guid.NewGuid();
                entity.IsHistory = false;
                _db.ClosurePackage.Add(entity);
            }
            else
            {
                if (entity.CreateTime == null)
                    entity.CreateTime = DateTime.Now;
                if (string.IsNullOrEmpty(entity.CreateUserAccount))
                    entity.CreateUserAccount = ClientCookie.UserCode;
                _db.ClosurePackage.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
            }
            string action = string.IsNullOrEmpty(entity.Action) ?
                ProjectCommentAction.Submit : entity.Action;

            SaveCommers(entity, action, ProjectCommentStatus.Save);

            int result = _db.SaveChanges();

            var task = TaskWork.FirstOrDefault(i => i.Status == TaskWorkStatus.UnFinish && i.RefID == entity.ProjectId && i.ReceiverAccount == ClientCookie.UserCode && i.SourceCode == FlowCode.Closure && i.TypeCode == FlowCode.Closure_ClosurePackage);
            if (task != null)
            {
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;
                task.Url = task.GetViewUrl();
                task.Update();
            }

            return Ok(entity.Id);
        }

        [Route("api/ClosurePackage/PostClosurePackage")]
        [HttpPost]
        public IHttpActionResult PostClosurePackage(ClosurePackage entity)
        {
            //var tran = _db.Database.BeginTransaction();

            try
            {
                // Log
                string _debugInfo = string.Format("Start Run PostClosurePackage - Entity: {0}", JsonConvert.SerializeObject(entity));
                Log4netHelper.WriteInfo(_debugInfo);
                //var en = closurePackageHandler.GetByProjectID(entity.ProjectId);
                var packageInfo = ClosurePackage.Get(entity.ProjectId);
                if (packageInfo == null)
                {
                    entity.Id = Guid.NewGuid();
                    entity.IsHistory = false;
                }
                if (entity.CreateTime == null)
                    entity.CreateTime = DateTime.Now;
                if (string.IsNullOrEmpty(entity.CreateUserAccount))
                    entity.CreateUserAccount = ClientCookie.UserCode;

                var task = TaskWork.FirstOrDefault(
                    e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                         && e.TypeCode == FlowCode.Closure_ClosurePackage && e.RefID == entity.ProjectId
                    );





                // Start K2 Process
                string _procCode = WFClosurePackage.ProcessCode;

                List<ProcessDataField> _listDataFields = new List<ProcessDataField>();

                _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));// 发起人

                _listDataFields.Add(new ProcessDataField("dest_MarketMgr", entity.MarketMgr));//entity.   Market Manager
                _listDataFields.Add(new ProcessDataField("dest_GMApprovers", entity.DD_GM_FC_RDD));//entity.   DD_GM_FC_RDD
                _listDataFields.Add(new ProcessDataField("dest_VPGM", entity.VPGM));//entity.   VPGM
                //_listDataFields.Add(new ProcessDataField("dest_DevVP", entity.Dev_VP));//entity.   Dev VP
                _listDataFields.Add(new ProcessDataField("dest_CDO", entity.CDO));//entity.   CDO
                _listDataFields.Add(new ProcessDataField("dest_CFO", entity.CFO));//entity.   CFO
                //_listDataFields.Add(new ProcessDataField("dest_Director", entity.Director));//entity.   CFO
                _listDataFields.Add(new ProcessDataField("dest_MngDirector", entity.MngDirector));

                var signedConfig = System.Configuration.ConfigurationManager.AppSettings["ClosurePackage_SignedPackage_Receiver"];

                _listDataFields.Add(new ProcessDataField("dest_AssetMgr", string.Join(";", signedConfig.Split(':')[1])));
                _listDataFields.Add(new ProcessDataField("IsNeedAssetMgrUpload", entity.NeedUploadSign().ToString(), "BOOLEAN"));


                //string strReceiver = "";
                //if (!string.IsNullOrEmpty(entity.MCCLAssetMgr))
                //    strReceiver = entity.MCCLAssetMgr;

                //if (strReceiver != "")
                //    strReceiver = strReceiver.Substring(0, strReceiver.Length - 1);

                //_listDataFields.Add(new ProcessDataField("dest_Receiver", strReceiver));
                _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));

                //var closureInfoEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
                //var printPath = GenPrintAttachment(closureInfoEntity, entity, PrintFileType.Image);
                //var diskPath = SiteFilePath.ConvertToDiskPath(printPath);
                //_listDataFields.Add(new ProcessDataField("EmailAttachments", printPath));

                if (_listDataFields.Exists(o => string.IsNullOrEmpty(o.DataFieldValue)))
                {
                    // 2014.09.26 victor.huang: 添加抛错控制，通知前端流程未能正常发起,同时回滚之前做的数据变更
                    var _emptyDFs = _listDataFields.Where(o => string.IsNullOrEmpty(o.DataFieldValue)).Select(o => o.DataFieldKey).ToList();

                    string _strEmptyDFs = ConvertHelper.ToDelimitedString(_emptyDFs, ",");

                    string _message = string.Format("输入的值为空的DataField Key是：{0}", _strEmptyDFs);
                    //tran.Rollback();
                    throw new Exception(_message);
                }
                else
                {

                    //将TaskWork生成任务传给K2
                    var taskJson = TaskWork.ConvertToJson(task);
                    _listDataFields.Add(new ProcessDataField("ProjectTaskInfo", taskJson));

                    _debugInfo = string.Format("ProcessDataFields: {0}", JsonConvert.SerializeObject(_listDataFields));
                    Log4netHelper.WriteInfo(_debugInfo);

                    int procInstId = K2FxContext.Current.StartProcess(_procCode, ClientCookie.UserCode, _listDataFields);
                    using (TransactionScope tranScope = new TransactionScope())
                    {
                        // Update Task
                        if (task != null)
                        {
                            //添加业务数据信息在taskwork表中--Cary
                            task.RefTableName = ClosurePackage.TableName;
                            task.RefTableId = entity.Id;

                            task.Status = TaskWorkStatus.Finished;
                            task.FinishTime = DateTime.Now;
                            //task.Url = SiteInfo.WebUrl + "/closure/Main#/Closure/WOCheckListView/" + task.RefID;
                            task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_ClosurePackage, task.RefID);
                            task.Update();
                        }
                        _debugInfo = string.Format(" ProcessInstID: {0}", procInstId);
                        Log4netHelper.WriteInfo(_debugInfo);

                        if (procInstId > 0)
                        {
                            // 更新业务表 ProcInstID
                            entity.ProcInstID = procInstId;
                            if (!ClosurePackage.Any(p => p.Id == entity.Id))
                            {
                                entity.Add();
                            }
                            else
                            {
                                entity.Update();
                            }
                            SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Submit);


                            entity.SaveApproveUsers(ProjectAction.Submit);

                            ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosurePackage, NodeCode.Closure_ClosurePackage_Input);
                        }
                        tranScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.Log4netWriteErrorLog("PostClosurePackage执行异常!", ex);
                throw ex;
            }


            return Ok(entity.Id);
        }

        private void SaveCommers(ClosurePackage entity, string action, ProjectCommentStatus status, bool isNeedSaveChange = true)
        {

            if (status == ProjectCommentStatus.Save)
            {

                var list = ProjectComment.Search(c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == ProjectCommentStatus.Save && c.RefTableName == ClosurePackage.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    if (!string.IsNullOrEmpty(entity.Comments))
                        closureCommens.Content = entity.Comments.Trim();
                    closureCommens.Update();
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }
            else
            {
                var currentComment = ProjectComment.FirstOrDefault(i => i.RefTableName == ClosurePackage.TableName && i.RefTableId == entity.Id && i.Status == ProjectCommentStatus.Save && i.SourceCode == FlowCode.Closure && i.CreateUserAccount == ClientCookie.UserCode);
                if (currentComment != null)
                {
                    currentComment.Content = entity.Comments == null ? string.Empty : entity.Comments.Trim();
                    currentComment.Status = ProjectCommentStatus.Submit;
                    currentComment.ProcInstID = entity.ProcInstID;
                    currentComment.Update();
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }

        }

        private void AddProjectComment(ClosurePackage entity, string action,
            ProjectCommentStatus status)
        {
            ProjectComment closureCommens = new ProjectComment();
            closureCommens.RefTableId = entity.Id;
            closureCommens.RefTableName = ClosurePackage.TableName;


            closureCommens.TitleNameENUS = ClientCookie.TitleENUS;
            closureCommens.TitleNameZHCN = ClientCookie.TitleENUS;
            closureCommens.TitleCode = ClientCookie.TitleENUS;

            closureCommens.CreateTime = DateTime.Now;
            closureCommens.CreateUserAccount = ClientCookie.UserCode;

            closureCommens.UserAccount = ClientCookie.UserCode;
            closureCommens.UserNameENUS = ClientCookie.UserNameENUS;
            closureCommens.UserNameZHCN = ClientCookie.UserNameZHCN;
            closureCommens.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            if (entity.ProcInstID > 0)
            {
                closureCommens.ProcInstID = entity.ProcInstID;
            }
            closureCommens.Id = Guid.NewGuid();
            if (!string.IsNullOrEmpty(entity.Comments))
            {
                closureCommens.Content = entity.Comments.Trim();
            }
            closureCommens.Action = action;
            closureCommens.Status = status;

            closureCommens.SourceCode = FlowCode.Closure;
            closureCommens.SourceNameENUS = FlowCode.Closure;
            closureCommens.SourceNameZHCN = "关店流程";
            closureCommens.Add();
        }

        [Route("api/ClosurePackage/Edit")]
        [HttpPost]
        public IHttpActionResult Edit(ClosurePackage entity)
        {
            //ModifyProject(entity, ProjectAction.Edit);
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

        [Route("api/ClosurePackage/Recall")]
        [HttpPost]
        public IHttpActionResult Recall(ClosurePackage entity)
        {
            //
            bool _recallSuccess = false;
            if (entity.ProcInstID != null)
            {
                _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value, WFClosurePackage.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, entity.Comments);
                if (_recallSuccess)
                {


                    SaveCommers(entity, ProjectCommentAction.Recall, ProjectCommentStatus.Submit);
                }
            }
            if (!_recallSuccess)
            {
                throw new Exception("Recall失败");
            }
            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosurePackage, ProjectStatus.Recalled);

            return Ok();
        }

        [Route("api/ClosurePackage/GetApproverUsers")]
        [HttpGet]
        public IHttpActionResult GetApproverUsers(string projectId, string flowCode)
        {
            var packageInfo = ClosurePackage.Get(projectId);
            var approveUser = ApproveDialogUser.GetApproveDialogUser(packageInfo.Id.ToString());
            //var approveUser = ApproveDialogUser.GetApproveDialogUser(projectId, flowCode);
            string[] _noticeUserCodes = new string[0];
            var __noticeUserList = new List<string>();
            if (approveUser != null && !string.IsNullOrEmpty(approveUser.NoticeUsers))
            {
                _noticeUserCodes = approveUser.NoticeUsers.Split(';');
                foreach (var _noticeUserCode in _noticeUserCodes)
                {
                    if (!string.IsNullOrEmpty(_noticeUserCode))
                        __noticeUserList.Add(_noticeUserCode);
                }
            }

            var noticeUsers = Employee.GetSimpleEmployeeByCodes(__noticeUserList.ToArray());

            string[] _necessaryNoticeUserCodes = new string[0];
            var __necessaryNoticeUserList = new List<string>();
            if (approveUser != null && !string.IsNullOrEmpty(approveUser.NecessaryNoticeUsers))
            {
                _necessaryNoticeUserCodes = approveUser.NecessaryNoticeUsers.Split(';');
                foreach (var _necessaryNoticeUserCode in _necessaryNoticeUserCodes)
                {
                    if (!string.IsNullOrEmpty(_necessaryNoticeUserCode))
                        __necessaryNoticeUserList.Add(_necessaryNoticeUserCode);
                }
            }
            if (__necessaryNoticeUserList.Count == 0)
            {
                var defaultUser = NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Closure_ClosurePackage);
                foreach (var _necessaryConfig in defaultUser)
                {
                    if (!string.IsNullOrEmpty(_necessaryConfig.DefaultUserCode))
                        __necessaryNoticeUserList.Add(_necessaryConfig.DefaultUserCode);
                }
            }

            var necessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(__necessaryNoticeUserList.ToArray());

            return Ok(new
            {
                ApproveUser = approveUser,
                NoticeUsers = noticeUsers,
                NecessaryNoticeUsers = necessaryNoticeUsers
            });
        }

        private int ModifyProject(ClosurePackage entity, string action)
        {
            // 2014-08-05 victor.huang: Recall 或Edit 后不需要重新再生成Task，不然会多生成一条冗余记录
            //TaskWorkCondition condition = new TaskWorkCondition();
            //condition.ProjectId = entity.ProjectId;
            //condition.Url = "/closure/Main#/closure/WOCheckList/" + entity.ProjectId;
            //condition.UserAccount = ClientCookie.UserCode;
            //condition.UserNameENUS = entity.UserNameENUS;
            //condition.UserNameZHCN = entity.UserNameZHCN;

            //taskWorkBll.ReSendTaskWork(condition);
            if (action == ProjectAction.Edit)
            {
                entity.IsHistory = true;
                entity.LastUpdateTime = DateTime.Now;
                entity.LastUpdateUserAccount = ClientCookie.UserCode;


                _db.ClosurePackage.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;

                var objectCopy = new ObjectCopy();
                var newWo = objectCopy.AutoCopy(entity);
                newWo.Id = Guid.NewGuid();
                _db.ClosurePackage.Add(newWo);

                var projectEntity = ProjectInfo.Get(entity.ProjectId, FlowCode.Closure_ClosurePackage);
                projectEntity.Status = ProjectStatus.UnFinish;
                ProjectInfo.Update(projectEntity);
            }
            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosurePackage);

            var result = 0;

            result = _db.SaveChanges();

            return result;
        }

    }
}
