/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/11/2014 3:04:46 PM
 * FileName     :   IWorkflowEntity
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Workflow
{
    public interface IWorkflowEntity
    {
        string ProjectId { get; set; }
        string CreateUserAccount { get; set; }
        Nullable<int> ProcInstId { get; set; }

        string Edit();
    }
}
