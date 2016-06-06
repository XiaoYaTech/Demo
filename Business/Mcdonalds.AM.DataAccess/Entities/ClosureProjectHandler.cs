using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Entities
{
   public class ClosureProjectHandler
    {
       public bool EnablePackage()
       {
           return false;
       }

       public int SendPackageTask(string projectId)
       {
           return 0;
       }

       public bool EnableExecutiveSummary(string projectId)
       {
           return false;
       }
    }
}
