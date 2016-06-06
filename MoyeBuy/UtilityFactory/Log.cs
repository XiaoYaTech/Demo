using System;
using System.Reflection;
using System.Configuration;
using System.Collections;
using MoyeBuy.Com.IUtility;

namespace MoyeBuy.Com.UtilityFactory
{
    public enum LogType
    { 
        LogToFile,
        LogToDB
    }
    public sealed class Log
    {
        private static readonly string strAssemblyPath =ConfigurationManager.AppSettings["LogAssembly"];
        private static readonly string strLogDBClassName = ConfigurationManager.AppSettings["LogDBClass"];
        private static readonly string strLogFileClassName = ConfigurationManager.AppSettings["LogFileClass"];
        public static void WriteLog(Hashtable hshParam, string strPosition, LogType logType)
        {
            Ilog log = null;
            string strFullPath=strAssemblyPath+".";
            if (logType == LogType.LogToDB)
                strFullPath = strFullPath + strLogDBClassName;
            else if (logType == LogType.LogToFile)
                strFullPath = strFullPath + strLogFileClassName;
            else
                strFullPath = strFullPath + strLogFileClassName;
            log = (Ilog)Assembly.Load(strAssemblyPath).CreateInstance(strFullPath);
            log.WriteLog(hshParam, strPosition);
        }
    }
}
