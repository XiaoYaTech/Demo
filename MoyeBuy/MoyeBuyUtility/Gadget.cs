using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Web.Security;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class Gadget
    {
        private static readonly string strConnection = ConfigurationManager.ConnectionStrings["DSN"].ConnectionString;
        private static readonly string strDomain = ConfigurationManager.AppSettings["DOMAIN"].ToString();
        private static Model.User user = null;
        public static string GetConnectionString(string strDBName)
        {
            return strConnection.Replace("XXXXXX",strDBName)+"MoyeBuy.Com";
        }
        public static string GetDomain()
        {
            return strDomain;
        }
        public static void Addparamater(ref Hashtable hshParamater, string strKey, object objValue)
        {
            try
            {
                if (objValue != null && !hshParamater.ContainsKey(strKey) && hshParamater != null && !String.IsNullOrEmpty(strKey) && !String.IsNullOrEmpty(objValue.ToString().Trim()))
                    hshParamater.Add(strKey, "N'" + objValue.ToString().Trim().Replace("'", "''") + "'");
            }
            catch (Exception ex)
            {
                hshParamater.Add("UID", Gadget.GetUserID());
                hshParamater.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParamater, "Gadget.Addparamater()", UtilityFactory.LogType.LogToFile);
            }
        }
        public static Model.User GetUserFromTicket()
        {
            if (HttpContext.Current.User.Identity is FormsIdentity)
            {
                FormsIdentity UserIdent = (FormsIdentity)HttpContext.Current.User.Identity;
                string strSerializUser = UserIdent.Ticket.UserData;
                //strSerializUser = MoyeBuyUtility.Encryption.SalRijndaelDecrypt(strSerializUser);
                user = Deserialize(strSerializUser);
            }
            return user;
        }
        public static string GetUserID()
        {
            user = GetUserFromTicket();
            if (user != null)
                return user.MoyeBuyComUserID;
            else
                return "NULL";
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
            {
                if (ds.Tables[0].Columns.Contains("StatusCode") && ds.Tables[0].Rows[0]["StatusCode"].ToString()!="0")
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        public static string GetDataRowStringValue(DataRow dr, string strColumn)
        {
            string strReturnValue = null;
            if (dr != null)
            {
                if (dr[strColumn] != DBNull.Value)
                    strReturnValue = dr[strColumn].ToString();
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
                    hshParam.Add("Error", ex.Message);
                    hshParam.Add("USP",dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowBoolValue()", UtilityFactory.LogType.LogToFile);
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowIntValue()", UtilityFactory.LogType.LogToFile);
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowDateTimeValue()", UtilityFactory.LogType.LogToFile);
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowDecimalValue()", UtilityFactory.LogType.LogToFile);
                }
                
            }
            return dReturn;
        }
        public static decimal GetDataRowDecimalValue(DataRow dr, string strColumn)
        {
            decimal dReturn = 0;
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetDataRowDecimalValue()", UtilityFactory.LogType.LogToFile);
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetNullableBoolValue()", UtilityFactory.LogType.LogToFile);
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
                    hshParam.Add("USP", dr.Table.TableName);
                    MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.GetNullableIntValue()", UtilityFactory.LogType.LogToFile);
                }
            }
            return intReturn;
        }

        public static string SalGenerateHashCode(string strInputText)
        {
            SHA1CryptoServiceProvider cspHash = new SHA1CryptoServiceProvider();
            byte[] arrInput = new byte[strInputText.Length];
            byte[] arrOutput = new byte[strInputText.Length];
            string strOutputText;

            ASCIIEncoding encAscii = new ASCIIEncoding();
            encAscii.GetBytes(strInputText, 0, strInputText.Length, arrInput, 0);

            arrOutput = cspHash.ComputeHash(arrInput);

            strOutputText = "";
            for (int i = 0; i < arrOutput.Length; i++)
                strOutputText += String.Format("{0:x2}", arrOutput[i]);

            cspHash.Clear();
            encAscii = null;

            return strOutputText;
        }

        public static string SalGenerateSalt(int intLength)
        {
            string strResult = "";

            int intRandomNum;
            Random objRandomClass = new Random();

            for (int i = 1; i <= intLength; i++)
            {
                intRandomNum = Convert.ToInt32(objRandomClass.NextDouble() * 62);
                if (intRandomNum < 10) //from 0 to 9, set 0 to 9
                    intRandomNum = intRandomNum + 48;
                else if (intRandomNum < 36)//from 10 to 35, set A to Z
                    intRandomNum = intRandomNum + 55;
                else //from 36 to 61, set a to z
                    intRandomNum = intRandomNum + 61;
                strResult += (char)intRandomNum;
            }
            return strResult;
        }
        /// <summary>
        /// 进行Escape编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Escape(string p)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = System.Text.Encoding.Unicode.GetBytes(p);

            for (int i = 0; i < byteArr.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(byteArr[i + 1].ToString("X2"));//把字节转换为十六进制的字符串表现形式

                sb.Append(byteArr[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static XmlDocument Serialize(Model.User user)
        {
            XmlSerializer seril = new XmlSerializer(typeof(User));
            XmlDocument doc = new XmlDocument();
            MemoryStream stream = new MemoryStream();
            
            try
            {
                seril.Serialize(stream, user);
                stream.Position = 0;
                //StreamReader read = new StreamReader(stream);
                doc.Load(stream);
            }
            catch (Exception ex)
            {
                stream.Dispose();
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.Serialize()", UtilityFactory.LogType.LogToFile);
            }
            return doc;
        }

        public static XmlDocument SerializeCartItems(List<Model.CartItem> listCartItems)
        {
            XmlSerializer seril = new XmlSerializer(typeof(List<Model.CartItem>));
            XmlDocument doc = new XmlDocument();
            MemoryStream stream = new MemoryStream();

            try
            {
                seril.Serialize(stream, listCartItems);
                stream.Position = 0;
                //StreamReader read = new StreamReader(stream);
                doc.Load(stream);
            }
            catch (Exception ex)
            {
                stream.Dispose();
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "Gadget.Serialize()", UtilityFactory.LogType.LogToFile);
            }
            return doc;
        }

        public static Model.User Deserialize(string strXmlDoc)
        {
            XmlSerializer seril = new XmlSerializer(typeof(Model.User));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXmlDoc);
            StringReader reader =new StringReader(doc.OuterXml);
            Model.User user = seril.Deserialize(reader) as Model.User;
            return user;
        }

        public static List<Model.CartItem> DeserializeCartItems(string strXmlDoc)
        {
            XmlSerializer seril = new XmlSerializer(typeof(List<Model.CartItem>));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXmlDoc);
            StringReader reader = new StringReader(doc.OuterXml);
            List<Model.CartItem> listCartItems = seril.Deserialize(reader) as List<Model.CartItem>;
            return listCartItems;
        }
        
    }
}
