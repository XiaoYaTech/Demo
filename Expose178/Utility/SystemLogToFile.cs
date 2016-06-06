using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;


namespace Expose178.Com.Utility
{
    public class SystemLogToFile:Expose178.Com.IUtility.Ilog
    {
        private static readonly string strVirtualPath = ConfigurationManager.AppSettings["LogPath"];
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
                        sbdFormat.Append(strKey+":\t");
                        sbdFormat.Append(hshParam[strKey].ToString()+"\t");
                    }
                    sw.WriteLine(sbdFormat.ToString());
                }
            }
            catch (IOException ex)
            { }
        }
    }
}
