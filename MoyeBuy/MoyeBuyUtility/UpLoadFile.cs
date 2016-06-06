using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class UpLoadFile
    {
        HttpContextBase httpContex = null;
        public UpLoadFile(HttpContextBase httpContex)
        {
            this.httpContex = httpContex;
        }
        public bool UpLoad()
        {
            return true;
        }
    }
}
