using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Infrastructure
{
    public class I18N
    {
        public static string GetValue(object obj, string key)
        {
            var value = obj.GetType().GetProperty(key + ClientCookie.Language).GetValue(obj);
            return value == null ?"":value.ToString();
        }

        public static string BoolValue(bool value)
        {
            switch (ClientCookie.Language)
            {
                case SystemLanguage.ZHCN:
                    return value ? "是" : "否";
                case SystemLanguage.ENUS:
                    return value ? "Yes" : "No";
                default:
                    return value ? "Yes" : "No";
            }
        }
    }
}
