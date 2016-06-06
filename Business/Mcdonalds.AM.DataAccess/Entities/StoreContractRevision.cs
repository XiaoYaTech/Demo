using System;
using System.Collections.Generic;
using System.Linq;
namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreContractRevision : BaseEntity<StoreContractRevision>
    {

        internal void Save()
        {
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
            if (StoreContractRevision.Any(e => e.Id == this.Id))
            {
                this.SyncTime = DateTime.Now;
                this.Update();
            }
            else
            {
                this.CreatedTime = DateTime.Now;
                this.SyncTime = DateTime.Now;
                this.Add();
            }
        }

        public ProjectContractRevision ToProjectContractRevision()
        {
            ProjectContractRevision revision = new ProjectContractRevision();
            revision.Id = Guid.NewGuid();
            revision.RevisionId = this.Id;
            revision.StoreContractInfoId = this.StoreContractInfoId;
            revision.StoreID = this.StoreID;
            revision.StoreCode = this.StoreCode;
            revision.LeaseRecapID = this.LeaseRecapID;
            revision.ChangeDate = !string.IsNullOrEmpty(ChangeDate) ? (Nullable<DateTime>)DateTime.Parse(ChangeDate) : null;
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

        public static List<StoreContractRevision> Get(string usCode)
        {
            return Search(e => e.StoreCode == usCode).ToList();
        }

        public static List<StoreContractRevision> Get(Guid contractId)
        {
            return Search(e => e.StoreContractInfoId == contractId).OrderBy(e=>e.ChangeDate).ToList();
        }
    }
}
