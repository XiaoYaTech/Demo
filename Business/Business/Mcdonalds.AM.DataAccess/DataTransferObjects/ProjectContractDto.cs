/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/15/2014 5:52:47 PM
 * FileName     :   ProjectContractDto
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
    public class ProjectContractDto
    {
        public ProjectContractInfo Current { get; set; }
        public List<ProjectContractInfo> Histories { get; set; }
    }
}
