/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   2/6/2015 6:44:49 PM
 * FileName     :   CheckPoint
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class CheckPoint
    {
        public string NodeCode { get; set; }
        public string NameENUS { get; set; }
        public string NameZHCN { get; set; }
        public string NameDisp
        {
            get
            {
                if (ClientCookie.Language == Infrastructure.SystemLanguage.ENUS)
                    return NameENUS;
                else
                    return NameZHCN;
            }
        }
        public ProjectNodeStatus Status { get; set; }
        public int Sequence { get; set; }
    }
}
