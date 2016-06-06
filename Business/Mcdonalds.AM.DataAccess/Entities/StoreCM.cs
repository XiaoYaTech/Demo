/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   12/10/2014 4:16:04 PM
 * FileName     :   StoreCM
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
    public partial class StoreCM : BaseEntity<StoreCM>
    {
        public static StoreCM Get(string usCode)
        {
            return Search(e => e.USCode == usCode).OrderByDescending(e => e.UpdateTime).FirstOrDefault();
        }
    }
}
