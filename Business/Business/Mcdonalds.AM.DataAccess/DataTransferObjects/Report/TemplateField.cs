/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/8/2015 4:57:12 PM
 * FileName     :   TemplateField
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
    public class TemplateField
    {
        public int ID { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldDispZHCN { get; set; }
        public string FieldDispENUS { get; set; }
        public bool IsFieldLocked { get; set; }
        public bool Checked { get; set; }
        public FieldConditionType ConditionType { get; set; }
        public string ConditionText { get; set; }
        public bool IsOrderBy { get; set; }
        public bool IsDESC { get; set; }
    }
}
