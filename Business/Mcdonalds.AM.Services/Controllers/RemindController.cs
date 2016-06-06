using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.DataModels.Condition;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.Services.Controllers
{
    public class RemindUserInfo
    {
        public string UserAccount
        {
            get;
            set;
        }

        public string UserNameENUS { get; set; }
        public string UserNameZHCN { get; set; }
    }

    public class RemindController : ApiController
    {
        private McdAMEntities db = new McdAMEntities();
        [HttpPost]
        [Route("api/Remind/QueryList")]
        public IHttpActionResult QueryList(RemindCondition condition)
        {
            condition.ReceiverAccount = ClientCookie.UserCode;
            int totalSize;
            var data = Remind.Query(condition, out totalSize).ToList();

            return Ok(new { data, totalSize });
        }


        [HttpPost]
        [Route("api/Remind/Save")]
        public IHttpActionResult Save(Remind remind)
        {
            remind.Update();

            return Ok(remind);
        }


        // GET api/Remind
        public IQueryable<Remind> GetRemind()
        {
            return db.Remind;
        }

        // GET api/Remind/5
        [ResponseType(typeof(Remind))]
        public IHttpActionResult GetRemind(string id)
        {

            Remind remind = db.Remind.Find(id);
            if (remind == null)
            {
                return NotFound();
            }

            return Ok(remind);
        }



        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="remind">消息体</param>
        /// <param name="remindUsers">接受用户</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Remind/PostRemaindList")]
        public IHttpActionResult PostRemaindList(Remind remind, List<RemindUserInfo> remindUsers)
        {
            var newGuid = new Guid();
            ObjectCopy objectCopy = new ObjectCopy();
            var entity = new Remind();
            foreach (var users in remindUsers)
            {
                entity = objectCopy.AutoCopy(remind);
                entity.Id = newGuid;
                entity.ReceiverNameENUS = users.UserNameENUS;
                entity.ReceiverNameZHCN = users.UserNameZHCN;
                entity.ReceiverAccount = users.UserAccount;
                entity.CreateTime = DateTime.Now;
                PostRemind(entity);
            }

            return Ok();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="remindregister"></param>
        /// <returns></returns>
        [ResponseType(typeof(Remind))]
        public IHttpActionResult PostRemind(Remind remind)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (remind.Id == new Guid())
            {
                remind.Id = Guid.NewGuid();
                remind.CreateTime = DateTime.Now;
                db.Remind.Add(remind);
            }
            else
            {

                db.Remind.Attach(remind);
                db.Entry(remind).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RemindExists(remind.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = remind.Id }, remind);
        }


        // PUT api/Remind/5
        public IHttpActionResult PutRemind(Guid id, Remind remind)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            db.Entry(remind).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RemindExists(id))
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



        // DELETE api/Remind/5
        [ResponseType(typeof(Remind))]
        public IHttpActionResult DeleteRemind(string id)
        {
            Remind remind = db.Remind.Find(id);
            if (remind == null)
            {
                return NotFound();
            }

            db.Remind.Remove(remind);
            db.SaveChanges();

            return Ok(remind);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RemindExists(Guid id)
        {
            return db.Remind.Count(e => e.Id == id) > 0;
        }


        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="sourceCode">来源编号</param>
        /// <param name="refId">引用的Id</param>
        /// <returns></returns>
        [Route("api/Remind/{registerCode}/{refId}")]
        public IHttpActionResult Get(string registerCode, string refId)
        {
            var task = db.Remind.First(c => c.RegisterCode == registerCode);

            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
    }
}