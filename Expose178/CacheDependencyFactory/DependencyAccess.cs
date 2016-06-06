using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using Expose178.Com.ICacheDependency;

namespace Expose178.Com.CacheDependencyFactory
{
    public static class DependencyAccess
    {
        public static IExpose178CacheDependency CreateArticleDependency()
        {
            return LoadInstance("ArticleDependency");
        }
        public static IExpose178CacheDependency CreateArticleTitleDependency()
        {
            return LoadInstance("ArticleTitleDependency");
        }
        public static IExpose178CacheDependency CreateAdsDependency()
        {
            return LoadInstance("AdsDependency");
        }
        private static IExpose178CacheDependency LoadInstance(string strClassName)
        {
            string path = ConfigurationManager.AppSettings["CacheDependencyAssembly"];
            string fullyQualifiedClass = path + "." + strClassName;
            return (IExpose178CacheDependency)Assembly.Load(path).CreateInstance(fullyQualifiedClass);
        }
    }
}
