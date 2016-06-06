using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.Services.Common;
using System.Web;
using System.IO;
using Mcdonalds.AM.Services.Infrastructure;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Enums;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class AttachmentController : ApiController
    {
        [HttpGet]
        [Route("api/attachment")]
        public IHttpActionResult GetAttachmentsByTable(string refTableName, string projectId, string flowCode, string refTableId = "", bool includeCover = true)
        {
            try
            {
                GenerateCover(projectId, refTableName);

                var attachments = Attachment.GetAllAttachmentsIncludeRequire(refTableName, projectId, flowCode, refTableId);
                if (!includeCover)
                {
                    attachments = attachments.Where(e => e.TypeCode != "Cover").ToList();
                }
                attachments.ForEach(att =>
                {
                    if (att.ID != Guid.Empty)
                    {
                        att.FileURL = SiteFilePath.UploadFiles_URL + att.InternalName;
                    }
                });
                return Ok(attachments);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void GenerateCover(string projectId, string refTableName)
        {
            string coverPath = string.Empty;
            string coverName = "Cover";
            string coverTempPath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + Guid.NewGuid() + ".xlsx";
            string coverExtention = Path.GetExtension(coverTempPath);

            switch (refTableName)
            {
                case "MajorLeaseChangePackage":
                    coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.MajorLeaseChangeCove_Template;
                    var majorLeaseChangePackage = MajorLeaseChangePackage.GetMajorPackageInfo(projectId);
                    majorLeaseChangePackage.GenerateCoverEexcel(coverPath, coverTempPath);
                    break;
                case "ReimagePackage":
                    var rmgPackage = ReimagePackage.Get(projectId);
                    if (rmgPackage == null)
                        return;
                    if (!Attachment.Any(i => i.TypeCode == "Cover" && i.RefTableName == ReimagePackage.TableName && i.RefTableID.ToString() == rmgPackage.Id.ToString()))
                    {
                        var reimageInfoEntity = ReimageInfo.GetReimageInfo(projectId);
                        //生成cover文件
                        coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_Reimage_Cover_Template;
                        var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == reimageInfoEntity.USCode);

                        File.Copy(coverPath, coverTempPath);
                        var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(coverTempPath), ExcelDataInputType.ReimageCover);
                        var inputInfo = new ExcelInputDTO
                        {
                            Region = store.Region,
                            Province = store.ProvinceENUS,
                            City = store.CityENUS,
                            StoreName = store.NameENUS,
                            USCode = reimageInfoEntity.USCode,
                            OpenDate = store.OpenDate
                        };
                        excelOutputDirector.Input(inputInfo);

                        var coverItem = new Attachment();
                        coverItem.Name = coverName + coverExtention;
                        coverItem.InternalName = Path.GetFileName(coverTempPath);
                        coverItem.RefTableName = ReimagePackage.TableName;
                        coverItem.RefTableID = rmgPackage.Id.ToString();
                        coverItem.RelativePath = "//";
                        coverItem.Extension = coverExtention;
                        coverItem.CreateTime = DateTime.Now;

                        coverItem.TypeCode = coverName;
                        coverItem.CreatorID = ClientCookie.UserCode;
                        coverItem.RequirementId = AttachmentRequirement.FirstOrDefault(e => e.FlowCode == FlowCode.Reimage_Package && e.NameENUS == "Cover").Id;
                        coverItem.CreatorNameENUS = ClientCookie.UserNameENUS;
                        coverItem.CreatorNameZHCN = ClientCookie.UserNameZHCN;

                        using (FileStream stream = new FileStream(coverTempPath, FileMode.Open))
                        {
                            coverItem.Length = (int)stream.Length;
                        }
                        Attachment.SaveSigleFile(coverItem);
                    }
                    break;
                case "TempClosurePackage":
                    var tempClosurePackage = TempClosurePackage.Get(projectId);
                    if (tempClosurePackage == null)
                        return;
                    if (!Attachment.Any(i => i.TypeCode == "Cover" && i.RefTableName == tempClosurePackage.TableName && i.RefTableID.ToString() == tempClosurePackage.Id.ToString()))
                    {
                        var tempClosureInfoEntity = TempClosureInfo.Get(projectId);
                        //生成cover文件
                        coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_TempClosure_Cover_Template;
                        var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == tempClosureInfoEntity.USCode);

                        File.Copy(coverPath, coverTempPath);
                        var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(coverTempPath), ExcelDataInputType.TempClosureCover);
                        var inputInfo = new ExcelInputDTO
                        {
                            StoreNameEN = store.NameENUS,
                            USCode = tempClosureInfoEntity.USCode,
                            City = store.CityENUS,
                            Market = store.MarketENUS,
                            ActualCloseDate = tempClosureInfoEntity.ActualTempClosureDate.ToString("yyyy-MM-dd")
                        };
                        excelOutputDirector.Input(inputInfo);

                        var coverItem = new Attachment();
                        coverItem.Name = coverName + coverExtention;
                        coverItem.InternalName = Path.GetFileName(coverTempPath);
                        coverItem.RefTableName = tempClosurePackage.TableName;
                        coverItem.RefTableID = tempClosurePackage.Id.ToString();
                        coverItem.RelativePath = "//";
                        coverItem.Extension = coverExtention;
                        coverItem.CreateTime = DateTime.Now;

                        coverItem.TypeCode = coverName;
                        coverItem.CreatorID = ClientCookie.UserCode;
                        //coverItem.RequirementId = AttachmentRequirement.FirstOrDefault(e => e.FlowCode == FlowCode.TempClosure_ClosurePackage && e.NameENUS == "Cover").Id;
                        coverItem.CreatorNameENUS = ClientCookie.UserNameENUS;
                        coverItem.CreatorNameZHCN = ClientCookie.UserNameZHCN;

                        using (FileStream stream = new FileStream(coverTempPath, FileMode.Open))
                        {
                            coverItem.Length = (int)stream.Length;
                        }
                        Attachment.SaveSigleFile(coverItem);
                    }
                    break;
                case "ClosurePackage":
                    var closurePackage = ClosurePackage.Get(projectId);
                    if (closurePackage == null)
                        return;
                    if (!Attachment.Any(i => i.TypeCode == "Cover" && i.RefTableName == ClosurePackage.TableName && i.RefTableID.ToString() == closurePackage.Id.ToString()))
                    {
                        var closureInfoEntity = ClosureInfo.GetByProjectId(projectId);
                        //生成cover文件
                        coverPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_Closure_Cover_Template;
                        var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == closureInfoEntity.USCode);

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

                        var coverItem = new Attachment();
                        coverItem.Name = coverName + coverExtention;
                        coverItem.InternalName = Path.GetFileName(coverTempPath);
                        coverItem.RefTableName = ClosurePackage.TableName;
                        coverItem.RefTableID = closurePackage.Id.ToString();
                        coverItem.RelativePath = "//";
                        coverItem.Extension = coverExtention;
                        coverItem.CreateTime = DateTime.Now;

                        coverItem.TypeCode = coverName;
                        coverItem.CreatorID = ClientCookie.UserCode;
                        //coverItem.RequirementId = AttachmentRequirement.FirstOrDefault(e => e.FlowCode == FlowCode.TempClosure_ClosurePackage && e.NameENUS == "Cover").Id;
                        coverItem.CreatorNameENUS = ClientCookie.UserNameENUS;
                        coverItem.CreatorNameZHCN = ClientCookie.UserNameZHCN;

                        using (FileStream stream = new FileStream(coverTempPath, FileMode.Open))
                        {
                            coverItem.Length = (int)stream.Length;
                        }
                        Attachment.SaveSigleFile(coverItem);
                    }
                    break;
            }
        }

        [HttpGet]
        [Route("api/Attachemen/GetAttachements/{refTableName}/{id}")]
        public IHttpActionResult GetAttachements(string refTableName, Guid id)
        {

            var list = Attachment.GetList(refTableName, id.ToString(), "Attachment");//_attHandler.GetList(refTableName, id.ToString(), "Attachment");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            return Ok(list);
        }
        [HttpGet]
        [Route("api/Attachemen/SearchAttachements/{refTableName}/{id}/{typeCode}")]
        public IHttpActionResult SearchAttachements(string refTableName, Guid id, string typeCode)
        {

            var list = Attachment.GetList(refTableName, id.ToString(), typeCode);//_attHandler.GetList(refTableName, id.ToString(), "Attachment");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            return Ok(list);
        }

        [Route("api/attachment/upload/{projectId}/{refTableName}/{requirementId?}")]
        [HttpPost]
        public IHttpActionResult Upload(string projectId, string refTableName, Guid? requirementId = null)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var refTableId = Attachment.GetRefTableId(refTableName, projectId);
                if (refTableId == Guid.Empty.ToString())
                {
                    var newId = Guid.NewGuid();
                    refTableId = newId.ToString();
                    switch (refTableName)
                    {
                        case "MajorLeaseChangePackage":
                            {
                                var entity = new MajorLeaseChangePackage();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ReimageSummary":
                            {
                                var entity = new ReimageSummary();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ReimageConsInfo":
                            {
                                var entity = new ReimageConsInfo();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ReimagePackage":
                            {
                                var entity = new ReimagePackage();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "RebuildLegalReview":
                            {
                                var entity = new RebuildLegalReview();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ClosureLegalReview":
                            {
                                var entity = new ClosureLegalReview();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ClosureExecutiveSummary":
                            {
                                var entity = new ClosureExecutiveSummary();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreatorAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                        case "ClosurePackage":
                            {
                                var entity = new ClosurePackage();
                                entity.ProjectId = projectId;
                                entity.Id = newId;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.IsHistory = false;
                                entity.Add();
                            }
                            break;
                    }
                }
                var userRole = ProjectUsers.Get(ClientCookie.UserCode, projectId);
                var files = HttpContext.Current.Request.Files;
                if (requirementId.HasValue)
                {
                    var attReq = AttachmentRequirement.Get(requirementId.Value);
                    var projectInfo = ProjectInfo.Get(projectId, attReq.FlowCode);
                    var store = StoreBasicInfo.GetStorInfo(projectInfo.USCode);
                    var attachNode = NodeInfo.FirstOrDefault(n => n.FlowCode == attReq.FlowCode && n.Code == attReq.NodeCode);
                    if (attachNode != null)
                    {
                        ProjectInfo.FinishNode(projectId, attReq.FlowCode, attReq.NodeCode);
                    }
                    var file = files[0];
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(file.FileName);
                    var current = System.Web.HttpContext.Current;
                    string internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                    file.SaveAs(absolutePath);

                    Attachment att = Attachment.Get(refTableId, requirementId.Value);

                    if (att == null)
                    {
                        att = new Attachment();
                        att.InternalName = internalName;
                        att.RefTableName = refTableName;
                        att.RefTableID = refTableId;
                        att.RelativePath = "//";
                        att.Name = fileName;
                        att.Extension = fileExtension;
                        att.Length = file.ContentLength;
                        att.CreateTime = DateTime.Now;
                        att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                        att.CreatorNameENUS = ClientCookie.UserNameENUS;
                        att.CreatorID = ClientCookie.UserCode;
                        att.ID = Guid.NewGuid();
                        att.RequirementId = requirementId;
                        att.TypeCode = "";
                        Attachment.Add(att);
                    }
                    else
                    {
                        att.InternalName = internalName;
                        att.RefTableName = refTableName;
                        att.RefTableID = refTableId;
                        att.RelativePath = "//";
                        att.Name = fileName;
                        att.Extension = fileExtension;
                        att.Length = file.ContentLength;
                        att.CreateTime = DateTime.Now;
                        att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                        att.CreatorNameENUS = ClientCookie.UserNameENUS;
                        att.CreatorID = ClientCookie.UserCode;
                        Attachment.Update(att);
                    }
                    //TempClosure上传SignAgreement
                    //if (requirementId.Value.Equals(new Guid("1E9B17AF-357A-4DC9-8A60-17766663FB75")))
                    //{
                    //    var url = "/TempClosure/Main#/ClosureMemo?projectId=" + projectId;
                    //    var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == projectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                    //    var title = string.Concat(projectId, " ", store.NameZHCN, " ", store.NameENUS);
                    //    TaskWork.Finish(t => t.ReceiverAccount == ClientCookie.UserCode && t.RefID == projectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == FlowCode.TempClosure_ClosurePackage);
                    //    //TaskWork.SendTask(projectId, title, projectInfo.USCode, url, actor, FlowCode.TempClosure, FlowCode.TempClosure_ClosureMemo, "Start");
                    //}
                }
                else
                {
                    List<Attachment> attachments = new List<Attachment>();
                    string typeCode = "";
                    if (refTableName == "RebuildLegalReview")
                    {
                        typeCode = "Contract";
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        string fileName = Path.GetFileName(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);
                        var current = System.Web.HttpContext.Current;
                        string internalName = Guid.NewGuid() + fileExtension;
                        string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                        file.SaveAs(absolutePath);

                        Attachment att = new Attachment();
                        att.InternalName = internalName;
                        att.RefTableName = refTableName;
                        att.RefTableID = refTableId;
                        att.RelativePath = "//";
                        att.Name = fileName;
                        att.Extension = fileExtension;
                        att.Length = file.ContentLength;
                        att.CreateTime = DateTime.Now;
                        att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                        att.CreatorNameENUS = ClientCookie.UserNameENUS;
                        att.CreatorID = ClientCookie.UserCode;
                        att.ID = Guid.NewGuid();
                        att.RequirementId = requirementId;
                        att.TypeCode = typeCode;
                        attachments.Add(att);
                    }
                    Attachment.Add(attachments.ToArray());
                }
                tranScope.Complete();
                return Ok();
            }
        }
        public void SaveRenewalLegalApprovalPDF(string refTableName, string projectId, Guid requirementId, string filePath)
        {
            using (TransactionScope sope = new TransactionScope())
            {
                var refTableId = Attachment.GetRefTableId(refTableName, projectId);
                string fileName = Path.GetFileName(filePath);
                string internalName = Guid.NewGuid() + ".pdf";
                var current = HttpContext.Current;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;
                File.Copy(filePath, absolutePath, true);
                Attachment att = Attachment.Get(refTableId, requirementId);
                if (att == null)
                {
                    att = new Attachment();
                    att.InternalName = internalName;
                    att.RefTableName = refTableName;
                    att.RefTableID = refTableId;
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = ".pdf";
                    att.Length = null;
                    att.CreateTime = DateTime.Now;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorID = ClientCookie.UserCode;
                    att.ID = Guid.NewGuid();
                    att.RequirementId = requirementId;
                    att.TypeCode = "";
                    Attachment.Add(att);
                }
                else
                {
                    att.InternalName = internalName;
                    att.RefTableName = refTableName;
                    att.RefTableID = refTableId;
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = ".pdf";
                    att.Length = null;
                    att.CreateTime = DateTime.Now;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorID = ClientCookie.UserCode;
                    Attachment.Update(att);
                }
                sope.Complete();
            }
        }
        [Route("api/attachment/preparePackDownload")]
        [HttpGet]
        public IHttpActionResult PreparePackDownload(string refTableName, string projectId)
        {
            string pdfPath = string.Empty;
            string refTableId = ProjectInfo.GetRefTableId(refTableName, projectId);
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            HtmlTempalteType templateType = HtmlTempalteType.Default;
            string flowName = string.Empty;
            string flowCode = string.Empty;
            switch (refTableName)
            {
                case "MajorLeaseChangePackage":
                    {
                        templateType = HtmlTempalteType.MajorLease;
                        flowName = FlowCode.MajorLease;
                        flowCode = FlowCode.MajorLease_Package;
                        var majorLeaseChangePkg = MajorLeaseChangePackage.GetMajorPackageInfo(projectId);
                        templateFileds = majorLeaseChangePkg.GetPrintTemplateFields();
                    }
                    break;
                case "TempClosurePackage":
                    {
                        templateType = HtmlTempalteType.TempClosure;
                        flowName = FlowCode.TempClosure;
                        flowCode = FlowCode.TempClosure_ClosurePackage;
                        var tempClosurePackage = TempClosurePackage.Get(projectId);
                        if (tempClosurePackage == null)
                        {
                            tempClosurePackage = new TempClosurePackage();
                            tempClosurePackage.ProjectId = projectId;
                        }
                        templateFileds = tempClosurePackage.GetPrintTemplateFields();

                    }
                    break;
                case "ReimagePackage":
                    {
                        var entity = ReimagePackage.GetReimagePackageInfo(projectId);
                        templateType = HtmlTempalteType.Reimage;
                        flowName = FlowCode.Reimage;
                        flowCode = FlowCode.Reimage_Package;
                        if (entity == null || entity.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                        {
                            entity = new ReimagePackage();
                            entity.ProjectId = projectId;
                        }
                        templateFileds = entity.GetPrintTemplateFields();
                    }
                    break;
                case "RenewalLegalApproval":
                    {
                        templateType = HtmlTempalteType.RenewalLegalApproval;
                        flowName = FlowCode.Renewal;
                        flowCode = FlowCode.Renewal_LegalApproval;
                        var legal = RenewalLegalApproval.Get(projectId);
                        if (legal == null)
                        {
                            legal = new RenewalLegalApproval();
                            legal.ProjectId = projectId;
                        }
                        templateFileds = legal.GetPrintTemplateFields();
                    }
                    break;
                case "RenewalPackage":
                    {
                        templateType = HtmlTempalteType.Renewal;
                        flowName = FlowCode.Renewal;
                        flowCode = FlowCode.Renewal_Package;
                        var package = RenewalPackage.Get(projectId);
                        if (package == null)
                        {
                            package = new RenewalPackage();
                            package.ProjectId = projectId;
                        }
                        templateFileds = package.GetPrintTemplateFields();
                    }
                    break;
                case "ClosurePackage":
                    {
                        templateType = HtmlTempalteType.Default;
                        flowName = FlowCode.Closure;
                        flowCode = FlowCode.Closure_ClosurePackage;
                        var closurePackage = ClosurePackage.Get(projectId);
                        if (closurePackage == null)
                        {
                            closurePackage = new ClosurePackage();
                            closurePackage.ProjectId = projectId;
                        }
                        templateFileds = closurePackage.GetPrintTemplateFields();
                    }
                    break;
                default:
                    break;
            }
            List<Attachment> attachments = Attachment.GetAllAttachmentsIncludeRequire(refTableName, projectId, flowCode).Where(att => att.ID != Guid.Empty
                                                                                                                            && (att.RequirementId.HasValue || att.TypeCode == "Cover")).ToList();
            //Submission and Approval Records - 只显示通过意见
            List<SubmissionApprovalRecord> recordList = new List<SubmissionApprovalRecord>();
            //Submission and Approval Records Details — 所有意见
            List<SubmissionApprovalRecord> recordDetailList = new List<SubmissionApprovalRecord>();
            ProjectCommentCondition condition = new ProjectCommentCondition();
            condition.RefTableName = refTableName;
            condition.RefTableId = Guid.Parse(refTableId);
            condition.Status = ProjectCommentStatus.Submit;

            var commentList = condition.RefTableId==Guid.Empty ? new List<VProjectComment>() : VProjectComment.SearchVListForPDF(condition);
            var commentDetailList = condition.RefTableId == Guid.Empty ? new List<VProjectComment>() : VProjectComment.SearchVList(condition);

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

            pdfPath = HtmlConversionUtility.HtmlConvertToPDF(templateType, templateFileds, recordList, recordDetailList);
            var pdfFileName = Path.GetFileName(pdfPath);
            var pdfExtension = Path.GetExtension(pdfPath);
            attachments.Add(new Attachment { InternalName = pdfFileName, Name = flowName + " Print", Extension = pdfExtension });

            if (refTableName == "RenewalLegalApproval")
            {
                SaveRenewalLegalApprovalPDF("RenewalLegalApproval", projectId,
                    new Guid("870BD738-A512-4B27-9FDB-C18058BFA817"), pdfPath);
                return Ok();
            }
            string packFileUrl = ZipHandle.ExeFiles(attachments);
            return Ok(new
            {
                PackUrl = string.Concat("~/", "Temp/", Path.GetFileName(packFileUrl))
            });
        }

        [Route("api/attachment/packDownload")]
        [HttpGet]
        public IHttpActionResult PackDownload(string packUrl, string projectId)
        {
            var projectInfo = ProjectInfo.FirstOrDefault(i => i.ProjectId == projectId && !i.FlowCode.Contains("_"));
            var packFileUrl = HttpContext.Current.Server.MapPath(packUrl);
            var httpcurrent = HttpContext.Current;
            string fileNames = projectInfo.USCode + projectInfo.FlowCode + "_Package_" + DateTime.Now.ToString("yyyyMM");
            httpcurrent.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileNames + ".zip", System.Text.Encoding.GetEncoding("utf-8")));
            httpcurrent.Response.ContentType = "application/octet-stream";
            httpcurrent.Response.WriteFile(packFileUrl);
            httpcurrent.Response.End();
            return Ok();
        }

        private string GenPrintReimageAttachment(ReimageInfo reimageInfo, ReimagePackage entity, PrintFileType fileType)
        {

            var storeInfo = StoreBasicInfo.GetStore(reimageInfo.USCode);
            StoreBasicInfo store = storeInfo.StoreBasicInfo;

            //生成Print文件
            var printDic = new Dictionary<string, string>();
            printDic.Add("WorkflowName", FlowCode.Reimage);
            printDic.Add("ProjectID", entity.ProjectId);
            printDic.Add("USCode", reimageInfo.USCode);
            printDic.Add("Region", store.RegionENUS);
            printDic.Add("Market", store.MarketENUS);
            printDic.Add("City", store.CityENUS);
            printDic.Add("AddressZHCN", store.AddressZHCN);
            printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            if (store.CloseDate.HasValue && store.CloseDate.Value.Year == 1900)
            {
                printDic.Add("ClosureDate", string.Empty);
            }
            else
            {
                printDic.Add("ClosureDate", store.CloseDate.HasValue ? (store.CloseDate.Value.ToString("yyyy-MM-dd")) : "");
            }
            printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            printDic.Add("AssetsRep", storeInfo.StoreDevelop.AssetRepName);
            printDic.Add("StoreAge", (DateTime.Now.Year - store.OpenDate.Year).ToString());
            var currentLeaseENDYear = storeInfo.CurrentYear - int.Parse(storeInfo.StoreContractInfo.EndYear);
            printDic.Add("CurrentLeaseENDYear", currentLeaseENDYear.ToString());
            printDic.Add("StoreNameEN", store.NameENUS);
            printDic.Add("StoreNameCN", store.NameENUS);
            var RmgSummaryentity = ReimageSummary.FirstOrDefault(e => e.ProjectId.Equals(entity.ProjectId) && e.IsHistory == false);
            var financialPreanalysis = FinancialPreanalysis.FirstOrDefault(e => e.RefId.Equals(RmgSummaryentity.Id));
            printDic.Add("SalesBuildingInvestment", financialPreanalysis.SalesBuildingInvestment);
            printDic.Add("NoneSalesBuildingInvst", financialPreanalysis.NoneSalesBuildingInvst);
            printDic.Add("TTMSales", financialPreanalysis.TTMSales.HasValue ? Math.Round(financialPreanalysis.TTMSales.Value, 2).ToString() : string.Empty);
            var reimage = ReimageConsInfo.GetConsInfo(entity.ProjectId, "");
            ReinvestmentCost reinCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == reimage.Id);
            printDic.Add("TotalReinvestmentNorm", reinCost.TotalReinvestmentNorm);
            printDic.Add("TotalSalesInc", financialPreanalysis.TotalSalesInc);
            printDic.Add("ROI", financialPreanalysis.ROI);
            ////printDic.Add("SalesInc", financialPreanalysis.ROI);
            printDic.Add("PaybackYears", financialPreanalysis.PaybackYears);
            printDic.Add("StoreCM", financialPreanalysis.StoreCM);
            printDic.Add("EstimatedWriteOffCost", financialPreanalysis.EstimatedWriteOffCost);
            printDic.Add("MarginInc", financialPreanalysis.MarginInc);
            printDic.Add("ISDWIP", financialPreanalysis.ISDWIP);
            printDic.Add("CurrStorePricingTier", financialPreanalysis.CurrentPriceTier);
            printDic.Add("SPTAR", financialPreanalysis.SPTAR);

            //Submission and Approval Records - 只显示通过意见
            List<SubmissionApprovalRecord> recordList = new List<SubmissionApprovalRecord>();
            //Submission and Approval Records Details — 所有意见
            List<SubmissionApprovalRecord> recordDetailList = new List<SubmissionApprovalRecord>();

            ProjectComment projectCommentBll = new ProjectComment();
            ProjectCommentCondition condition = new ProjectCommentCondition();
            var package = ReimagePackage.Get(entity.ProjectId);
            condition.RefTableName = ReimagePackage.TableName;
            condition.RefTableId = entity.Id;
            condition.SourceCode = FlowCode.Reimage;

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






            string result = string.Empty;
            if (fileType == PrintFileType.Pdf)
            {
                result = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.Reimage, printDic, recordList, recordDetailList);
            }
            else
            {
                result = HtmlConversionUtility.ConvertToImage(HtmlTempalteType.Reimage, printDic, recordList, recordDetailList);
            }
            return result;
        }

        [Route("api/attachment/download")]
        [HttpGet]
        public IHttpActionResult Download(Guid id)
        {
            Attachment att = Attachment.Get(id);
            if (att == null)
                return Ok();
            var current = HttpContext.Current;

            string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + att.InternalName;
            var fileName = att.Name;
            if (fileName.IndexOf(".") < 0)
            {
                fileName += att.Extension;
            }
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DataConverter.ToHexString(fileName));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile(absolutePath);
            current.Response.End();
            return Ok();
        }

        [HttpPost]
        [Route("api/attachment/delete")]
        public IHttpActionResult Delete(Guid id, string projectId = "", Guid? requirementId = null)
        {
            var result = Attachment.Delete(id);
            //TempClosure流程 删除TempClosure Agreement
            if (requirementId.Equals(new Guid("EA0ED677-8CC1-4628-9A76-0462D4409CBE")))
            {
                ProjectInfo.UnFinishNode(projectId, FlowCode.TempClosure_LegalReview, NodeCode.TempClosure_LegalReview_Input);
            }
            //Closure流程
            else if (requirementId.Equals(new Guid("8b086d16-b65b-412f-9e81-013566f732ff")))
            {
                ProjectInfo.UnFinishNode(projectId, FlowCode.Closure_LegalReview, NodeCode.Closure_LegalReview_UploadAgreement);
            }
            else if (requirementId.Equals(new Guid("5ef6f0f9-0177-4f1e-bf84-f081462dc6d7")))
            {
                ProjectInfo.UnFinishNode(projectId, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_ClosingCost);
            }
            return Ok();
        }

        [HttpGet]
        [Route("api/attachment/Download")]
        public IHttpActionResult DownloadPackage(string fileName)
        {
            var current = HttpContext.Current;
            var ext = Path.GetExtension(fileName);
            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName;
            string tempFileName = DateTime.Now.ToString("yyyy_MM_dd");
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(tempFileName + ext, System.Text.Encoding.GetEncoding("utf-8")));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();
            return Ok();
        }

    }
}
