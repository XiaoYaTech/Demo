using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using NPOI.SS.Formula.Functions;
using System.Web;
using System.IO;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Constants;

namespace Mcdonalds.AM.Services.Controllers
{
    public class StoreController : ApiController
    {
        [Route("api/StoreSTLocation/{storeCode}")]
        [HttpGet]
        public IHttpActionResult GetStoreSTLocationInfo(string storeCode)
        {
            var entity = StoreSTLocation.GetStoreSTLocation(storeCode);
            if (entity != null)
            {
                var storeBasicInfo = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == entity.StoreCode);
                if (storeBasicInfo != null)
                {
                    entity.StoreTypeName = storeBasicInfo.StoreTypeName;
                }
            }
            return Ok(entity);
        }

        [Route("api/store/basic")]
        [HttpGet]
        public IHttpActionResult GetStoreBasic(string usCode)
        {
            string _USCode = usCode;

            var lsStoreBasicInfo = new List<StoreBasicInfo>();
            var mStoreBasicInfo = new StoreBasicInfo();
            lsStoreBasicInfo = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreBasicInfo>();
            if (lsStoreBasicInfo.Count > 0) mStoreBasicInfo = lsStoreBasicInfo[0];

            mStoreBasicInfo.ProjectContractRevision = StoreContractInfo.MappingProjectContractRevision(usCode);

            return Ok(mStoreBasicInfo);
        }


        [Route("api/Store/{usCode}")]
        public IHttpActionResult GetStore(string usCode)
        {
            string _USCode = usCode;

            var lsStoreBasicInfo = new List<StoreBasicInfo>();
            var mStoreBasicInfo = new StoreBasicInfo();
            lsStoreBasicInfo = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).ToList<StoreBasicInfo>();
            if (lsStoreBasicInfo.Count > 0) mStoreBasicInfo = lsStoreBasicInfo[0];

