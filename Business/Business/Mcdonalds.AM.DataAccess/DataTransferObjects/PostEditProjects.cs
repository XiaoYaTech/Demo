/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/23/2014 1:59:19 PM
 * FileName     :   PostEditProjects
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
    public class PostEditProjects
    {
        public string ProjectId { get; set; }
        public List<TopNavigator> EditProjects { get; set; }
    }
}
