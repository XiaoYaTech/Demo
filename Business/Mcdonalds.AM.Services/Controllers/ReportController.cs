using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Report;
using Mcdonalds.AM.DataAccess.Report;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ReportController : ApiController
    {
        [Route("api/report/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage()
        {
            return Ok(new
            {
                Templates = RPTemplate.GetByEID().Select(e => new SimpleTemplate(e)).ToList()
            });
        }

        [Route("api/report/getTemplateDetails")]
        [HttpGet]
        public IHttpActionResult GetTemplateDetails(int templateID)
        {
            List<TemplateTable> list = RPTemplateFieldDetail.Get(templateID);
            var tablebasicinfo = list.FirstOrDefault(t => t.DispENUS == "Basic Info");
            tablebasicinfo.Checked = true;
            tablebasicinfo.Fields.Where(f => f.IsFieldLocked).ToList().ForEach(f =>
            {
                f.Checked = true;
            });
            HttpContext.Current.Session["TemplateDetail"] = list;
            return Ok(list);
        }

        [Route("api/report/table/field/get")]
        [HttpGet]
        public IHttpActionResult GetTableFields(int tableId)
        {
            return Ok(RPFieldSetting.GetFieldsByTableID(tableId));
        }

        [Route("api/report/field/stringfilter")]
        [HttpGet]
        public IHttpActionResult GetTableFields(string tableName, string fieldName)
        {
            return Ok(RPHelp.GetStringFilter(tableName, fieldName));
        }

        [Route("api/report/UpdateCondition")]
        [HttpGet]
        public IHttpActionResult UpdateCondition(int FieldId, string ConditionText, int PageSize)
        {
            int totalItems = 0;
            List<TemplateTable> list = HttpContext.Current.Session["TemplateDetail"] as List<TemplateTable>;
            foreach (var table in list)
            {
                if (table.Fields.Any(f => f.ID == FieldId))
                {
                    var field = table.Fields.First(f => f.ID == FieldId);
                    field.ConditionText = ConditionText;
                    break;
                }
            }
            var tablebasicinfo = list.FirstOrDefault(t => t.DispENUS == "Basic Info");
            tablebasicinfo.Checked = true;
            tablebasicinfo.Fields.Where(f => f.IsFieldLocked).ToList().ForEach(f =>
            {
                f.Checked = true;
            });
            HttpContext.Current.Session["TemplateDetail"] = list;
            List<TemplateTable> lockedTables = new List<TemplateTable>();
            List<TemplateTable> unlockedTables = new List<TemplateTable>();
            List<string> lockedDatas = new List<string>();
            List<string> unlockedDatas = new List<string>();
            DataTable dt = ReportGenerator.GetReportData(list, 0, PageSize, out totalItems);
            ReportGenerator.GetFormData(dt, list, out lockedTables, out unlockedTables, out lockedDatas, out unlockedDatas);
            return Ok(new
            {
                LockedTables = lockedTables,
                UnLockedTables = unlockedTables,
                LockedDatas = lockedDatas,
                UnLockedDatas = unlockedDatas,
                TotalItems = totalItems
            });
        }

        [Route("api/report/getData")]
        [HttpPost]
        public IHttpActionResult GetData(ReportGridGetData data)
        {
            int totalItems = 0;
            List<TemplateTable> list = HttpContext.Current.Session["TemplateDetail"] as List<TemplateTable>;
            //List<TemplateTable> olist = HttpContext.Current.Session["TemplateDetail"] as List<TemplateTable>;
            //更新ConditionText数据
            //foreach (var table in list)
            //{
            //    var ot = olist.FirstOrDefault(t => t.ID == table.ID);
            //    foreach (var field in table.Fields)
            //    {
            //        var of = ot.Fields.FirstOrDefault(f => f.ID == field.ID);
            //        field.ConditionText = of.ConditionText;
            //    }
            //}
            //HttpContext.Current.Session["TemplateDetail"] = list;
            List<TemplateTable> lockedTables = new List<TemplateTable>();
            List<TemplateTable> unlockedTables = new List<TemplateTable>();
            List<string> lockedDatas = new List<string>();
            List<string> unlockedDatas = new List<string>();
            DataTable dt = ReportGenerator.GetReportData(list, data.PageIndex, data.PageSize, out totalItems);
            ReportGenerator.GetFormData(dt, list, out lockedTables, out unlockedTables, out lockedDatas, out unlockedDatas);
            return Ok(new
            {
                LockedTables = lockedTables,
                UnLockedTables = unlockedTables,
                LockedDatas = lockedDatas,
                UnLockedDatas = unlockedDatas,
                TotalItems = totalItems
            });
        }

        [Route("api/report/ExportData")]
        [HttpPost]
        public IHttpActionResult ExportData()
        {
            var current = HttpContext.Current;
            List<TemplateTable> Tables = current.Session["TemplateDetail"] as List<TemplateTable>;
            DataTable dt = ReportGenerator.GetAllReportData(Tables);
            string tableBegin = @"<meta http-equiv=Content-Type content=text/html;charset=utf-8 />
                <table cellspacing='1' cellpadding='3' rules='all' align='center' border='1'>		";
            StringBuilder sbHeader1 = new StringBuilder();
            sbHeader1.AppendFormat("<tr style='font-weight:bold; font-size:13; color: #000000; text-align: center; height: 30px'>");
            StringBuilder sbHeader2 = new StringBuilder();
            sbHeader2.AppendFormat("<tr style='font-weight:bold; font-size:13; color: #000000; text-align: center; height: 30px'>");
            StringBuilder sbBody = new StringBuilder();
            foreach (TemplateTable table in Tables)
            {
                if (table.Checked)
                {
                    var CheckedFieldCount = table.Fields.Count(f => f.Checked);
                    if (CheckedFieldCount > 0)
                    {
                        sbHeader1.AppendFormat(" <td colspan='{0}'>{1}</td>", CheckedFieldCount, table.DispENUS);
                        var CheckedFieldList = table.Fields.Where(f => f.Checked).ToList();
                        foreach (var CheckedField in CheckedFieldList)
                        {
                            sbHeader2.AppendFormat(" <td>{0}</td>", CheckedField.FieldDispENUS);
                        }
                    }
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                sbBody.Append("<tr style='text-align: center; font-size:13; height: 28'>");
                foreach (TemplateTable table in Tables)
                {
                    if (table.Checked)
                    {
                        var CheckedFieldList = table.Fields.Where(f => f.Checked).ToList();
                        if (CheckedFieldList.Count > 0)
                        {
                            foreach (var CheckedField in CheckedFieldList)
                            {
                                string data = dr[table.TableName + "_" + CheckedField.FieldName].ToString();
                                if (CheckedField.ConditionType == FieldConditionType.Date)
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
                                    sbBody.AppendFormat("<td>{0}</td>", data);
                                }
                                else if (CheckedField.ConditionType == FieldConditionType.Money ||
                                    CheckedField.ConditionType == FieldConditionType.Number)
                                {
                                    if (data != "")
                                    {
                                        double m = 0;
                                        double.TryParse(data, out m);
                                        if (m != 0)
                                            data = m.ToString("0.#");
                                        else
                                            data = "0";
                                    }
                                    sbBody.AppendFormat("<td>{0}</td>", data);
                                }
                                else if (CheckedField.ConditionType == FieldConditionType.Boolean)
                                {
                                    if (data != "")
                                    {
                                        if (data.ToLower() == "true" || data == "1")
                                            data = "Yes";
                                        else
                                            data = "No";
                                    }
                                    sbBody.AppendFormat("<td>{0}</td>", data);
                                }
                                else
                                {
                                    if (data.ToLower() == "true")
                                        data = "Yes";
                                    else if (data.ToLower() == "false")
                                        data = "No";
                                    sbBody.AppendFormat("<td>{0}</td>", data);
                                }
                            }
                        }
                    }
                }
                sbBody.Append("</tr>");
            }
            sbHeader1.Append("</tr>");
            sbHeader2.Append("</tr>");
            string tableEnd = @"</table>";
            string Table = tableBegin + sbHeader1.ToString() + sbHeader2.ToString() + sbBody.ToString() + tableEnd;
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + "Report_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            current.Response.ContentType = "application/octet-stream";
            current.Response.Write(Table);
            current.Response.End();
            return Ok();
        }

        [Route("api/report/UpdateSearch")]
        [HttpPost]
        public IHttpActionResult UpdateSearch(TemplateData data)
        {
            var list = data.Tables;
            var tablebasicinfo = list.FirstOrDefault(t => t.DispENUS == "Basic Info");
            tablebasicinfo.Checked = true;
            tablebasicinfo.Fields.Where(f => f.IsFieldLocked).ToList().ForEach(f =>
            {
                f.Checked = true;
            });
            HttpContext.Current.Session["TemplateDetail"] = list;
            return Ok(new { status = true });
        }

        [Route("api/report/SaveTemplate")]
        [HttpPost]
        public IHttpActionResult SaveTemplate(TemplateData data)
        {
            try
            {
                if (RPTemplate.Any(r => r.TName == data.TemplateName && r.CreateBy == ClientCookie.UserCode))
                {
                    return Ok(new { status = false, message = "已存在相同名字的模板" });
                }
                List<TemplateTable> list = data.Tables;
                List<TemplateTable> olist = HttpContext.Current.Session["TemplateDetail"] as List<TemplateTable>;
                //更新ConditionText数据
                foreach (var table in list)
                {
                    var ot = olist.FirstOrDefault(t => t.ID == table.ID);
                    foreach (var field in table.Fields)
                    {
                        var of = ot.Fields.FirstOrDefault(f => f.ID == field.ID);
                        field.ConditionText = of.ConditionText;
                    }
                }
                var tablebasicinfo = list.FirstOrDefault(t => t.DispENUS == "Basic Info");
                tablebasicinfo.Checked = true;
                tablebasicinfo.Fields.Where(f => f.IsFieldLocked).ToList().ForEach(f =>
                {
                    f.Checked = true;
                });
                HttpContext.Current.Session["TemplateDetail"] = list;
                if (data.TemplateId == 0)
                {
                    RPTemplate.SaveAs(data.TemplateName, data.Tables);
                }
                else
                {
                    RPTemplate.Save(data.TemplateId, data.Tables);
                }
                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }

        [Route("api/report/RemoveTemplate/{TemplateId}")]
        [HttpPost]
        public IHttpActionResult RemoveTemplate(int TemplateId)
        {
            try
            {
                RPTemplate.Delete(TemplateId); ;
                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }
    }
}
