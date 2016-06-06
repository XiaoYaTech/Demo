using AutoMapper;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/15/2014 5:38:00 PM
 * FileName     :   ProjectContractInfo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectContractInfo : BaseEntity<ProjectContractInfo>
    {
        public static ProjectContractInfo Get(string projectId)
        {
            var contract = FirstOrDefault(c => c.ProjectId == projectId);
            if (contract == null)
            {
                var storeContract = StoreContractInfo.GetCurrentContract(projectId);
                Mapper.CreateMap<StoreContractInfo, ProjectContractInfo>();
                contract = Mapper.Map<ProjectContractInfo>(storeContract);
                contract.ContractInfoId = storeContract.Id;
                if (!string.IsNullOrEmpty(storeContract.BGCommencementDate))
                {
                    contract.BGCommencementDate = DateTime.Parse(storeContract.BGCommencementDate);
                }
                if (!string.IsNullOrEmpty(storeContract.BGEndDate))
                {
                    contract.BGEndDate = DateTime.Parse(storeContract.BGEndDate);
                }
                contract.Id = Guid.NewGuid();
                contract.ProjectId = projectId;
            }
            return contract;
        }
        public static ProjectContractDto GetContractWithHistory(string projectId)
        {
            var contract = Search(c => c.ProjectId == projectId).OrderByDescending(c => c.CreatedTime).FirstOrDefault(c => c.ProjectId == projectId);
            if (contract == null)
            {
                var storeContract = StoreContractInfo.GetCurrentContract(projectId);
                Mapper.CreateMap<StoreContractInfo, ProjectContractInfo>();
                contract = Mapper.Map<ProjectContractInfo>(storeContract);
                contract.ContractInfoId = storeContract.Id;
                contract.Id = Guid.NewGuid();
                //if (contract.McDLegalEntity != null && contract.McDLegalEntity.StartsWith("suoya"))
                //    contract.McDLegalEntity = Dictionary.Search(d => d.Value == contract.McDLegalEntity).FirstOrDefault().NameZHCN;
                //if (contract.McDOwnership != null && contract.McDOwnership.StartsWith("suoya"))
                //    contract.McDOwnership = Dictionary.Search(d => d.Value == contract.McDOwnership).FirstOrDefault().NameZHCN;
                //if (contract.LeasePurchase != null && contract.LeasePurchase.StartsWith("suoya"))
                //    contract.LeasePurchase = Dictionary.Search(d => d.Value == contract.LeasePurchase).FirstOrDefault().NameZHCN;
                //if (contract.RentPaymentArrangement != null && contract.RentPaymentArrangement.StartsWith("suoya"))
                //    contract.RentPaymentArrangement = Dictionary.Search(d => d.Value == contract.RentPaymentArrangement).FirstOrDefault().NameZHCN;
                contract.ProjectId = projectId;
            }
            var histories = StoreContractInfo.SearchByProject(projectId).Skip(1).ToList().Select(c =>
            {
                Mapper.CreateMap<StoreContractInfo, ProjectContractInfo>();
                var history = Mapper.Map<ProjectContractInfo>(c);
                history.ContractInfoId = c.Id;
                history.ProjectId = projectId;
                return history;
            }).ToList();
            return new ProjectContractDto
            {
                Current = contract,
                Histories = histories
            };
        }

        public void Save()
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (ProjectContractInfo.Any(c => c.Id == this.Id))
                {
                    this.Update();
                }
                else
                {
                    this.CreatedTime = DateTime.Now;
                    this.Add();
                }
                tranScope.Complete();
            }
        }

        public void Save(List<ProjectContractRevision> revisions)
        {
            revisions = revisions ?? new List<ProjectContractRevision>();
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (ProjectContractInfo.Any(c => c.Id == this.Id))
                {
                    this.Update();
                }
                else
                {
                    this.CreatedTime = DateTime.Now;
                    this.Add();
                }
                revisions = revisions.OrderBy(r => r.ChangeDate).ToList();
                revisions.ForEach(r =>
                {
                    r.ProjectContractId = this.Id;
                    r.Save();
                    if (r.Rent == "Y")
                    {
                        this.RentStructure = r.RentStructureNew;
                    }
                    if (r.Size == "Y")
                    {
                        this.TotalLeasedArea = r.RedlineAreaNew;
                    }
                    if (r.LeaseTerm == "Y")
                    {
                        this.EndDate = r.LeaseChangeExpiryNew;
                    }
                    if (r.Entity == "Y")
                    {
                        this.PartyAFullName = r.LandlordNew;
                    }
                });
                this.Update();
                var rIds = revisions.Select(e => e.Id).ToList();
                ProjectContractRevision.Delete(e => e.ProjectContractId == this.Id && !rIds.Contains(e.Id));
                tranScope.Complete();
            }
        }

        public void Submit(List<ProjectContractRevision> revisions, string flowCode)
        {
            revisions = revisions ?? new List<ProjectContractRevision>();
            using (TransactionScope tranScope = new TransactionScope())
            {
                StoreContractInfo contractInfo = this.ToStoreContractInfo();
                ProjectContractRevision reInfo = new ProjectContractRevision();
                #region 初始化项目的Revision信息
                reInfo.Id = Guid.NewGuid();
                reInfo.ProjectId = this.ProjectId;
                Guid srId = Guid.NewGuid();
                reInfo.ProjectContractId = this.Id;
                reInfo.RevisionId = srId;
                reInfo.StoreContractInfoId = contractInfo.Id;
                reInfo.StoreID = this.StoreId;
                reInfo.StoreCode = this.StoreCode;
                reInfo.LeaseRecapID = this.LeaseRecapID;
                reInfo.ChangeDate = DateTime.Now;
                if (flowCode == FlowCode.MajorLease_ContractInfo)
                {
                    MajorLeaseInfo info = MajorLeaseInfo.Search(e => e.ProjectId == ProjectId).FirstOrDefault();
                    reInfo.Rent = info.ChangeRentalType.HasValue && info.ChangeRentalType.Value ? "Y" : null;
                    reInfo.Size = info.ChangeRedLineType.HasValue && info.ChangeRedLineType.Value ? "Y" : null;
                    reInfo.LeaseTerm = info.ChangeLeaseTermType.HasValue && info.ChangeLeaseTermType.Value ? "Y" : null;
                    reInfo.Entity = info.ChangeLandlordType.HasValue && info.ChangeLandlordType.Value ? "Y" : null;
                    reInfo.Others = info.ChangeOtherType.HasValue && info.ChangeOtherType.Value ? "Y" : null;
                    reInfo.RentStructureOld = info.OldRentalStructure;
                    reInfo.RentStructureNew = info.NewRentalStructure;
                    reInfo.RedlineAreaOld = info.OldChangeRedLineRedLineArea.HasValue ? info.OldChangeRedLineRedLineArea.ToString() : null;
                    reInfo.RedlineAreaNew = info.NewChangeRedLineRedLineArea.HasValue ? info.NewChangeRedLineRedLineArea.ToString() : null;
                    reInfo.LeaseChangeExpiryOld = info.OldChangeLeaseTermExpiraryDate;
                    reInfo.LeaseChangeExpiryNew = info.NewChangeLeaseTermExpiraryDate;
                    reInfo.LandlordOld = info.OldLandlord;
                    reInfo.LandlordNew = info.NewLandlord;
                    reInfo.OthersDescription = info.Others;
                    reInfo.Description = info.LeaseChangeDescription;
                }
                else if (flowCode == FlowCode.Rebuild_ContractInfo)
                {
                    RebuildPackage info = RebuildPackage.GetRebuildPackageInfo(ProjectId);
                    reInfo.Rent = info.ChangeRentalType.HasValue && info.ChangeRentalType.Value ? "Y" : null;
                    reInfo.Size = info.ChangeRedLineType.HasValue && info.ChangeRedLineType.Value ? "Y" : null;
                    reInfo.LeaseTerm = info.ChangeLeaseTermType.HasValue && info.ChangeLeaseTermType.Value ? "Y" : null;
                    reInfo.Entity = info.ChangeLandlordType.HasValue && info.ChangeLandlordType.Value ? "Y" : null;
                    reInfo.Others = info.ChangeOtherType.HasValue && info.ChangeOtherType.Value ? "Y" : null;
                    reInfo.RentStructureOld = info.OldRentalStructure;
                    reInfo.RentStructureNew = info.NewRentalStructure;
                    reInfo.RedlineAreaOld = info.OldChangeRedLineRedLineArea.HasValue ? info.OldChangeRedLineRedLineArea.ToString() : null;
                    reInfo.RedlineAreaNew = info.NewChangeRedLineRedLineArea.HasValue ? info.NewChangeRedLineRedLineArea.ToString() : null;
                    reInfo.LeaseChangeExpiryOld = info.OldChangeLeaseTermExpiraryDate;
                    reInfo.LeaseChangeExpiryNew = info.NewChangeLeaseTermExpiraryDate;
                    reInfo.LandlordOld = info.OldLandlord;
                    reInfo.LandlordNew = info.NewLandlord;
                    reInfo.OthersDescription = info.Others;
                    reInfo.Description = info.LeaseChangeDescription;
                }
                #endregion
                if (this.EditMode == "EDIT")
                {
                    if (flowCode == FlowCode.Closure_ContractInfo)
                    {
                        revisions = revisions.OrderBy(r => r.ChangeDate).ToList();
                        revisions.ForEach(r =>
                        {
                            var sr = r.ToStoreContractRevision();
                            sr.Save();
                            r.RevisionId = sr.Id;
                            //回写到ContractInfo中
                            if (sr.Rent == "Y")
                            {
                                this.RentStructure = sr.RentStructureNew;
                            }
                            if (sr.Size == "Y")
                            {
                                this.TotalLeasedArea = sr.RedlineAreaNew;
                            }
                            if (sr.LeaseTerm == "Y")
                            {
                                this.EndDate = sr.LeaseChangeExpiryNew;
                            }
                            if (sr.Entity == "Y")
                            {
                                this.PartyAFullName = sr.LandlordNew;
                            }

                        });
                        contractInfo = this.ToStoreContractInfo();
                        var sRIds = revisions.Select(e => e.RevisionId).ToList();
                        StoreContractRevision.Delete(e => e.StoreContractInfoId == contractInfo.Id && !sRIds.Contains(e.Id));
                        this.Save(revisions);
                    }
                    else
                    {
                        this.Save();
                        reInfo.Save();
                        var sr = reInfo.ToStoreContractRevision();
                        sr.Id = srId;
                        sr.Save();
                    }
                    contractInfo.Update();
                }
                else
                {
                    contractInfo.Id = Guid.NewGuid();
                    contractInfo.CreatedTime = DateTime.Now;
                    contractInfo.Add();
                    this.ContractInfoId = contractInfo.Id;
                    if (Any(e => e.Id == this.Id))
                    {
                        this.Update();
                    }
                    else
                    {
                        this.Add();
                    }
                    //新增时不需要带入修订信息
                    ProjectContractRevision.Delete(r => r.ProjectContractId == this.Id);
                    //if (flowCode != FlowCode.Renewal_ContractInfo)
                    //{
                    //    reInfo.StoreContractInfoId = contractInfo.Id;
                    //    reInfo.Save();
                    //    var sr = reInfo.ToStoreContractRevision();
                    //    sr.Id = srId;
                    //    sr.Save();
                    //}
                }
                //关闭任务
                var task = TaskWork.FirstOrDefault(e => e.ReceiverAccount == ClientCookie.UserCode
                  && e.TypeCode.Contains("ContractInfo") && e.RefID == this.ProjectId);
                if (task != null)
                {
                    task.Finish();
                }
                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == this.ProjectId && e.FlowCode.Contains("ContractInfo"));
                ProjectInfo.FinishNode(this.ProjectId, projectInfo.FlowCode, NodeCode.Finish, ProjectStatus.Finished);
                ProjectInfo.CompleteMainIfEnable(this.ProjectId);
                if (projectInfo.FlowCode == FlowCode.Renewal_ContractInfo)
                {
                    if (ProjectInfo.IsFlowFinished(ProjectId, FlowCode.Renewal_SiteInfo))
                    {
                        ProjectProgress.SetProgress(ProjectId, "100%");
                    }
                }
                
                tranScope.Complete();
            }
        }

        public StoreContractInfo ToStoreContractInfo()
        {
            StoreContractInfo contract = new StoreContractInfo();
            contract.Id = this.ContractInfoId;
            contract.StoreID = this.StoreId;
            contract.StoreCode = this.StoreCode;
            contract.LeaseRecapID = this.LeaseRecapID;
            contract.PartyAFullName = this.PartyAFullName;
            contract.McDLegalEntity = this.McDLegalEntity;
            contract.McDOwnership = this.McDOwnership;
            contract.ContactPerson = this.ContactPerson;
            contract.ContactMode = this.ContactMode;
            contract.RentType = this.RentType;
            contract.TotalLeasedArea = this.TotalLeasedArea;
            contract.LeasePurchaseTerm = this.LeasePurchaseTerm;
            contract.LeasePurchase = this.LeasePurchase;
            contract.StartDate = this.StartDate;
            contract.StartYear = this.StartYear;
            contract.RentCommencementDate = this.RentCommencementDate;
            contract.EndDate = this.EndDate;
            contract.EndYear = this.EndYear;
            contract.DeadlineToNotice = this.DeadlineToNotice;
            contract.Changedafter2010 = this.Changedafter2010;
            contract.RentStructure = this.RentStructure;
            contract.WithEarlyTerminationClause = this.WithEarlyTerminationClause;
            contract.EarlyTerminationClauseDetail = this.EarlyTerminationClauseDetail;
            contract.RentalPaidto = this.RentalPaidto;
            contract.RentPaymentArrangement = this.RentPaymentArrangement;
            contract.HasDeposit = this.HasDeposit;
            contract.DepositAmount = this.DepositAmount;
            contract.Refundable = this.Refundable;
            contract.RefundableDate = this.RefundableDate;
            contract.WithPenaltyClause = this.WithPenaltyClause;
            contract.HasBankGuarantee = this.HasBankGuarantee;
            contract.BGNumber = this.BGNumber;
            contract.BGAmount = this.BGAmount;
            contract.BGCommencementDate = this.BGCommencementDate.HasValue ? this.BGCommencementDate.Value.ToString("yyyy-MM-dd") : "";
            contract.BGEndDate = this.BGEndDate.HasValue ? this.BGEndDate.Value.ToString("yyyy-MM-dd") : "";
            contract.FilePath = this.FilePath;
            contract.OverallAccessibility = this.OverallAccessibility;
            contract.RentSupervalueDay = this.RentSupervalueDay;
            contract.Keyword3 = this.Keyword3;
            contract.Remarks = this.Remarks;
            contract.CreatedTime = this.CreatedTime;
            contract.LastEditTime = this.LastEditTime;
            contract.FreeRentalPeriod = this.FreeRentalPeriod;
            contract.ExclusivityClause = this.ExclusivityClause;
            return contract;
        }
    }
}
