using Mcdonalds.AM.DataAccess;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/1/2014 1:28:13 AM
 * FileName     :   PostClosureTool
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostClosureTool
    {
        public ClosureTool Entity { get; set; }
        public List<ClosureToolImpactOtherStore> ImpactStores { get; set; }
        public string yearMonth { get; set; }
    }
}