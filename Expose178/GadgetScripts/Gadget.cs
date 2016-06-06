using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Web;

namespace Expose178.Com.GadgetScripts
{
    public class Gadget
    {
        private static readonly string strConnection = ConfigurationManager.ConnectionStrings["DSN"].ConnectionString;
        public static string GetConnectionString(string strDBName)
        {
            return strConnection.Replace("XXXXXX", strDBName) + "Expose178.Com";
        }
        public static void Addparamater(ref Hashtable hshParamater, string strKey, string strValue)
        {
            try
            {
                if (!hshParamater.ContainsKey(strKey) && hshParamater != null && !String.IsNullOrEmpty(strKey) && !String.IsNullOrEmpty(strValue))
                    hshParamater.Add(strKey, "N'" + strValue.Replace("'", "''") + "'");
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error",ex.Message);
                hshParam.Add("Key",strKey);
                hshParam.Add("Value",strValue);
                hshParam.Add("UID", Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "GadgetScripts.Addparamater()", UtilityFactory.LogType.LogToFile);
            }
            
        }

        //public static string GetUIDFromCookie()
        //{
        //    return HttpContext.Current.Server.UrlDecode(Encryption.SalRijndaelDecrypt(HttpContext.Current.Request.Cookies["UINFO"]["UID"]));
        //}

        public static string GetUserID()
        {
            return "UID5E11B70C09056791140BEDEE36";
        }

        public static ArrayList Split(string strValue, string strDelimiter)
        {
            ArrayList arlResults = new ArrayList();
            int intIdxOfDelim;
            int intStart = 0;
            int intEnd = 0;

            if (strValue != "")
            {
                intIdxOfDelim = strValue.IndexOf(strDelimiter, 0);
                if (intIdxOfDelim > -1)
                {
                    //Make sure the last value will be handled by adding the delimiter at the end
                    //removed by miaomiao because the condition of the end by the strDelimiter
                    //if (strValue.Substring(strValue.Length - strDelimiter.Length, strDelimiter.Length) != strDelimiter)
                    strValue = strValue + strDelimiter;

                    while (intIdxOfDelim > -1)
                    {
                        intEnd = intIdxOfDelim;
                        arlResults.Add(strValue.Substring(intStart, intEnd - intStart));
                        intStart = intIdxOfDelim + strDelimiter.Length;
                        intIdxOfDelim = strValue.IndexOf(strDelimiter, intStart);
                    }
                }
                else
                {
                    arlResults.Add(strValue);
                }
            }
            else
                arlResults = null;
            return arlResults;
        }
        public static IList<T> Split<T>(string strValue, string strDelimiter)
        {
            IList<T> listResult = new List<T>();
            int intIdxOfDelim;
            int intStart = 0;
            int intEnd = 0;
            if (strValue != "")
            {
                intIdxOfDelim = strValue.IndexOf(strDelimiter, 0);
                if (intIdxOfDelim > -1)
                {
                    strValue = strValue + strDelimiter;
                    while (intIdxOfDelim > -1)
                    {
                        intEnd = intIdxOfDelim;
                        listResult.Add((T)(Object)strValue.Substring(intStart, intEnd - intStart));
                        intStart = intIdxOfDelim + strDelimiter.Length;
                        intIdxOfDelim = strValue.IndexOf(strDelimiter, intStart);
                    }
                }
                else
                    listResult.Add((T)(Object)strValue);
            }
            else
                listResult = null;
            return listResult;
        }
        public static string SalRemoveXSS(object objInput)
        {
            StringBuilder sbdOutput = new StringBuilder();

            if (objInput != null)
            {
                StringBuilder sbdInput = new StringBuilder(objInput.ToString());
                for (int intIndex = 0; intIndex < sbdInput.Length; intIndex++)
                {
                    char chrInput = sbdInput[intIndex];
                    int intChar = System.Convert.ToInt32(chrInput);
                    //We will encode every characters except "A-Z", "a-z", "0-9", "&", "#", ";", "\", " "  by default
                    if (intChar == 32 || intChar == 35 || intChar == 38 || intChar == 39 || (intChar >= 48 && intChar <= 57) ||
                           intChar == 59 || (intChar >= 65 && intChar <= 90) || intChar == 92 || (intChar >= 97 && intChar <= 122) || intChar == 58)
                    {
                        sbdOutput.Append(chrInput);
                    }

                    else
                    {
                        //strOutput = strOutput + chrInput;
                        sbdOutput.Append("&#" + intChar + ";");
                    }
                }
            }
            return sbdOutput.ToString();
        }

