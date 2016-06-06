using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AutoMapper;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreContractInfo : BaseEntity<StoreContractInfo>
    {

        public static StoreContractInfo Get(string usCode)
        {
            return Search(sc => sc.StoreCode == usCode).OrderByDescending(sc => sc.CreatedTime).ThenBy(sc => sc.Id).FirstOrDefault();
        }

        public static List<StoreContractInfo> GetAll(string usCode)
        {
            var list = Search(sc => sc.StoreCode == usCode).OrderByDescending(sc => sc.CreatedTime).ThenBy(sc => sc.Id).ToList();
            foreach (var contract in list)
            {
                if (contract.McDLegalEntity != null && contract.McDLegalEntity.StartsWith("suoya"))
                    contract.McDLegalEntity = Dictionary.Search(d => d.Value == contract.McDLegalEntity).FirstOrDefault().NameZHCN;
                if (contract.McDOwnership != null && contract.McDOwnership.StartsWith("suoya"))
                    contract.McDOwnership = Dictionary.Search(d => d.Value == contract.McDOwnership).FirstOrDefault().NameZHCN;
                if (contract.LeasePurchase != null && contract.LeasePurchase.StartsWith("suoya"))
                    contract.LeasePurchase = Dictionary.Search(d => d.Value == contract.LeasePurchase).FirstOrDefault().NameZHCN;
                if (contract.RentPaymentArrangement != null && contract.RentPaymentArrangement.StartsWith("suoya"))
                    contract.RentPaymentArrangement = Dictionary.Search(d => d.Value == contract.RentPaymentArrangement).FirstOrDefault().NameZHCN;
            }
            return list;
        }

        public static ProjectContractRevision MappingProjectContractRevision(string usCode)
        {
            ProjectContractRevision projectContractRevision = null;
            var storeContractInfo = Get(usCode);
            if (storeContractInfo != null)
            {
                projectContractRevision = new ProjectContractRevision();
                Mapper.CreateMap<StoreContractInfo, ProjectContractRevision>()
                    .ForMember(dest => dest.StoreContractInfoId, opts => opts.MapFrom(src => src.Id))
                    .ForMember(dest => dest.RentStructureOld, opts => opts.MapFrom(src => src.RentStructure))
                    .ForMember(dest => dest.RedlineAreaOld, opts => opts.MapFrom(src => src.TotalLeasedArea))
                    .ForMember(dest => dest.LeaseChangeExpiryOld, opts => opts.MapFrom(src => src.EndDate))
                    .ForMember(dest => dest.LandlordOld, opts => opts.MapFrom(src => src.PartyAFullName));

                Mapper.Map(storeContractInfo, projectContractRevision);

                projectContractRevision.Id = Guid.Empty;
            }

            return projectContractRevision;
        }
        public static StoreContractInfo GetCurrentContract(string projectId)
        {
            return SearchByProject(projectId).FirstOrDefault();
        }

        public static IQueryable<StoreContractInfo> SearchByProject(string projectId)
        {
            var db = PrepareDb();
            return (
                from p in db.ProjectInfo
                from c in db.StoreContractInfo
                where c.StoreCode == p.USCode && p.ProjectId == projectId
                select c
            ).OrderByDescending(c => c.CreatedTime).Distinct().AsNoTracking().OrderByDescending(c => c.CreatedTime);
        }

        public StoreContractInfo GetStoreContractInfo(Guid id)
        {
            var entity = FirstOrDefault(e => e.Id.Equals(id));
            var listCode = GetDisplayFileds(entity);
            var listDic = Dictionary.GetDictionary(listCode);
            if (listDic != null && listDic.Count > 0)
            {
                foreach (var code in listDic)
                {
                    var strDisplayName = listDic[code.Key].NameZHCN;
                    entity.GetType().GetProperty(code.Key).SetValue(entity, strDisplayName);
                }
            }
            return entity;
        }
        public List<StoreContractInfo> GetOriginalStoreContractInfo(string strStoreCode)
        {
            var lsStoreContractInfo =
                StoreContractInfo.Search(o => o.StoreCode.Equals(strStoreCode)).OrderByDescending(o => o.StartYear).ToList<StoreContractInfo>();
            return lsStoreContractInfo;
        }

        public List<StoreContractInfo> GetStoreContractInfo(string strStoreCode)
        {
            var lsStoreContractInfo =
                StoreContractInfo.Search(o => o.StoreCode.Equals(strStoreCode)).OrderByDescending(o => o.StartYear).ToList<StoreContractInfo>();
            foreach (var entity in lsStoreContractInfo)
            {
                var listCode = GetDisplayFileds(entity);
                var listDic = Dictionary.GetDictionary(listCode);
                if (listDic != null && listDic.Count > 0)
                {
                    foreach (var code in listDic)
                    {
                        var strDisplayName = listDic[code.Key].NameZHCN;
                        entity.GetType().GetProperty(code.Key).SetValue(entity, strDisplayName);
                    }
                }
            }
            return lsStoreContractInfo;
        }

        private Dictionary<string, string> GetDisplayFileds(StoreContractInfo entity)
        {
            var listCode = new Dictionary<string, string>();

            listCode.Add("McDLegalEntity", entity.McDLegalEntity);
            listCode.Add("McDOwnership", entity.McDOwnership);
            listCode.Add("LeasePurchase", entity.LeasePurchase);
            listCode.Add("RentPaymentArrangement", entity.RentPaymentArrangement);

            return listCode;
        }

        public static dynamic GetContractByUsCode(string usCode)
        {
            var contract = StoreContractInfo.Get(usCode);

            List<StoreContractInfoAttached> attachContract = null;
            if (contract != null)
                attachContract = StoreContractInfoAttached.Search(c => c.LeaseRecapID == contract.LeaseRecapID.ToString()).ToList();

            List<Attachment> attachProject = null;
            if (contract != null)
            {
                var projectContractInfo = ProjectContractInfo.Search(i => i.ContractInfoId == contract.Id).Select(p => p.Id.ToString()).ToList();
                attachProject = Attachment.Search(i => projectContractInfo.Contains(i.RefTableID)).ToList();
            }
            //if (contract.McDLegalEntity != null && contract.McDLegalEntity.StartsWith("suoya"))
            //    contract.McDLegalEntity = Dictionary.Search(d => d.Value == contract.McDLegalEntity).FirstOrDefault().NameZHCN;
            //if (contract.McDOwnership != null && contract.McDOwnership.StartsWith("suoya"))
            //    contract.McDOwnership = Dictionary.Search(d => d.Value == contract.McDOwnership).FirstOrDefault().NameZHCN;
            //if (contract.LeasePurchase != null && contract.LeasePurchase.StartsWith("suoya"))
            //    contract.LeasePurchase = Dictionary.Search(d => d.Value == contract.LeasePurchase).FirstOrDefault().NameZHCN;
            //if (contract.RentPaymentArrangement != null && contract.RentPaymentArrangement.StartsWith("suoya"))
            //    contract.RentPaymentArrangement = Dictionary.Search(d => d.Value == contract.RentPaymentArrangement).FirstOrDefault().NameZHCN;

            var histories = StoreContractInfo.GetAll(usCode).ToList();
            return new
            {
                Current = contract,
                Histories = histories,
                Attachments = new
                {
                    contract = attachContract,
                    project = attachProject
                }
            };
        }

        public void Save(List<StoreContractRevision> revisions)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {

                if (revisions.Count > 0)
                {
                    revisions = revisions.OrderBy(r => r.ChangeDate).ToList();
                    revisions.ForEach(r =>
                    {
                        r.ChangeDate = DateTime.Parse(r.ChangeDate).ToString("yyyy-MM-dd");
                        r.Save();
                        //回写到ContractInfo中
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
                    var rIds = revisions.Select(e => e.Id).ToList();
                    StoreContractRevision.Delete(e => e.StoreContractInfoId == this.Id && !rIds.Contains(e.Id));
                }
                else
                {
                    StoreContractRevision.Delete(e => e.StoreContractInfoId == this.Id);
                }

                if (StoreContractInfo.Any(c => c.Id == this.Id))
                {
                    this.LastEditTime = DateTime.Now;
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

    }
}
