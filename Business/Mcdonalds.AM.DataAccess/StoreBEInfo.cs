//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mcdonalds.AM.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreBEInfo
    {
        public System.Guid Id { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string StoreCode { get; set; }
        public Nullable<int> BEID { get; set; }
        public string BECode { get; set; }
        public string BETypeName { get; set; }
        public string LaunchDate { get; set; }
        public string CloseDate { get; set; }
        public string MonthlyNetTotalSales { get; set; }
        public string MonthlyTotalGC { get; set; }
        public Nullable<int> IsSingleContract { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> SyncTime { get; set; }
        public string SyncType { get; set; }
    }
}
