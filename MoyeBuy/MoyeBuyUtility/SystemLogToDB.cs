using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MoyeBuy.Com.IUtility;
using MoyeBuy.Com.DALFactory;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class SystemLogToDB:Ilog
    {
        private static readonly MoyeBuy.Com.IDAL.IDBBaseOperator dbOperator = DataAcess.CreateDBBaseOperator();

        public void WriteLog(System.Collections.Hashtable hshParam, string strPosition)
        {
            string strUID = "NULL";
            string strMsg = "NULL";
            if (hshParam.ContainsKey("UID"))
                strUID = hshParam["UID"].ToString();
            if (hshParam.ContainsKey("Error"))
                strMsg = hshParam["strMsg"].ToString();
            ProcessDataAccess(strUID, strMsg, strPosition);
        }
        private static void ProcessDataAccess(string strUID, string strLogMsg, string strPosition)
        {
            string strDSN = Gadget.GetConnectionString("MoyeBuyComLog");
            Hashtable hshParama = new Hashtable();
            Gadget.Addparamater(ref hshParama, "SystemLogMsg", strLogMsg);
            Gadget.Addparamater(ref hshParama, "UpdatedByUserID", strUID);
            Gadget.Addparamater(ref hshParama, "SystemLogPosition", strPosition);
            Gadget.Addparamater(ref hshParama, "LastUpdatedDate", DateTime.Now.ToShortDateString());
            dbOperator.ProcessData("usp_AddSystemLog", hshParama, strDSN);
        }
    }
}
