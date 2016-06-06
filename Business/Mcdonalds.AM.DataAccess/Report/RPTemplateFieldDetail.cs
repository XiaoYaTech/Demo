using Mcdonalds.AM.DataAccess.DataTransferObjects.Report;
using Mcdonalds.AM.DataAccess.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RPTemplateFieldDetail : BaseEntity<RPTemplateFieldDetail>
    {
        public static List<TemplateTable> Get(int templateId)
        {
            var result = new List<TemplateTable>();
            var db = PrepareDb();
            var sql = @"select isnull(rf.TemplateID,0) as TemplateID, 
t.ID AS TableID,
t.TableType AS TableType,
t.TableName AS TableName,
t.DispZHCN AS TableDispZHCN,
t.DispENUS AS TableDispENUS,
CAST(CASE WHEN rt.ID IS NULL THEN 0 ELSE 1 END AS BIT) AS TableChecked,
isnull(f.ID,0) AS FieldID,
f.FieldName AS FieldName,
f.DispZHCN AS FieldDispZHCN,
f.DispENUS AS FieldDispENUS,
CAST(CASE WHEN rf.ID IS NULL THEN 0 ELSE 1 END AS BIT) AS Checked,
isnull(f.ConditionType,0) AS ConditionType,
rf.ConditionText AS ConditionText,
isnull(f.IsLocked,0) AS IsFieldLocked,
isnull(rf.IsOrderBy,0) AS IsOrderBy,
isnull(rf.IsDESC,0) AS IsDESC
from RPTableSetting t 
left join RPFieldSetting f on f.TableID = t.ID 
left join RPTemplateTableDetail rt ON rt.TableID = t.ID and  rt.TemplateID = @TemplateID 
left join RPTemplateFieldDetail rf ON rf.FieldID = f.ID and  rf.TemplateID = @TemplateID  
order by t.OrderBy
";
            return fillTable(SqlQuery<TemplateDetail>(sql, new
            {
                TemplateID = templateId
            }).ToList());
        }

        private static List<TemplateTable> fillTable(IEnumerable<TemplateDetail> details)
        {
            var tables = details.Select(e => new
            {
                ID = e.TableID,
                TableType = e.TableType,
                Checked = e.TableChecked,
                Name = e.TableName,
                DispZHCN = e.TableDispZHCN,
                DispENUS = e.TableDispENUS
            }).Distinct().Select(e =>
            {
                var table = new TemplateTable();
                table.ID = e.ID;
                table.TableType = e.TableType;
                table.TableName = e.Name;
                table.DispZHCN = e.DispZHCN;
                table.DispENUS = e.DispENUS;
                table.Checked = e.Checked;
                table.Fields = details.Where(item => item.TableName == e.Name).Select(item => new TemplateField
                {
                    ID = item.FieldID,
                    FieldName = item.FieldName,
                    FieldDispENUS = item.FieldDispENUS,
                    FieldDispZHCN = item.FieldDispZHCN,
                    IsFieldLocked = item.IsFieldLocked,
                    ConditionType = item.ConditionType,
                    ConditionText = item.ConditionText,
                    Checked = item.Checked,
                    IsOrderBy = item.IsOrderBy,
                    IsDESC = item.IsDESC
                }).ToList();
                return table;
            }).ToList() ?? new List<TemplateTable>();
            return tables;
        }
    }
}
