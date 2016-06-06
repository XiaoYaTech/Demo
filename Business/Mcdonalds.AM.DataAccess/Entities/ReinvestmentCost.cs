using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReinvestmentCost : BaseEntity<ReinvestmentCost>
    {
        public string ReinCostUser { get; set; }

        public DateTime? ReinCostTime { get; set; }

        public void Save()
        {
            var oldReinCost = FirstOrDefault(c => c.ConsInfoID == ConsInfoID);
            if (oldReinCost!= null)
            {
                Id = oldReinCost.Id;
                Update(this);
            }
            else
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                }
                Add(this);
            }
        }
        public void SaveByRebuild()
        {
            if (!Any(c => c.Id == Id))
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
        public static ReinvestmentCost GetByConsInfoId(Guid consInfoId)
        {
            var ci = FirstOrDefault(e => e.ConsInfoID == consInfoId);
            var att = Attachment.FirstOrDefault(a => a.RefTableID == consInfoId.ToString() && a.TypeCode == "ReinCost");
            if (att != null && ci != null)
            {
                if (att.CreateTime.HasValue)
                {
                    ci.ReinCostTime = att.CreateTime.Value;
                }
                
                ci.ReinCostUser = att.CreatorNameENUS;
            }
            return ci;
        }
    }
}
