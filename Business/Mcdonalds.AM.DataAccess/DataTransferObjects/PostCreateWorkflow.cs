/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/24/2014 11:08:14 AM
 * FileName     :   PostCreateWorkflow
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
    public class PostCreateWorkflow<T> where T : BaseAbstractEntity
    {
        public T Entity { get; set; }
        public PostProjectTeam Team { get; set; }
        public ProjectUsers AssetMgr { get; set; }
        //public ProjectUsers CM { get; set; }
        public List<ProjectUsers> NecessaryViewers { get; set; }
        public List<ProjectUsers> Viewers { get; set; }
    }
}
