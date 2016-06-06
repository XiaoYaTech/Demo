using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common
{
    public class EFIntercepterLogging : DbCommandInterceptor
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public override void ScalarExecuting(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<object> interceptionContext)
        {
            base.ScalarExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ScalarExecuted(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<object> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} rn --> Error executing command: {0}", command.CommandText,
                    interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("rn执行时间:{0} 毫秒rn-->ScalarExecuted.Command:{1}rn", _stopwatch.ElapsedMilliseconds,
                    command.CommandText);
            }
            base.ScalarExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void NonQueryExecuted(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<int> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} rn --> Error executing command:rn {0}", command.CommandText,
                    interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("rn执行时间:{0} 毫秒rn-->NonQueryExecuted.Command:rn{1}",
                    _stopwatch.ElapsedMilliseconds, command.CommandText);
            }
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ReaderExecuting(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            base.ReaderExecuting(command, interceptionContext);
            _stopwatch.Restart();
        }

        public override void ReaderExecuted(System.Data.Common.DbCommand command,
            DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            _stopwatch.Stop();
            if (interceptionContext.Exception != null)
            {
                Trace.TraceError("Exception:{1} rn --> Error executing command:rn {0}", command.CommandText,
                    interceptionContext.Exception.ToString());
            }
            else
            {
                Trace.TraceInformation("rn执行时间:{0} 毫秒 rn -->ReaderExecuted.Command:rn{1}",
                    _stopwatch.ElapsedMilliseconds, command.CommandText);
            }
            base.ReaderExecuted(command, interceptionContext);
        }
    }
}
