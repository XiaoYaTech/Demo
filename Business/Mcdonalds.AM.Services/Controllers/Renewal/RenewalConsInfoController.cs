using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalConsInfoController : ApiController
    {
        [Route("api/renewalConsInfo/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId, string id = null)
        {
            return Ok(RenewalConsInfo.InitPage(projectId, id));
        }

        [Route("api/renewalConsInfo/save")]
        [HttpPost]
        public IHttpActionResult Save(ConsInfoDTO<RenewalInfo, RenewalConsInfo> postData)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                postData.Entity.Save(postData.ProjectComment, () =>
                {
                    var tool = RenewalTool.Get(postData.Entity.ProjectId);
                    postData.Info.NeedProjectCostEst = postData.Entity.HasReinvenstment;
                    postData.Info.Update();
                    if (postData.ReinBasicInfo != null)
                    {
                        postData.ReinBasicInfo.ConsInfoID = postData.Entity.Id;
                        postData.ReinBasicInfo.Save();
                    }
                    if (postData.ReinCost != null)
                    {
                        postData.ReinCost.ConsInfoID = postData.Entity.Id;
                        postData.ReinCost.Save();
                    }
                    if (postData.WriteOff != null)
                    {
                        postData.WriteOff.ConsInfoID = postData.Entity.Id;
                        postData.WriteOff.Save();
                    }
                });
                tranScope.Complete();
            }
            return Ok();

        }
        [Route("api/renewalConsInfo/submit")]
        [HttpPost]
        public IHttpActionResult Submit(ConsInfoDTO<RenewalInfo, RenewalConsInfo> postData)
        {
            postData.Entity.Submit(postData.ProjectComment, () =>
            {
                postData.Info.NeedProjectCostEst = postData.Entity.HasReinvenstment;
                postData.Info.Update();
                if (postData.Info.NeedProjectCostEst)
                {
                    if (postData.ReinBasicInfo != null)
                    {
                        postData.ReinBasicInfo.ConsInfoID = postData.Entity.Id;
                        postData.ReinBasicInfo.Save();
                    }
                    if (postData.ReinCost != null)
                    {
                        postData.ReinCost.ConsInfoID = postData.Entity.Id;
                        postData.ReinCost.Save();
                    }
                    if (postData.WriteOff != null)
                    {
                        postData.WriteOff.ConsInfoID = postData.Entity.Id;
                        postData.WriteOff.Save();
                    }
                }
            });
            return Ok();
        }

        [Route("api/renewalConsInfo/approve")]
        [HttpPost]
        public IHttpActionResult Approve(ConsInfoDTO<RenewalInfo, RenewalConsInfo> postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalConsInfo/return")]
        [HttpPost]
        public IHttpActionResult Return(ConsInfoDTO<RenewalInfo, RenewalConsInfo> postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalConsInfo/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(ConsInfoDTO<RenewalInfo, RenewalConsInfo> postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN, () =>
            {
                postData.Info.NeedProjectCostEst = postData.Entity.HasReinvenstment;
                postData.Info.Update();
                if (postData.Info.NeedProjectCostEst)
                {
                    if (postData.ReinBasicInfo != null)
                    {
                        postData.ReinBasicInfo.ConsInfoID = postData.Entity.Id;
                        postData.ReinBasicInfo.Save();
                    }
                    if (postData.ReinCost != null)
                    {
                        postData.ReinCost.ConsInfoID = postData.Entity.Id;
                        postData.ReinCost.Save();
                    }
                    if (postData.WriteOff != null)
                    {
                        postData.WriteOff.ConsInfoID = postData.Entity.Id;
                        postData.WriteOff.Save();
                    }
                }
            });
            return Ok();
        }

        [Route("api/renewalConsInfo/recall")]
        [HttpPost]
        public IHttpActionResult Recall(ConsInfoDTO<RenewalInfo,RenewalConsInfo> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalConsInfo/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalConsInfo entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
