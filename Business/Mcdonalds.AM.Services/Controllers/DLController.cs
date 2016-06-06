using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class DLController : ApiController
    {
        [Route("api/DL/List")]
        [HttpGet]
        public IHttpActionResult List(string usCode, int index, int size)
        {
            int count = 0;
            var data = ProjectInfo.Search(p => p.USCode == usCode && !p.FlowCode.Contains("_"), (p => p.CreateTime.Value), index, size, out count, true);
            var totalItems = count;
            var result = new PagedDataSource(totalItems, data.ToArray());
            return Ok(result);
        }

        [Route("api/DL/Detail")]
        [HttpGet]
        public IHttpActionResult Detail(string flowCode, Guid Id)
        {
            var result = new Object();

            switch (flowCode)
            {
                case FlowCode.Closure:
                    result = V_AM_DL_Closure.Get(Id);
                    break;
                case FlowCode.Reimage:
                    result = V_AM_DL_Reimage.Get(Id);
                    break;
                case FlowCode.MajorLease:
                    result = V_AM_DL_MajorLeaseChange.Get(Id);
                    break;
                case FlowCode.Rebuild:
                    result = V_AM_DL_Rebuild.Get(Id);
                    break;
                case FlowCode.Renewal:
                    result = V_AM_DL_Renewal.Get(Id);
                    break;
            }
            return Ok(result);
        }

        #region 详情页提交
        [Route("api/DL/Submit/Closure")]
        [HttpPost]
        public IHttpActionResult SubmitClosure(PostDL.ClosureDL closureDL)
        {
            var entity = closureDL.Entity;
            entity.Save(closureDL.PushOrNot);
            return Ok();
        }

        [Route("api/DL/Submit/Reimage")]
        [HttpPost]
        public IHttpActionResult SubmitReimage(PostDL.ReimageDL reimageDL)
        {
            var entity = reimageDL.Entity;
            entity.Save(reimageDL.PushOrNot);
            return Ok();
        }

        [Route("api/DL/Submit/MajorLease")]
        [HttpPost]
        public IHttpActionResult SubmitMajorLease(PostDL.MajorLeaseDL mlcDL)
        {
            var entity = mlcDL.Entity;
            entity.Save(mlcDL.PushOrNot);
            return Ok();
        }

        [Route("api/DL/Submit/Rebuild")]
        [HttpPost]
        public IHttpActionResult SubmitRebuild(PostDL.RebuildDL rebuildDL)
        {
            var entity = rebuildDL.Entity;
            entity.Save(rebuildDL.PushOrNot);
            return Ok();
        }

        [Route("api/DL/Submit/Renewal")]
        [HttpPost]
        public IHttpActionResult SubmitRenewal(PostDL.RenewalDL renewalDL)
        {
            var entity = renewalDL.Entity;
            entity.Save(renewalDL.PushOrNot);
            return Ok();
        }
        #endregion

        #region 附件相关
        [Route("api/DL/Attachments/Get")]
        [HttpGet]
        public IHttpActionResult GetAttachments(Guid Id, string flowCode)
        {
            var refTableId = string.Empty;
            var refTableName = string.Empty;
            var projectInfo = ProjectInfo.Get(Id);

            if (projectInfo != null)
            {
                switch (flowCode)
                {
                    case FlowCode.Closure:
                        refTableName = "ClosureInfo";
                        var closureInfo = ClosureInfo.GetByProjectId(projectInfo.ProjectId);
                        if (closureInfo != null)
                            refTableId = closureInfo.Id.ToString();
                        break;
                    case FlowCode.Rebuild:
                        refTableName = "RebuildInfo";
                        var rebuildInfo = RebuildInfo.FirstOrDefault(i => i.ProjectId == projectInfo.ProjectId);
                        if (rebuildInfo != null)
                            refTableId = rebuildInfo.Id.ToString();
                        break;
                    case FlowCode.MajorLease:
                        refTableName = "MajorLeaseInfo";
                        var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(i => i.ProjectId == projectInfo.ProjectId);
                        if (majorLeaseInfo != null)
                            refTableId = majorLeaseInfo.Id.ToString();
                        break;
                    case FlowCode.Renewal:
                        refTableName = "RenewalInfo";
                        var renewalInfo = RenewalInfo.Get(projectInfo.ProjectId);
                        if (renewalInfo != null)
                            refTableId = renewalInfo.Id.ToString();
                        break;
                    case FlowCode.Reimage:
                        refTableName = "ReimageInfo";
                        var reimageInfo = ReimageInfo.GetReimageInfo(projectInfo.ProjectId);
                        if (reimageInfo != null)
                            refTableId = reimageInfo.Id.ToString();
                        break;
                }
                if (!string.IsNullOrEmpty(refTableId))
                {
                    var list = Attachment.Search(i => i.RefTableName == refTableName && i.RefTableID == refTableId).ToList();
                    foreach (var item in list)
                    {
                        item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
                    }
                    return Ok(list);
                }
            }
            return Ok();
        }

        [Route("api/DL/Attachments/Upload/{flowCode}/{usCode}/{typeCode}/{Id?}")]
        [HttpPost]
        public IHttpActionResult UploadAttachment(string flowCode, string typeCode, string usCode, Guid Id)
        {
            var refTableId = string.Empty;
            var refTableName = string.Empty;
            var projectInfo = ProjectInfo.Get(Id);
            var projectId = string.Empty;

            if (projectInfo == null)
            {
                projectId = ProjectInfo.CreateDLProject(Id, flowCode, usCode, NodeCode.Start, ClientCookie.UserCode, false);
            }
            else
            {
                projectId = projectInfo.ProjectId;
            }
            using (TransactionScope tranScope = new TransactionScope())
            {
                switch (flowCode)
                {
                    case FlowCode.Closure:
                        refTableName = "ClosureInfo";
                        var closureInfo = ClosureInfo.FirstOrDefault(i => i.ProjectId == projectId);
                        if (closureInfo != null)
                            refTableId = closureInfo.Id.ToString();
                        else
                        {
                            closureInfo = new ClosureInfo();
                            closureInfo.Id = Guid.NewGuid();
                            closureInfo.ProjectId = projectId;
                            closureInfo.USCode = usCode;
                            closureInfo.CreateDate = DateTime.Now;
                            closureInfo.CreateUserAccount = ClientCookie.UserCode;
                            closureInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                            closureInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                            closureInfo.Add();
                            refTableId = closureInfo.Id.ToString();
                        }
                        break;
                    case FlowCode.Rebuild:
                        refTableName = "RebuildInfo";
                        var rebuildInfo = RebuildInfo.FirstOrDefault(i => i.ProjectId == projectId);
                        if (rebuildInfo != null)
                            refTableId = rebuildInfo.Id.ToString();
                        else
                        {
                            rebuildInfo = new RebuildInfo();
                            rebuildInfo.Id = Guid.NewGuid();
                            rebuildInfo.ProjectId = projectId;
                            rebuildInfo.USCode = usCode;
                            rebuildInfo.CreateTime = DateTime.Now;
                            rebuildInfo.CreateUserAccount = ClientCookie.UserCode;
                            rebuildInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                            rebuildInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                            rebuildInfo.Add();
                            refTableId = rebuildInfo.Id.ToString();
                        }
                        break;
                    case FlowCode.MajorLease:
                        refTableName = "MajorLeaseInfo";
                        var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(i => i.ProjectId == projectId);
                        if (majorLeaseInfo != null)
                            refTableId = majorLeaseInfo.Id.ToString();
                        else
                        {
                            majorLeaseInfo = new MajorLeaseInfo();
                            majorLeaseInfo.Id = Guid.NewGuid();
                            majorLeaseInfo.ProjectId = projectId;
                            majorLeaseInfo.USCode = usCode;
                            majorLeaseInfo.CreateTime = DateTime.Now;
                            majorLeaseInfo.CreateUserAccount = ClientCookie.UserCode;
                            majorLeaseInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                            majorLeaseInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                            majorLeaseInfo.Add();
                            refTableId = majorLeaseInfo.Id.ToString();
                        }
                        break;
                    case FlowCode.Renewal:
                        refTableName = "RenewalInfo";
                        var renewalInfo = RenewalInfo.Get(projectId);
                        if (renewalInfo != null)
                            refTableId = renewalInfo.Id.ToString();
                        else
                        {
                            renewalInfo = new RenewalInfo();
                            renewalInfo.Id = Guid.NewGuid();
                            renewalInfo.ProjectId = projectId;
                            renewalInfo.USCode = usCode;
                            renewalInfo.CreateTime = DateTime.Now;
                            renewalInfo.CreateUserAccount = ClientCookie.UserCode;
                            renewalInfo.Add();
                            refTableId = renewalInfo.Id.ToString();
                        }
                        break;
                    case FlowCode.Reimage:
                        refTableName = "ReimageInfo";
                        var reimageInfo = ReimageInfo.GetReimageInfo(projectId);
                        if (reimageInfo != null)
                            refTableId = reimageInfo.Id.ToString();
                        else
                        {
                            reimageInfo = new ReimageInfo();
                            reimageInfo.Id = Guid.NewGuid();
                            reimageInfo.ProjectId = projectId;
                            reimageInfo.USCode = usCode;
                            reimageInfo.CreateDate = DateTime.Now;
                            reimageInfo.CreateUserAccount = ClientCookie.UserCode;
                            reimageInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                            reimageInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                            reimageInfo.Add();
                            refTableId = reimageInfo.Id.ToString();
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(refTableId))
                {
                    var files = HttpContext.Current.Request.Files;
                    var file = files[0];
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(file.FileName);
                    var current = System.Web.HttpContext.Current;
                    string internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;
                    file.SaveAs(absolutePath);

                    Attachment att = Attachment.FirstOrDefault(i => i.RefTableID == refTableId && i.RefTableName == refTableName && i.TypeCode == typeCode);
                    if (att != null)
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
                    else
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
                        att.TypeCode = typeCode;
                        Attachment.Add(att);
                    }
                }
                tranScope.Complete();
                return Ok();
            }
        }

        [Route("api/DL/Attachments/Delete/{AttachmentId}")]
        [HttpPost]
        public IHttpActionResult DeleteAttachment(Guid AttachmentId)
        {
            var result = Attachment.Delete(AttachmentId);
            return Ok();
        }
        #endregion

        [Route("api/DL/Authority")]
        [HttpGet]
        public IHttpActionResult Authority(string usCode)
        {
            bool enableView = false;
            bool enableEdit = false;

            var adminUsers = Employee.GetEmployeesByRole(RoleCode.System_Admin);
            if (adminUsers.Any(i => i.Code == ClientCookie.UserCode))
            {
                enableView = true;
                enableEdit = true;
            }
            else
            {
                var storeInfo = StoreBasicInfo.GetStorInfo(usCode);
                using (var amdb = new McdAMEntities())
                {
                    var org = amdb.Organization.FirstOrDefault(i => i.Code == storeInfo.RegionCode && i.Type == 102);
                    if (org.DL.HasValue && org.DL.Value)
                    {
                        enableView = true;
                        var repUsers = Employee.GetStoreAssetRepByStoreCode(usCode);
                        if (repUsers.Any(i => i.Code == ClientCookie.UserCode))
                            enableEdit = true;
                    }
                }
            }
            return Ok(new
            {
                EnableView = enableView,
                EnableEdit = enableEdit
            });
        }
    }
}