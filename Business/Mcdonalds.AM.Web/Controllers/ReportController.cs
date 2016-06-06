using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Form()
        {
            //RPHelp.InitBaseAccess();
            return View();
        }

        public ActionResult ReportData() {
            return View();
        }

        public JsonResult GetReportData()
        {
            return null;
        }

        public JsonResult SaveTemplate()
        {
            return null;
        }

        public JsonResult SaveAsTemplate()
        {
            return null;
        }

        public FileResult ExportExcel()
        {
            var dt = RPHelp.GetTable();
            var excel = RPHelp.DataTableTOExcel(dt);
            byte[] fileContents = System.Text.Encoding.Default.GetBytes(excel);
            return File(fileContents, "application/ms-excel", "AM_RP_Excel.xls");
        }

        public string InitData()
        {
            RPHelp.InitBaseAccess();
            return "数据初始化成功！";
        }
    }
}
