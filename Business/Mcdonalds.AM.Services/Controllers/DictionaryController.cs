using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.Entity.Validation;
using Mcdonalds.AM.DataAccess.DataModels.Condition;
using Mcdonalds.AM.DataAccess;
namespace Mcdonalds.AM.Services.Controllers
{
    public class DictionaryController : ApiController
    {

        private McdAMEntities _db = new McdAMEntities();

        [Route("api/dictionary/query")]
        [HttpGet]
        public IHttpActionResult Query(string parentCode)
        {
            return Ok(Dictionary.Search(d => d.ParentCode == parentCode).OrderByDescending(d => d.Status).ToList());
        }

        [Route("api/Dictionary/DesignType")]
        [HttpGet]
        public IHttpActionResult QueryDesignType()
        {
            return Ok(Dictionary.Search(d => d.ParentCode.Equals("DesignType")).OrderByDescending(d => d.Status).ToList());
        }

        [Route("api/Dictionary/NormType")]
        [HttpGet]
        public IHttpActionResult QueryNormType()
        {
            return Ok(Dictionary.Search(d => d.ParentCode.Equals("NormType")).OrderByDescending(d => d.Status).ToList());
        }

        /// <summary>
        /// 分页获取字典数据
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页的数据量</param>
        /// <param name="parentCode">父节点编码</param>
        /// <returns></returns>
        [Route("api/Dictionary/QueryList")]
        [HttpPost]
        public dynamic QueryList(DictionaryCondition condition)
        {
            //从多少页开始取数据
            var skipSize = condition.PageSize * (condition.PageIndex - 1);
            var list = _db.Dictionary
                .Where(d => d.ParentCode == condition.ParentCode);

            var result = list.OrderBy(c => c.Sequence)
                .Skip(skipSize)
                .Take(condition.PageSize).ToList();

            //int totalCount = 0;

            //var result = Dictionary.Bll.Search(d => d.ParentCode == condition.ParentCode, c => c.Sequence, condition.PageIndex,
            //    condition.PageSize, out totalCount).ToList();

            return result;
        }

        /// <summary>
        /// 查询字典数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dic">查询字典实体类，父节点编号必须，其它属性可选</param>
        /// <returns></returns>
        [Route("api/{controller}/SearchList/{pageIndex}/{pageSize}")]
        [HttpPost]
        public List<Dictionary> SearchList(int pageIndex, int pageSize, Dictionary dic)
        {
            //从多少页开始取数据
            var skipSize = pageSize * (pageIndex - 1);
            var list = _db.Dictionary
                .Where(d => d.ParentCode == dic.ParentCode);
            if (!string.IsNullOrEmpty(dic.NameENUS))
            {
                list = list.Where(d => d.NameENUS.Contains(dic.NameENUS));
            }
            if (!string.IsNullOrEmpty(dic.NameZHCN))
            {
                list = list.Where(d => d.NameZHCN.Contains(dic.NameZHCN));
            }
            if (!string.IsNullOrEmpty(dic.Code))
            {
                list = list.Where(d => d.Code.Contains(dic.Code));
            }

            var result = list.OrderBy(c => c.Sequence)
                 .Skip(skipSize)
                 .Take(pageSize).ToList();

            return result;
        }
        /// <summary>
        /// 查询上一级的字典列表
        /// </summary>
        /// <param name="condition">查询字典实体类，父节点编号必须</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Dictionary/QueryUpperList")]
        public IEnumerable<Dictionary> QueryUpperList(DictionaryCondition condition)
        {
            string code = string.Empty;
            if (condition.ParentCode == "root")
            {
                code = "root";
            }
            else
            {
                var dic = _db.Dictionary.Where(c => c.Code == condition.ParentCode).Take(1).ToList();
                if (dic.Count > 0)
                {
                    code = dic[0].ParentCode;
                }
            }

            condition.PageIndex = 1;
            condition.ParentCode = code;
            var list = QueryList(condition);



            return list;
        }

        /// <summary>
        /// 获取单个字典项
        /// </summary>
        /// <param name="code">字典编码</param>
        /// <returns></returns>
        public IHttpActionResult Get(string code)
        {

            var dic = _db.Dictionary.First(c => c.Code == code);
            //if (dic != null)
            //{
            //    //统一使用Name
            //    if (language.ToLower() == "en-us")
            //    {
            //        dic.Name = dic.EngName;
            //    }
            //}

            if (dic == null)
            {
                return NotFound();
            }

            return Ok(dic);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dic">实体对象</param>
        /// <returns></returns>
        [Route("api/Dictionary/Save")]
        [HttpPost]
        public IHttpActionResult Save(Dictionary dic)
        {

            if (dic.Id > 0)
            {

                dic.LastUpdateTime = DateTime.Now;
                _db.Dictionary.Attach(dic);
                _db.Entry(dic).State = EntityState.Modified;
            }
            else
            {
                var list = _db.Dictionary.Where(c => c.Code == dic.Code).ToList();
                if (list.Count > 0)
                {
                    throw new Exception("编码不能重复！");
                }


                dic.CreateTime = DateTime.Now;
                //dic.CreateUserAccount = RequestContext.
                _db.Dictionary.Add(dic);

            }
            _db.SaveChanges();
            return Ok(dic);
        }




        /// <summary>
        /// 根据id号批量删除
        /// </summary>
        /// <param name="dic">查询实体类，Id可以是多个用','号分割，id号码必须</param>
        /// <returns></returns>
        [Route("api/Dictionary/delete")]
        [HttpPost]
        [HttpGet]
        public int Delete(DictionaryCondition dic)
        {
            var sql = string.Format("DELETE FROM [dbo].[Dictionary] WHERE ID IN('{0}')", dic.Ids);
            var result = _db.Database.ExecuteSqlCommand(sql);
            return result;
            //var dic = _db.Dictionary.Find(id);
            //_db.Dictionary.RemoveRange()
            //_db.SaveChanges();
        }

        /// <summary>
        /// 批量更新字典
        /// </summary>
        /// <param name="dics">要更新的字典实体对象集合</param>
        /// <returns></returns>
        [Route("api/{controller}/UpdateList")]
        [HttpPost]
        public int UpdateList(List<Dictionary> dics)
        {

            foreach (var dic in dics)
            {
                _db.Dictionary.Attach(dic);
                _db.Entry(dic).State = EntityState.Modified;
            }
            int result = 0;
            try
            {
                result = _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

            }
            return result;
            //item.Sequence = dic.Sequence;

            //
            //
        }

    }
}