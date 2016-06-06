using System.Web.Caching;

namespace MoyeBuy.Com.ICacheDependency
{
    public interface IMoyeBuyCacheDependency
    {
        AggregateCacheDependency GetDependency();
    }
}
