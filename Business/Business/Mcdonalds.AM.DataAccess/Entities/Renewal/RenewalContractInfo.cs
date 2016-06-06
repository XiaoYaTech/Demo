/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:29:42 PM
 * FileName     :   RenewalContractInfo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalContractInfo
    {
        public static void Create(string projectId)
        {
            var projectContractInfo = ProjectContractInfo.GetContractWithHistory(projectId).Current;
            projectContractInfo.Add();
        }
    }
}
