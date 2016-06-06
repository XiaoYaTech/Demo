/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/8/2015 4:56:47 PM
 * FileName     :   TemplateTable
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
    public class TemplateTable
    {
        public int ID { get; set; }
        public ReportTableType TableType { get; set; }
        public bool Checked { get; set; }
        public string TableName { get; set; }
        public string DispZHCN { get; set; }
        public string DispENUS { get; set; }
        public List<TemplateField> Fields { get; set; }
    }
}
