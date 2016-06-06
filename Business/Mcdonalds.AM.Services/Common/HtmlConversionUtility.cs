using ExpertPdf.HtmlToPdf;
using HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mcdonalds.AM.Services.Common
{
    public static class HtmlConversionUtility
    {
        /// <summary>
        /// 根据文件将html转换为image
        /// </summary>
        /// <param name="templateFile">html模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>生产的图片所存放的物理路径</returns>
        public static string ConvertToImage(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var image = HtmlConvertToImageObject(templateType, values, records);
            string filePath = ConfigurationManager.AppSettings["HtmlConversionFolder"];
            string serverPath = HttpContext.Current.Request.PhysicalApplicationPath + filePath;
            string fileNameWithPath = serverPath + @"\" + Guid.NewGuid().ToString() + ".png";
            image.Save(fileNameWithPath);
            return fileNameWithPath;
        }

        /// <summary>
        /// 根据文件将html转换为image
        /// SubmissionAndApprovalRecords用recordDetails
        /// SubmissionApprovalRecord用records
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="values"></param>
        /// <param name="records"></param>
        /// <param name="recordDetails"></param>
        /// <returns></returns>
        public static string ConvertToImage(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records, List<SubmissionApprovalRecord> recordDetails)
        {
            var image = HtmlConvertToImageObject(templateType, values, records, recordDetails);
            string filePath = ConfigurationManager.AppSettings["HtmlConversionFolder"];
            string serverPath = HttpContext.Current.Request.PhysicalApplicationPath + filePath;
            string fileNameWithPath = serverPath + @"\" + Guid.NewGuid().ToString() + ".png";
            image.Save(fileNameWithPath);
            return fileNameWithPath;
        }

        /// <summary>
        /// 根据文件将html转换为image
        /// </summary>
        /// <param name="templateFile">html模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>图片对象</returns>
        public static System.Drawing.Image HtmlConvertToImageObject(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records);
            System.Drawing.Image image = HtmlRender.RenderToImage(html);
            return image;
        }

        /// <summary>
        /// 根据文件将html转换为image
        /// SubmissionAndApprovalRecords用recordDetails
        /// SubmissionApprovalRecord用records
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="values"></param>
        /// <param name="records"></param>
        /// <param name="recordDetails"></param>
        /// <returns></returns>
        public static System.Drawing.Image HtmlConvertToImageObject(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records, List<SubmissionApprovalRecord> recordDetails)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records, recordDetails);
            System.Drawing.Image image = HtmlRender.RenderToImage(html);
            return image;
        }

        /// <summary>
        /// 将HTML文件转换为PDF字节流
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF字节流</returns>
        public static byte[] ConvertToPDFBytes(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records);
            var pdfConverter = new PdfConverter();
            var bytes = pdfConverter.GetPdfBytesFromHtmlString(html);
            return bytes;
        }

        /// <summary>
        /// 将HTML文件转换为PDF字节流
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF字节流</returns>
        public static byte[] ConvertToPDFBytesWithComments(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplateWithComments(templateType, values, records);
            var pdfConverter = new PdfConverter();
            var bytes = pdfConverter.GetPdfBytesFromHtmlString(html);
            return bytes;
        }

        /// <summary>
        /// 将HTML文件转换为PDF文件流
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF文件流</returns>
        public static Stream HtmlConvertToPDFStream(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records);
            var pdfConverter = new PdfConverter();
            Stream stream = null;
            pdfConverter.SavePdfFromHtmlStringToStream(html, stream);
            return stream;
        }

        /// <summary>
        /// 将HTML文件转换为PDF文件流
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF文件流</returns>
        public static Stream HtmlConvertToPDFStreamWithComments(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplateWithComments(templateType, values, records);
            var pdfConverter = new PdfConverter();
            Stream stream = null;
            pdfConverter.SavePdfFromHtmlStringToStream(html, stream);
            return stream;
        }


        /// <summary>
        /// 将HTML文件转换为PDF文件
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF文件路径</returns>
        public static string HtmlConvertToPDF(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records);
            var pdfConverter = new PdfConverter();
            string filePath = ConfigurationManager.AppSettings["HtmlConversionFolder"];
            string serverPath = HttpContext.Current.Request.PhysicalApplicationPath + filePath;
            string fileNameWithPath = serverPath + @"\" + Guid.NewGuid().ToString() + ".pdf";
            pdfConverter.SavePdfFromHtmlStringToFile(html, fileNameWithPath);
            return fileNameWithPath;
        }

        /// <summary>
        /// 将HTML文件转换为PDF文件
        /// SubmissionAndApprovalRecords用recordDetails
        /// SubmissionApprovalRecord用records
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="values"></param>
        /// <param name="records"></param>
        /// <param name="recordDetails"></param>
        /// <returns></returns>
        public static string HtmlConvertToPDF(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records, List<SubmissionApprovalRecord> recordDetails)
        {
            var html = GenerateHtmlFromTemplate(templateType, values, records, recordDetails);
            var pdfConverter = new PdfConverter();
            string filePath = ConfigurationManager.AppSettings["HtmlConversionFolder"];
            string serverPath = HttpContext.Current.Request.PhysicalApplicationPath + filePath;
            string fileNameWithPath = serverPath + @"\" + Guid.NewGuid().ToString() + ".pdf";
            if (templateType == HtmlTempalteType.RenewalLegalApproval)
                fileNameWithPath = serverPath + @"\RenewalLegalAF" + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
            pdfConverter.SavePdfFromHtmlStringToFile(html, fileNameWithPath);
            return fileNameWithPath;
        }

        /// <summary>
        /// 将HTML文件转换为PDF文件
        /// </summary>
        /// <param name="templateFile">模板文件名称</param>
        /// <param name="values">需要填充的值</param>
        /// <param name="impactOnCurrentYearData">表格形式的值</param>
        /// <param name="futureImpactData">表格形式的值</param>
        /// <returns>PDF文件路径</returns>
        public static string HtmlConvertToPDFWithComments(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            var html = GenerateHtmlFromTemplateWithComments(templateType, values, records);
            var pdfConverter = new PdfConverter();
            string filePath = ConfigurationManager.AppSettings["HtmlConversionFolder"];
            string serverPath = HttpContext.Current.Request.PhysicalApplicationPath + filePath;
            string fileNameWithPath = serverPath + @"\" + Guid.NewGuid().ToString() + ".pdf";
            pdfConverter.SavePdfFromHtmlStringToFile(html, fileNameWithPath);
            return fileNameWithPath;
        }

        /// <summary>
        /// 根据HTML模板和业务数据获取html内容
        /// </summary>
        /// <param name="templateFile">html模板文件</param>
        /// <param name="values">业务数据的键值对</param>
        /// <param name="impactOnCurrentYearData">表格数据</param>
        /// <param name="futureImpactData">表格数据</param>
        /// <returns>html格式的内容</returns>
        public static string GenerateHtmlFromTemplate(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            values.Add("SubmissionAndApprovalRecords", GenerateTableHtml(records));
            values.Add("SubmissionAndApprovalRecordsWithComments", GenerateTableHtmlAdding(records));
            string file = GetTemplateFile(templateType);
            string html = GenerateHtmlContentByTemplate(file, values);
            return html;
        }

        /// <summary>
        /// 根据HTML模板和业务数据获取html内容
        /// SubmissionAndApprovalRecords用recordDetails
        /// SubmissionApprovalRecord用records
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="values"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public static string GenerateHtmlFromTemplate(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records, List<SubmissionApprovalRecord> recordDetails)
        {
            values.Add("SubmissionAndApprovalRecords", GenerateTableHtml(records));
            values.Add("SubmissionAndApprovalRecordsWithComments", GenerateTableHtmlAdding(recordDetails));
            string file = GetTemplateFile(templateType);
            string html = GenerateHtmlContentByTemplate(file, values);
            return html;
        }

        /// <summary>
        /// 根据HTML模板和业务数据获取html内容
        /// </summary>
        /// <param name="templateFile">html模板文件</param>
        /// <param name="values">业务数据的键值对</param>
        /// <param name="impactOnCurrentYearData">表格数据</param>
        /// <param name="futureImpactData">表格数据</param>
        /// <returns>html格式的内容</returns>
        public static string GenerateHtmlFromTemplateWithComments(HtmlTempalteType templateType, Dictionary<string, string> values, List<SubmissionApprovalRecord> records)
        {
            values.Add("SubmissionAndApprovalRecords", GenerateTableHtml(records));
            values.Add("SubmissionAndApprovalRecordsWithComments", GenerateTableHtmlAdding(records));
            string file = GetTemplateFile(templateType);
            string html = GenerateHtmlContentByTemplate(file, values);
            return html;
        }

        private static string GetTemplateFile(HtmlTempalteType templateType)
        {
            string file = string.Empty;
            switch (templateType)
            {
                case HtmlTempalteType.Default:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.DefaultHtmlTemplate;
                    break;
                case HtmlTempalteType.DefaultAdding:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.DefaultHtmlTemplateAdding;
                    break;
                case HtmlTempalteType.DefaultWithComments:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.DefaultHtmlTemplateWithComments;
                    break;
                case HtmlTempalteType.ClosureMemo:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.ClosureMemoHtmlTemplate;
                    break;
                case HtmlTempalteType.MajorLease:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.MajorLeaseHtmlTemplate;
                    break;
                case HtmlTempalteType.TempClosure:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.TempClosureHtmlTemplate;
                    break;
                case HtmlTempalteType.TempClosureReopenMemo:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.TempClosureReopenMemoHtmlTemplate;
                    break;
                case HtmlTempalteType.Reimage:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.ReimageHtmlTemplate;
                    break;
                case HtmlTempalteType.GBMemo:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.GBMemoHtmlTemplate;
                    break;
                case HtmlTempalteType.ReopenMemo:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.ReopenMemoHtmlTemplate;
                    break;
                case HtmlTempalteType.Renewal:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.RenewalHtmlTemplate;
                    break;
                case HtmlTempalteType.RenewalLegalApproval:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.RenewalLegalApprovalTemplate;
                    break;
                case HtmlTempalteType.RebuildPackage:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.RebuildPackageHtmlTemplate;
                    break;
                default:
                    file = HttpContext.Current.Request.PhysicalApplicationPath + Constants.DefaultHtmlTemplate;
                    break;
            }
            return file;
        }

        /// <summary>
        /// 根据数据表生产表控件的HTML.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GenerateTableHtml(List<SubmissionApprovalRecord> records)
        {
            var recordsHtml = string.Empty;
            //if (records != null && records.Count > 0)
            //{
            //    recordsHtml += "<tr>";
            //    string tableHtml = "<td><table width=\"413\" cellpadding=\"0\" cellspacing=\"0\" style=\"padding: 0 20px; border-right: 1px dashed #ddd; font-size: 8pt;\">{0}</table></td>";
            //    string oddHtml = string.Empty;
            //    string evenHtml = string.Empty;
            //    for (int i = 0; i < records.Count; i++)
            //    {
            //        if (!i.IsOdd())//--0,2,4,6 偶数行
            //        {
            //            evenHtml += "<tr>";
            //            if (i < records.Count)
            //            {
            //                evenHtml += string.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", records[i].OperatorName, records[i].OperatorTitle, records[i].OperationDate.ToShortDateString(), records[i].ActionName);
            //            }
            //            evenHtml += "</tr>";
            //        }
            //        else //--1,3,5,7 奇数行
            //        {
            //            oddHtml += "<tr>";
            //            if (i < records.Count)
            //            {
            //                oddHtml += string.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", records[i].OperatorName, records[i].OperatorTitle, records[i].OperationDate.ToShortDateString(), records[i].ActionName);
            //            }
            //            oddHtml += "</tr>";
            //        }
            //        if (records.Count.IsOdd() && (i + 1 == records.Count))
            //        {
            //            oddHtml += "<tr>";
            //            oddHtml += string.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", "-", "", "", "");
            //            oddHtml += "</tr>";
            //        }
            //    }
            //    //偶数列
            //    if (!string.IsNullOrEmpty(evenHtml))
            //    {
            //        recordsHtml += string.Format(tableHtml, evenHtml);
            //    }
            //    //奇数列
            //    if (!string.IsNullOrEmpty(oddHtml))
            //    {
            //        recordsHtml += string.Format(tableHtml, oddHtml);
            //    }
            //    recordsHtml += "</tr>";
            //}
            if (records != null && records.Count > 0)
            {
                recordsHtml += "<tr>";
                string tableHtml = "<td><table width=\"826\" cellpadding=\"0\" cellspacing=\"0\" style=\"padding: 0 20px; border-right: 1px dashed #ddd; font-size: 8pt;\">{0}</table></td>";
                string Html = string.Empty; ;
                for (int i = 0; i < records.Count; i++)
                {
                    Html += "<tr>";
                    if (i < records.Count)
                    {
                        Html += string.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", records[i].OperatorName, records[i].OperatorTitle, records[i].OperationDate.ToShortDateString(), records[i].ActionName);
                    }
                    Html += "</tr>";
                }
                recordsHtml += string.Format(tableHtml, Html);
                recordsHtml += "</tr>";
            }
            return recordsHtml;
        }

        /// <summary>
        /// 根据数据表生产表控件的HTML-带Comments的审批记录.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GenerateTableHtmlAdding(List<SubmissionApprovalRecord> records)
        {
            var recordsHtml = string.Empty;
            if (records != null && records.Count > 0)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    recordsHtml += string.Format("<tr><td width=\"25%\">{0}</td><td width=\"25%\">{1}</td><td width=\"25%\">{2}</td><td width=\"25%\">{3}</td></tr>", records[i].OperatorName, records[i].OperatorTitle, records[i].OperationDate.ToShortDateString(), records[i].ActionName);
                    recordsHtml += string.Format("<tr><td colspan=\"4\" style=\"line-height: 18px; padding-bottom: 10px;\"><pre>{0}</pre></td></tr>", records[i].Comments);
                }
            }
            return recordsHtml;
        }

        /// <summary>
        /// 通过模板获取html的内容
        /// </summary>
        /// <param name="templatePath">html模板</param>
        /// <param name="values">参数值</param>
        public static string GenerateHtmlContentByTemplate(string templatePath, Dictionary<string, string> values)
        {
            return GenerateHtmlContentByTemplate(templatePath, values, "[$", "]");
        }


        /// <summary>
        /// 读取文件html模板文件并生产html内容
        /// </summary>
        /// <param name="templatePath">模板路径</param>
        /// <param name="emailContentValues">需要替换的键值对</param>
        /// <param name="prefix">前缀</param>
        /// <param name="postfix">后缀</param>
        /// <returns>html内容</returns>
        private static string GenerateHtmlContentByTemplate(string templatePath, Dictionary<string, string> values, string prefix, string postfix)
        {
            string template = string.Empty;
            //读取模板文件（html格式的文件）
            using (var reader = new StreamReader(templatePath))
            {
                template = reader.ReadToEnd();
            }

            string htmlContent = string.Empty;
            if (values != null)
            {
                htmlContent = values.Keys.Aggregate(template, (current, key) => current.Replace(string.Format("{0}{1}{2}", prefix, key, postfix), values[key]));
            }
            return htmlContent;
        }

        /// <summary>
        /// 空间渲染
        /// </summary>
        /// <param name="control">被渲染的空间对象</param>
        /// <returns>字符串</returns>
        public static string RenderControl(Control control)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    control.RenderControl(htw);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从字符串中获取指定的两个字符之间的内容
        /// </summary>
        /// <param name="str">原始的字符串</param>
        /// <param name="start">开始的字符串</param>
        /// <param name="end">结束的字符串</param>
        /// <returns>开始和结束之间的字符串</returns>
        public static string GetValueFromString(this string str, string start, string end)
        {
            var rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);

            return rg.Match(str).Value;
        }

        /// <summary>
        /// 将对象的属性转换为NameValue Collection
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>NameValueCollection</returns>
        public static NameValueCollection ToNameValueCollection(this object obj)
        {
            NameValueCollection values = new NameValueCollection();
            Type type = obj.GetType();
            foreach (PropertyInfo info in type.GetProperties())
            {
                values.Add(info.Name, info.GetValue(obj).ToString());
            }
            return values;
        }

        /// <summary>
        /// 奇数偶数判断
        /// </summary>
        /// <param name="n">整数</param>
        /// <returns>是否奇数</returns>
        public static bool IsOdd(this int n)
        {
            return Convert.ToBoolean(n % 2);
        }

        /// <summary>
        /// 将图片对象转换为base 64编码的字符串
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// 将图片的URL地址转换为base 64编码的字符串。
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(string imageUrl, System.Drawing.Imaging.ImageFormat format)
        {
            string base64String = "data:image/png;base64,";
            using (WebClient wc = new WebClient())
            {
                byte[] bytes = wc.DownloadData(imageUrl);
                MemoryStream ms = new MemoryStream(bytes);
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                base64String += ImageToBase64(img, System.Drawing.Imaging.ImageFormat.Png);
            }
            return base64String;
        }

        /// <summary>
        /// 将base 64编码的字符串转换为图片对象
        /// </summary>
        /// <param name="base64String">base 64编码的字符串</param>
        /// <returns></returns>
        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        /// <summary>
        /// 从邮件中的任务连接跳转到Portal，然后进入详细任务页面的方式。（不需要认证即可直接进入）
        /// </summary>
        /// <param name="EID">邮件接收人</param>
        /// <param name="detailUrl">需要跳转的URL地址</param>
        /// <param name="language">当前的语言环境</param>
        /// <returns>返回的url直接返回给邮件中的Form Url即可</returns>
        public static string FormatPortalUrl(string EID, string detailUrl, string language)
        {
            //TODO:: 需要把portalUrlFormat写进配置文件，然后从配置文件读取。
            const string portalUrlFormat = "http://portal.mcd.com.cn:4994/LinkRedirect.aspx?SystemId={0}&EID={1}&URL={2}&LanguageId={3}&IsHttps=0";
            
            //需要跳转的邮件中的url一定要用url encode，否则有特殊的字符如#号等，会被浏览器认为i额非法参数。
            //TODO::加密EID的第二个参数“PortalLinkRedirect” 写进配置文件，然后从配置文件读取。
            //需要把邮件综合那个跳转的url 的http:// 去掉。
            //返回的url直接返回给邮件中的view url即可。
            var url = string.Format(portalUrlFormat, 8,
                Cryptography.Encrypt(EID, "PortalLinkRedirect", "oms"), HttpUtility.UrlEncode(detailUrl.Replace("http://", string.Empty)), language);
            return url;
        }
    }

    /// <summary>
    /// 提交和审批日志记录对象
    /// </summary>
    public class SubmissionApprovalRecord
    {
        /// <summary>
        /// 操作人的ID
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作步骤名称
        /// </summary>
        public string OperatorTitle { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperationDate { get; set; }

        /// <summary>
        /// 动作名称： e.g.: Submit/Approval
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Comments详细信息
        /// </summary>
        public string Comments { get; set; }
    }


}