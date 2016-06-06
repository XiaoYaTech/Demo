/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   11/21/2014 4:01:01 PM
 * FileName     :   SelectItem
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
    public class SelectItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }

        public int IntValue { get; set; }
    }
}
