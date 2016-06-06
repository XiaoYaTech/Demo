/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/21/2015 11:30:01 AM
 * FileName     :   ReportGridGetData
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
    public class ReportGridGetData
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class TemplateData
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public List<TemplateTable> Tables { get; set; }
    }
}
