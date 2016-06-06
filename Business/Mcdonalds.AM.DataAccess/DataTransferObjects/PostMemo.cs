/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/14/2014 10:21:27 AM
 * FileName     :   PostMemo
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
    public class PostMemo<T> where T:BaseAbstractEntity
    {
        public T Entity { get; set; }
        public List<Employee> Receivers { get; set; }
    }
}
