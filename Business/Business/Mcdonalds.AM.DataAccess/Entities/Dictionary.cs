using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class Dictionary : BaseEntity<Dictionary>
    {
        public string NameDisp
        {
            get
            {
                if (ClientCookie.Language == Infrastructure.SystemLanguage.ENUS)
                    return NameENUS;
                else
                    return NameZHCN;
            }
        }

        public static List<Dictionary> GetDictionaryList(string strCode)
        {
            return Search(e => e.Code.Equals(strCode)).ToList();
        }
        public static List<Dictionary> GetDictionaryListByParentCode(string strCode)
        {
            return Search(e => e.ParentCode.Equals(strCode)).ToList();
        }

        public static Dictionary GetDictionary(string strCode)
        {
            return FirstOrDefault(e => e.Code.Equals(strCode));
        }

        public static Dictionary<string, Dictionary> GetDictionary(Dictionary<string, string> listCode)
        {
            var dic = new Dictionary<string, Dictionary>();
            var list = Search(e => listCode.Values.Contains(e.Code)).ToList();
            foreach (var code in listCode)
            {
                foreach (var entity in list)
                {
                    if (entity.Code == code.Value)
                    {
                        dic.Add(code.Key, entity);
                        break;
                    }
                }
            }
            return dic;
        }

        public static string ParseDisplayName(string code)
        {
            var displayName = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {
                var dic = FirstOrDefault(e => e.Code == code);
                if (dic != null)
                {
                    displayName = dic.NameENUS;
                }
            }

            return displayName;
        }
    }
}
