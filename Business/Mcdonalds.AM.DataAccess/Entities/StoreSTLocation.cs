using System;
using System.Collections.Generic;
using System.Transactions;
using AutoMapper;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreSTLocation : BaseEntity<StoreSTLocation>
    {
        public Guid ProjectIdentifier { get; set; }

        public string StoreTypeName { get; set; }

        public string DTTypeNameDisplay
        {
            get { return Dictionary.ParseDisplayName(DTTypeName); }
        }

        public string DesignStyleDisplay
        {
            get
            {
                var designStyleDisplay = string.Empty;
                if (!string.IsNullOrEmpty(DesignStyle))
                {
                    var dic = Dictionary.FirstOrDefault(e => e.Code == DesignStyle);
                    if (dic != null)
                    {
                        designStyleDisplay = dic.NameENUS;
                    }
                }

                return designStyleDisplay;
            }
        }
        public string ExteriorDesignDisplay
        {
            get
            {
                var exteriorDesignDisplay = string.Empty;
                if (!string.IsNullOrEmpty(ExteriorDesign))
                {
                    var dic = Dictionary.FirstOrDefault(e => e.Code == ExteriorDesign);
                    if (dic != null)
                    {
                        exteriorDesignDisplay = dic.NameENUS;
                    }
                }

                return exteriorDesignDisplay;
            }
        }

        public string PortfolioTypeNameDisplay
        {
            get
            {
                var portfolioTypeNameDisplay = string.Empty;
                if (!string.IsNullOrEmpty(PortfolioTypeName))
                {
                    var dic = Dictionary.FirstOrDefault(e => e.Code == PortfolioTypeName);
                    if (dic != null)
                    {
                        portfolioTypeNameDisplay = dic.NameENUS;
                    }
                }

                return portfolioTypeNameDisplay;
            }
        }




        public EstimatedVsActualConstruction EstimatedVsActualConstruction { get; set; }
        public static StoreSTLocation GetStoreSTLocation(string strStorCode)
        {
            var entity = FirstOrDefault(e => e.StoreCode.Equals(strStorCode));
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
        private static Dictionary<string, string> GetDisplayFileds(StoreSTLocation entity)
        {
            var listCode = new Dictionary<string, string>();

            listCode.Add("DirectionalEffect", entity.DirectionalEffect);
            //listCode.Add("PortfolioTypeName", entity.PortfolioTypeName);
            //listCode.Add("DesignStyle", entity.DesignStyle);
            //listCode.Add("ExteriorDesign", entity.ExteriorDesign);

            return listCode;
        }

        public static StoreSTLocation GetStoreSTLocationStoreList(string strStorCode)
        {
            var entity = FirstOrDefault(e => e.StoreCode.Equals(strStorCode));
            var listCode = GetDisplayFiledsStoreList(entity);
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
        private static Dictionary<string, string> GetDisplayFiledsStoreList(StoreSTLocation entity)
        {
            var listCode = new Dictionary<string, string>();

            listCode.Add("DirectionalEffect", entity.DirectionalEffect);
            listCode.Add("PortfolioTypeName", entity.PortfolioTypeName);
            listCode.Add("DesignStyle", entity.DesignStyle);
            listCode.Add("ExteriorDesign", entity.ExteriorDesign);

            return listCode;
        }
        public void Save()
        {
            if (Id == Guid.Empty)
            {
                Add(this);
            }
            else
                Update(this);
        }

        public void Submit(bool isOverwrite = false)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    Mapper.CreateMap<StoreSTLocation, StoreSTLocationHistory>();
                    var history = Mapper.Map<StoreSTLocationHistory>(this);
                    var his = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == ProjectIdentifier);
                    if (his == null)
                    {
                        history.Id = Guid.NewGuid();
                        history.RefId = ProjectIdentifier;
                        history.CreatedTime = DateTime.Now;
                    }
                    else
                    {
                        history.Id = his.Id;
                        history.RefId = ProjectIdentifier;
                    }

                    history.Save();

                    if (isOverwrite)
                    {
                        var storeLocation = FirstOrDefault(e => e.StoreCode == StoreCode);
                        if (storeLocation != null)
                        {
                            this.Id = storeLocation.Id;
                            this.Save();
                            //storeLocation.Save();
                        }
                    }

                    if (EstimatedVsActualConstruction != null)
                    {
                        EstimatedVsActualConstruction.RefId = ProjectIdentifier;
                        EstimatedVsActualConstruction.Save();
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
