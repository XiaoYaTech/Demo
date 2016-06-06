using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalLLNegotiationController : ApiController
    {
        [HttpGet]
        [Route("api/renewalLLNegotiation/initPage")]
        public IHttpActionResult InitPage(string projectId)
        {
            var info = RenewalInfo.Get(projectId);
            var nego = RenewalLLNegotiation.Get(projectId);
            return Ok(new
            {
                LLNegotiation = nego,
                USCode = info.USCode,
                Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_LLNegotiation),
                Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_LLNegotiation),
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_LLNegotiation)
            });
        }

        [HttpGet]
        [Route("api/renewalLLNegotiation/getRecords")]
        public IHttpActionResult GetRecords(Guid negotiationId, int pageIndex, int pageSize)
        {
            int totalItems = 0;
            var records = RenewalLLNegotiationRecord.GetRecords(negotiationId, pageIndex, pageSize, out totalItems);
            return Ok(new PagedDataSource(totalItems, records.ToArray()));
        }

        [HttpPost]
        [Route("api/renewalLLNegotiation/saveRecord")]
        public IHttpActionResult SaveRecord(RenewalLLNegotiationRecord record)
        {
            var nego = RenewalLLNegotiation.Get(record.RenewalLLNegotiationId);
            if (nego.ProcInstId > 0)
            {
                nego.Save();
            }
            else
            {
                nego.Submit();
            }
            if (RenewalLLNegotiationRecord.Any(e=>e.Id ==record.Id))
            {
                record.LastUpdateTime = DateTime.Now;
                record.LastUpdateUserAccount = ClientCookie.UserCode;
                record.Update();
            }
            else
            {
                record.CreateTime = DateTime.Now;
                record.CreateUserAccount = ClientCookie.UserCode;
                record.Valid = true;
                record.Add();
            }
            nego.GenerateAttachment();
            return Ok();
        }

        [HttpPost]
        [Route("api/renewalLLNegotiation/deleteRecord")]
        public IHttpActionResult DeleteRecord(RenewalLLNegotiationRecord record)
        {
            record.Valid = false;
            record.Update();
            return Ok();
        }

        [HttpGet]
        [Route("api/renewalLLNegotiation/exportRecords")]
        public void ExportRecords(string projectId)
        {
            var response = HttpContext.Current.Response;
            string fileName = string.Format("{0}_LLNegotiationRecords.xlsx", projectId);
            var exportfileName = RenewalLLNegotiationRecord.ExportRecords(projectId);
            response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.GetEncoding("utf-8")));
            response.ContentType = "application/octet-stream";
            response.WriteFile(exportfileName);
            response.End();
        }

        [Route("api/renewalLLNegotiation/getRecordAttachments")]
        public IHttpActionResult GetRecordAttachments(Guid recordId)
        {
            var results = Attachment.Search(att => att.RefTableID == recordId.ToString()).ToList();
            return Ok(results);
        }

        [Route("api/renewalLLNegotiation/UploadRecordAttachment/{recordId}")]
        [HttpPost]
        public IHttpActionResult UploadRecordAttachment(Guid recordId)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var files = HttpContext.Current.Request.Files;
                List<Attachment> attachments = new List<Attachment>();
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
                    att.RefTableName = "RenewalLLNegotiationRecord";
                    att.RefTableID = recordId.ToString();
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = fileExtension;
                    att.Length = file.ContentLength;
                    att.CreateTime = DateTime.Now;
                    att.UpdateTime = DateTime.Now;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorID = ClientCookie.UserCode;
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "";
                    attachments.Add(att);
                    Attachment.Add(attachments.ToArray());
                }
                tranScope.Complete();
            }
            return Ok();
        }

        [Route("api/renewalLLNegotiation/removeRecordAttachment")]
        public IHttpActionResult RemoveRecordAttachment(Attachment att)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                att.Delete();
                var fileName = HttpContext.Current.Server.MapPath("~/") + "UploadFiles/" + att.InternalName;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                tranScope.Complete();
            }
            return Ok();
        }

        [Route("api/renewalLLNegotiation/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalLLNegotiation entity)
        {
            entity.Save();
            return Ok();
        }
        [Route("api/renewalLLNegotiation/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalLLNegotiation entity)
        {
            entity.Submit();
            return Ok();
        }

        [Route("api/renewalLLNegotiation/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(PostWorkflowData<RenewalLLNegotiation> postData)
        {
            postData.Entity.Resubmit(postData.SN);
            return Ok();
        }

        [Route("api/renewalLLNegotiation/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalLLNegotiation> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalLLNegotiation/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalLLNegotiation entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
