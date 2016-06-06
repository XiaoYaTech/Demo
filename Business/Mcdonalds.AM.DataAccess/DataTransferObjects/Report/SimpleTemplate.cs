/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/6/2015 5:07:39 PM
 * FileName     :   SimpleTemplate
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects.Report
{
    public class SimpleTemplate
    {
        public SimpleTemplate()
        {

        }
        public SimpleTemplate(RPTemplate template)
        {
            ID = template.ID;
            TName = template.TName;
            IsCommon = template.IsCommon.Value;
        }

        public int ID { get; set; }
        public string TName { get; set; }
        public bool IsCommon { get; set; }
    }
}
