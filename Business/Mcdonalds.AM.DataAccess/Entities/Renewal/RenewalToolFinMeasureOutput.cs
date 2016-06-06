/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/21/2014 5:20:39 PM
 * FileName     :   RenewalToolFinMeasureOutput
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
    public partial class RenewalToolFinMeasureOutput : BaseEntity<RenewalToolFinMeasureOutput>
    {
        public static RenewalToolFinMeasureOutput GetByToolId(Guid toolId)
        {
            return FirstOrDefault(e => e.ToolId == toolId);
        }
    }
}
