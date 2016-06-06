using Mcdonalds.AM.DataAccess.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/6/2014 2:15:20 PM
 * FileName     :   ProjectTeamMember
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class ProjectTeamMember
    {
        public Guid? Id { get; set; }
        public string UserAccount { get; set; }
        public string UserNameZHCN { get; set; }
        public string UserNameENUS { get; set; }
        public string UserName
        {
            get
            {
                return I18N.GetValue(this, "UserName");
            }
        }

        public string RoleCode { get; set; }
        public string RoleNameZHCN { get; set; }
        public string RoleNameENUS { get; set; }
        public bool IsSelected { get; set; }
    }
}