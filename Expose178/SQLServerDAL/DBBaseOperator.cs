using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Expose178.Com.IDAL;

namespace Expose178.Com.SQLServerDAL
{
    public class DBBaseOperator : IDBBaseOperator
    {
        public DataSet ProcessData(string strComandText, System.Collections.Hashtable hshParamater, string strDSN)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataAdapter adpt = null;
            DataSet ds = null;
            string strTemCmd = "";
            try
            {
                StringBuilder sbCmd = new StringBuilder("");
                foreach (string strKey in hshParamater.Keys)
                {
                    sbCmd.Append("@" + strKey);
                    sbCmd.Append("=");
                    sbCmd.Append(hshParamater[strKey].ToString());
                    sbCmd.Append(",");
                }
                strTemCmd = sbCmd.ToString();
                if (strTemCmd.Length > 1)
                {
                    strTemCmd = strTemCmd.Substring(0, strTemCmd.Length - 1);
                }
                strTemCmd = "EXEC " + strComandText + " " + strTemCmd;

                ds = new DataSet();
                con = new SqlConnection(strDSN);
                cmd = new SqlCommand(strTemCmd, con);
                cmd.CommandType = System.Data.CommandType.Text;
                adpt = new SqlDataAdapter(cmd);
                adpt.Fill(ds);
            }
            catch (SqlException ex)
            {
                hshParamater.Add("DSN", strDSN);
                hshParamater.Add("ComandText", strComandText);
                hshParamater.Add("Error", ex.Message);
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParamater, "SQLServerDAL.DBBaseOperator.ProcessData()", UtilityFactory.LogType.LogToFile);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                    con = null;
                }
                if (cmd != null)
                    cmd.Dispose();
                if (adpt != null)
                    adpt.Dispose();
            }
            return ds;
        }
    }
}
