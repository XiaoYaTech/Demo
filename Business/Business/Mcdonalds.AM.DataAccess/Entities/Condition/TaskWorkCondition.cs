using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Entities.Condition
{
   public class TaskWorkCondition
    {

       public string ProjectId { get; set; }

       /// <summary>
       /// 任务的URL
       /// </summary>
       public string Url { get; set; }

       public string UserAccount { get; set; }

       public string UserNameZHCN { get; set; }

       public string UserNameENUS { get; set; }
    }
}
