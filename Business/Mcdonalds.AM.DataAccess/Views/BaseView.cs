using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public abstract class BaseView<T>:BaseAbstractEntity where T:BaseAbstractEntity,new()
    {
        public static int Count(Expression<Func<T, bool>> predicate)
        {
            return PrepareDb().Set<T>().AsNoTracking().Count(predicate);
        }

        public static IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            return PrepareDb().Set<T>().Where(predicate).AsNoTracking();
        }

        public static IQueryable<T> Search<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, out int totalRecords, bool isDesc = false) where TKey : IComparable
        {
            totalRecords = Count(predicate);
            var result = PrepareDb().Set<T>().Where(predicate);
            if (isDesc)
            {
                result = result.OrderByDescending(orderBy);
            }
            else
            {
                result = result.OrderBy(orderBy);
            }
            return result.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking();

        }
    }
}
