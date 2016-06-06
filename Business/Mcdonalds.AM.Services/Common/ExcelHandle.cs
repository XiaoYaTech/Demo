using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using MyExcel.NPOI;

namespace Mcdonalds.AM.Services.Common
{
    public class ExcelHandle
    {

        /// <summary>    
        /// 把数据从Excel装载到DataTable    
        /// </summary>    
        /// <param name="pathName">带路径的Excel文件名</param>    
        /// <param name="sheetName">工作表名</param>
        /// <param name="tbContainer">将数据存入的DataTable</param>
        /// <returns></returns> 
        public static DataTable ExcelToDataTable(string pathName, string sheetName)
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = "Sheet1";
            }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists)
            {
                throw new Exception("文件不存在");
            }
            string extension = file.Extension;
            //switch (extension)
            //{
            //    case ".xls":
            //        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
            //        break;
            //    case ".xlsx":
            //        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
            //        break;
            //    default:
            //        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
            //        break;
            //}
            strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + @";Extended Properties=Excel 8.0;";
            //链接Excel
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            //读取Excel里面有 表Sheet1
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            //将Excel里面有表内容装载到内存表中！
            oda.Fill(tbContainer);
            return tbContainer;
        }

        public static DataTable ReadExcel(string excelPath, string sheet)
        {
            //读Excel：
            OleDbConnection ConnectExcel = new OleDbConnection();
            try
            {
                ConnectExcel.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelPath +
                                                @";Extended Properties=Excel 8.0;";
                ConnectExcel.Open();

                string selectCmdStr = string.Format(@"select * from [{0}$]", sheet);
                OleDbCommand selectCmd = new OleDbCommand(selectCmdStr, ConnectExcel);
                OleDbDataReader ExcelDR = selectCmd.ExecuteReader();
                int rowIndex = 1;
                while (ExcelDR.Read()) //依次读行
                {
                    rowIndex++;

                    if (string.IsNullOrEmpty(ExcelDR.GetValue(6).ToString())) //6代表第6列
                    {
                        //TODO:   
                    }
                }
            }
            catch (Exception ex)
            {
                //...
            }
            finally
            {
                ConnectExcel.Close();
            }
            return null;
        }
    


    //    Excel excel = new Excel();
    //        string templatePath = Server.MapPath(@"~\Template\ReportExportExcel\公务卡申请报表模板.xls");
    //        string excelPath = Server.MapPath(@"~\Template\ReportExportExcel\公务卡申请报表_生成文件.xls");
    //        excel.Open(templatePath);

    //        int i = 1;
    //        foreach(var item in data)
    //        {
    //            //excel.Sheets["公务卡申请"].Cells[i, 0].StrValue = item.CardNumber;
    //            excel.Sheets["公务卡申请"].Cells[i, 0].StrValue = item.CardType;
    //            excel.Sheets["公务卡申请"].Cells[i, 1].StrValue = item.SerialNumber;
    //            excel.Sheets["公务卡申请"].Cells[i, 2].StrValue = item.ExpenseUser;
    //            excel.Sheets["公务卡申请"].Cells[i, 3].StrValue = item.Company;
    //            excel.Sheets["公务卡申请"].Cells[i, 4].StrValue = item.Dept;
    //            excel.Sheets["公务卡申请"].Cells[i, 5].StrValue = item.ExpenseStatus;
    //            excel.Sheets["公务卡申请"].Cells[i, 6].StrValue = item.SubmitTime;
    //            excel.Sheets["公务卡申请"].Cells[i, 7].StrValue = item.LeaderAuditor;
    //            excel.Sheets["公务卡申请"].Cells[i, 8].StrValue = item.FinancialAuditor;
    //            i++;
    //        }
    //        excel.Save(excelPath);
    }
}