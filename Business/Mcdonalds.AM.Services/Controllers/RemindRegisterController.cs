using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataModels.Condition;

namespace Mcdonalds.AM.Services.Controllers
{
    public class RemindRegisterController : ApiController
    {
        private McdAMEntities db = new McdAMEntities();

        
        // GET api/RemindRegister
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RemindRegister/QueryList")]
        public IQueryable<RemindRegister> PostQueryList(RemindRegisterCondition condition)
        {
            //从多少页开始取数据
            var skipSize = condition.PageSize * (condition.PageIndex - 1);
            var list = db.RemindRegister;
         
            IQueryable<RemindRegister> result = list.Where(c=>true);
            if (!string.IsNullOrEmpty(condition.Name))
            {
                result = result.Where(c => c.Name == condition.Name);
            }
            if (!string.IsNullOrEmpty(condition.Code))
            {
                result = result.Where(c => c.Code == condition.Code);
            }
            if (!string.IsNullOrEmpty(condition.ModuleCode))
            {
                result = result.Where(c => c.ModuleCode == condition.ModuleCode);

            }
            if (!string.IsNullOrEmpty(condition.ModuleName))
            {
                result = result.Where(c => c.ModuleName == condition.ModuleName);

            }
            result = result.OrderBy(c => c.Sequence)
                .Skip(skipSize)
                .Take(condition.PageSize);

            return result;
        }

        // GET api/RemindRegister/5
        [ResponseType(typeof(RemindRegister))]
        public IHttpActionResult GetRemindRegister(string Code)
        {
            RemindRegister remindregister = db.RemindRegister.First(c => c.Code == Code);
            if (remindregister == null)
            {
                return NotFound();
            }

            return Ok(remindregister);
        }

        // PUT api/RemindRegister/5
        public IHttpActionResult PutRemindRegister(Guid id, RemindRegister remindregister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           

            db.Entry(remindregister).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemindRegisterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/RemindRegister
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="remindregister"></param>
        /// <returns></returns>
        [ResponseType(typeof(RemindRegister))]
        public IHttpActionResult PostRemindRegister(RemindRegister remindregister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (remindregister.Id == new Guid())
            {
                remindregister.Id = Guid.NewGuid();remindregister.CreateTime = new DateTime();
                db.RemindRegister.Add(remindregister);
            }
            else
            {
                db.RemindRegister.Attach(remindregister);
                db.Entry(remindregister).State = EntityState.Modified; 
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RemindRegisterExists(remindregister.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = remindregister.Id }, remindregister);
        }
       
        // DELETE api/RemindRegister/5
        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(RemindRegister))]
        public IHttpActionResult DeleteRemindRegister(string id)
        {
            RemindRegister remindregister = db.RemindRegister.Find(id);
            if (remindregister == null)
            {
                return NotFound();
            }

            db.RemindRegister.Remove(remindregister);
            db.SaveChanges();

            return Ok(remindregister);
        }


        // DELETE api/<controller>/5
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entity.Ids">主键集合</param>
        [Route("api/RemindRegister/delete")]
        [HttpPost]
        [HttpGet]
        public IHttpActionResult Delete(RemindRegisterCondition entity)
        {
            var sql = string.Format("DELETE FROM [dbo].[RemindRegister] WHERE ID IN({0})", entity.Ids);
            var result = db.Database.ExecuteSqlCommand(sql);
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RemindRegisterExists(Guid id)
        {
            return db.RemindRegister.Count(e => e.Id == id) > 0;
        }
    }
}