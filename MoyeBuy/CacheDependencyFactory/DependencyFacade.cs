using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Configuration;
using MoyeBuy.Com.ICacheDependency;

namespace MoyeBuy.Com.CacheDependencyFactory
{
    public static class DependencyFacade
    {
        private static readonly string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
        public static AggregateCacheDependency GetLayoutMenuDependency()
        {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateLayoutMenuDependency().GetDependency();
            else
                return null;
        }
        public static AggregateCacheDependency GetProductDependency()
        {
            if (!string.IsNullOrEmpty(path))
                return DependencyAccess.CreateProductDependency().GetDependency();
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
