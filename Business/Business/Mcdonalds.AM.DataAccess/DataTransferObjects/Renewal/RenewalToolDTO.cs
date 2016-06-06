/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/16/2014 2:18:00 PM
 * FileName     :   RenewalToolDTO
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
    public class RenewalToolDTO
    {
        public RenewalInfo Info { get; set; }
        public RenewalTool Entity { get; set; }

        public RenewalToolFinMeasureInput FinMeasureInput { get; set; }

        public RenewalToolWriteOffAndReinCost WriteOffAndReinCost { get; set; }

        public List<string> TTMDataYearMonths { get; set; }

        public string SN { get; set; }

        public string ProjectComment { get; set; }

        public bool Editable { get; set; }
        public bool Recallable { get; set; }
        public bool Uploadable { get; set; }
        public bool Savable { get; set; }
        public bool ComSavable { get; set; }

        public bool IsFinished { get; set; }

    }
}
