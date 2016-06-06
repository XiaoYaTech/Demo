using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class EstimatedVsActualConstruction : BaseEntity<EstimatedVsActualConstruction>
    {
        public void Save()
        {

            if (Any(e => e.Id == Id
                && e.RefId == RefId))
            {
                Update(this);
            }
            else
            {
                Add(this);
            }


        }

        public EstimatedVsActualConstruction GetEAC(Guid refId)
        {
            return FirstOrDefault(e => e.RefId == refId);
        }
    }
}
