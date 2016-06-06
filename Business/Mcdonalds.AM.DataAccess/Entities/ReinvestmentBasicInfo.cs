using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReinvestmentBasicInfo : BaseEntity<ReinvestmentBasicInfo>
    {

        public string NewDesignTypeDisplay
        {
            get
            {
                return Dictionary.ParseDisplayName(NewDesignType);
            }
        }
        public void Save()
        {
            var oldInfo = FirstOrDefault(e => e.ConsInfoID == ConsInfoID);
            if (oldInfo != null)
            {
                this.Id = oldInfo.Id;
                this.Update();
            }
            else
            {
                Add(this);
            }
        }



        public static ReinvestmentBasicInfo GetByConsInfoId(Guid consInfoId)
        {
            return Search(e => e.ConsInfoID.Equals(consInfoId)).FirstOrDefault();
        }
    }
}
