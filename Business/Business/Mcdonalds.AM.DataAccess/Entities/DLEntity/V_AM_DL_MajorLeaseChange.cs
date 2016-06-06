using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class V_AM_DL_MajorLeaseChange : BaseEntity<V_AM_DL_MajorLeaseChange>
    {
        public bool Editable { get; set; }

        public static V_AM_DL_MajorLeaseChange Get(Guid ID)
        {
            var mlcInfo = FirstOrDefault(i => i.Id == ID);
            var projectInfo = ProjectInfo.Search(i => i.USCode == mlcInfo.USCode && i.FlowCode == FlowCode.MajorLease).OrderByDescending(i => i.CreateTime).ToList();
            if (mlcInfo != null)
            {
                if (projectInfo.Count > 0 && projectInfo[0].Id == mlcInfo.Id)
                    mlcInfo.Editable = true;
                else
                    mlcInfo.Editable = false;
            }
            return mlcInfo;
        }

        public void Save(bool pushOrNot)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.Get(Id);
                if (projectInfo == null)
                {
                    ProjectId = ProjectInfo.CreateDLProject(Id, FlowCode.MajorLease, USCode, NodeCode.Start, ClientCookie.UserCode, pushOrNot);

                    var mlcInfo = new MajorLeaseInfo();
                    mlcInfo.Id = Guid.NewGuid();
                    mlcInfo.ProjectId = ProjectId;
                    mlcInfo.USCode = USCode;
                    mlcInfo.ChangeRentalType = ChangeRentalType;
                    mlcInfo.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                    mlcInfo.ChangeRedLineType = ChangeRedLineType;
                    mlcInfo.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                    mlcInfo.ChangeLeaseTermType = ChangeLeaseTermType;
                    mlcInfo.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                    mlcInfo.CreateTime = DateTime.Now;
                    mlcInfo.CreateUserAccount = ClientCookie.UserCode;
                    mlcInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                    mlcInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    mlcInfo.ChangeLandlordType = ChangeLandlordType;
                    mlcInfo.ChangeLandLordDESC = ChangeLandLordDESC;
                    mlcInfo.Add();

                    var mlcPackage = new MajorLeaseChangePackage();
                    mlcPackage.Id = Guid.NewGuid();
                    mlcPackage.ProjectId = ProjectId;
                    mlcPackage.WriteOff = MLCNetWriteOff_Act;
                    mlcPackage.IsHistory = false;
                    mlcPackage.CreateUserAccount = ClientCookie.UserCode;
                    mlcPackage.CreateTime = DateTime.Now;
                    mlcPackage.Add();

                    var mlcConsInvtChecking = new MajorLeaseConsInvtChecking();
                    mlcConsInvtChecking.Id = Guid.NewGuid();
                    mlcConsInvtChecking.ProjectId = ProjectId;
                    mlcConsInvtChecking.IsHistory = false;
                    mlcConsInvtChecking.CreateTime = DateTime.Now;
                    mlcConsInvtChecking.Add();

                    var reinvestmentCost = new ReinvestmentCost();
                    reinvestmentCost.Id = Guid.NewGuid();
                    reinvestmentCost.ConsInfoID = mlcConsInvtChecking.Id;
                    reinvestmentCost.TotalReinvestmentFAAct = MLC_TotalReinvestment_Act;
                    reinvestmentCost.Add();

                    var projectContractInfo = new ProjectContractInfo();
                    projectContractInfo.Id = Guid.NewGuid();
                    projectContractInfo.ProjectId = ProjectId;
                    projectContractInfo.ContractInfoId = Guid.Empty;
                    projectContractInfo.CreatedTime = DateTime.Now;
                    projectContractInfo.WriteBack = false;
                    projectContractInfo.EditMode = EditMode;
                    projectContractInfo.PartyAFullName = PartyAFullName;
                    projectContractInfo.McDLegalEntity = ContractEntityName;
                    projectContractInfo.McDOwnership = McdOwnership;
                    projectContractInfo.ContactPerson = ContactPerson;
                    projectContractInfo.ContactMode = ContactMode;
                    projectContractInfo.RentType = RentType;
                    projectContractInfo.TotalLeasedArea = RentSize;
                    projectContractInfo.LeasePurchaseTerm = ContractTerm;
                    projectContractInfo.LeasePurchase = ContractType;
                    projectContractInfo.StartDate = ContractStartDate;
                    projectContractInfo.EndDate = ContraceEndDate;
                    projectContractInfo.StartYear = ContractStartYear;
                    projectContractInfo.EndYear = ContraceEndYear;
                    projectContractInfo.RentCommencementDate = RentPaymentStartDate;
                    projectContractInfo.DeadlineToNotice = DeadlineToNoticeLL;
                    projectContractInfo.Changedafter2010 = ChangedAfter2010;
                    projectContractInfo.RentStructure = RentStructure;
                    projectContractInfo.WithEarlyTerminationClause = EarlyTerminationClause;
                    projectContractInfo.EarlyTerminationClauseDetail = EarlyTerminationClauseDescription;
                    projectContractInfo.RentPaymentArrangement = RentPaymentArrangement;
                    projectContractInfo.HasDeposit = Deposit;
                    projectContractInfo.DepositAmount = DepositAmount;
                    projectContractInfo.RefundableDate = WhenRefund;
                    projectContractInfo.WithPenaltyClause = withPenaltyClause;
                    projectContractInfo.HasBankGuarantee = BankGuarantee;
                    projectContractInfo.BGNumber = BankGuaranteeNumber;
                    projectContractInfo.BGAmount = BankGuaranteeAmount;
                    projectContractInfo.BGCommencementDate = BGCommencementDate;
                    projectContractInfo.BGEndDate = BGEndDate;
                    projectContractInfo.Add();
                }
                else
                {
                    ProjectId = projectInfo.ProjectId;
                    projectInfo.IsPushed = pushOrNot;
                    projectInfo.Update();

                    var mlcInfo = MajorLeaseInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (mlcInfo != null)
                    {
                        mlcInfo.ChangeRentalType = ChangeRentalType;
                        mlcInfo.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                        mlcInfo.ChangeRedLineType = ChangeRedLineType;
                        mlcInfo.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                        mlcInfo.ChangeLeaseTermType = ChangeLeaseTermType;
                        mlcInfo.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                        mlcInfo.ChangeLandlordType = ChangeLandlordType;
                        mlcInfo.ChangeLandLordDESC = ChangeLandLordDESC;
                        mlcInfo.Update();
                    }
                    else
                    {
                        mlcInfo = new MajorLeaseInfo();
                        mlcInfo.Id = Guid.NewGuid();
                        mlcInfo.ProjectId = ProjectId;
                        mlcInfo.USCode = USCode;
                        mlcInfo.ChangeRentalType = ChangeRentalType;
                        mlcInfo.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                        mlcInfo.ChangeRedLineType = ChangeRedLineType;
                        mlcInfo.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                        mlcInfo.ChangeLeaseTermType = ChangeLeaseTermType;
                        mlcInfo.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                        mlcInfo.CreateTime = DateTime.Now;
                        mlcInfo.CreateUserAccount = ClientCookie.UserCode;
                        mlcInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                        mlcInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                        mlcInfo.ChangeLandlordType = ChangeLandlordType;
                        mlcInfo.ChangeLandLordDESC = ChangeLandLordDESC;
                        mlcInfo.Add();
                    }

                    var mlcPackage = MajorLeaseChangePackage.GetMajorPackageInfo(ProjectId);
                    if (mlcPackage != null)
                    {
                        mlcPackage.WriteOff = MLCNetWriteOff_Act;
                        mlcPackage.Update();
                    }
                    else
                    {
                        mlcPackage = new MajorLeaseChangePackage();
                        mlcPackage.Id = Guid.NewGuid();
                        mlcPackage.ProjectId = ProjectId;
                        mlcPackage.WriteOff = MLCNetWriteOff_Act;
                        mlcPackage.IsHistory = false;
                        mlcPackage.CreateUserAccount = ClientCookie.UserCode;
                        mlcPackage.CreateTime = DateTime.Now;
                        mlcPackage.Add();
                    }

                    var mlcConsInvtChecking = MajorLeaseConsInvtChecking.FirstOrDefault(i => i.ProjectId == ProjectId && i.IsHistory == false);
                    if (mlcConsInvtChecking == null)
                    {
                        mlcConsInvtChecking = new MajorLeaseConsInvtChecking();
                        mlcConsInvtChecking.Id = Guid.NewGuid();
                        mlcConsInvtChecking.ProjectId = ProjectId;
                        mlcConsInvtChecking.IsHistory = false;
                        mlcConsInvtChecking.CreateTime = DateTime.Now;
                        mlcConsInvtChecking.Add();
                    }

                    var reinvestmentCost = ReinvestmentCost.GetByConsInfoId(mlcConsInvtChecking.Id);
                    if (reinvestmentCost != null)
                    {
                        reinvestmentCost.TotalReinvestmentFAAct = MLC_TotalReinvestment_Act;
                        reinvestmentCost.Update();
                    }
                    else
                    {
                        reinvestmentCost = new ReinvestmentCost();
                        reinvestmentCost.Id = Guid.NewGuid();
                        reinvestmentCost.ConsInfoID = mlcConsInvtChecking.Id;
                        reinvestmentCost.TotalReinvestmentFAAct = MLC_TotalReinvestment_Act;
                        reinvestmentCost.Add();
                    }

                    var projectContractInfo = ProjectContractInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (projectContractInfo != null)
                    {
                        projectContractInfo.EditMode = EditMode;
                        projectContractInfo.PartyAFullName = PartyAFullName;
                        projectContractInfo.McDLegalEntity = ContractEntityName;
                        projectContractInfo.McDOwnership = McdOwnership;
                        projectContractInfo.ContactPerson = ContactPerson;
                        projectContractInfo.ContactMode = ContactMode;
                        projectContractInfo.RentType = RentType;
                        projectContractInfo.TotalLeasedArea = RentSize;
                        projectContractInfo.LeasePurchaseTerm = ContractTerm;
                        projectContractInfo.LeasePurchase = ContractType;
                        projectContractInfo.StartDate = ContractStartDate;
                        projectContractInfo.EndDate = ContraceEndDate;
                        projectContractInfo.StartYear = ContractStartYear;
                        projectContractInfo.EndYear = ContraceEndYear;
                        projectContractInfo.RentCommencementDate = RentPaymentStartDate;
                        projectContractInfo.DeadlineToNotice = DeadlineToNoticeLL;
                        projectContractInfo.Changedafter2010 = ChangedAfter2010;
                        projectContractInfo.RentStructure = RentStructure;
                        projectContractInfo.WithEarlyTerminationClause = EarlyTerminationClause;
                        projectContractInfo.EarlyTerminationClauseDetail = EarlyTerminationClauseDescription;
                        projectContractInfo.RentPaymentArrangement = RentPaymentArrangement;
                        projectContractInfo.HasDeposit = Deposit;
                        projectContractInfo.DepositAmount = DepositAmount;
                        projectContractInfo.RefundableDate = WhenRefund;
                        projectContractInfo.WithPenaltyClause = withPenaltyClause;
                        projectContractInfo.HasBankGuarantee = BankGuarantee;
                        projectContractInfo.BGNumber = BankGuaranteeNumber;
                        projectContractInfo.BGAmount = BankGuaranteeAmount;
                        projectContractInfo.BGCommencementDate = BGCommencementDate;
                        projectContractInfo.BGEndDate = BGEndDate;
                        projectContractInfo.Update();
                    }
                    else
                    {
                        projectContractInfo = new ProjectContractInfo();
                        projectContractInfo.Id = Guid.NewGuid();
                        projectContractInfo.ProjectId = ProjectId;
                        projectContractInfo.ContractInfoId = Guid.Empty;
                        projectContractInfo.CreatedTime = DateTime.Now;
                        projectContractInfo.WriteBack = false;
                        projectContractInfo.EditMode = EditMode;
                        projectContractInfo.PartyAFullName = PartyAFullName;
                        projectContractInfo.McDLegalEntity = ContractEntityName;
                        projectContractInfo.McDOwnership = McdOwnership;
                        projectContractInfo.ContactPerson = ContactPerson;
                        projectContractInfo.ContactMode = ContactMode;
                        projectContractInfo.RentType = RentType;
                        projectContractInfo.TotalLeasedArea = RentSize;
                        projectContractInfo.LeasePurchaseTerm = ContractTerm;
                        projectContractInfo.LeasePurchase = ContractType;
                        projectContractInfo.StartDate = ContractStartDate;
                        projectContractInfo.EndDate = ContraceEndDate;
                        projectContractInfo.StartYear = ContractStartYear;
                        projectContractInfo.EndYear = ContraceEndYear;
                        projectContractInfo.RentCommencementDate = RentPaymentStartDate;
                        projectContractInfo.DeadlineToNotice = DeadlineToNoticeLL;
                        projectContractInfo.Changedafter2010 = ChangedAfter2010;
                        projectContractInfo.RentStructure = RentStructure;
                        projectContractInfo.WithEarlyTerminationClause = EarlyTerminationClause;
                        projectContractInfo.EarlyTerminationClauseDetail = EarlyTerminationClauseDescription;
                        projectContractInfo.RentPaymentArrangement = RentPaymentArrangement;
                        projectContractInfo.HasDeposit = Deposit;
                        projectContractInfo.DepositAmount = DepositAmount;
                        projectContractInfo.RefundableDate = WhenRefund;
                        projectContractInfo.WithPenaltyClause = withPenaltyClause;
                        projectContractInfo.HasBankGuarantee = BankGuarantee;
                        projectContractInfo.BGNumber = BankGuaranteeNumber;
                        projectContractInfo.BGAmount = BankGuaranteeAmount;
                        projectContractInfo.BGCommencementDate = BGCommencementDate;
                        projectContractInfo.BGEndDate = BGEndDate;
                        projectContractInfo.Add();
                    }
                }
                tranScope.Complete();
            }
        }
    }
}
