using System.Web.Caching;

namespace Expose178.Com.ICacheDependency
{
    public interface IExpose178CacheDependency
    {
        AggregateCacheDependency GetDependency();
    }
}
