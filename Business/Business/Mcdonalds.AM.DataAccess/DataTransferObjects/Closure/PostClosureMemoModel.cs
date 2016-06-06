using Mcdonalds.AM.DataAccess;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/27/2014 3:43:56 PM
 * FileName     :   PostClosureMemoModel
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostClosureMemoModel
    {
        public ClosureMemo Entity { get; set; }
        public List<Employee> Receivers { get; set; }
    }
}