/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/8/2015 11:59:59 AM
 * FileName     :   TemplateDetail
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
    public class TemplateDetail
    {
        public int TemplateID { get; set; }
        public int TableID { get; set; }
        public ReportTableType TableType { get; set; }
        public bool TableChecked { get; set; }
        public string TableName { get; set; }
        public string TableDispZHCN { get; set; }
        public string TableDispENUS { get; set; }
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public string FieldDispZHCN { get; set; }
        public string FieldDispENUS { get; set; }
        public FieldConditionType ConditionType { get; set; }
        public string ConditionText { get; set; }
        public bool IsFieldLocked { get; set; }
        public bool Checked { get; set; }
        public bool IsOrderBy { get; set; }
        public bool IsDESC { get; set; }
    }
}
