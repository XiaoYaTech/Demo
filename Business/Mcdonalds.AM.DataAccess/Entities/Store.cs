using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreBasicInfo : BaseEntity<StoreBasicInfo>
    {
        public class StoreEntity
        {
            public string Code { get; set; }

            public string NameZHCN { get; set; }

            public string CityName { get; set; }

            public string MarketName { get; set; }

            public DateTime? OpenDate { get; set; }

            public string TotalSeatsNo { get; set; }

            public string LeasePurchaseTerm { get; set; }

            public DateTime? EndDate { get; set; }

            public DateTime? RentCommencementDate { get; set; }

            public string RentStructure { get; set; }

            public string RentType { get; set; }

            public string TotalLeasedArea { get; set; }

            /// <summary>
            /// 业主联系人
            /// </summary>
            public string LL0Contact { get; set; }
        }

        public static StoreBasicInfo GetStorInfo(string usCode)
        {
            return FirstOrDefault(e => e.StoreCode.Equals(usCode));
        }

        public StoreEntity GetStoreEntity(string usCode)
        {
            string sql = string.Format(@"SELECT TOP 1 tb_stll.LL0Contact, store.Code,NameZHCN,CityName,MarketName,OpenDate,st.TotalSeatsNo,
			sc.LeasePurchaseTerm,sc.EndDate,RentCommencementDate,sc.RentStructure,sc.RentType,sc.TotalLeasedArea			
			 FROM dbo.Store
			left JOIN
			(SELECT TOP 1* FROM 
			 dbo.StoreContractInfo WHERE StoreCode = '{0}'
			 ORDER BY CreatedTime DESC
			 ) AS SC ON sc.StoreCode = store.Code
			left JOIN StoreSTLocation st ON st.StoreCode = store.Code
	left JOIN (SELECT TOP 1 * FROM StoreSTLLRecord WHERE StoreCode = '{0}'   ORDER BY CreatedTime DESC) 
	AS tb_stll
			ON tb_stll.StoreCode = store.Code
			WHERE Code = '{0}'
			ORDER BY SC.CreatedTime DESC
		", usCode);
            StoreEntity entity = null;
            var list = SqlQuery<StoreEntity>(sql, null).ToList();
            if (list.Count > 0)
            {
                entity = list[0];
            }
            return entity;
        }



        public static StoreInfo GetStore(string usCode)
        {
            string _USCode = usCode;

            List<StoreBasicInfo> lsStoreBasicInfo = new List<StoreBasicInfo>();
            StoreBasicInfo mStoreBasicInfo = new StoreBasicInfo();
            lsStoreBasicInfo = StoreBasicInfo.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().ToList<StoreBasicInfo>();
            if (lsStoreBasicInfo.Count > 0) mStoreBasicInfo = lsStoreBasicInfo[0];

            List<StoreDevelop> lsStoreDevelop = new List<StoreDevelop>();
            StoreDevelop mStoreDevelop = new StoreDevelop();
            lsStoreDevelop = StoreDevelop.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().ToList<StoreDevelop>();
            if (lsStoreDevelop.Count > 0) mStoreDevelop = lsStoreDevelop[0];

            List<StoreOps> lsStoreOp = new List<StoreOps>();
            StoreOps mStoreOp = new StoreOps();
            lsStoreOp = StoreOps.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().ToList<StoreOps>();
            if (lsStoreOp.Count > 0) mStoreOp = lsStoreOp[0];

            List<StoreContractInfo> lsStoreContractInfo = new List<StoreContractInfo>();
            StoreContractInfo mStoreContractInfo = new StoreContractInfo();
            lsStoreContractInfo = StoreContractInfo.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking()
                .OrderByDescending(o => o.CreatedTime).ToList<StoreContractInfo>();
            if (lsStoreContractInfo.Count > 0) mStoreContractInfo = lsStoreContractInfo[0];

            List<StoreSTLocation> lsStoreSTLocation = new List<StoreSTLocation>();
            StoreSTLocation mStoreSTLocation = new StoreSTLocation();
            lsStoreSTLocation = StoreSTLocation.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().ToList<StoreSTLocation>();
            if (lsStoreSTLocation.Count > 0) mStoreSTLocation = lsStoreSTLocation[0];

            List<StoreSTLLRecord> lsStoreSTLLRecord = new List<StoreSTLLRecord>();
            StoreSTLLRecord mStoreSTLLRecord = new StoreSTLLRecord();
            lsStoreSTLLRecord = StoreSTLLRecord.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().ToList<StoreSTLLRecord>();
            if (lsStoreSTLLRecord.Count > 0) mStoreSTLLRecord = lsStoreSTLLRecord[0];
            int? beId = StoreBEInfo.Search(o => o.StoreCode.Contains(_USCode)).AsNoTracking().Select(be => be.BEID).FirstOrDefault();
            StoreBEContractInfo storeBeContractInfo = StoreBEContractInfo.FirstOrDefault(c => c.BEID == beId);
            //StoreContractRevision = new StoreContractRevision(),
            //   StoreContractInfoAttached = new StoreContractInfoAttached()
            var lsStoreBEInfo = StoreBEInfo.Search(o => o.StoreCode == _USCode).AsNoTracking().ToList<StoreBEInfo>();
            List<StoreBEInfo> remoteBeList = new List<StoreBEInfo>();
            List<StoreBEInfo> attachedBeList = new List<StoreBEInfo>();
            List<StoreBEInfo> mdsList = new List<StoreBEInfo>();
            List<StoreBEInfo> mcCafeList = new List<StoreBEInfo>();
            List<StoreBEInfo> hour24List = new List<StoreBEInfo>();
            if (lsStoreBEInfo.Count > 0)
            {
                foreach (var beInfo in lsStoreBEInfo)
                {
                    switch (beInfo.BETypeName)
                    {
                        case "Remote Kiosk":
                            remoteBeList.Add(beInfo);
                            break;
                        case "Attached Kiosk":
                            attachedBeList.Add(beInfo);
                            break;
                        case "MDS":
                            mdsList.Add(beInfo);
                            break;
                        case "McCafe":
                            mcCafeList.Add(beInfo);
                            break;
                        case "24 Hour":
                            hour24List.Add(beInfo);
                            break;

                    }
                }
            }




            var resultStoreAllInfo = new StoreInfo
            {
                StoreBasicInfo = mStoreBasicInfo,
                StoreDevelop = mStoreDevelop,
                StoreOp = mStoreOp,
                StoreContractInfo = mStoreContractInfo,
                StoreSTLocation = mStoreSTLocation,
                StoreSTLLRecord = mStoreSTLLRecord,
                StoreBeContractInfo = storeBeContractInfo,
                StoreBEInfoList = lsStoreBEInfo,
                CurrentYear = DateTime.Now.Year,
                RemoteBeCount = remoteBeList.Count,
                AttachedBeCount = attachedBeList.Count,
                MDSBeCount = mdsList.Count,
                MCCafeCount = mcCafeList.Count,
                Hour24Count = hour24List.Count

            };
            return resultStoreAllInfo;
        }
    }

    public class StoreInfo
    {
        public StoreBasicInfo StoreBasicInfo;
        public StoreDevelop StoreDevelop;
        public StoreOps StoreOp;
        public StoreContractInfo StoreContractInfo;
        public StoreSTLocation StoreSTLocation;
        public StoreSTLLRecord StoreSTLLRecord;
        public StoreBEContractInfo StoreBeContractInfo;
        public List<StoreBEInfo> StoreBEInfoList;
        public int CurrentYear;
        public int RemoteBeCount;
        public int AttachedBeCount;
        public int MDSBeCount;
        public int MCCafeCount;
        public int Hour24Count;
    }
}
