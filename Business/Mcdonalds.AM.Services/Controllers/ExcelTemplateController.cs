using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Collections.Generic;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ExcelTemplateController : ApiController
    {
        private Guid? _refId;
        private ClosureWOCheckList _woCheckList = null;
        [HttpPost]
        [Route("api/ExcelTemplate/UploadWriteOffTpl/{flowTable}/{projectId}")]
        public IHttpActionResult UploadWriteOffTpl(string projectId, string flowTable)
        {
            var request = HttpContext.Current.Request;
            var fileCollection = request.Files;
            if (fileCollection.Count > 0)
            {
                _refId = GetRefId(projectId, flowTable);
                if (_refId.HasValue)
                {
                    var file = fileCollection[0];
                    var templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.FAWrite_offTool_Template_X);
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        PluploadHandler.WriteErrorMsg("文件类型不正确!");
                    }
                    if (!ExcelHelper.MatchVersionNumber(templateFileName, file.InputStream))
                    {
                        PluploadHandler.WriteErrorMsg("上传的是非标准模板，请下载标准模板再上传");
                        //throw new Exception("上传的是非标准模板，请下载标准模板再上传");
                    }


                    var name = _refId.Value.ToString() + "WriteOff";
                    var fileName = string.Format(@"{0}UploadFiles/{1}{2}",
                        HttpContext.Current.Server.MapPath("~/"),
                        name, fileExtension);
                    file.SaveAs(fileName);

                    var fileInfo = new FileInfo(fileName);
                    var importDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.WriteOffAmount);
                    importDirector.FillEntityEvent += FillWriteOffAmount;
                    using (var scope = new TransactionScope())
                    {
                        importDirector.ParseAndImport();
                        AddUpdateAttachment(name + fileExtension, flowTable, "WriteOff", fileCollection);
                        scope.Complete();
                    }
                }
            }

            return Ok();
        }

        private void FillWriteOffAmount(BaseAbstractEntity entity)
        {
            var writeOff = entity as WriteOffAmount;
            if (writeOff != null)
            {
                writeOff.Id = Guid.NewGuid();
                if (_refId == null)
                {
                    throw new Exception("ConsInfo Id is null, please check it!");
                }

                writeOff.ConsInfoID = _refId.Value;

            }
        }

        private void FillClosureWOCheckList(BaseAbstractEntity entity)
        {
            var woCheckList = entity as ClosureWOCheckList;
            if (woCheckList != null)
            {
                woCheckList.Id = Guid.NewGuid();
                if (_refId == null)
                {
                    throw new Exception("Id is null, please check it!");
                }
                woCheckList.Id = _refId.Value;
            }
        }

        private void AddUpdateAttachment(string internalName, string refTableName, string strTypeCode, HttpFileCollection fileCollection)
        {
            var file = fileCollection[0];
            string fileName = Path.GetFileName(file.FileName);
            string fileExtension = Path.GetExtension(file.FileName);

            var att = new Attachment();
            att.InternalName = internalName;
            att.RefTableName = refTableName;
            att.RefTableID = _refId.Value.ToString();
            att.RelativePath = "//";
            att.Name = fileName;
            att.Extension = fileExtension;
            att.Length = file.ContentLength;
            att.CreateTime = DateTime.Now;
            att.ID = Guid.NewGuid();
            att.TypeCode = strTypeCode;
            att.CreatorID = ClientCookie.UserCode;
            att.CreatorNameENUS = ClientCookie.UserNameENUS;
            att.CreatorNameZHCN = ClientCookie.UserNameZHCN;

            var ar = AttachmentRequirement.FirstOrDefault(
                e => e.RefTableName == refTableName && e.TypeCode == strTypeCode);

            if (ar != null)
                att.RequirementId = ar.Id;

            att.UpdateTime = DateTime.Now;
            att.IsDelete = 0;
            Attachment.SaveSigleFile(att);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ExcelTemplate/UploadReinvestmentCostTpl/{flowTable}/{projectId}")]
        public IHttpActionResult UploadReinvestmentCostTpl(string projectId, string flowTable)
        {
            var request = HttpContext.Current.Request;
            var fileCollection = request.Files;
            if (fileCollection.Count > 0)
            {
                _refId = GetRefId(projectId, flowTable);
                if (_refId.HasValue)
                {
                    var file = fileCollection[0];
                    var templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.FATool_Update_Template);
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        PluploadHandler.WriteErrorMsg("文件类型不正确!");
                    }
                    if (!ExcelHelper.MatchVersionNumber(templateFileName, file.InputStream))
                    {
                        PluploadHandler.WriteErrorMsg("上传的是非标准模板，请下载标准模板再上传");
                    }

                    var name = _refId.Value.ToString() + "FATool";
                    var fileName = string.Format(@"{0}UploadFiles/{1}{2}", HttpContext.Current.Server.MapPath("~/"),
                        name, fileExtension);
                    file.SaveAs(fileName);

                    var fileInfo = new FileInfo(fileName);
                    var importDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.ReinvestmentCost);
                    importDirector.FillEntityEvent += FillReinvestmentCostEntity;

                    using (var scope = new TransactionScope())
                    {
                        importDirector.ParseAndImport();
                        AddUpdateAttachment(name + fileExtension, flowTable, "ReinCost", fileCollection);
                        scope.Complete();
                    }
                }

            }

            return Ok();
        }
        private void FillReinvestmentCostEntity(BaseAbstractEntity entity)
        {
            var reinvestmentCost = entity as ReinvestmentCost;
            if (reinvestmentCost != null)
            {
                var existReinvestmentCost =
                ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID.ToString().Equals(_refId.ToString()));
                if (existReinvestmentCost != null)
                {
                    reinvestmentCost.Id = existReinvestmentCost.Id;
                    reinvestmentCost.NormType = existReinvestmentCost.NormType;
                }
                else
                {
                    reinvestmentCost.Id = Guid.NewGuid();
                }

                if (_refId == null)
                {
                    throw new Exception("ConsInfo Id is null, please check it!");
                }

                reinvestmentCost.ConsInfoID = _refId.Value;

            }
        }
        public Guid? GetRefId(string projectId, string flowTable)
        {
            Guid? refId = null;
            switch (flowTable)
            {
                case "MajorLeaseConsInfo":
                    var majorConsInfo = MajorLeaseConsInfo.FirstOrDefault(e => e.ProjectId == projectId && !e.IsHistory);
                    if (majorConsInfo != null && majorConsInfo.Id != Guid.Empty)
                    {
                        refId = majorConsInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        majorConsInfo = new MajorLeaseConsInfo
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            CreateTime = DateTime.Now,
                            IsHistory = false
                        };
                        MajorLeaseConsInfo.Add(majorConsInfo);
                    }
                    break;
                case "MajorLeaseConsInvtChecking":
                    var checkInfo = MajorLeaseConsInvtChecking.Search(e => e.ProjectId == projectId && !e.IsHistory).FirstOrDefault();
                    if (checkInfo != null && checkInfo.Id != Guid.Empty)
                    {
                        refId = checkInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        checkInfo = new MajorLeaseConsInvtChecking
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            CreateTime = DateTime.Now,
                            IsHistory = false
                        };

                        MajorLeaseConsInvtChecking.Add(checkInfo);
                    }
                    break;
                case "ReimageConsInfo":
                    var reimageConsInfo = ReimageConsInfo.FirstOrDefault(e => e.ProjectId == projectId && !e.IsHistory);
                    if (reimageConsInfo != null && reimageConsInfo.Id != Guid.Empty)
                    {
                        refId = reimageConsInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        reimageConsInfo = new ReimageConsInfo
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            IsHistory = false,
                            CreateTime = DateTime.Now,
                            CreateUserAccount = ClientCookie.UserCode
                        };
                        ReimageConsInfo.Add(reimageConsInfo);
                    }
                    break;
                case "ReimageConsInvtChecking":
                    var reimageCheckInfo = ReimageConsInvtChecking.Search(e => e.ProjectId == projectId && !e.IsHistory).FirstOrDefault();
                    if (reimageCheckInfo != null && reimageCheckInfo.Id != Guid.Empty)
                    {
                        refId = reimageCheckInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        reimageCheckInfo = new ReimageConsInvtChecking
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            CreateTime = DateTime.Now,
                            IsHistory = false
                        };

                        ReimageConsInvtChecking.Add(reimageCheckInfo);
                    }
                    break;
                case "RebuildConsInfo":
                    var rbdConsInfo = RebuildConsInfo.FirstOrDefault(e => e.ProjectId == projectId && !e.IsHistory);
                    if (rbdConsInfo != null && rbdConsInfo.Id != Guid.Empty)
                    {
                        refId = rbdConsInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        rbdConsInfo = new RebuildConsInfo
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            IsHistory = false,
                            CreateTime = DateTime.Now,
                            LastUpdateTime = DateTime.Now,
                            CreateUserAccount = ClientCookie.UserCode,
                            CreateUserNameENUS = ClientCookie.UserNameENUS,
                            CreateUserNameZHCN = ClientCookie.UserNameZHCN
                        };
                        RebuildConsInfo.Add(rbdConsInfo);
                    }
                    break;
                case "RebuildConsInvtChecking":
                    var rbdCheckInfo = RebuildConsInvtChecking.Search(e => e.ProjectId == projectId && !e.IsHistory).FirstOrDefault();
                    if (rbdCheckInfo != null && rbdCheckInfo.Id != Guid.Empty)
                    {
                        refId = rbdCheckInfo.Id;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        rbdCheckInfo = new RebuildConsInvtChecking
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            IsHistory = false,
                            CreateTime = DateTime.Now,
                            LastUpdateTime = DateTime.Now,
                            CreateUserAccount = ClientCookie.UserCode,
                            CreateUserNameENUS = ClientCookie.UserNameENUS,
                            CreateUserNameZHCN = ClientCookie.UserNameZHCN
                        };

                        RebuildConsInvtChecking.Add(rbdCheckInfo);
                    }
                    ProjectInfo.FinishNode(projectId, FlowCode.Rebuild_ConsInvtChecking,
                        NodeCode.Rebuild_ConsInvtChecking_Downlod);
                    ProjectInfo.FinishNode(projectId, FlowCode.Rebuild_ConsInvtChecking,
                        NodeCode.Rebuild_ConsInvtChecking_Input);
                    ProjectInfo.FinishNode(projectId, FlowCode.Rebuild_ConsInvtChecking,
                        NodeCode.Rebuild_ConsInvtChecking_Upload);
                    break;
                case "ClosureWOCheckList":
                    var closureWOCheckList = ClosureWOCheckList.Get(projectId);
                    if (closureWOCheckList != null && closureWOCheckList.Id != Guid.Empty)
                    {
                        refId = closureWOCheckList.Id;
                        _woCheckList = closureWOCheckList;
                    }
                    else
                    {
                        refId = Guid.NewGuid();
                        closureWOCheckList = new ClosureWOCheckList
                        {
                            Id = refId.Value,
                            ProjectId = projectId,
                            CreateTime = DateTime.Now,
                            CreateUserAccount = ClientCookie.UserCode,
                            CreateUserName = ClientCookie.UserNameENUS,
                            IsHistory = false
                        };
                        ClosureWOCheckList.Add(closureWOCheckList);
                    }
                    break;
            }

            return refId;
        }
        [Route("api/ExcelTemplate/DownLoadTemplate/{templateType}/{projectID}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult DownLoadTemplate(string templateType, string projectId, string consInfoID = "", string NormType = "", string flowCode = "")
        {
            var current = HttpContext.Current;
            ExcelDataInputType outputType;

            if (!Enum.TryParse(templateType, out outputType))
            {
                throw new Exception("Cannot find the specific template type!");
            }

            if (string.IsNullOrEmpty(projectId))
            {
                throw new Exception("Porject Id is Empty !");
            }
            var siteFilePath = default(string);

            switch (outputType)
            {
                case ExcelDataInputType.WriteOffAmount:
                    siteFilePath = SiteFilePath.FAWrite_offTool_Template_X;
                    break;
                case ExcelDataInputType.ReinvestmentCost:
                    siteFilePath = SiteFilePath.FATool_Update_Template;
                    break;
                case ExcelDataInputType.ClosureWOCheckList:
                    siteFilePath = SiteFilePath.Closure_FAWrite_offTool_Template;
                    break;
            }
            var path = string.Format(@"{0}\{1}", SiteFilePath.Template_DIRECTORY, siteFilePath);

            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + Guid.NewGuid() + ".xlsx";

            bool isNotUploadExcel = false;
            if (!string.IsNullOrEmpty(consInfoID))
            {
                string refTableName = "MajorLeaseConsInfo";
                if (projectId.ToLower().IndexOf("rebuild") != -1)
                {
                    refTableName = "RebuildConsInfo";
                }
                if (projectId.ToLower().IndexOf("reimage") != -1)
                {
                    refTableName = "ReimageConsInfo";
                }

                var strFileName = GetAttatchFile(refTableName, templateType, consInfoID);//download consifno upload file
                if (string.IsNullOrEmpty(strFileName))
                {
                    isNotUploadExcel = true;
                    tempFilePath = current.Server.MapPath("~/") + "UploadFiles\\" + Guid.NewGuid() + ".xlsx";
                }
                else
                    tempFilePath = current.Server.MapPath("~/") + "UploadFiles\\" + strFileName;

            }

            if (isNotUploadExcel)
                consInfoID = null;// if not upload file in consinfo flow,then we get empty template

            var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == projectId);
            if (projectInfo == null)
            {
                throw new Exception("Cannot find the project info!");
            }
            if (string.IsNullOrEmpty(consInfoID))
            {
                File.Copy(path, tempFilePath);

                var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(tempFilePath), outputType);
                var store = StoreBasicInfo.Search(e => e.StoreCode == projectInfo.USCode).FirstOrDefault();
                if (store == null)
                {
                    throw new Exception("Cannot find Store info!");
                }

                var inputInfo = new ExcelInputDTO
                {
                    Region = store.RegionENUS,
                    Market = store.MarketENUS,
                    City = store.CityENUS,
                    StoreNameCN = store.NameZHCN,
                    StoreNameEN = store.NameENUS,
                    StoreName = store.NameZHCN,
                    USCode = store.StoreCode,
                    StoreType = store.StoreType,
                    StoreTypeName = store.StoreTypeName,
                    OpenDate = store.OpenDate,
                    ClosureDate = store.CloseDate.HasValue ? store.CloseDate.Value : default(DateTime)
                };

                switch (outputType)
                {
                    case ExcelDataInputType.ReinvestmentCost:
                        Guid consInfoId = Guid.Empty;
                        if (projectId.ToLower().IndexOf("rebuild") != -1)
                        {
                            var rbdInfo = new RebuildConsInfo();
                            consInfoId = rbdInfo.GetConsInfo(projectId).Id;
                        }
                        else if (projectId.ToLower().IndexOf("majorlease") != -1)
                        {
                            var mjInfo = new MajorLeaseConsInfo();
                            consInfoId = mjInfo.GetConsInfo(projectId).Id;
                        }
                        else if (projectId.ToLower().IndexOf("reimage") != -1)
                        {
                            consInfoId = ReimageConsInfo.GetConsInfo(projectId).Id;
                        }
                        var reinvestmentBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == consInfoId);
                        if (reinvestmentBasicInfo != null)
                        {
                            inputInfo.NewDesignType = reinvestmentBasicInfo.NewDesignType;
                            inputInfo.NormType = MappingNormType(NormType);
                            inputInfo.GBDate = reinvestmentBasicInfo.GBDate;
                            inputInfo.ConsCompletionDate = reinvestmentBasicInfo.ConsCompletionDate;
                            inputInfo.EstimatedSeatNO = reinvestmentBasicInfo.EstimatedSeatNo;
                            inputInfo.NewDTSiteArea = reinvestmentBasicInfo.NewDTSiteArea;
                            inputInfo.NewOperationArea = reinvestmentBasicInfo.NewOperationSize;
                            inputInfo.NewDiningArea = reinvestmentBasicInfo.NewDiningArea;
                            inputInfo.WallPanelArea = reinvestmentBasicInfo.WallPanelArea;
                            inputInfo.WallGraphicArea = reinvestmentBasicInfo.WallGraphicArea;
                            inputInfo.FacadeACMArea = reinvestmentBasicInfo.FacadeACMArea;
                            inputInfo.NewRemoteKiosk = reinvestmentBasicInfo.NewRemoteKiosk;
                            inputInfo.NewAttachedKiosk = reinvestmentBasicInfo.NewAttachedKiosk;
                            inputInfo.NewMcCafe = reinvestmentBasicInfo.NewMcCafe;
                            inputInfo.NewMDS = reinvestmentBasicInfo.NewMDS;
                        }

                        break;
                    case ExcelDataInputType.WriteOffAmount:
                        string pmName = string.Empty;
                        if (projectId.ToLower().IndexOf("rebuild") != -1)
                        {
                            var rbdInfo = new RebuildInfo();
                            pmName = rbdInfo.GetRebuildInfo(projectId).PMNameENUS;
                        }
                        else if (projectId.ToLower().IndexOf("majorlease") != -1)
                        {
                            var mjInfo = new MajorLeaseInfo();
                            pmName = mjInfo.GetMajorLeaseInfo(projectId).PMNameENUS;
                        }
                        else if (projectId.ToLower().IndexOf("reimage") != -1)
                        {
                            pmName = ReimageInfo.GetReimageInfo(projectId).PMNameENUS;
                        }
                        inputInfo.PMNameENUS = pmName;
                        break;
                    case ExcelDataInputType.ClosureWOCheckList:
                        var closure = ClosureInfo.GetByProjectId(projectId);
                        if (closure != null)
                        {
                            inputInfo.ActualCloseDate = closure.ActualCloseDate.HasValue ? closure.ActualCloseDate.Value.ToString("yyyy-MM-dd") : "";
                            inputInfo.PMNameENUS = closure.PMNameENUS;
                        }
                        break;
                }

                excelOutputDirector.Input(inputInfo);
            }

            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + SiteFilePath.GetTemplateFileName(projectInfo.USCode, flowCode, siteFilePath));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();

            return Ok();


        }

        private string MappingNormType(string NormType)
        {
            if (string.IsNullOrEmpty(NormType))
                return "";
            switch (NormType)
            {
                case "1":
                    return "Reimage-IS (>= 8 year)";
                    break;
                case "2":
                    return "Reimage-IS (< 8 year)";
                    break;
                case "3":
                    return "Reimage - China Design - IS";
                    break;
                case "4":
                    return "Reimage-DT (>= 8 year)";
                    break;
                case "5":
                    return "Reimage-DT (< 8 year)";
                    break;
                case "6":
                    return "Reimage - China Design - DT";
                    break;
                case "7":
                    return "Rebuild (no norm)";
                    break;
                case "8":
                    return "Renewal (no morm)";
                    break;
                case "9":
                    return "Major Lease Change (no morm)";
                    break;
                default:
                    return "";
                    break;
            }
        }

        [Route("api/ExcelTemplate/GenerateExcel/{templateType}/{flowCode}/{projectId}")]
        [HttpGet]
        public IHttpActionResult GenerateExcel(string templateType, string flowCode, string projectId)
        {

            var current = HttpContext.Current;

            ExcelDataInputType outputType;

            if (!Enum.TryParse(templateType, out outputType))
            {
                throw new Exception("Cannot find the specific template type!");
            }

            if (string.IsNullOrEmpty(projectId))
            {
                throw new Exception("Porject Id is Empty !");
            }
            var fileName = Guid.NewGuid() + ".xlsx";
            var siteFilePath = default(string);
            switch (outputType)
            {
                case ExcelDataInputType.WriteOffAmount:
                    siteFilePath = SiteFilePath.FAWrite_offTool_Template_X;
                    break;
                case ExcelDataInputType.ReinvestmentCost:
                    siteFilePath = SiteFilePath.FATool_Update_Template;
                    break;
                case ExcelDataInputType.ReimageSummary:
                    siteFilePath = SiteFilePath.Reimage_Summary_Template;
                    //AddUpdateAttachment(name + fileExtension, "ReimageSummary", "Reimage_Summary", fileCollection);
                    break;
            }
            var path = string.Format(@"{0}\{1}", SiteFilePath.Template_DIRECTORY, siteFilePath);


            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName;

            File.Copy(path, tempFilePath);
            var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(tempFilePath), outputType);

            var inputInfo = new ExcelInputDTO
            {
                ProjectId = projectId,
                FlowCode = flowCode

            };

            excelOutputDirector.Input(inputInfo);

            //switch (outputType)
            //{
            //    case ExcelDataInputType.ReimageSummary:
            //        var refTableId = ProjectInfo.GetRefTableId("ReimageSummary",projectId);
            //        Attachment attachment = Attachment.GetAttachment("ReimageSummary", refTableId, "ReimageSummary");
            //        if (attachment == null)
            //        {
            //            attachment = new Attachment();
            //            attachment.ID = Guid.NewGuid();
            //            attachment.ContentType = "application/ms-excel";
            //            attachment.CreateTime = DateTime.Now;
            //            attachment.CreatorID = ClientCookie.UserCode;
            //            attachment.CreatorNameENUS = ClientCookie.UserNameENUS;
            //            attachment.CreatorNameZHCN = ClientCookie.UserNameZHCN;
            //            attachment.Extension = ".xlsx";
            //            attachment.InternalName = "ReimageSummary.xlsx";

            //        }
            //        break;
            //}

            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(siteFilePath, System.Text.Encoding.GetEncoding("utf-8")));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();

            return Ok();


        }

        [Route("api/ExcelTemplate/GenerateReimageSummary")]
        [HttpGet]
        public IHttpActionResult GenerateReimageSummary(string projectId)
        {

            string filePath;
            var summary = ReimageSummary.Get(projectId);
            var att = summary.GenerateExcel(out filePath);

            return Ok(att);


        }

        public string GetAttatchFile(string flowTable, string templateType, string consInfoID)
        {
            if (string.IsNullOrEmpty(consInfoID))
                throw new Exception("consInfoID IS NULL");

            ExcelDataInputType outputType;
            if (!Enum.TryParse(templateType, out outputType))
            {
                throw new Exception("Cannot find the specific template type!");
            }

            string strTypeCode = "";
            switch (outputType)
            {
                case ExcelDataInputType.WriteOffAmount:
                    strTypeCode = "WriteOff";
                    break;
                case ExcelDataInputType.ReinvestmentCost:
                    strTypeCode = "ReinCost";
                    break;
            }

            var att = Attachment.GetAttachment(flowTable, consInfoID, strTypeCode);
            if (att != null)
            {
                if (att.InternalName.ToString().IndexOf(".") != -1)
                    return att.InternalName;
                else
                    return att.InternalName + att.Extension;
            }
            else
                return null;
        }

        [Route("api/ClosureWOCheckList/UploadTemplate/{projectID}")]
        [HttpPost]
        public IHttpActionResult UploadClosureWOCheckListTemplate(string projectid)
        {
            var request = HttpContext.Current.Request;
            var fileCollection = request.Files;
            if (fileCollection.Count > 0)
            {
                _refId = GetRefId(projectid, ClosureWOCheckList.TableName);
                if (_refId.HasValue)
                {
                    var file = fileCollection[0];
                    var fileExtension = Path.GetExtension(file.FileName);
                    var name = _refId.Value.ToString() + "WriteOff";
                    var fileName = string.Format(@"{0}UploadFiles/{1}{2}",
                        HttpContext.Current.Server.MapPath("~/"),
                        name, fileExtension);
                    file.SaveAs(fileName);

                    if (!IsMeetVersion(ExcelDataInputType.ClosureWOCheckList.ToString(), fileName))
                    {
                        PluploadHandler.WriteErrorMsg("上传的是非标准模板，请下载标准模板再上传");
                    }

                    var fileInfo = new FileInfo(fileName);
                    var importDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.ClosureWOCheckList);

                    decimal RE_NBV = 0;
                    decimal.TryParse(importDirector.GetCellValue(9, "E"), out RE_NBV);
                    decimal LHI_NBV = 0;
                    decimal.TryParse(importDirector.GetCellValue(10, "E"), out LHI_NBV);
                    decimal ESSD_NBV = 0;
                    decimal.TryParse(importDirector.GetCellValue(11, "E"), out ESSD_NBV);
                    decimal Equipment_Transfer = 0;
                    decimal.TryParse(importDirector.GetCellValue(16, "E"), out Equipment_Transfer);

                    importDirector.FillEntityEvent += FillClosureWOCheckList;
                    using (TransactionScope scope = new TransactionScope())
                    {
                        importDirector.ParseAndImport();

                        AddUpdateAttachment(name + fileExtension, ClosureWOCheckList.TableName, "Template", fileCollection);

                        var currentNode = NodeInfo.GetCurrentNode(projectid, FlowCode.Closure_WOCheckList);
                        var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_WOCheckList_ResultUpload);
                        if (newNode.Sequence > currentNode.Sequence)
                        {
                            ProjectInfo.FinishNode(projectid, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_DownLoadTemplate);
                            ProjectInfo.FinishNode(projectid, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_WriteOffData);
                            ProjectInfo.FinishNode(projectid, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_ResultUpload);
                        }

                        scope.Complete();
                    }

                    if (_woCheckList != null)
                    {
                        var closureToolHandler = new ClosureTool();

                        var oldRE_NBV = _woCheckList.RE_NBV ?? 0;
                        var oldLHI_NBV = _woCheckList.LHI_NBV ?? 0;
                        var oldESSD_NBV = _woCheckList.ESSD_NBV ?? 0;
                        var oldEquipmentTransfer = _woCheckList.EquipmentTransfer ?? 0;

                        if ((RE_NBV != oldRE_NBV) || (LHI_NBV != oldLHI_NBV) || (ESSD_NBV != oldESSD_NBV) || (Equipment_Transfer != oldEquipmentTransfer))
                        {
                            var __woCheckList = ClosureWOCheckList.FirstOrDefault(e => e.Id.ToString().Equals(_woCheckList.Id.ToString()));
                            if (__woCheckList != null)
                            {
                                __woCheckList.RefreshClosureTool = true;
                                __woCheckList.Update();
                            }
                            //var closureToolController = new ClosureToolController();
                            //var toolEntity = ClosureTool.Get(projectid);
                            //if (toolEntity != null)
                            //{
                            //    //判断是否满足生成closureTools的条件
                            //    if (toolEntity.EnableReGenClosureTool())
                            //    {

                            //        closureToolController.GenClosureTool(toolEntity.Id, toolEntity.UserAccount, toolEntity.UserNameZHCN, toolEntity.UserNameENUS);
                            //        closureToolController.CallClosureTool(toolEntity.Id);

                            //        //通知Finance Specialist和Asset Actor
                            //        var closureInfo = ClosureInfo.FirstOrDefault(i => i.ProjectId == projectid);
                            //        List<string> receiverList = new List<string>();
                            //        receiverList.Add(closureInfo.AssetActorAccount);
                            //        receiverList.Add(closureInfo.FinanceAccount);
                            //        var notificationMsg = new NotificationMsg()
                            //        {
                            //            FlowCode = FlowCode.Closure_WOCheckList,
                            //            ProjectId = projectid,
                            //            SenderCode = ClientCookie.UserCode,
                            //            Title = "由于WO Tool数据发生变化，Closure Tool文件已自动更新",
                            //            RefId = _woCheckList.Id,
                            //            UsCode = _woCheckList.USCode,
                            //            IsSendEmail = false,
                            //            ReceiverCodeList = receiverList
                            //        };
                            //        Notification.Send(notificationMsg);
                            //    }
                            //}
                        }
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// 判断上传的文件与模板的版本号是否相同
        /// </summary>
        /// <param name="templateType">模板类型</param>
        /// <param name="actualFilePath">上传的文件服务器保存路径</param>
        /// <returns></returns>
        private bool IsMeetVersion(string templateType, string actualFilePath)
        {
            ExcelDataInputType outputType;
            if (!Enum.TryParse(templateType, out outputType))
            {
                throw new Exception("Cannot find the specific template type!");
            }
            var siteFilePath = default(string);
            switch (outputType)
            {
                case ExcelDataInputType.WriteOffAmount:
                    siteFilePath = SiteFilePath.FAWrite_offTool_Template_X;
                    break;
                case ExcelDataInputType.ReinvestmentCost:
                    siteFilePath = SiteFilePath.FATool_Update_Template;
                    break;
                case ExcelDataInputType.ClosureWOCheckList:
                    siteFilePath = SiteFilePath.Closure_FAWrite_offTool_Template;
                    break;
            }
            var templatePath = string.Format(@"{0}\{1}", SiteFilePath.Template_DIRECTORY, siteFilePath);
            if (ExcelHelper.GetExcelVersionNumber(templatePath) == ExcelHelper.GetExcelVersionNumber(actualFilePath))
                return true;
            return false;
        }
    }
}