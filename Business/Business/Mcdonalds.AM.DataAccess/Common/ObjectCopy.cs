using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common
{
  
        public class ObjectCopy
        {
            /// <summary>
            /// 复制对象副本
            /// </summary>
            /// <typeparam name="TObject"></typeparam>
            /// <param name="originalObj"></param>
            /// <returns></returns>
            public TObject AutoCopy<TObject>(TObject originalObj) where TObject:new()
            {
                TObject newObj = new TObject();
                var OriginalType = typeof(TObject);
                var Properties = OriginalType.GetProperties();
                foreach (var Propertie in Properties)
                {
                    //循环遍历属性                
                    if (Propertie.CanRead && Propertie.CanWrite)
                    {
                        //进行属性拷贝                  
                        Propertie.SetValue(newObj, Propertie.GetValue(originalObj, null), null);
                    }
                }
                return newObj;
            }
        }

}
