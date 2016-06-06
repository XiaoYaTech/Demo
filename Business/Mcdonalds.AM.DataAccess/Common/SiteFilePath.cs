using Mcdonalds.AM.Services.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common
{
    public class SiteFilePath
    {


        public static string ROOTPATH
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/"); }
        }

        public static string ConvertToDiskPath(string url)
        {
            return System.Web.HttpContext.Current.Server.MapPath(url);
        }


        /// <summary>
        /// 模板目录的路径
        /// </summary>
        public static string Template_DIRECTORY
        {
            get
            {
                return ROOTPATH + "Template";
            }
        }


        public static string UploadFiles_DIRECTORY
        {
            get
            {
                return ROOTPATH + "UploadFiles";

            }
        }

        public static string UploadFiles_URL
        {
            get
            {
                return SiteInfo.ServiceUrl + "UploadFiles/";
            }
        }

        /// <summary>
        /// 临时文件的路径
        /// </summary>
        public static string TEMP_DIRECTORY
        {
            get
            {
                return ROOTPATH + "TEMP";
            }
        }


        /// <summary>
        /// 临时目录的路径
        /// </summary>
        public const string FATool_Update_Template = "FA Tool.xlsx";
        public const string FAWrite_offTool_Template = "Write-off Tool.xlsx";
        public const string Closure_FAWrite_offTool_Template = "Write-off Tool.xlsx";
        public const string FAWrite_offTool_Template_X = "Write-off Tool.xlsx";
        public const string Executive_Summary_Template = "Executive Summary.xlsx";
        public const string Store_Closure_Cover_Template = "Store Closure Cover_Template_ v20120716.xlsx";
        public const string Store_Closure_Tool_Template = "Closure Tool.xlsx";
        public const string MajorLeaseChangeCove_Template = "MajorLeaseChangeCover_Template_v20130910.xlsx";
        public const string RebuildCove_Template = "RebuildCover_Template_v20130910.xlsx";
        public const string Reimage_Summary_Template = "Reimage Summary.xlsx";
        public const string Renewal_LLNegotiationRecord_Template = "Renewal_LLNegotiationRecord_Template.xlsx";
        public const string RenewalAnalysis_Template = "Renewal Analysis.xlsx";
        public const string RenewalCover_Template = "RenewalCover_Template_v20130922.xlsx";
        public const string RenewalTool_Template = "Renewal Tool.xlsx";
        public const string Store_Reimage_Cover_Template = "ReimageCover_Template_v20130910.xlsx";
        public const string CommentsList_Template = "CommentsList_Template .xlsx";
        public const string Store_TempClosure_Cover_Template = "Store Closure Cover_Template_ v20120716.xlsx";


        //PMT附件存放路径
        public static string PMTAttachmentPath
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/") + "UploadFilesPMT\\";
            }
        }

        public static string PMTAttachmenttURL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ContractAttachServer"];
            }
        }

        public static string GetTemplateFileName(string uscode, string flowCode, string fileName)
        {
            return DataConverter.ToHexString(uscode + " " + flowCode + " " + fileName);
        }
    }
}
