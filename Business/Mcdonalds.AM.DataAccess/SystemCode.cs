using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/9/2014 8:17:24 PM
 * FileName     :   SystemCode
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Mcdonalds.AM.DataAccess
{
    public class SystemCode
    {
        private static SystemCode _instance = null;
        public static SystemCode Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SystemCode();
                }
                return _instance;
            }
        }
        private XDocument doc;
        internal SystemCode()
        {
            doc = XDocument.Load(HttpContext.Current.Server.MapPath("~/SystemCode.xml"));
        }

        public string GetCodeName(string code, SystemLanguage lang)
        {
            var result = "";
            var element = doc.Root.Elements("Code").FirstOrDefault(e => e.Attribute("Key")!= null && e.Attribute("Key").Value == code);
            if (element != null)
            {
                var attr = element.Attribute(lang.ToString());
                result = attr != null ? attr.Value : "";
            }
            return result;
        }
    }
}
