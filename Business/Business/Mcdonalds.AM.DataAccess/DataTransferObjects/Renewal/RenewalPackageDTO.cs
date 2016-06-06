/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/30/2014 3:22:15 PM
 * FileName     :   RenewalPackageDTO
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
    public class RenewalPackageDTO
    {
        public RenewalInfo Info { get; set; }
        public RenewalPackage Entity { get; set; }

        public RenewalToolFinMeasureOutput FinMeasureOutput { get; set; }

        public RenewalAnalysis Analysis { get; set; }

        public string SN { get; set; }

        public string ProjectComment { get; set; }

        public bool Editable { get; set; }
        public bool Recallable { get; set; }
        public bool Uploadable { get; set; }
        public bool Rejectable { get; set; }
        public bool IsLindaLu { get; set; }
        public bool Savable { get; set; }
    }
}
