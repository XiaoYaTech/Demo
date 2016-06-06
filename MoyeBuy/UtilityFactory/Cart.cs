using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using MoyeBuy.Com.IUtility;

namespace MoyeBuy.Com.UtilityFactory
{
    public static class Cart
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["CartAssembly"];
        private static readonly string strClassName = ConfigurationManager.AppSettings["CartClass"];
        private static ICart cart = null;
        public static ICart CreateCart()
        {
            string strCalssname = strAssemblyPath + "." + strClassName;
            return (ICart)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
    }
}
