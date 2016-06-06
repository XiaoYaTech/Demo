using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Configuration;
using Expose178.Com.ICacheDependency;

namespace Expose178.Com.TableCacheDependency
{
    public abstract class TableDependency : IExpose178CacheDependency
    {
        protected char[] configurationSeparator = new char[] { ';' };
        protected AggregateCacheDependency dependency = new AggregateCacheDependency();
        protected TableDependency(string strTableConfigKey, string strDataBaseConfigKey)
        {
            string dbName = ConfigurationManager.AppSettings[strDataBaseConfigKey];
            string tableConfig = ConfigurationManager.AppSettings[strTableConfigKey];
            string[] tables = tableConfig.Split(configurationSeparator);
            string strDNS = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;
            SqlCacheDependencyAdmin.EnableNotifications(strDNS);
            SqlCacheDependencyAdmin.EnableTableForNotifications(strDNS, tables);
            foreach (string tableName in tables)
            {
                dependency.Add(new SqlCacheDependency(dbName, tableName));
            }
        }
        public System.Web.Caching.AggregateCacheDependency GetDependency()
        {
            return dependency;
        }
    }
}
