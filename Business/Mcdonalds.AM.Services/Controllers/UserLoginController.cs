using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Controllers
{
    public class UserLoginController : ApiController
    {
        private McdAMEntities _db = new McdAMEntities();

        public IHttpActionResult PostUserLogin(string account)
        {

           var entity = _db.ClosureUsers.First(e => e.UserAccount == account);
            return Ok(entity);
        }
    }
}