        public static string SalJavaCleaner(object objInput)
        {
            string strInput = objInput.ToString().Trim();
            StringBuilder strTemp = new StringBuilder();
            if (strInput != null)
            {
                if (String.Compare(strInput, "", true) != 0)
                {
                    strTemp.Append(strInput);
                    strTemp.Replace("\"", "&#34;");
                    strTemp.Replace("%", "%25");
                    strTemp.Replace("\\", "\\\\");
                    strTemp.Replace("'", "\\'");   //this action will create new "\",so it should be behind Replace("\\", "\\\\");
                    strTemp.Replace("\b", "\\b");
                    strTemp.Replace("\f", "\\f");
                    strTemp.Replace("\n", "\\n");
                    strTemp.Replace("\r", "\\r");
                    strTemp.Replace("\t", "\\t");
                }

            }
            return strTemp.ToString();
        }

        //public static AjaxPro.JavaScriptObject SalXSSFilterAjaxObject(AjaxPro.JavaScriptObject arrHshParam)
        //{
        //    if (arrHshParam == null || arrHshParam.Keys.Length <= 0)
        //        return arrHshParam;

        //    ArrayList arlKey = new ArrayList(arrHshParam.Keys);

        //    for (int intK = 0; intK < arlKey.Count; intK++)
        //    {
        //        string strKey = arlKey[intK].ToString();

        //        // the minimum length of XSS should be longer than 5.
        //        if (arrHshParam[strKey] != null &&
        //               string.Compare(arrHshParam[strKey].GetType().Name, "JavaScriptString", true) == 0 &&
        //               arrHshParam[strKey].ToString().Length > 5)
        //        {
        //            string strInput = arrHshParam[strKey].ToString();
        //            arrHshParam[strKey] = new AjaxPro.JavaScriptString(strInput);
        //        }
        //    }

        //    return arrHshParam;

        //}

        public static bool DatatSetIsNotNullOrEmpty(DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
        public static string GetDataRowStringValue(DataRow dr, string strColumn)
        {
            string strReturnValue = null;
            try
            {
                if (dr != null)
                {
                    if (dr[strColumn] != DBNull.Value)
                        strReturnValue = dr[strColumn].ToString();
                }
            }
            catch(Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowStringValue()", UtilityFactory.LogType.LogToDB);
            }
            return strReturnValue;
        }
        public static bool GetDataRowBoolValue(DataRow dr, string strColumn)
        {
            bool bReturn = false;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        bReturn = Convert.ToBoolean(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error",ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowBoolValue()",UtilityFactory.LogType.LogToDB);
                }
            }
            return bReturn;
        }
        public static int GetDataRowIntValue(DataRow dr, string strColumn)
        {
            int intReturn = -1;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        intReturn = Convert.ToInt32(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error", ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowIntValue()", UtilityFactory.LogType.LogToDB);
                }
            }
            return intReturn;
        }

        public static DateTime GetDataRowDateTimeValue(DataRow dr, string strColumn)
        {
            DateTime dt = DateTime.Now;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        dt = Convert.ToDateTime(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error", ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowDateTimeValue()", UtilityFactory.LogType.LogToDB);
                }
            }
            return dt;
        }
        public static decimal? GetNullableDecimalValue(DataRow dr, string strColumn)
        {
            decimal? dReturn = null;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        dReturn = Convert.ToDecimal(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error", ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetNullableDecimalValue()", UtilityFactory.LogType.LogToDB);
                }

            }
            return dReturn;
        }

        public static bool? GetNullableBoolValue(DataRow dr, string strColumn)
        {
            bool? bReturn = null;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        bReturn = Convert.ToBoolean(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error", ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetNullableBoolValue()", UtilityFactory.LogType.LogToDB);
                }
            }
            return bReturn;
        }
        public static int? GetNullableIntValue(DataRow dr, string strColumn)
        {
            int? intReturn = null;
            if (dr != null)
            {
                try
                {
                    if (dr[strColumn] != DBNull.Value)
                        intReturn = Convert.ToInt32(dr[strColumn]);
                }
                catch (Exception ex)
                {
                    Hashtable hshParam = new Hashtable();
                    hshParam.Add("UID", Gadget.GetUserID());
                    hshParam.Add("Error", ex.Message);
                    Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetNullableIntValue()", UtilityFactory.LogType.LogToDB);
                }
            }
            return intReturn;
        }
    }
}
