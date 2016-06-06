using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class HoldingStatusChangeLog : BaseEntity<HoldingStatusChangeLog>
    {
        public void Save()
        {
            CreateUserAccount = ClientCookie.UserCode;
            if (Any(e => e.RefId == RefId))
            {
                UpdateTime = DateTime.Now;
                Update();
            }
            else
            {
                CreateTime = DateTime.Now;
                
                Add();
            }
        }
    }
}
