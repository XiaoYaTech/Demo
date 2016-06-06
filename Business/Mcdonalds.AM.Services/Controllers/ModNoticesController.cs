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
using System.Web;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataModels.Condition;


namespace Mcdonalds.AM.Services.Controllers
{
    public class ModNoticesController : ApiController
    {

        /// <summary>
        /// Gets the notices list.分页获取Notice数据
        /// </summary>
        /// <param name="userCode">The user code.代入用户帐号</param>
        /// <param name="pageSize">Size of the page.每页的数据量</param>
        /// <param name="pageIndex">Index of the page.当前页Index</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/notices/list/{receiver}/{pageSize}/{pageIndex}")]
        [HttpPost]
        public IHttpActionResult GetNoticesList( int pageSize, int pageIndex, string receiver, SearchNoticeCondition condtion )
        {
            var queryString = HttpContext.Current.Request.QueryString;

            if (!string.IsNullOrEmpty(receiver))
            {
                condtion.Receiver = receiver;
            }

            var bll = new ModNotices();
            var result = bll.QueryNoticesByConditions(condtion);

            var skipSize = pageSize * (pageIndex - 1);

            int totalItems = result.Count();

            var list =
                result.OrderByDescending(c => c.CreatedTime).Skip(skipSize)
                .Take(pageSize).ToList();

            return Ok(new PagedDataSource(totalItems, list.ToArray()));
        }

        /// <summary>
        /// Get the notice data.
        /// </summary>
        /// <param name="noticeId">The notice identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/notices/detail")]
        public IHttpActionResult GetNoticeData( string noticeId, string receiver )
        {
            Guid _noticeGUID = new Guid( noticeId );

            var _detail = ModNotices.FirstOrDefault(c => c.Id == _noticeGUID);

            if (_detail == null)
            {
                return NotFound();
            }
            else
            {
                // 更新Notice 读过的状态
                var _list = ModNoticeReceivers.Search(o => o.NoticeId == _noticeGUID && o.Receiver == receiver ).ToList();

                foreach (var item in _list)
                {
                    item.IsReaded = 1;
                }
                ModNoticeReceivers.UpdateList( _list );
            }
            return Ok(_detail);
        }

        /*
        // GET api/ModNotices/5
        [ResponseType(typeof(ModNotices))]
        public IHttpActionResult GetModNotices(Guid id)
        {
            ModNotices modnotices = db.ModNotices.Find(id);
            if (modnotices == null)
            {
                return NotFound();
            }

            return Ok(modnotices);
        }

        // PUT api/ModNotices/5
        public IHttpActionResult PutModNotices(Guid id, ModNotices modnotices)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modnotices.Id)
            {
                return BadRequest();
            }

            db.Entry(modnotices).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModNoticesExists(id))
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

        // POST api/ModNotices
        [ResponseType(typeof(ModNotices))]
        public IHttpActionResult PostModNotices(ModNotices modnotices)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ModNotices.Add(modnotices);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ModNoticesExists(modnotices.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = modnotices.Id }, modnotices);
        }

        // DELETE api/ModNotices/5
        [ResponseType(typeof(ModNotices))]
        public IHttpActionResult DeleteModNotices(Guid id)
        {
            ModNotices modnotices = db.ModNotices.Find(id);
            if (modnotices == null)
            {
                return NotFound();
            }

            db.ModNotices.Remove(modnotices);
            db.SaveChanges();

            return Ok(modnotices);
        }*/

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }

        private bool ModNoticesExists(Guid id)
        {
            //return db.ModNotices.Count(e => e.Id == id) > 0;
            return false;
        }
    }
}