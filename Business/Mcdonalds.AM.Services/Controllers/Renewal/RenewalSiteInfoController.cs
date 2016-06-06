using AutoMapper;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalSiteInfoController : ApiController
    {
        [Route("api/renewalSiteInfo/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId)
        {
            var info = RenewalInfo.Get(projectId);
            var siteInfoProject = ProjectInfo.Get(projectId, FlowCode.Renewal_SiteInfo);

            var estimatedVsActualConstruction = EstimatedVsActualConstruction.FirstOrDefault(e => e.RefId == siteInfoProject.Id);
            if (estimatedVsActualConstruction == null)
            {
                var consInfo = RenewalConsInfo.Get(projectId);
                var reinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id);
                var storeInfo = StoreSTLocation.FirstOrDefault(e => e.StoreCode == info.USCode);
                estimatedVsActualConstruction = new EstimatedVsActualConstruction
                {
                    RefId = siteInfoProject.Id,
                    GBDate = reinBasicInfo != null ? reinBasicInfo.GBDate : null,
                    CompletionDate = reinBasicInfo != null ? reinBasicInfo.ConsCompletionDate : null,
                    ARDC = reinBasicInfo != null ? reinBasicInfo.NewDesignType : null,
                    OriginalOperationSize = storeInfo.TotalArea,
                    OriginalSeatNumber = storeInfo.TotalSeatsNo,
                    ClosureDays = reinBasicInfo != null ? (reinBasicInfo.ConsCompletionDate.Value - reinBasicInfo.GBDate.Value).TotalDays.ToString() : ""
                };
            }
            var result = new
            {
                Info = info,
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_SiteInfo),
                EstimatedVsActualConstruction = estimatedVsActualConstruction
            };
            return Ok(result);
        }

        [Route("api/renewalSiteInfo/save")]
        [HttpPost]
        public IHttpActionResult Save(StoreSTLocation store)
        {
            using (var scope = new TransactionScope())
            {
                //store.Save();

                Mapper.CreateMap<StoreSTLocation, StoreSTLocationHistory>();
                var history = Mapper.Map<StoreSTLocationHistory>(store);
                var his = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == store.ProjectIdentifier);
                if (his == null)
                {
                    history.Id = Guid.NewGuid();
                    history.RefId = store.ProjectIdentifier;
                }
                else
                {
                    history.Id = his.Id;
                    history.RefId = store.ProjectIdentifier;
                }
                history.Save();

                if (store.EstimatedVsActualConstruction != null)
                {
                    store.EstimatedVsActualConstruction.Save();
                }

                scope.Complete();
            }
            return Ok();
        }

        [Route("api/renewalSiteInfo/submit")]
        [HttpPost]
        public IHttpActionResult Submit(StoreSTLocation store)
        {
            using (var scope = new TransactionScope())
            {
                //store.Save();

                Mapper.CreateMap<StoreSTLocation, StoreSTLocationHistory>();
                var history = Mapper.Map<StoreSTLocationHistory>(store);
                var his = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == store.ProjectIdentifier);
                var project = ProjectInfo.Get(store.ProjectIdentifier);
                if (his == null)
                {
                    history.Id = Guid.NewGuid();
                    history.RefId = store.ProjectIdentifier;
                }
                else
                {
                    history.Id = his.Id;
                    history.RefId = store.ProjectIdentifier;
                }
                history.Save();

                if (store.EstimatedVsActualConstruction != null)
                {
                    store.EstimatedVsActualConstruction.Save();
                }
                TaskWork.Finish(t => t.ReceiverAccount == ClientCookie.UserCode && t.TypeCode == FlowCode.Renewal_SiteInfo && t.RefID == project.ProjectId);
                ProjectInfo.FinishNode(project.ProjectId, FlowCode.Renewal_SiteInfo, NodeCode.Finish,ProjectStatus.Finished);
                ProjectInfo.CompleteMainIfEnable(project.ProjectId);
                if (ProjectInfo.IsFlowFinished(project.ProjectId, FlowCode.Renewal_ContractInfo))
                {
                    ProjectProgress.SetProgress(project.ProjectId, "100%");
                }
                scope.Complete();
            }
            return Ok();
        }
    }
}
