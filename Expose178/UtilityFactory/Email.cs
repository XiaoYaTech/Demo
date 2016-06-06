using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Expose178.Com.UtilityFactory
{
    public static class Email
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["EmailAssembly"];
        private static readonly string strClassName = ConfigurationManager.AppSettings["EmailClass"];
        private static Expose178.Com.IUtility.IEmail email = null;
        public static void SendEmail(string strSendTo, string strSendFrom, string strSendCC, string strTitle, string strMsgBody)
        {
            email = (Expose178.Com.IUtility.IEmail)Assembly.Load(strAssemblyPath).CreateInstance(strClassName);
            email.SendEmail(strSendTo, strSendFrom, strSendCC, strTitle, strMsgBody);
        }
    }
}
