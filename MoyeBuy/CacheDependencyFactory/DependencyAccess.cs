using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using MoyeBuy.Com.ICacheDependency;

namespace MoyeBuy.Com.CacheDependencyFactory
{
    public static class DependencyAccess
    {
        public static IMoyeBuyCacheDependency CreateProductDependency()
        {
            return LoadInstance("ProductDependency");
        }
        public static IMoyeBuyCacheDependency CreateLayoutMenuDependency()
        {
            return LoadInstance("LayoutMenuDependency");
        }
        public static IMoyeBuyCacheDependency CreateAdsDependency()
        {
            return LoadInstance("AdsDependency");
        }
        public static IMoyeBuyCacheDependency CreateProductCagegoryDependency()
        {
            return LoadInstance("ProductCagegoryDependency");
        }
        public static IMoyeBuyCacheDependency CreateSupplierDependency()
        {
            return LoadInstance("SupplierDependency");
        }
        private static IMoyeBuyCacheDependency LoadInstance(string strClassName)
        {
            string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
            string fullyQualifiedClass = path + "." + strClassName;
            return (IMoyeBuyCacheDependency)Assembly.Load(path).CreateInstance(fullyQualifiedClass);
        }
    }
}
