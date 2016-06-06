using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using MoyeBuy.Com.IUtility;


namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class SystemLogToFile:Ilog
    {
        private static readonly string strVirtualPath = ConfigurationManager.AppSettings["LogPath"];
        public SystemLogToFile() { }
        public void WriteLog(System.Collections.Hashtable hshParam, string strFileName)
        {
            string strFolderPath = System.Web.HttpContext.Current.Server.MapPath(strVirtualPath) + @"\" + DateTime.Now.ToString("yyyyMMdd");
            string strFilePath = strFolderPath + @"\" + strFileName + ".log";
            try
            {
                if (!Directory.Exists(strFolderPath))
                    Directory.CreateDirectory(strFolderPath);
                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                {
                    StringBuilder sbdFormat = new StringBuilder();
                    foreach (string strKey in hshParam.Keys)
                    {
                        sbdFormat.Append(strKey + ":\t");
                        sbdFormat.Append(hshParam[strKey].ToString() + "\t");
                    }
                    sbdFormat.Append("DateTime:\t"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine(sbdFormat.ToString());
                }
            }
            catch (IOException ex)
            { }
        }
    }
}
