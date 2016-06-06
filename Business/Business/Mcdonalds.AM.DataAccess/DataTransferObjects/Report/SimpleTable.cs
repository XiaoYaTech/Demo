/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/6/2015 6:08:10 PM
 * FileName     :   SimpleTable
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
    public class SimpleTable
    {
        public SimpleTable()
        {

        }

        public SimpleTable(RPTableSetting table)
        {

        }

        public string ID { get; set; }

        public string TableName { get; set; }
        public string DispForCN { get; set; }
        public string DispForEN { get; set; }
    }
}
