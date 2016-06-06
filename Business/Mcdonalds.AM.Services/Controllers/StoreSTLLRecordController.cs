using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Controllers
{
    public class StoreSTLLRecordController : ApiController
    {
        [Route("api/StoreSTLLRecord/{usCode}")]
        public IHttpActionResult GetStoreSTLLRecord(string usCode)
        {
            List<StoreSTLLRecord> lsStoreSTLLRecord = new List<StoreSTLLRecord>();
            StoreSTLLRecord mStoreSTLLRecord = null;
            lsStoreSTLLRecord = StoreSTLLRecord.Search(o => o.StoreCode.Contains(usCode)).ToList<StoreSTLLRecord>();
            if (lsStoreSTLLRecord.Count > 0) mStoreSTLLRecord = lsStoreSTLLRecord[0];
            return Ok(mStoreSTLLRecord);
        }
    }
}
