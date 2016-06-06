/*================================ge===========================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/7/2014 12:23:04 PM
 * FileName     :   ProjectTeam
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostProjectTeam
    {
        public ProjectUsers AssetRep { get; set; }

        public ProjectUsers AssetActor { get; set; }

        public ProjectUsers Finance { get; set; }

        public ProjectUsers PM { get; set; }

        public ProjectUsers Legal { get; set; }

        public ProjectUsers AssetMgr { get; set; }

        public ProjectUsers CM { get; set; }
    }
}
