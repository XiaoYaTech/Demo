using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Collections;

namespace Expose178.Com.UtilityFactory
{
    public enum LogType
    {
        LogToFile,
        LogToDB
    }

    public sealed class Log
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["LogAssembly"];
        private static readonly string strLogDBClassName = ConfigurationManager.AppSettings["LogDBClass"];
        private static readonly string strLogFileClassName = ConfigurationManager.AppSettings["LogFileClass"];
        public static void WriteLog(Hashtable hshParam, string strPosition, LogType logType)
        {
            Expose178.Com.IUtility.Ilog log = null;
            string strFullPath = strAssemblyPath + ".";
            if (logType == LogType.LogToDB)
                strFullPath = strFullPath + strLogDBClassName;
            else if (logType == LogType.LogToFile)
                strFullPath = strFullPath + strLogFileClassName;
            else
                strFullPath = strFullPath + strLogFileClassName;
            log = (Expose178.Com.IUtility.Ilog)Assembly.Load(strAssemblyPath).CreateInstance(strFullPath);
            log.WriteLog(hshParam, strPosition);
        }
    }
}