            return Ok(mStoreBasicInfo);
        }



        [Route("api/Store/Details/{usCode}")]
        public IHttpActionResult GetStoreDetails(string usCode)
        {
            var resultStoreAllInfo = StoreBasicInfo.GetStore(usCode);

            return Ok(resultStoreAllInfo);
        }

        [Route("api/Store/DetailInfo")]
        public IHttpActionResult GetStoreDetail(string usCode)
        {
            var storeInfo = StoreBasicInfo.GetStore(usCode);
            return Ok(new { storeInfo });
        }

        [Route("api/Store/DetailsByUserCode/{usCode}/{eid}")]
        public IHttpActionResult GetStoreDetails(string usCode, string eid)
        {
            //var resultStoreAllInfo = storeBll.GetStoreDetailsByEID(eid, usCode);
            var resultStoreAllInfo = StoreBasicInfo.GetStore(usCode);
            return Ok(resultStoreAllInfo);
        }


        [Route("api/store/{pageSize?}")]
        public List<StoreBasicInfo> GetStores(int pageSize = 10, string code = "", string name = "")
        {
            var list = StoreBasicInfo.Search(o => (string.IsNullOrEmpty(code) || o.StoreCode.StartsWith(code))
                && (string.IsNullOrEmpty(name) || o.NameENUS.Contains(name) || o.NameZHCN.Contains(name))).OrderBy(o => o.NameENUS).Take(pageSize).ToList();
            return list;
        }

        [Route("api/store/fuzzy/{pageSize?}")]
        public List<StoreBasicInfo> GetStores(int pageSize = 10, string name = "")
        {
            var list = StoreBasicInfo.Search(o =>
                string.IsNullOrEmpty(name) || o.StoreCode.StartsWith(name)
                || o.NameENUS.Contains(name) || o.NameZHCN.Contains(name)).OrderBy(o => o.NameENUS).Take(pageSize).ToList();
            return list;
        }

        /// <summary>
        /// 根据用户输入的值查询Store信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("api/store/SearchStores/{count}")]
        [HttpGet]
        public IHttpActionResult SearchStores(int count, string code = "", string name = "")
        {
            var list = StoreBasicInfo.Search(e => e.StoreCode.Contains(code) || e.NameENUS.Contains(name) || e.NameZHCN.Contains(name)).OrderBy(e => e.StoreCode).Skip(0).Take(count);
            return Ok(list);
        }

        [Route("api/store/{pageIndex}/{pageSize}/{userCode}")]
        public PagedDataSource GetStores(int pageIndex, int pageSize, string userCode, string code = "", string name = "")
        {
            var bll = new StoreBasicInfo();
            int totalRecords = 0;
            var list = bll.GetStoresByEmployeeCode(pageIndex, pageSize, userCode, name, code, out totalRecords).ToArray();
            return new PagedDataSource(totalRecords, list);
        }

        [Route("api/store/user/{pageSize}/{userCode}")]
        [HttpGet]
        public List<StoreBasicInfo> GetStores(int pageSize, string userCode, string code = "", string name = "")
        {
            var bll = new StoreBasicInfo();
            return bll.GetStoresByAssetRepOrMgrEID(pageSize, userCode, code, name);
        }

        [Route("api/store/searchMyStore")]
        [HttpGet]
        public List<StoreBasicInfo> SearchMyStore(int pageSize, string userCode, string code = "", string name = "", string flowCode = null)
        {
            var bll = new StoreBasicInfo();
            return bll.GetStoresByAssetRepOrMgrEID(pageSize, userCode, code, name, flowCode);
        }

        [Route("api/store/checkStore")]
        [HttpGet]
        public IHttpActionResult CheckStore(string code)
        {
            var bll = new StoreBasicInfo();
            string storeName = "";
            var result = bll.CheckStore(code, out storeName);
            return Ok(new
            {
                StoreName = storeName,
                StoreValid = result
            });
        }

        [Route("api/store/checkStoreFlow")]
        [HttpGet]
        public IHttpActionResult CheckStoreFlow(string code, string flowCode)
        {
            var store = StoreBasicInfo.GetStorInfo(code);
            return Ok(new
            {
                Store = store,
                StoreValid = ProjectInfo.CheckIfExistStore(code, flowCode),
                //StoreValid = true,
            });
        }

        [Route("api/StoreSTLocation/update")]
        [HttpPost]
        public IHttpActionResult UpdateStoreLocation(StoreSTLocation stl)
        {
            stl.Update();
            return Ok();
        }

        [Route("api/StoreSTLicense/upload/{usCode}/{Id}")]
        [HttpPost]
        public IHttpActionResult UploadStoreSTLicense(string usCode, Guid Id)
        {
            var files = HttpContext.Current.Request.Files;
            var file = files[0];
            string fileName = Path.GetFileName(file.FileName);
            string fileExtension = Path.GetExtension(file.FileName);
            string internalName = Guid.NewGuid() + fileExtension;
            string absolutePath = SiteFilePath.PMTAttachmentPath + "Common\\" + internalName;
            file.SaveAs(absolutePath);

            var license = StoreSTLicense.FirstOrDefault(store => store.Id == Id);
            if (license != null)
            {
                if (license.Status == null)
                {
                    license.CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                license.Status = 1;
                license.DocName = fileName;
                license.ModifyDate = DateTime.Now.ToString("yyyy-MM-dd");
                license.Owner = ClientCookie.UserNameENUS;
                license.FilePath = SiteFilePath.PMTAttachmenttURL + "Common/" + internalName;

                license.Update();
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/StoreSTLicense/delete/{Id}")]
        public IHttpActionResult Delete(Guid Id)
        {
            var result = string.Empty;
            var license = StoreSTLicense.FirstOrDefault(store => store.Id == Id);
            if (license != null)
            {
                license.Status = 2;
                license.DocName = null;
                license.CreateDate = null;
                license.ModifyDate = null;
                license.EndDate = null;
                license.Owner = null;
                license.FilePath = null;

                license.Update();

                result = license.StoreCode;
            }
            return Ok(result);
        }
    }
}
