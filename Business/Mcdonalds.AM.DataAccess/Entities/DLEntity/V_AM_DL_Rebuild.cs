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
    public partial class V_AM_DL_Rebuild : BaseEntity<V_AM_DL_Rebuild>
    {
        public bool Editable { get; set; }

        public static V_AM_DL_Rebuild Get(Guid ID)
        {
            var rebuild = FirstOrDefault(i => i.Id == ID);
            var projectInfo = ProjectInfo.Search(i => i.USCode == rebuild.USCode && i.FlowCode == FlowCode.Rebuild).OrderByDescending(i => i.CreateTime).ToList();
            if (rebuild != null)
            {
                if (projectInfo.Count > 0 && projectInfo[0].Id == rebuild.Id)
                    rebuild.Editable = true;
                else
                    rebuild.Editable = false;
            }
            return rebuild;
        }

        public void Save(bool pushOrNot)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.Get(Id);
                if (projectInfo == null)
                {
                    ProjectId = ProjectInfo.CreateDLProject(Id, FlowCode.Rebuild, USCode, NodeCode.Start, ClientCookie.UserCode, pushOrNot);

                    var rebuildInfo = new RebuildInfo();
                    rebuildInfo.Id = Guid.NewGuid();
                    rebuildInfo.ProjectId = ProjectId;
                    rebuildInfo.USCode = USCode;
                    rebuildInfo.CreateTime = DateTime.Now;
                    rebuildInfo.CreateUserAccount = ClientCookie.UserCode;
                    rebuildInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                    rebuildInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    rebuildInfo.GBDate = GBDate;
                    rebuildInfo.ReopenDate = ReopenDate;
                    rebuildInfo.Add();

                    var rebuildPackage = new RebuildPackage();
                    rebuildPackage.Id = Guid.NewGuid();
                    rebuildPackage.ProjectId = ProjectId;
                    rebuildPackage.IsHistory = false;
                    rebuildPackage.ChangeLandlordType = ChangeLandlordType;
                    rebuildPackage.NewLandlord = ChangeLandLordDESC;
                    rebuildPackage.ChangeRentalType = ChangeRentalType;
                    rebuildPackage.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                    rebuildPackage.ChangeLeaseTermType = ChangeLeaseTermType;
                    rebuildPackage.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                    rebuildPackage.ChangeRedLineType = ChangeRedLineType;
                    rebuildPackage.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                    //rebuildPackage.NewRentalStructure = NewRentalStructure;
                    rebuildPackage.Add();

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

                    var rebuildConsInfo = new RebuildConsInfo();
                    rebuildConsInfo.Id = Guid.NewGuid();
                    rebuildConsInfo.ProjectId = ProjectId;
                    rebuildConsInfo.IsHistory = false;
                    rebuildConsInfo.CreateTime = DateTime.Now;
                    rebuildConsInfo.CreateUserAccount = ClientCookie.UserCode;
                    rebuildConsInfo.Add();

                    var reinvestmentBasicInfo = new ReinvestmentBasicInfo();
                    reinvestmentBasicInfo.ConsInfoID = rebuildConsInfo.Id;
                    reinvestmentBasicInfo.EstimatedSeatNo = OriginalSeatNO;
                    reinvestmentBasicInfo.RightSizingSeatNo = AfterRebuildSeatNO;
                    reinvestmentBasicInfo.NewDesignType = AfterRebuildDesignType;
                    reinvestmentBasicInfo.NewOperationSize = AfterRebuildOperationArea;
                    reinvestmentBasicInfo.Add();

                    var rebuildConsInvtChecking = new RebuildConsInvtChecking();
                    rebuildConsInvtChecking.Id = Guid.NewGuid();
                    rebuildConsInvtChecking.ProjectId = ProjectId;
                    rebuildConsInvtChecking.IsHistory = false;
                    rebuildConsInvtChecking.Add();

                    var writeOffAmount = new WriteOffAmount();
                    writeOffAmount.Id = Guid.NewGuid();
                    writeOffAmount.ConsInfoID = rebuildConsInvtChecking.Id;
                    writeOffAmount.TotalActual = Rebuild_TotalWO_Act;
                    writeOffAmount.Add();

                    var reinvestmentCost = new ReinvestmentCost();
                    reinvestmentCost.Id = Guid.NewGuid();
                    reinvestmentCost.ConsInfoID = rebuildConsInvtChecking.Id;
                    reinvestmentCost.TotalReinvestmentFAAct = Rebuild_TotalReinvestment_Act;
                    reinvestmentCost.Add();
                }
                else
                {
                    ProjectId = projectInfo.ProjectId;
                    projectInfo.IsPushed = pushOrNot;
                    projectInfo.Update();

                    var rebuildInfo = RebuildInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (rebuildInfo != null)
                    {
                        rebuildInfo.GBDate = GBDate;
                        rebuildInfo.ReopenDate = ReopenDate;
                        rebuildInfo.Update();
                    }
                    else
                    {
                        rebuildInfo = new RebuildInfo();
                        rebuildInfo.Id = Guid.NewGuid();
                        rebuildInfo.ProjectId = ProjectId;
                        rebuildInfo.USCode = USCode;
                        rebuildInfo.CreateTime = DateTime.Now;
                        rebuildInfo.CreateUserAccount = ClientCookie.UserCode;
                        rebuildInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                        rebuildInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                        rebuildInfo.GBDate = GBDate;
                        rebuildInfo.ReopenDate = ReopenDate;
                        rebuildInfo.Add();
                    }

                    var rebuildPackage = RebuildPackage.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (rebuildPackage != null)
                    {
                        rebuildPackage.ChangeLandlordType = ChangeLandlordType;
                        rebuildPackage.NewLandlord = ChangeLandLordDESC;
                        rebuildPackage.ChangeRentalType = ChangeRentalType;
                        rebuildPackage.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                        rebuildPackage.ChangeLeaseTermType = ChangeLeaseTermType;
                        rebuildPackage.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                        rebuildPackage.ChangeRedLineType = ChangeRedLineType;
                        rebuildPackage.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                        //rebuildPackage.NewRentalStructure = NewRentalStructure;
                        rebuildPackage.Update();
                    }
                    else
                    {
                        rebuildPackage = new RebuildPackage();
                        rebuildPackage.Id = Guid.NewGuid();
                        rebuildPackage.ProjectId = ProjectId;
                        rebuildPackage.IsHistory = false;
                        rebuildPackage.ChangeLandlordType = ChangeLandlordType;
                        rebuildPackage.NewLandlord = ChangeLandLordDESC;
                        rebuildPackage.ChangeRentalType = ChangeRentalType;
                        rebuildPackage.ChangeRentalTypeDESC = ChangeRentalTypeDESC;
                        rebuildPackage.ChangeLeaseTermType = ChangeLeaseTermType;
                        rebuildPackage.ChangeLeaseTermDESC = ChangeLeaseTermDESC;
                        rebuildPackage.ChangeRedLineType = ChangeRedLineType;
                        rebuildPackage.ChangeRedLineTypeDESC = ChangeRedLineTypeDESC;
                        //rebuildPackage.NewRentalStructure = NewRentalStructure;
                        rebuildPackage.Add();
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

                    var rebuildConsInfo = RebuildConsInfo.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (rebuildConsInfo == null)
                    {
                        rebuildConsInfo = new RebuildConsInfo();
                        rebuildConsInfo.Id = Guid.NewGuid();
                        rebuildConsInfo.ProjectId = ProjectId;
                        rebuildConsInfo.IsHistory = false;
                        rebuildConsInfo.CreateTime = DateTime.Now;
                        rebuildConsInfo.CreateUserAccount = ClientCookie.UserCode;
                        rebuildConsInfo.Add();
                    }

                    var reinvestmentBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(i => i.ConsInfoID == rebuildConsInfo.Id);
                    if (reinvestmentBasicInfo != null)
                    {
                        reinvestmentBasicInfo.EstimatedSeatNo = OriginalSeatNO;
                        reinvestmentBasicInfo.RightSizingSeatNo = AfterRebuildSeatNO;
                        reinvestmentBasicInfo.NewDesignType = AfterRebuildDesignType;
                        reinvestmentBasicInfo.NewOperationSize = AfterRebuildOperationArea;
                        reinvestmentBasicInfo.Update();
                    }
                    else
                    {
                        reinvestmentBasicInfo = new ReinvestmentBasicInfo();
                        reinvestmentBasicInfo.ConsInfoID = rebuildConsInfo.Id;
                        reinvestmentBasicInfo.EstimatedSeatNo = OriginalSeatNO;
                        reinvestmentBasicInfo.RightSizingSeatNo = AfterRebuildSeatNO;
                        reinvestmentBasicInfo.NewDesignType = AfterRebuildDesignType;
                        reinvestmentBasicInfo.NewOperationSize = AfterRebuildOperationArea;
                        reinvestmentBasicInfo.Add();
                    }

                    var rebuildConsInvtChecking = RebuildConsInvtChecking.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (rebuildConsInvtChecking == null)
                    {
                        rebuildConsInvtChecking = new RebuildConsInvtChecking();
                        rebuildConsInvtChecking.Id = Guid.NewGuid();
                        rebuildConsInvtChecking.ProjectId = ProjectId;
                        rebuildConsInvtChecking.IsHistory = false;
                        rebuildConsInvtChecking.Add();
                    }

                    var writeOffAmount = WriteOffAmount.FirstOrDefault(i => i.ConsInfoID == rebuildConsInvtChecking.Id);
                    if (writeOffAmount != null)
                    {
                        writeOffAmount.TotalActual = Rebuild_TotalWO_Act;
                        writeOffAmount.Update();
                    }
                    else
                    {
                        writeOffAmount = new WriteOffAmount();
                        writeOffAmount.Id = Guid.NewGuid();
                        writeOffAmount.ConsInfoID = rebuildConsInvtChecking.Id;
                        writeOffAmount.TotalActual = Rebuild_TotalWO_Act;
                        writeOffAmount.Add();
                    }

                    var reinvestmentCost = ReinvestmentCost.FirstOrDefault(i => i.ConsInfoID == rebuildConsInvtChecking.Id);
                    if (reinvestmentCost != null)
                    {
                        reinvestmentCost.TotalReinvestmentFAAct = Rebuild_TotalReinvestment_Act;
                        reinvestmentCost.Update();
                    }
                    else
                    {
                        reinvestmentCost = new ReinvestmentCost();
                        reinvestmentCost.Id = Guid.NewGuid();
                        reinvestmentCost.ConsInfoID = rebuildConsInvtChecking.Id;
                        reinvestmentCost.TotalReinvestmentFAAct = Rebuild_TotalReinvestment_Act;
                        reinvestmentCost.Add();
                    }
                }
                tranScope.Complete();
            }
        }
    }
}
