using System.Web;
using System.Web.Mvc;

namespace Expose178.Com.Expose178Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}