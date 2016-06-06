using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Configuration;
using Expose178.Com.ICacheDependency;

namespace Expose178.Com.CacheDependencyFactory
{
    public static class DependencyFacade
    {
        private static readonly string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
        public static AggregateCacheDependency GetArticleTitleDependency()
        {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateArticleTitleDependency().GetDependency();
            else
                return null;
        }
        public static AggregateCacheDependency GetArticleDependency()
        {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateArticleDependency().GetDependency();
            else
                return null;
        }
        public static AggregateCacheDependency GetAdsDependency()
        {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateAdsDependency().GetDependency();
            else
                return null;
        }
    }
}
