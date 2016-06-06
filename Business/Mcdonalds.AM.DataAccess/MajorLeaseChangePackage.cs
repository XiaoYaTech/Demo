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
    
    public partial class MajorLeaseChangePackage
    {
        public System.Guid Id { get; set; }
        public string ProjectId { get; set; }
        public Nullable<int> ProcInstID { get; set; }
        public bool IsHistory { get; set; }
        public Nullable<decimal> WriteOff { get; set; }
        public Nullable<decimal> CashCompensation { get; set; }
        public Nullable<decimal> NetWriteOff { get; set; }
        public Nullable<decimal> NewInvestment { get; set; }
        public Nullable<decimal> CashFlowNVPCurrent { get; set; }
        public Nullable<decimal> CashFlowNVPAfterChange { get; set; }
        public Nullable<decimal> OtherCompensation { get; set; }
        public Nullable<decimal> NetGain { get; set; }
        public string ReasonDesc { get; set; }
        public string OtherCompenDesc { get; set; }
        public string DecisionLogicRecomendation { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public string LastUpdateUserAccount { get; set; }
        public string LastUpdateUserNameZHCN { get; set; }
        public string LastUpdateUserNameENUS { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserAccount { get; set; }
        public string CreateUserNameZHCN { get; set; }
        public string CreateUserNameENUS { get; set; }
        public Nullable<System.DateTime> ChangeRentalExpiraryDate { get; set; }
        public Nullable<decimal> ChangeRentalRedLineArea { get; set; }
        public Nullable<System.DateTime> ChangeRedLineExpiraryDate { get; set; }
        public Nullable<decimal> ChangeRedLineRedLineArea { get; set; }
        public Nullable<System.DateTime> ChangeLeaseTermExpiraryDate { get; set; }
        public Nullable<decimal> ChangeLeaseTermRedLineArea { get; set; }
    }
}