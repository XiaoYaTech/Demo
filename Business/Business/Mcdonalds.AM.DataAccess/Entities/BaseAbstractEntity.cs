using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    /// <summary>
    /// 数据库实体基类
    /// Author: Stephen.Wang
    /// Date:   2014-06-15
    /// </summary>
    public class BaseAbstractEntity:IDisposable
    {
        private McdAMEntities _db = PrepareDb();
        public McdAMEntities GetDb()
        {
            return _db;
        }

        /// <summary>
        /// 复印机~,用于复制对象副本
        /// </summary>
        protected ObjectCopy Duplicator = new ObjectCopy();

        protected static McdAMEntities PrepareDb()
        {
            return new McdAMEntities();
        }

        public BaseAbstractEntity()
        {
        }


        public void ShareDbContextFrom(BaseAbstractEntity entity)
        {
            _db = entity.GetDb();
        }

        public static IQueryable<TResult> SqlQuery<TResult>(string sql, object sqlParams)
        {
            var db = PrepareDb();
            using (var cmd = db.Database.Connection.CreateCommand())
            {
                DbParameter[] _params = new DbParameter[0];
                if (sqlParams != null)
                {
                    _params = sqlParams.GetType()
                        .GetProperties().Where(p => p.CanRead)
                        .Select(p =>
                        {
                            DbParameter param = cmd.CreateParameter();
                            param.ParameterName = p.Name;
                            param.Direction = ParameterDirection.InputOutput;
                            param.Value = p.GetValue(sqlParams) ?? DBNull.Value;
                            return param;
                        }).ToArray();
                }
                return db.Database.SqlQuery<TResult>(sql, _params).AsQueryable();
            }
        }

        public static List<TResult> SqlQueryTerminal<TResult>(string sql, Dictionary<string,object> sqlParams) where TResult : class
        {
            var db = PrepareDb();
            using (var cmd = db.Database.Connection.CreateCommand())
            {
                DbParameter[] _params = new DbParameter[0];
                if (sqlParams != null)
                {
                    _params = sqlParams.Select(dict =>
                    {
                        DbParameter param = cmd.CreateParameter();
                        param.ParameterName = dict.Key;
                        param.Direction = ParameterDirection.InputOutput;
                        param.Value = dict.Value;
                        return param;
                    }).ToArray();
                }
                var list = db.Database.SqlQuery<TResult>(sql, _params).ToList();
                foreach (DbParameter param in _params)
                {
                    sqlParams[param.ParameterName] = param.Value;
                }
                return list;
            }
        }

        //#region transaction
        //private DbContextTransaction transaction { get; set; }

        //public void BeginTransAction()
        //{
        //    if (transaction != null)
        //    {
        //        throw new Exception("another transaction already exist!");
        //    }
        //    transaction = _db.Database.BeginTransaction();
        //}

        //public void BeginTransAction(System.Data.IsolationLevel transLevel)
        //{
        //    if (transaction != null)
        //    {
        //        throw new Exception("another transaction already exist!");
        //    }
        //    transaction = _db.Database.BeginTransaction(transLevel);
        //}

        //public void Rollback()
        //{
        //    if (transaction != null)
        //    {
        //        transaction.Rollback();
        //        transaction.Dispose();
        //        transaction = null;
        //    }
        //    else
        //    {
        //        throw new Exception("no transaction exist!");
        //    }
        //}

        //public void Commit()
        //{
        //    if (transaction != null)
        //    {
        //        transaction.Commit();
        //        transaction.Dispose();
        //        transaction = null;
        //    }
        //    else
        //    {
        //        throw new Exception("no transaction exist!");
        //    }
        //}
        //#endregion

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
