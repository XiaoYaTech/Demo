using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.Services.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ContractController : ApiController
    {
        [Route("api/contract/get/project")]
        [HttpGet]
        public IHttpActionResult Search(string projectId)
        {
            return Ok(ProjectContractInfo.GetContractWithHistory(projectId));
        }

        [Route("api/contract/get/store")]
        [HttpGet]
        public IHttpActionResult StoreSearch(string usCode)
        {
            return Ok(StoreContractInfo.GetContractByUsCode(usCode));
        }

        [Route("api/contract/save")]
        [HttpPost]
        public IHttpActionResult Save(PostContractModel entity)
        {
            entity.Contract.Save(entity.Revisions);
            return Ok();
        }

        [Route("api/contract/storesave")]
        [HttpPost]
        public IHttpActionResult StoreSave(PostStoreContractModel entity)
        {
            entity.Contract.Save(entity.Revisions);
            return Ok();
        }


        [Route("api/contract/submit")]
        [HttpPost]
        public IHttpActionResult Submit(PostContractModel entity)
        {
            entity.Contract.Submit(entity.Revisions, entity.FlowCode);
            return Ok(entity);
        }


        [Route("api/contract/getAttachmentCount")]
        [HttpGet]
        public IHttpActionResult GetAttachmentCount(string projectId)
        {
            var contrainfo = StoreContractInfo.SearchByProject(projectId).FirstOrDefault();
            int? leaseRecapID = null;
            if (contrainfo != null)
            {
                leaseRecapID = contrainfo.LeaseRecapID ?? 0;
            }
            else
                leaseRecapID = 0;
            return Ok(StoreContractInfoAttached.Count(c => c.LeaseRecapID == leaseRecapID.ToString()));
        }

        [Route("api/contract/attachments")]
        [HttpGet]
        public IHttpActionResult GetAttachments(string projectId)
        {
            var leaseRecapID = StoreContractInfo.SearchByProject(projectId).FirstOrDefault().LeaseRecapID ?? 0;
            return Ok(StoreContractInfoAttached.Search(c => c.LeaseRecapID == leaseRecapID.ToString()).ToList());
        }

        [Route("api/contract/downloadAttachment")]
        [HttpGet]
        public IHttpActionResult DownloadAttachment(Guid id)
        {
            StoreContractInfoAttached att = StoreContractInfoAttached.Get(id);
            var current = HttpContext.Current;

            string attachUrl = ConfigurationManager.AppSettings["ContractAttachServer"] + att.FilePath;
            WebClient client = new WebClient();
            try
            {
                byte[] bytes = client.DownloadData(attachUrl);
                current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(att.DocName));
                current.Response.ContentType = "application/octet-stream";
                current.Response.BinaryWrite(bytes);
                current.Response.End();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/contract/querySaveable")]
        [HttpGet]
        public IHttpActionResult QuerySaveable(string projectId)
        {
            string flowCode = "";
            if (projectId.ToLower().IndexOf("rebuild") != -1)
                flowCode = FlowCode.Rebuild_ContractInfo;
            return Ok(new
            {
                IsShowSave = ProjectInfo.IsFlowSavable(projectId, flowCode)
            });
        }
    }
}
