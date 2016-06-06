using Mcdonalds.AM.DataAccess.DataTransferObjects.Report;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   1/21/2015 11:34:30 AM
 * FileName     :   ReportGenerator
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Report
{
    public class ReportGenerator
    {
        public static string BuildJoinByWorkflowTable(List<TemplateTable> Tables)
        {
            var workflowTable = Tables.FirstOrDefault(t => t.TableType == ReportTableType.Workflow && t.Checked);
            string fromClause = "";
            if (!string.IsNullOrEmpty(workflowTable.TableName))
            {
                fromClause = string.Format(@" Report_StoreBasicInfo INNER JOIN {0}
                        ON {0}.USCode = Report_StoreBasicInfo.USCode", workflowTable.TableName);
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreSiteInfo"))
                {
                    fromClause += " LEFT JOIN Report_StoreSiteInfo ON Report_StoreSiteInfo.USCode = Report_StoreBasicInfo.USCode ";
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreTAInfo"))
                {
                    fromClause += " LEFT JOIN Report_StoreTAInfo ON Report_StoreTAInfo.USCode = Report_StoreBasicInfo.USCode";
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreContractInfo"))
                {
                    fromClause += string.Format(@"  LEFT JOIN Report_StoreContractInfo 
                ON Report_StoreContractInfo.Id = {0}.ContractInfoId ", workflowTable.TableName);
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreFinanceData"))
                {
                    fromClause += string.Format(@" LEFT JOIN Report_StoreFinanceData
                        ON Report_StoreFinanceData.USCode = Report_StoreBasicInfo.USCode
                            AND Report_StoreFinanceData.FinanceYear = {0}.FinanceYear
                            AND Report_StoreFinanceData.FinanceMonth = {0}.FinanceMonth", workflowTable.TableName);
                }
            }
            else
            {
                fromClause = " Report_StoreBasicInfo";
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreSiteInfo"))
                {
                    fromClause += " LEFT JOIN Report_StoreSiteInfo ON Report_StoreSiteInfo.USCode = Report_StoreBasicInfo.USCode ";
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreTAInfo"))
                {
                    fromClause += " LEFT JOIN Report_StoreTAInfo ON Report_StoreTAInfo.USCode = Report_StoreBasicInfo.USCode";
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreContractInfo"))
                {
                    fromClause += @" LEFT JOIN Report_StoreContractInfo on Report_StoreContractInfo.ID in (
select top(1) ID from Report_StoreContractInfo where StoreCode=Report_StoreBasicInfo.USCode  
order by CreatedTime desc)";
                }
                if (Tables.Any(t => t.Checked && t.TableName == "Report_StoreFinanceData"))
                {
                    fromClause += @" left join Report_StoreFinanceData on Report_StoreFinanceData.USCode=Report_StoreBasicInfo.USCode 
and cast((Report_StoreFinanceData.FinanceYear+'-'+Report_StoreFinanceData.FinanceMonth+'-01') as datetime)
in (select Max(cast((FinanceYear+'-'+FinanceMonth+'-01') as datetime))
  from [Report_StoreFinanceData] f
  where f.USCode=Report_StoreBasicInfo.USCode)";
                }

            }
            return fromClause;
        }

        public static DataTable GetReportData(List<TemplateTable> tables, int pageIndex, int pageSize, out int totalItems)
        {
            var fields = new List<string>();
            var fieldsInnerQuery = new List<string>();
            var orderBies = new List<string>();
            var orderBiesInner = new List<string>();
            var where = new StringBuilder();
            where.Append("1=1");
            tables.ForEach(e =>
            {
                if (e.Checked)
                {
                    e.Fields.ForEach(f =>
                    {
                        if (f.Checked)
                        {
                            fields.Add(string.Format("[{0}__{1}]", e.TableName, f.FieldName));
                            fieldsInnerQuery.Add(string.Format("[{0}].[{1}] AS [{0}__{1}]", e.TableName, f.FieldName));
                            if (f.FieldName == "USCode")
                            {
                                orderBies.Add(string.Format("[{0}__{1}]", e.TableName, f.FieldName));
                                orderBiesInner.Add(string.Format("[{0}].[{1}]", e.TableName, f.FieldName));
                            }
                            if (!string.IsNullOrEmpty(f.ConditionText))
                            {
                                switch (f.ConditionType)
                                {
                                    case FieldConditionType.String:
                                        where.AppendFormat(" and [{0}].[{1}] like '%{2}%'", e.TableName, f.FieldName, f.ConditionText);
                                        break;
                                    case FieldConditionType.Boolean:
                                        if (f.ConditionText == "1")
                                            where.AppendFormat(" and ( cast([{0}].[{1}] as nvarchar) ='1' or cast([{0}].[{1}] as nvarchar)='Yes')", e.TableName, f.FieldName);
                                        else if (f.ConditionText == "0")
                                            where.AppendFormat(" and ( cast([{0}].[{1}] as nvarchar) ='0' or cast([{0}].[{1}] as nvarchar)='No')", e.TableName, f.FieldName);
                                        break;
                                    case FieldConditionType.Dictionary:
                                        where.AppendFormat(" and [{0}].[{1}] in ('{2}')", e.TableName, f.FieldName, f.ConditionText.Replace(",", "','"));
                                        break;
                                    case FieldConditionType.Date:
                                        var StartDate = "";
                                        try
                                        {
                                            StartDate = f.ConditionText.Split('|')[0].Split(':')[1];
                                        }
                                        catch { }
                                        var EndDate = "";
                                        try
                                        {
                                            EndDate = f.ConditionText.Split('|')[1].Split(':')[1];
                                        }
                                        catch { }
                                        if (StartDate != "" || EndDate != "")
                                        {
                                            string st = "1=1";
                                            if (StartDate != "")
                                            {
                                                StartDate = StartDate + " 00:00";
                                                st += string.Format(" and [{0}].[{1}] >='{2}'", e.TableName, f.FieldName, StartDate);
                                            }
                                            if (EndDate != "")
                                            {
                                                EndDate = EndDate + " 23:59";
                                                st += string.Format(" and [{0}].[{1}] <='{2}'", e.TableName, f.FieldName, EndDate);
                                            }
                                            where.AppendFormat(" and ({0})", st);
                                        }
                                        break;
                                    case FieldConditionType.Number:
                                    case FieldConditionType.Money:
                                    case FieldConditionType.Percent:
                                        var LessNum = "";
                                        try
                                        {
                                            LessNum = f.ConditionText.Split('|')[0].Split(':')[1];
                                        }
                                        catch { }
                                        var GreatNum = "";
                                        try
                                        {
                                            GreatNum = f.ConditionText.Split('|')[1].Split(':')[1];
                                        }
                                        catch { }
                                        if (LessNum != "" || GreatNum != "")
                                        {
                                            string st = "1=1 ";
                                            if (LessNum != "")
                                                st += string.Format("and (case when ISNUMERIC([{0}].[{1}])=1 then Cast([{0}].[{1}] as float) else null end ) >={2}", e.TableName, f.FieldName, LessNum);
                                            if (GreatNum != "")
                                                st += string.Format("and (case when ISNUMERIC([{0}].[{1}])=1 then Cast([{0}].[{1}] as float) else null end ) <={2}", e.TableName, f.FieldName, GreatNum);
                                            where.AppendFormat(" and ({0})", st);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    });
                }
            });
            var sql = new StringBuilder();
            sql.AppendFormat(" SELECT {0} FROM (", string.Join(",", fields.ToArray()));
            var sqlCount = new StringBuilder(" SELECT COUNT(1) FROM " + BuildJoinByWorkflowTable(tables) + " where " + where.ToString());
            var sqlInner = new StringBuilder();
            sqlInner.AppendFormat("SELECT {0},ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumAll FROM {2} where {3}",
                string.Join(",", fieldsInnerQuery.ToArray()), string.Join(",", orderBiesInner.ToArray()),
                BuildJoinByWorkflowTable(tables), where.ToString());
            sql.AppendFormat(" {0} ) AS temp WHERE RowNumAll >= {1} AND RowNumAll<{2} ORDER BY {3}",
                sqlInner,
                pageIndex * pageSize,
                (pageIndex + 1) * pageSize,
                string.Join(",", orderBies.ToArray()));
            DBHelper dbHelper = new DBHelper();
            var cmdSql = dbHelper.GetSqlStringCommond(sql.ToString());
            cmdSql.CommandTimeout = int.MaxValue;
            DataTable table = dbHelper.ExecuteDataTable(cmdSql);
            cmdSql.Dispose();
            var cmdCount = dbHelper.GetSqlStringCommond(sqlCount.ToString());
            cmdCount.CommandTimeout = int.MaxValue;
            totalItems = (int)dbHelper.ExecuteScalar(cmdCount);
            cmdCount.Dispose();
            return table;

        }

        public static DataTable GetAllReportData(List<TemplateTable> tables)
        {
            var fields = new List<string>();
            var fieldsInnerQuery = new List<string>();
            var orderBies = new List<string>();
            var orderBiesInner = new List<string>();
            var where = new StringBuilder();
            where.Append("1=1");
            tables.ForEach(e =>
            {
                if (e.Checked)
                {
                    e.Fields.ForEach(f =>
                    {
                        if (f.Checked)
                        {
                            fieldsInnerQuery.Add(string.Format("[{0}].[{1}] as [{0}_{1}]", e.TableName, f.FieldName));
                            if (f.FieldName == "USCode")
                            {
                                orderBiesInner.Add(string.Format("[{0}].[{1}]", e.TableName, f.FieldName));
                            }
                            if (!string.IsNullOrEmpty(f.ConditionText))
                            {
                                switch (f.ConditionType)
                                {
                                    case FieldConditionType.String:
                                        where.AppendFormat(" and [{0}].[{1}] like '%{2}%'", e.TableName, f.FieldName, f.ConditionText);
                                        break;
                                    case FieldConditionType.Dictionary:
                                        where.AppendFormat(" and [{0}].[{1}] in ('{2}')", e.TableName, f.FieldName, f.ConditionText.Replace(",", "','"));
                                        break;
                                    case FieldConditionType.Date:
                                        var StartDate = "";
                                        try
                                        {
                                            StartDate = f.ConditionText.Split('|')[0].Split(':')[1];
                                        }
                                        catch { }
                                        var EndDate = "";
                                        try
                                        {
                                            EndDate = f.ConditionText.Split('|')[1].Split(':')[1];
                                        }
                                        catch { }
                                        if (StartDate != "" && EndDate != "")
                                        {
                                            string st = "1=1";
                                            if (StartDate != "")
                                            {
                                                StartDate = StartDate + " 00:00";
                                                st += string.Format(" and [{0}].[{1}] >='{2}'", e.TableName, f.FieldName, StartDate);
                                            }
                                            if (EndDate != "")
                                            {
                                                EndDate = EndDate + " 23:59";
                                                st += string.Format(" and [{0}].[{1}] <='{2}'", e.TableName, f.FieldName, EndDate);
                                            }
                                            where.AppendFormat(" and ({0})", st);
                                        }
                                        break;
                                    case FieldConditionType.Number:
                                        var LessNum = "";
                                        try
                                        {
                                            LessNum = f.ConditionText.Split('|')[0].Split(':')[1];
                                        }
                                        catch { }
                                        var GreatNum = "";
                                        try
                                        {
                                            GreatNum = f.ConditionText.Split('|')[1].Split(':')[1];
                                        }
                                        catch { }
                                        if (LessNum != "" && GreatNum != "")
                                        {
                                            string st = "1=1 ";
                                            if (LessNum != "")
                                                st += string.Format("and [{0}].[{1}] >={2}", e.TableName, f.FieldName, LessNum);
                                            if (GreatNum != "")
                                                st += string.Format("and [{0}].[{1}] <={2}", e.TableName, f.FieldName, GreatNum);
                                            where.AppendFormat(" and ({0})", st);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    });
                }
            });
            var sqlInner = new StringBuilder();
            sqlInner.AppendFormat("SELECT {0},ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumAll FROM {2} where {3}",
                string.Join(",", fieldsInnerQuery.ToArray()), string.Join(",", orderBiesInner.ToArray()),
                BuildJoinByWorkflowTable(tables), where.ToString());
            DBHelper dbHelper = new DBHelper();
            var cmdSql = dbHelper.GetSqlStringCommond(sqlInner.ToString());
            cmdSql.CommandTimeout = int.MaxValue;
            DataTable table = dbHelper.ExecuteDataTable(cmdSql);
            cmdSql.Dispose();
            return table;

        }

        public static void GetFormData(DataTable dt, List<TemplateTable> list,
            out List<TemplateTable> lockedTables, out List<TemplateTable> unlockedTables,
            out List<string> lockedDatas, out List<string> unlockedDatas)
        {
            lockedTables = new List<TemplateTable>();
            unlockedTables = new List<TemplateTable>();
            lockedDatas = new List<string>();
            unlockedDatas = new List<string>();
            foreach (var tempTable in list)
            {
                if (tempTable.Checked)
                {
                    if (tempTable.Fields.Any(f => f.IsFieldLocked && f.Checked))
                    {
                        var lockedtable = new TemplateTable();
                        lockedtable.ID = tempTable.ID;
                        lockedtable.TableType = tempTable.TableType;
                        lockedtable.Checked = tempTable.Checked;
                        lockedtable.TableName = tempTable.TableName;
                        lockedtable.DispZHCN = tempTable.DispZHCN;
                        lockedtable.DispENUS = tempTable.DispENUS;
                        lockedtable.Fields = tempTable.Fields.Where(f => f.IsFieldLocked && f.Checked).ToList();
                        if (lockedtable.Fields.Count > 0)
                        {
                            lockedTables.Add(lockedtable);
                        }
                    }
                    var unlockedtable = new TemplateTable();
                    unlockedtable.ID = tempTable.ID;
                    unlockedtable.TableType = tempTable.TableType;
                    unlockedtable.Checked = tempTable.Checked;
                    unlockedtable.TableName = tempTable.TableName;
                    unlockedtable.DispZHCN = tempTable.DispZHCN;
                    unlockedtable.DispENUS = tempTable.DispENUS;
                    unlockedtable.Fields = tempTable.Fields.Where(f => !f.IsFieldLocked && f.Checked).ToList();
                    if (unlockedtable.Fields.Count > 0)
                    {
                        unlockedTables.Add(unlockedtable);
                    }
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                StringBuilder sbL = new StringBuilder();
                foreach (var table in lockedTables)
                {
                    foreach (var field in table.Fields)
                    {
                        string data = dr[table.TableName + "__" + field.FieldName].ToString();
                        data = DataTransfer(data, field.ConditionType);
                        sbL.AppendFormat("<td>{0}</td>", data);
                    }
                }
                lockedDatas.Add(sbL.ToString());
                StringBuilder sbU = new StringBuilder();
                foreach (var table in unlockedTables)
                {
                    foreach (var field in table.Fields)
                    {
                        string data = dr[table.TableName + "__" + field.FieldName].ToString();
                        data = DataTransfer(data, field.ConditionType);
                        sbU.AppendFormat("<td>{0}</td>", data);
                    }
                }
                unlockedDatas.Add(sbU.ToString());
            }
        }

        private static string DataTransfer(string data, FieldConditionType ConditionType)
        {
            if (ConditionType == FieldConditionType.Date)
            {
                if (data != "")
                {
                    DateTime date = DateTime.Parse("1900-01-01");
                    DateTime.TryParse(data, out date);
                    if (date.Year > 1990)
                        data = date.ToString("yyyy-MM-dd");
                    else
                        data = "";
                }
            }
            else if (ConditionType == FieldConditionType.Money || ConditionType == FieldConditionType.Number)
            {
                if (data != "")
                {
                    double m = 0;
                    double.TryParse(data, out m);
                    if (m != 0)
                        data = m.ToString("0,0.#");
                    else
                        data = "0";
                }
            }
            else if (ConditionType == FieldConditionType.Percent)
            {
                if (data != "")
                {
                    data = data.Replace("%", "");
                    double m = 0;
                    double.TryParse(data, out m);
                    if (m != 0)
                        data = m.ToString("P1");
                    else
                        data = "0%";
                }
            }
            else if (ConditionType == FieldConditionType.Boolean)
            {
                if (data != "")
                {
                    if (data.ToLower() == "true" || data == "1")
                        data = "Yes";
                    else
                        data = "No";
                }
            }
            else
            {
                if (data.ToLower() == "true")
                    data = "Yes";
                else if (data.ToLower() == "false")
                    data = "No";
            }
            return data;
        }
    }
}
