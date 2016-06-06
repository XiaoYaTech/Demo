using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Repository
{
    interface IRepository<T>
    {
        int Add(params T[] entities);

        void Delete(params T[] entities);

        void Update(T entity);

        int Update(params T[] entities);

        IQueryable<T> Search(Expression<Func<T, bool>> predicate);
    }
}
