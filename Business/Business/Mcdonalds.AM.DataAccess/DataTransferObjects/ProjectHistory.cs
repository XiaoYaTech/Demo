using System;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
   public class ProjectHistory
    {
       public string ItemName 
       { get; set; }

       public DateTime CreateTime { get; set; }

       public string CreatorNameZHCN { get; set; }
       public string CreatorNameENUS { get; set; }

       public string ReceiverNameZHCN { get; set; }
       public string ReceiverNameENUS { get; set; }
       public DateTime FinishTime { get; set; }
    }
}
