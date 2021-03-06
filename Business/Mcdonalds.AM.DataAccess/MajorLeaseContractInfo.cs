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
    
    public partial class MajorLeaseContractInfo
    {
        public System.Guid Id { get; set; }
        public string ProjectId { get; set; }
        public string FirstParty { get; set; }
        public string USCode { get; set; }
        public string MCDOwersShop { get; set; }
        public string OwnerContact { get; set; }
        public string ContractModeName { get; set; }
        public string RentType { get; set; }
        public double Area { get; set; }
        public Nullable<int> PurchaseAuthorityYear { get; set; }
        public int ContractType { get; set; }
        public System.DateTime EstimateStartTime { get; set; }
        public System.DateTime EstimateEndTime { get; set; }
        public System.DateTime ContractStartYear { get; set; }
        public System.DateTime ContractEndYear { get; set; }
        public System.DateTime RenewSubmitTime { get; set; }
        public Nullable<bool> Is2010YearModefied { get; set; }
        public string RentStructure { get; set; }
        public Nullable<System.DateTime> CancelContractDate { get; set; }
        public string CancelContractDESC { get; set; }
        public string RentPayer { get; set; }
        public string RentPayWay { get; set; }
        public Nullable<System.DateTime> RentPayDate { get; set; }
        public Nullable<bool> IsHasDepositeMoney { get; set; }
        public Nullable<decimal> DepositeMoney { get; set; }
        public Nullable<bool> IsDepositeRefund { get; set; }
        public Nullable<System.DateTime> DepositeRefundDate { get; set; }
        public Nullable<bool> IsOverduePayClause { get; set; }
        public Nullable<bool> IsBankGuarantee { get; set; }
        public string BankGuaranteeCode { get; set; }
        public Nullable<decimal> BankGuaranteeMoney { get; set; }
        public Nullable<System.DateTime> BankGuaranteeStartTime { get; set; }
        public Nullable<System.DateTime> BankGuaranteeEndTime { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Sequence { get; set; }
        public bool IsHistory { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public string LastUpdateUserAccount { get; set; }
        public string LastUpdateUserNameZHCN { get; set; }
        public string LastUpdateUserNameENUS { get; set; }
        public string CreateUserAccount { get; set; }
        public string CreateUserNameZHCN { get; set; }
        public string CreateUserNameENUS { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
