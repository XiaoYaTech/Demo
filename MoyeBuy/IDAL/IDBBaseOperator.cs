using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace MoyeBuy.Com.IDAL
{
    public interface IDBBaseOperator
    {
        DataSet ProcessData(string strComandText, Hashtable hshParamater, string strDSN);
    }
}
