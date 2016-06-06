using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Expose178.Com.DALFactory
{
    public sealed class DataAcess
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["WebDAL"];
        public static Expose178.Com.IDAL.IDBBaseOperator CreateDBBaseOperator()
        {
            string strCalssname = strAssemblyPath + ".DBBaseOperator";
            return (Expose178.Com.IDAL.IDBBaseOperator)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
    }
}
