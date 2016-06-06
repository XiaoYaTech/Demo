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
    public partial class V_AM_DL_Renewal : BaseEntity<V_AM_DL_Renewal>
    {
        public bool Editable { get; set; }

        public static V_AM_DL_Renewal Get(Guid ID)
        {
            var renewal = FirstOrDefault(i => i.Id == ID);
            var projectInfo = ProjectInfo.Search(i => i.USCode == renewal.USCode && i.FlowCode == FlowCode.Renewal).OrderByDescending(i => i.CreateTime).ToList();
            if (renewal != null)
            {
                if (projectInfo.Count > 0 && projectInfo[0].Id == renewal.Id)
                    renewal.Editable = true;
                else
                    renewal.Editable = false;
            }
            return renewal;
        }

        public void Save(bool pushOrNot)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.Get(Id);
                if (projectInfo == null)
                {
                    ProjectId = ProjectInfo.CreateDLProject(Id, FlowCode.Renewal, USCode, NodeCode.Start, ClientCookie.UserCode, pushOrNot);

                    var renewalInfo = new RenewalInfo();
                    renewalInfo.Id = Guid.NewGuid();
                    renewalInfo.ProjectId = ProjectId;
                    renewalInfo.USCode = USCode;
                    renewalInfo.RenewalYears = 0;
                    renewalInfo.NeedProjectCostEst = false;
                    renewalInfo.CreateTime = DateTime.Now;
                    renewalInfo.CreateUserAccount = ClientCookie.UserCode;
                    renewalInfo.Add();

                    var renewalConsInfo = new RenewalConsInfo();
                    renewalConsInfo.Id = Guid.NewGuid();
                    renewalConsInfo.ProjectId = ProjectId;
                    renewalConsInfo.HasReinvenstment = false;
                    renewalConsInfo.IsHistory = false;
                    renewalConsInfo.CreateTime = DateTime.Now;
                    renewalConsInfo.CreateUserAccount = ClientCookie.UserCode;
                    renewalConsInfo.ProcInstId = 0;
                    renewalConsInfo.Add();

                    var writeOffAmount = new WriteOffAmount();
                    writeOffAmount.Id = Guid.NewGuid();
                    writeOffAmount.ConsInfoID = renewalConsInfo.Id;
                    writeOffAmount.TotalWriteOff = Renewal_Total_WO_Proj;
                    writeOffAmount.TotalNBV = Renewal_Total_WO_Act;
                    writeOffAmount.Add();

                    var reinvestmentCost = new ReinvestmentCost();
                    reinvestmentCost.Id = Guid.NewGuid();
                    reinvestmentCost.ConsInfoID = renewalConsInfo.Id;
                    reinvestmentCost.TotalReinvestmentNorm = Renewal_Total_Reinvestment_Proj;
                    reinvestmentCost.LHINorm = Renewal_Total_Reinvestment_Act;
                    reinvestmentCost.Add();

                    var renewalAnalysis = new RenewalAnalysis();
                    renewalAnalysis.Id = Guid.NewGuid();
                    renewalAnalysis.ProjectId = ProjectId;
                    renewalAnalysis.CreateUserAccount = ClientCookie.UserCode;
                    renewalAnalysis.CreateTime = DateTime.Now;
                    renewalAnalysis.IsHistory = false;
                    renewalAnalysis.FairMarketRentAgent = Fair_Market_Rent;
                    renewalAnalysis.DR1stTYAmount = DR1stTYAmount;
                    renewalAnalysis.RentDeviation = RentDeviation;
                    renewalAnalysis.DRMFLastTYSales = DRMFLastTYSales;
                    renewalAnalysis.DRMF1stTY = DRMF1stTY;
                    renewalAnalysis.Add();

                    var renewalTool = new RenewalTool();
                    renewalTool.Id = Guid.NewGuid();
                    renewalTool.ProjectId = ProjectId;
                    renewalTool.IsHistory = false;
                    renewalTool.CreateTime = DateTime.Now;
                    renewalTool.CreateUserAccount = ClientCookie.UserCode;
                    renewalTool.Add();

                    var renewalToolFinMeasureOutput = new RenewalToolFinMeasureOutput();
                    renewalToolFinMeasureOutput.Id = Guid.NewGuid();
                    renewalToolFinMeasureOutput.ToolId = renewalTool.Id;
                    renewalToolFinMeasureOutput.RentAsProdSalesYr1 = SRMFLastTYSales;
                    renewalToolFinMeasureOutput.RentAsProdSalesAvg = SRMF1stTYSales;
                    renewalToolFinMeasureOutput.Add();

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

                    var renewalInfo = RenewalInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (renewalInfo == null)
                    {
                        renewalInfo = new RenewalInfo();
                        renewalInfo.Id = Guid.NewGuid();
                        renewalInfo.ProjectId = ProjectId;
                        renewalInfo.USCode = USCode;
                        renewalInfo.RenewalYears = 0;
                        renewalInfo.NeedProjectCostEst = false;
                        renewalInfo.CreateTime = DateTime.Now;
                        renewalInfo.CreateUserAccount = ClientCookie.UserCode;
                        renewalInfo.Add();
                    }

                    var renewalConsInfo = RenewalConsInfo.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (renewalConsInfo == null)
                    {
                        renewalConsInfo = new RenewalConsInfo();
                        renewalConsInfo.Id = Guid.NewGuid();
                        renewalConsInfo.ProjectId = ProjectId;
                        renewalConsInfo.HasReinvenstment = false;
                        renewalConsInfo.IsHistory = false;
                        renewalConsInfo.CreateTime = DateTime.Now;
                        renewalConsInfo.CreateUserAccount = ClientCookie.UserCode;
                        renewalConsInfo.ProcInstId = 0;
                        renewalConsInfo.Add();
                    }

                    var writeOffAmount = WriteOffAmount.FirstOrDefault(i => i.ConsInfoID == renewalConsInfo.Id);
                    if (writeOffAmount == null)
                    {
                        writeOffAmount = new WriteOffAmount();
                        writeOffAmount.Id = Guid.NewGuid();
                        writeOffAmount.ConsInfoID = renewalConsInfo.Id;
                        writeOffAmount.TotalWriteOff = Renewal_Total_WO_Proj;
                        writeOffAmount.TotalNBV = Renewal_Total_WO_Act;
                        writeOffAmount.Add();
                    }
                    else
                    {
                        writeOffAmount.TotalWriteOff = Renewal_Total_WO_Proj;
                        writeOffAmount.TotalNBV = Renewal_Total_WO_Act;
                        writeOffAmount.Update();
                    }

                    var reinvestmentCost = ReinvestmentCost.FirstOrDefault(i => i.ConsInfoID == renewalConsInfo.Id);
                    if (reinvestmentCost == null)
                    {
                        reinvestmentCost = new ReinvestmentCost();
                        reinvestmentCost.Id = Guid.NewGuid();
                        reinvestmentCost.ConsInfoID = renewalConsInfo.Id;
                        reinvestmentCost.TotalReinvestmentNorm = Renewal_Total_Reinvestment_Proj;
                        reinvestmentCost.LHINorm = Renewal_Total_Reinvestment_Act;
                        reinvestmentCost.Add();
                    }
                    else
                    {
                        reinvestmentCost.TotalReinvestmentNorm = Renewal_Total_Reinvestment_Proj;
                        reinvestmentCost.LHINorm = Renewal_Total_Reinvestment_Act;
                        reinvestmentCost.Update();
                    }

                    var renewalAnalysis = RenewalAnalysis.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (renewalAnalysis == null)
                    {
                        renewalAnalysis = new RenewalAnalysis();
                        renewalAnalysis.Id = Guid.NewGuid();
                        renewalAnalysis.ProjectId = ProjectId;
                        renewalAnalysis.CreateUserAccount = ClientCookie.UserCode;
                        renewalAnalysis.CreateTime = DateTime.Now;
                        renewalAnalysis.IsHistory = false;
                        renewalAnalysis.FairMarketRentAgent = Fair_Market_Rent;
                        renewalAnalysis.DR1stTYAmount = DR1stTYAmount;
                        renewalAnalysis.RentDeviation = RentDeviation;
                        renewalAnalysis.DRMFLastTYSales = DRMFLastTYSales;
                        renewalAnalysis.DRMF1stTY = DRMF1stTY;
                        renewalAnalysis.Add();
                    }
                    else
                    {
                        renewalAnalysis.FairMarketRentAgent = Fair_Market_Rent;
                        renewalAnalysis.DR1stTYAmount = DR1stTYAmount;
                        renewalAnalysis.RentDeviation = RentDeviation;
                        renewalAnalysis.DRMFLastTYSales = DRMFLastTYSales;
                        renewalAnalysis.DRMF1stTY = DRMF1stTY;
                        renewalAnalysis.Update();
                    }

                    var renewalTool = RenewalTool.FirstOrDefault(i => i.ProjectId == ProjectId && !i.IsHistory);
                    if (renewalTool == null)
                    {
                        renewalTool = new RenewalTool();
                        renewalTool.Id = Guid.NewGuid();
                        renewalTool.ProjectId = ProjectId;
                        renewalTool.IsHistory = false;
                        renewalTool.CreateTime = DateTime.Now;
                        renewalTool.CreateUserAccount = ClientCookie.UserCode;
                        renewalTool.Add();
                    }

                    var renewalToolFinMeasureOutput = RenewalToolFinMeasureOutput.FirstOrDefault(i => i.ToolId == renewalTool.Id);
                    if (renewalToolFinMeasureOutput == null)
                    {
                        renewalToolFinMeasureOutput = new RenewalToolFinMeasureOutput();
                        renewalToolFinMeasureOutput.Id = Guid.NewGuid();
                        renewalToolFinMeasureOutput.ToolId = renewalTool.Id;
                        renewalToolFinMeasureOutput.RentAsProdSalesYr1 = SRMFLastTYSales;
                        renewalToolFinMeasureOutput.RentAsProdSalesAvg = SRMF1stTYSales;
                        renewalToolFinMeasureOutput.Add();
                    }
                    else
                    {
                        renewalToolFinMeasureOutput.RentAsProdSalesYr1 = SRMFLastTYSales;
                        renewalToolFinMeasureOutput.RentAsProdSalesAvg = SRMF1stTYSales;
                        renewalToolFinMeasureOutput.Update();
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
