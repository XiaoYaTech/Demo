using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.Services.Controllers
{
    public class NotificationController : ApiController
    {
        [HttpPost]
        [Route("api/Notification/Query")]
        public IHttpActionResult Query(NotificationSearchCondition searchCondition)
        {
            searchCondition.ReceiverAccount = ClientCookie.UserCode;

            int totalSize;
            var data = Notification.Query(searchCondition, out totalSize).ToList();

            return Ok(new { data, totalSize });

        }

        [HttpPost]
        [Route("api/Notification/Save")]
        public IHttpActionResult Save(Notification notification)
        {
            notification.Update();

            return Ok(notification);
        }
    }
}