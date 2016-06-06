using Mcdonalds.AM.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess.Report
{
    public class RPHelp
    {
        public static string CreateBaseSQL()
        {
            var tbs = RPTableSetting.GetTables();

            return "";
        }

        //public static string CreateJoinSQL(List<RPTableSetting> tbs)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.Append(" from " + tbs[0].TableName + " as " + tbs[0].OtherName);
        //    for (int i = 0; i < tbs.Count() - 1; i++)
        //    {
        //        sql.AppendLine(" left join " + tbs[i + 1].TableName + " as " + tbs[i + 1].OtherName);
        //        sql.Append(" on " + tbs[i].OtherName + "." + tbs[i].MainKey + " = " + tbs[i + 1].OtherName + "." + tbs[i + 1].OtherName);
        //    }
        //    return sql.ToString();
        //}

        public static string CreateFiledsSQL(List<RPTableSetting> tbs, SystemLanguage language)
        {
            StringBuilder sql = new StringBuilder();
            var list = tbs.OrderBy(p => p.OrderBy);
            foreach (var tb in list)
            {
                var fields = tb.Fields.OrderBy(p => p.OrderBy);
                foreach (var field in fields)
                {
                    switch (language)
                    {
                        case SystemLanguage.UNKNOWN:
                        case SystemLanguage.ZHCN:
                            sql.Append(tb.OtherName + "." + field.FieldName + " as " + field.OtherZHCN + ",");
                            break;
                        case SystemLanguage.ENUS:
                            sql.Append(tb.OtherName + "." + field.FieldName + " as " + field.OtherENUS + ",");
                            break;
                        default:
                            break;
                    }

                }
            }

            return sql.ToString().TrimEnd(',');
        }

        public static List<string> GetStringFilter(string tableName, string fieldName)
        {
            DBHelper db = new DBHelper();
            var result = new List<string>();
            var sql = string.Format("SELECT DISTINCT {0} FROM {1} where {0} is not null",fieldName,tableName);
            var dt = db.ExecuteDataTable(db.GetSqlStringCommond(sql));
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr[0].ToString());
            }
            return result;
        }

        public static void InitBaseAccess()
        {
            DBHelper db = new DBHelper();
            var tbs = RPTableSetting.GetTables();

            //清空Fields表
            DeleteField();

            int num = 0;
            foreach (var tb in tbs)
            {
                var sql = string.Format("select name from syscolumns WHERE (id = OBJECT_ID('{0}'))", tb.TableName);
                var cmd = db.GetSqlStringCommond(sql);
                DataTable fields = db.ExecuteDataTable(cmd);
                if (fields != null && fields.Rows.Count > 0)
                {
                    var data = new List<RPFieldSetting>();
                    for (int i = 0; i < fields.Rows.Count; i++)
                    {
                        var fieldName = fields.Rows[i][0].ToString();
                        var model = new RPFieldSetting();
                        model.TableID = tb.ID;
                        model.FieldName = fieldName;
                        model.OtherZHCN = fieldName;
                        model.OtherENUS = fieldName;
                        model.DispZHCN = fieldName;
                        model.DispENUS = fieldName;
                        model.OrderBy = i;
                        model.GlobalBy = num;
                        model.ConditionType = FieldConditionType.None;
                        data.Add(model);
                        num++;
                    }
                    using (var scope = new TransactionScope())
                    {
                        RPFieldSetting.Add(data.ToArray());
                        scope.Complete();
                    }

                }

            }
        }
        private static void DeleteField()
        {
            DBHelper db = new DBHelper();
            var sql = " truncate table [RPFieldSetting] ";
            var cmd = db.GetSqlStringCommond(sql);
            db.ExecuteNonQuery(cmd);
        }

        public static DataTable GetTable()
        {
            DBHelper db = new DBHelper();
            var sql = " select * from [V_AM_Report_StoreBasicInfo] ";
            var cmd = db.GetSqlStringCommond(sql);
            return db.ExecuteDataTable(cmd);
        }

        public static string DataTableTOExcel(DataTable dt)
        {
            var sbHtml = new System.Text.StringBuilder();

            sbHtml.Append("<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\"><head>" +
                            @"<!--[if gte mso 9]>
                        <xml><x:ExcelWorkbook>
                            <x:ExcelWorksheets>
                                <x:ExcelWorksheet>     
                                    <x:Name></x:Name>    
                                    <x:WorksheetOptions>
                                        <x:Selected/> 
                                    </x:WorksheetOptions>                               
                                </x:ExcelWorksheet>  
                            </x:ExcelWorksheets>
                        </x:ExcelWorkbook> </xml><![endif]--></head><body>");
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");

            foreach (DataColumn item in dt.Columns)
            {
                sbHtml.AppendFormat("<th style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</th>", item.Caption);
            }
            sbHtml.Append("</tr>");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sbHtml.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var value = dt.Rows[i][j];

                    if (value == null || System.DBNull.Value == value)
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'></td>");
                    }
                    else if (dt.Columns[j].DataType.FullName == typeof(DateTime).FullName)
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", Convert.ToDateTime(value).ToString("yyyy-MM-dd hh:mm"));
                    }
                    else
                    {
                        sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", value.ToString());
                    }
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table></body></html>");
            return sbHtml.ToString();
            //第一种:使用FileContentResult
            //byte[] fileContents = System.Text.Encoding.Default.GetBytes(sbHtml.ToString());

            //return File(fileContents, "application/ms-excel", "fileContents.xls");
        }
    }


}
