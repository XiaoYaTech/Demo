using Mcdonalds.AM.DataAccess.Workflow;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/26/2014 4:24:29 PM
 * FileName     :   PostWorkflowData
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
    public class PostWorkflowData<T> where
        T : BaseAbstractEntity
    {
        public T Entity { get; set; }
        public string SN { get; set; }
        public string ProjectComment { get; set; }
        public ProjectDto ProjectDto { get; set; }
    }
}
