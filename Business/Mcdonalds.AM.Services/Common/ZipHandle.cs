using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Common
{
    public class ZipHandle
    {

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filesUrls">文件路径列表</param>
        /// <returns></returns>
        public static string ExeFiles(IEnumerable<string> filesUrls)
        {
            string fileName = DateTime.Now.ToString("yyMMddHHmmssff");

            var current = System.Web.HttpContext.Current;
            string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName + ".zip";

            //ZipFile实例化一个压缩文件保存路径的一个对象zip
            using (ZipFile zip = new ZipFile(tempFilePath, Encoding.Default))
            {
                //加密压缩
                //zip.Password = "123456";
                //将要压缩的文件夹添加到zip对象中去(要压缩的文件夹路径和名称)
                //zip.AddDirectory(@"E:\\yangfeizai\\" + "12051214544443");
                //将要压缩的文件添加到zip对象中去,如果文件不存在抛错FileNotFoundExcept
                //zip.AddFile(@"E:\\yangfeizai\\12051214544443\\"+"Jayzai.xml");
                zip.AddFiles(filesUrls);
                zip.Save();
            }
            return tempFilePath;
        }


        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filesUrls">文件路径列表</param>
        /// <returns></returns>
        public static string ExeFiles(IEnumerable<Attachment> atts)
        {
            string fileName = DateTime.Now.ToString("yyMMddHHmmssff");

            var current = System.Web.HttpContext.Current;
            string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName + ".zip";

            //ZipFile实例化一个压缩文件保存路径的一个对象zip
            using (ZipFile zip = new ZipFile(tempFilePath, Encoding.Default))
            {
                //加密压缩
                //zip.Password = "123456";
                //将要压缩的文件夹添加到zip对象中去(要压缩的文件夹路径和名称)
                //zip.AddDirectory(@"E:\\yangfeizai\\" + "12051214544443");
                //将要压缩的文件添加到zip对象中去,如果文件不存在抛错FileNotFoundExcept
                //zip.AddFile(@"E:\\yangfeizai\\12051214544443\\"+"Jayzai.xml");

                string filePath = string.Empty;
                string innerFileName = string.Empty;
                List<string> attNames = new List<string>();
                string folder = SiteFilePath.UploadFiles_DIRECTORY;
                foreach (var att in atts)
                {
                    if (att.InternalName == null || att.FileURL == "#")
                        continue;

                    if (att.Name == "LL Negotiation Record List")
                    {
                        folder = SiteFilePath.TEMP_DIRECTORY;
                    }
                    else
                    {
                        folder = SiteFilePath.UploadFiles_DIRECTORY;
                    }

                    if (att.InternalName.IndexOf(".") != -1)
                        filePath = folder + "\\" + att.InternalName;
                    else
                        filePath = folder + "\\" + att.InternalName + att.Extension;
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (att.RequirementId.HasValue)
                        {
                            var ar = AttachmentRequirement.Get(att.RequirementId.Value);
                            innerFileName = ar != null ? ar.NameENUS : att.Name;
                        }
                        else
                        {
                            innerFileName = att.Name;
                        }


                        if (!attNames.Contains(innerFileName))
                        {
                            attNames.Add(innerFileName);
                            zip.AddEntry(innerFileName + att.Extension, fs);
                            zip.Save();
                        }
                    }
                }


            }
            string downLoadPath = "Temp/" + fileName + ".zip";
            return tempFilePath;
        }
    }
}