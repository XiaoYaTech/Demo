using AutoMapper;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/15/2014 6:06:29 PM
 * FileName     :   ProjectContractRevision
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Globalization;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectContractRevision : BaseEntity<ProjectContractRevision>
    {
        public static List<ProjectContractRevision> Get(string projectId, Guid contractId)
        {
            if (ProjectContractRevision.Any(r => r.StoreContractInfoId == contractId && r.ProjectId == projectId))
            {
                return Search(r => r.StoreContractInfoId == contractId && r.ProjectId == projectId).OrderBy(r=> r.ChangeDate).ToList();
            }
            else
            {
                return StoreContractRevision.Search(r => r.StoreContractInfoId == contractId).AsNoTracking().ToList().Select(r =>
                {
                    var revision = r.ToProjectContractRevision();
                    revision.ProjectId = projectId;
                    return revision;
                }).OrderBy(r => r.ChangeDate).ToList();
            }
        }

        /// <summary>
        /// This method only should be used in MajorLease(Create) or Rebuild(Package)
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public static ProjectContractRevision Get(string projectId)
        {
            return ProjectContractRevision.FirstOrDefault(e => e.ProjectId == projectId);
        }

        /// <summary>
        /// This method only should be used in MajorLease(Create) or Rebuild(Package)
        /// </summary>
        public void Append()
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (this.Id == Guid.Empty)
                {
                    this.Id = Guid.NewGuid();
                }
                if (!ProjectContractInfo.Any(e => e.ProjectId == this.ProjectId))
                {
                    var contract = StoreContractInfo.GetCurrentContract(this.ProjectId);
                    Mapper.CreateMap<StoreContractInfo, ProjectContractInfo>();
                    var projContract = Mapper.Map<ProjectContractInfo>(contract);
                    projContract.ContractInfoId = contract.Id;
                    if (!string.IsNullOrEmpty(contract.BGCommencementDate))
                    {
                        projContract.BGCommencementDate = DateTime.Parse(contract.BGCommencementDate);
                    }
                    if (!string.IsNullOrEmpty(contract.BGEndDate))
                    {
                        projContract.BGEndDate = DateTime.Parse(contract.BGEndDate);
                    }
                    projContract.Id = Guid.NewGuid();
                    projContract.ProjectId = this.ProjectId;
                    projContract.Add();
                }
                var revisions = ProjectContractRevision.Get(this.ProjectId, this.ProjectContractId);
                var oldRev = revisions.FirstOrDefault(e => e.ProjectId == this.ProjectId);
                if (oldRev == null)
                {
                    revisions.Add(this); 
                }
                else
                {
                    oldRev.ChangeDate = this.ChangeDate;
                    oldRev.Description = this.Description;
                    oldRev.Entity = this.Entity;
                    oldRev.LandlordNew = this.LandlordNew;
                    oldRev.LandlordOld = this.LandlordOld;
                    oldRev.LeaseTerm = this.LeaseTerm;
                    oldRev.LeaseChangeExpiryNew = this.LeaseChangeExpiryNew;
                    oldRev.LeaseChangeExpiryOld = this.LeaseChangeExpiryOld;
                    oldRev.Others = this.Others;
                    oldRev.OthersDescription = this.OthersDescription;
                    oldRev.Rent = this.Rent;
                    oldRev.RentStructureNew = this.RentStructureNew;
                    oldRev.RentStructureOld = this.RentStructureOld;
                    oldRev.Size = this.Size;
                    oldRev.RedlineAreaNew = this.RedlineAreaNew;
                    oldRev.RedlineAreaOld = this.RedlineAreaOld;
                }
                revisions.ForEach(r => r.Save());
                tranScope.Complete();
            }
        }

        internal void Save()
        {
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
            if (!ProjectContractRevision.Any(e => e.Id == this.Id))
            {
                this.CreatedTime = DateTime.Now;
                this.Add();
            }
            else
            {
                this.LastModifyTime = DateTime.Now;
                this.Update();
            }
        }

        public StoreContractRevision ToStoreContractRevision()
        {
            StoreContractRevision revision = new StoreContractRevision();
            revision.Id = this.RevisionId;
            revision.StoreContractInfoId = this.StoreContractInfoId;
            revision.StoreID = this.StoreID;
            revision.StoreCode = this.StoreCode;
            revision.LeaseRecapID = this.LeaseRecapID;
            revision.ChangeDate = this.ChangeDate.HasValue ? this.ChangeDate.Value.ToString("yyyy-MM-dd") : null;
            revision.Rent = this.Rent;
            revision.Size = this.Size;
            revision.LeaseTerm = this.LeaseTerm;
            revision.Entity = this.Entity;
            revision.Others = this.Others;
            revision.RentStructureOld = this.RentStructureOld;
            revision.RentStructureNew = this.RentStructureNew;
            revision.RedlineAreaOld = this.RedlineAreaOld;
            revision.RedlineAreaNew = this.RedlineAreaNew;
            revision.LeaseChangeExpiryOld = this.LeaseChangeExpiryOld;
            revision.LeaseChangeExpiryNew = this.LeaseChangeExpiryNew;
            revision.LandlordOld = this.LandlordOld;
            revision.LandlordNew = this.LandlordNew;
            revision.Description = this.Description;
            revision.CreatedTime = this.CreatedTime;
            revision.OthersDescription = this.OthersDescription;
            return revision;
        }
    }
}
