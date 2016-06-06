using System;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common.Extensions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class WriteOffAmount : BaseEntity<WriteOffAmount>
    {

        public string WriteOffUser { get; set; }

        public DateTime? WriteOffTime { get; set; }
        //public decimal TotalVariance
        //{
        //    get
        //    {
        //        decimal totalVariance = 0;
        //        var totalBudget = TotalII.As<decimal>();
        //        var totalFinanceAct = TotalActual.As<decimal>();
        //        if (totalBudget != 0)
        //        {
        //            totalVariance = (totalFinanceAct - totalBudget) / totalBudget;
        //        }

        //        return totalVariance;
        //    }
        //}

        public void Save()
        {
            var oldWO = FirstOrDefault(e => e.ConsInfoID == ConsInfoID);
            if (oldWO!= null)
            {
                this.Id = oldWO.Id;
                this.Update();
            }
            else
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                }
                this.Add();
            }
        }
        public void SaveByRebuild()
        {
            if (!Any(w => w.Id == Id))
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                }
                Add(this);
            }
            else
            {
                Update(this);
            }
        }
        public static WriteOffAmount GetByConsInfoId(Guid consInfoId)
        {
            var wa = FirstOrDefault(e => e.ConsInfoID == consInfoId);
            var att = Attachment.FirstOrDefault(a => a.RefTableID == consInfoId.ToString() && a.TypeCode == "WriteOff");
            if (att != null && wa != null)
            {
                if (att.CreateTime.HasValue)
                {
                    wa.WriteOffTime = att.CreateTime.Value;
                }
                    
                wa.WriteOffUser = att.CreatorNameENUS;
            }
            return wa;
        }
    }
}
