using System.Collections.Generic;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class ApproveUsers
    {
        public SimpleEmployee MarketMgr { get; set; }
        public SimpleEmployee RegionalMgr { get; set; }
        public SimpleEmployee DD { get; set; }
        public SimpleEmployee MDD { get; set; }
        public SimpleEmployee GM { get; set; }
        public SimpleEmployee FC { get; set; }
        public SimpleEmployee DO { get; set; }
        public SimpleEmployee RDD { get; set; }
        public SimpleEmployee VPGM { get; set; }
        public SimpleEmployee CDO { get; set; }
        public SimpleEmployee CFO { get; set; }
        public SimpleEmployee MCCLAssetMgr { get; set; }
        public SimpleEmployee MCCLAssetDtr { get; set; }

        public SimpleEmployee ManagingDirector { get; set; }
        public SimpleEmployee FinanceController { get; set; }
        public SimpleEmployee FM { get; set; }
        public List<SimpleEmployee> NecessaryNoticeUsers { get; set; }
        public List<SimpleEmployee> NoticeUsers { get; set; }

        public SimpleEmployee ConstructionManager { get; set; }

        public SimpleEmployee MCCLConsManager { get; set; }

        public SimpleEmployee Legal { get; set; }
        public SimpleEmployee GeneralCounsel { get; set; }


    }
}
