/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/27/2014 4:07:30 PM
 * FileName     :   RenewalLegalApprovalDTO
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal
{
    public class RenewalLegalApprovalDTO
    {
        public RenewalInfo Info { get; set; }
        public RenewalLegalApproval Entity { get; set; }
        public string SN { get; set; }
        public string ProjectComment { get; set; }
        public bool Editable { get; set; }
        public bool Recallable { get; set; }
        public bool IsGeneralCounsel { get; set; }

        public bool Savable { get; set; }
    }
}
