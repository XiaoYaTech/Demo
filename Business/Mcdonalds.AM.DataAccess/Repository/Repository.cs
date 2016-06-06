using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Repository
{
    public class Repository<T> : IRepository<T>, IDisposable where T : BaseAbstractEntity, new()
    {
        private McdAMEntities _db = new McdAMEntities();
        public McdAMEntities GetDb()
        {
            return _db;
        }
        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// 传入实体主键值，返回实体
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public T Get(Guid key)
        {
            return GetDb().Set<T>().Find(key);
        }

        /// <summary>
        /// 传入实体主键值， 判断实体是否存在
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public bool IsExist(Guid key)
        {
            return Get(key) != null;
        }

        /// <summary>
        /// 传入Lamda表达式， 判断数据库中是否存在
        /// </summary>
        /// <param name="keys">Lamda表达式</param>
        /// <returns></returns>
        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return GetDb().Set<T>().Where(predicate).Any();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetDb().Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalRecords">记录总数</param>
        /// <returns></returns>
        public List<T> GetAll(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = GetDb().Set<T>().Count();
            return GetDb().Set<T>().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }


        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name="entities">实体们</param>
        public int Add(params T[] entities)
        {
            if (entities != null && entities.Any())
            {
                GetDb().Set<T>().AddRange(entities);
                return GetDb().SaveChanges();
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entities">实体们</param>
        public void Delete(params T[] entities)
        {
            if (entities != null && entities.Any())
            {
                GetDb().Set<T>().RemoveRange(entities);
                GetDb().SaveChanges();
                return;
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids">实体主键们</param>
        public void Delete(params Guid[] ids)
        {
            var myDb = GetDb();
            if (ids != null && ids.Count() > 0)
            {
                foreach (var entity in ids.Select(id => myDb.Set<T>().Find(id)))
                {
                    myDb.Set<T>().Remove(entity);
                }
                myDb.SaveChanges();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Update(T entity)
        {
            if (entity != null)
            {
                GetDb().Set<T>().Attach(entity);
                GetDb().Entry(entity).State = EntityState.Modified;
                GetDb().SaveChanges();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体们</param>
        public int Update(params T[] entities)
        {
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    GetDb().Set<T>().Attach(entity);
                    GetDb().Entry(entity).State = EntityState.Modified;
                }
                return GetDb().SaveChanges();
            }
            return 0;
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return GetDb().Set<T>().Count(predicate);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="Tkey">排序键</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderByPredicate">排序条件</param>
        /// <param name="IsDesc">是否倒序</param>
        /// <returns></returns>
        public IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            return GetDb().Set<T>().Where(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey">排序键</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderByPredicate">排序条件</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalRecords">记录总数</param>
        /// <param name="IsDesc">是否倒序</param>
        /// <returns></returns>
        public IQueryable<T> Search<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageIndex, int pageSize, out int totalRecords, bool IsDesc = false) where Tkey : IComparable
        {
            var query = GetDb().Set<T>();
            IOrderedQueryable<T> result;
            if (IsDesc)
            {
                result = query.Where(predicate).OrderByDescending(orderByPredicate);
            }
            else
            {
                result = query.Where(predicate).OrderBy(orderByPredicate);
            }
            totalRecords = query.Count(predicate);
            return result.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }
    }
}
