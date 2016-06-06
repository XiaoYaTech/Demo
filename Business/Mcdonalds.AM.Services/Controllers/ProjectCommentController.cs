using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ProjectCommentController : ApiController
    {
        private ProjectComment projectComment = new ProjectComment();

        [Route("api/ProjectComment/Search/{sourceCode}/{refTableName}/{refTableId}")]
        [HttpGet]
        public IHttpActionResult Search(string sourceCode, string refTableName, Guid refTableId)
        {
            if (refTableId == Guid.Empty)
                return Ok();
            //var list = ProjectComment.Search(e => e.RefTableName == refTableName
            //   && e.SourceCode == sourceCode && e.RefTableId == refTableId && e.Status == ProjectCommentStatus.Submit)
            //   .OrderBy(e => e.CreateTime).ToList();
            var db = new McdAMEntities();
            var list = db.VProjectComment.Where(e => e.RefTableName == refTableName
               && e.SourceCode == sourceCode && e.RefTableId == refTableId && e.Status == (int)ProjectCommentStatus.Submit).OrderBy(e => e.CreateTime).ToList();
            return Ok(list);
        }
    }
}
