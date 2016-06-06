using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mcdonalds.AM.DataAccess
{
    /// <summary>
    /// 数据库实体基类
    /// Author: Stephen Wang
    /// Date:2014-06-15
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEntity<T> : BaseAbstractEntity where T : BaseAbstractEntity, new()
    {

        public virtual int Add()
        {
            var db = GetDb();
            try
            {
                db.Set(this.GetType()).Attach(this);
                db.Entry(this).State = EntityState.Added;
            }
            catch
            {
                db.Set(this.GetType()).Add(this);
            }
            return db.SaveChanges();
        }

        public virtual int Update()
        {
            var db = GetDb();
            try
            {
                db.Set(this.GetType()).Attach(this);
            }
            catch
            {

            }
            db.Entry(this).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public virtual int Delete()
        {
            var db = GetDb();
            try
            {
                db.Set(this.GetType()).Attach(this);
                db.Entry(this).State = EntityState.Deleted;
            }
            catch
            {
                db.Set(this.GetType()).Remove(this);
            }
            return db.SaveChanges();
        }
        /// <summary>
        /// 传入实体主键值，返回实体
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public static T Get(Guid key)
        {
            var db = PrepareDb();
            var entity = db.Set<T>().Find(key);
            return entity;
        }

        public static int Add(params T[] entities)
        {
            var db = PrepareDb();
            if (entities != null && entities.Any())
            {
                db.Set<T>().AddRange(entities);
                return db.SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public static int Update(params T[] entities)
        {
            var db = PrepareDb();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                return db.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 传入实体主键值， 判断实体是否存在
        /// </summary>
        /// <param name="keys">实体主键</param>
        /// <returns></returns>
        public static bool Any(Guid key)
        {
            var db = PrepareDb();
            return db.Set<T>().Find(key) != null;
        }

        /// <summary>
        /// 传入Lamda表达式， 判断数据库中是否存在
        /// </summary>
        /// <param name="keys">Lamda表达式</param>
        /// <returns></returns>
        public static bool Any(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            return db.Set<T>().Where(predicate).AsNoTracking().Any();
        }

        public static T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            var entity = db.Set<T>().AsNoTracking().FirstOrDefault(predicate);
            return entity;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalRecords">记录总数</param>
        /// <returns></returns>
        public static List<T> GetAll(int pageIndex, int pageSize, out int totalRecords)
        {
            var db = PrepareDb();
            totalRecords = db.Set<T>().Count();
            var entities = db.Set<T>().Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsNoTracking().ToList();
            return entities;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entities">实体们</param>
        public static int Delete(params T[] entities)
        {
            var db = PrepareDb();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry(entity).State = EntityState.Deleted;
                }
                return db.SaveChanges();
            }
            return 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids">实体主键们</param>
        public static int Delete(params Guid[] ids)
        {
            var db = PrepareDb();
            if (ids != null && ids.Count() > 0)
            {
                foreach (var entity in ids.Select(id => db.Set<T>().Find(id)))
                {
                    db.Set<T>().Remove(entity);
                }
                return db.SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public static int Delete(Expression<Func<T, bool>> predicate)
        {
            var entities = Search(predicate).ToArray();
            return Delete(entities);
        }

        public static int Count(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            return db.Set<T>().Count(predicate);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="Tkey">排序键</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderByPredicate">排序条件</param>
        /// <param name="IsDesc">是否倒序</param>
        /// <returns></returns>
        public static IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            var db = PrepareDb();
            var entities = db.Set<T>().Where(predicate);
            return entities.AsNoTracking();
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
        public static IQueryable<T> Search<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageIndex, int pageSize, out int totalRecords, bool IsDesc = false) where Tkey : IComparable
        {
            var db = PrepareDb();

            return Search<Tkey>(db, predicate, orderByPredicate, pageIndex, pageSize, out totalRecords, IsDesc);
        }

        public static IQueryable<T> Search<Tkey>(McdAMEntities context, Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageIndex, int pageSize, out int totalRecords, bool IsDesc = false) where Tkey : IComparable
        {
            var query = context.Set<T>();
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
            var entities = result.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return entities;
        }

        public static IQueryable<T> OrderBy<Tkey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, Tkey>> orderByPredicate)
        {
            var db = PrepareDb();
            var query = db.Set<T>();

            var result = query.Where(predicate).OrderBy(orderByPredicate);
            return result;
        }

        public static IQueryable<T> OrderByDescending<Tkey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, Tkey>> orderByPredicate)
        {
            var db = PrepareDb();
            var query = db.Set<T>();

            var result = query.Where(predicate).OrderByDescending(orderByPredicate);
            return result;
        }

        public static string GetRefTableId(string refTableName, string projectId)
        {
            var db = PrepareDb();
            //ContractInfo没有IsHistory
            if (refTableName.ToLower() == "projectcontractinfo")
                return db.Database.SqlQuery<Guid>(string.Format(@"
                SELECT Id FROM {0} 
                WHERE ProjectId = '{1}'
                ", refTableName, projectId)).FirstOrDefault().ToString();
            else
                return db.Database.SqlQuery<Guid>(string.Format(@"
                SELECT Id FROM {0} 
                WHERE ProjectId = '{1}'
                AND IsHistory = 0
                ", refTableName, projectId)).FirstOrDefault().ToString();
        }
    }
}
