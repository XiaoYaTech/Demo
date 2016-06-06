using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using MoyeBuy.Com.IUtility;

namespace MoyeBuy.Com.UtilityFactory
{
    public static class Email
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["EmailAssembly"];
        private static readonly string strClassName = ConfigurationManager.AppSettings["EmailClass"];
        private static IEmail email = null;
        public static void SendEmail(string strSendTo,string strSendFrom,string strSendCC,string strTitle,string strMsgBody,string strUID)
        {
            email = (IEmail)Assembly.Load(strAssemblyPath).CreateInstance(strClassName);
            email.SendEmail(strSendTo, strSendFrom, strSendCC, strTitle, strMsgBody,strUID);
        }
    }
}
