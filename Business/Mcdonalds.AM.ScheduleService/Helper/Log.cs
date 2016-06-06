using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.ScheduleService
{
    public class Log
    {
        /// <summary>
        /// 写正常消息
        /// </summary>
        /// <param name="message">消息</param>
        public static void WriteLog(string message)
        {
            string filePath = Config.LogInfoPath;
            WriteLog(filePath, message);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="filePath">日志文件路径</param>
        /// <param name="message">日志消息</param>
        public static void WriteLog(string filePath, string message)
        {
            try
            {
                if (filePath == null)
                    return;

                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                FileInfo fi = new FileInfo(filePath);
                long length = 0;
                {
                    length = fi.Length;
                }
                if (length > 1048576)
                {
                    fi.Delete();
                    fi = new FileInfo(filePath);
                }
                StreamWriter sw = fi.AppendText();
                sw.WriteLine(DateTime.Now.ToString("yyyy年M月d日HH:mm:ss"));
                sw.WriteLine(message);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorMessage(Exception ex)
        {
            string filePath = Config.LogErrorPath;
            WriteErrorMessage(filePath, ex);
        }

        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="filePath">日志路径</param>
        /// <param name="ex">异常对象</param>
        public static void WriteErrorMessage(string filePath, Exception ex)
        {
            try
            {
                if (filePath == null)
                    return;

                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                FileInfo fi = new FileInfo(filePath);
                long length = 0;
                using (fi.OpenWrite())
                {
                    length = fi.Length;
                }
                if (length > 1048576)
                {
                    fi.Delete();
                    fi = new FileInfo(filePath);
                }
                StreamWriter sw = fi.AppendText();
                sw.WriteLine(DateTime.Now.ToString("yyyy年M月d日HH:mm:ss"));
                sw.WriteLine("Message:");
                sw.WriteLine(ex.Message);
                sw.WriteLine("StackTrace:");
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
    }
}
