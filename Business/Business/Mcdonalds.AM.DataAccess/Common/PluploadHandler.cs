using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mcdonalds.AM.DataAccess.Common
{
    public class PluploadHandler
    {
        public static void WriteErrorMsg(string msg)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/json";
            response.StatusCode = 500;
            response.TrySkipIisCustomErrors = true;
            response.Write(JsonConvert.SerializeObject(GetErrorMsg(msg)));
            response.End();
        }

        public static object GetErrorMsg(string msg)
        {
            return new
            {
                jsonrpc = "2.0",
                error = new
                {
                    code = 500,
                    message = msg
                },
                ExceptionMessage = msg,
                id = "id"
            };
        }
    }
}
