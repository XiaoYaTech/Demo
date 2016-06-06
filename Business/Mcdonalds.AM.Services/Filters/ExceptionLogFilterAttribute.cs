using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Mcdonalds.AM.Services
{
    public class ExceptionLogFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                string error = "-----------------------------------------Error------------------------------------------------\r\n" + context.Exception.Message + "\r\n" + (context.Exception.InnerException == null ? string.Empty : context.Exception.InnerException.Message) + "\r\n" + context.Exception.StackTrace + "\r\n";
                Log4netHelper.WriteErrorLog(error);
            }
        }

    }
}