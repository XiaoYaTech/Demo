using Mcdonalds.AM.DataAccess;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/27/2014 12:35:19 PM
 * FileName     :   PostContractModel
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostContractModel
    {
        public ProjectContractInfo Contract { get; set; }
        public List<ProjectContractRevision> Revisions { get; set; }
        public string FlowCode { get; set; }
    }
}