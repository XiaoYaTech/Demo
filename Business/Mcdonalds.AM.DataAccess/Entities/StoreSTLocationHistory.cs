using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreSTLocationHistory : BaseEntity<StoreSTLocationHistory>
    {

        public void Save()
        {

            if (Any(e => e.Id == Id))
            {
                Update(this);
            }
            else
            {
                Add(this);
            }


        }
    }
}
