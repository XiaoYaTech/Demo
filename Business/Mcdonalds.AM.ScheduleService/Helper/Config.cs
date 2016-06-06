using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Mcdonalds.AM.ScheduleService
{
    public class Config
    {
        /// <summary>
        /// 错误日志记录存放的路径
        /// </summary>
        public static string LogErrorPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogErrorFilePath"];
            }
        }

        /// <summary>
        /// 日志信息记录存放的路径
        /// </summary>
        public static string LogInfoPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogInfoFilePath"];
            }
        }
    }
}
