using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MoyeBuy.Com.IUtility
{
    public interface Ilog
    {
        void WriteLog(Hashtable hshParam, string strPosition);
    }
}
