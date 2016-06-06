using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.Utility
{
    public class SystemLogToDB : Expose178.Com.IUtility.Ilog
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        public void WriteLog(System.Collections.Hashtable hshParam, string strPosition)
        {
            string strUID = "NULL";
            string strMsg="NULL";
            if (hshParam.ContainsKey("UID"))
                strUID = hshParam["UID"].ToString();
            if (hshParam.ContainsKey("Error"))
                strMsg = hshParam["Error"].ToString();
            ProcessDataAccess(strUID, strMsg, strPosition);
        }
        private static void ProcessDataAccess(string strUID, string strLogMsg, string strPosition)
        {
            string strDSN = Gadget.GetConnectionString("Expose178ComLog");
            Hashtable hshParama = new Hashtable();
            Gadget.Addparamater(ref hshParama, "SystemLogMsg", strLogMsg);
            Gadget.Addparamater(ref hshParama, "UpdatedByUserID", strUID);
            Gadget.Addparamater(ref hshParama, "SystemLogPosition", strPosition);
            Gadget.Addparamater(ref hshParama, "LastUpdatedDate", DateTime.Now.ToShortDateString());
            dbOperator.ProcessData("usp_AddSystemLog", hshParama, strDSN);
        }
    }
}
