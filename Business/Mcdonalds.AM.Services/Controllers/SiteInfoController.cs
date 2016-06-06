using AutoMapper;
using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Entities;

namespace Mcdonalds.AM.Services.Controllers
{
    public class SiteInfoController : ApiController
    {
        [HttpGet]
        [Route("api/SiteInfo/GetSiteInfo")]
        public IHttpActionResult GetSiteInfo(string usCode, Guid identifier, string projectId="", string flowCode="")
        {
            var storeSTLocation = StoreSTLocation.GetStoreSTLocation(usCode);
            var history = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == identifier);
            if (history != null)
            {
                Mapper.CreateMap<StoreSTLocationHistory, StoreSTLocation>();
                storeSTLocation = Mapper.Map<StoreSTLocation>(history);
            }
            if (!string.IsNullOrEmpty(projectId)
                && !string.IsNullOrEmpty(flowCode))
            {
                var wfEntity = BaseWFEntity.GetWorkflowEntity(projectId, BaseWFEntity.GetMainProjectFlowCode(flowCode));

                var designStyle = wfEntity.GetDesignStypleForSiteInfo();
                if (!string.IsNullOrEmpty(designStyle))
                {
                    storeSTLocation.DesignStyle = designStyle;
                }
            }
            storeSTLocation.ProjectIdentifier = identifier;

            return Ok(storeSTLocation);
        }

        [HttpGet]
        [Route("api/SiteInfo/GetStoreSiteInfo")]
        public IHttpActionResult GetStoreSiteInfo(string usCode)
        {
            var storeSTLocation = StoreSTLocation.GetStoreSTLocation(usCode);
            return Ok(storeSTLocation);
        }

        [HttpGet]
        [Route("api/SiteInfo/GetDropdownDatas")]
        public IHttpActionResult GetDropdownDatas()
        {

            var listDirectionalEffects = new List<Dictionary>();
            var listDTTypeNames = new List<Dictionary>();
            var listFloors = new List<Dictionary>();
            var listExteriorDesigns = new List<Dictionary>();
            var listInnerDesign = new List<Dictionary>();
            var listAppearDesign = new List<Dictionary>();
            listExteriorDesigns = Dictionary.GetDictionaryListByParentCode("DesignType");
            listFloors = Dictionary.GetDictionaryList("Floors");
            listDTTypeNames = Dictionary.GetDictionaryList("DTType");
            //listInnerDesign = Dictionary.GetDictionaryListByParentCode("InnerDesign");
            listInnerDesign = Dictionary.GetDictionaryListByParentCode("DesignType");
            listDirectionalEffects = Dictionary.GetDictionaryList("DirectionalEffect").OrderBy(e => e.Sequence).ToList();
            listAppearDesign = Dictionary.GetDictionaryListByParentCode("AppearDesign");
            return Ok(new
            {
                DirectionalEffects = listDirectionalEffects,
                DTTypeNames = listDTTypeNames,
                Floors = listFloors,
                KitchenFloors = listFloors,
                FrontCounterFloors = listFloors,
                ExteriorDesigns = listExteriorDesigns,
                InnerDesign = listInnerDesign,
                AppearDesign = listAppearDesign
            });
        }

        [HttpPost]
        [Route("api/SiteInfo/Save")]
        public IHttpActionResult Save(StoreSTLocation store)
        {
            store.Submit();

            return Ok("SUCCESS");
        }

        [HttpPost]
        [Route("api/SiteInfo/StoreSave")]
        public IHttpActionResult StoreSave(StoreSTLocation store)
        {
            store.Save();

            return Ok("SUCCESS");
        }

        [HttpPost]
        [Route("api/SiteInfo/Submit")]
        public IHttpActionResult Submit(StoreSTLocation store)
        {
            store.Submit(true);

            return Ok("SUCCESS");
        }
    }
}