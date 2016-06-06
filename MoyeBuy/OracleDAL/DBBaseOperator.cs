using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using MoyeBuy.Com.IDAL;

namespace MoyeBuy.Com.OracleDAL
{
    public class DBBaseOperator : IDBBaseOperator
    {
        public System.Data.DataSet ProcessData(string strComandText, System.Collections.Hashtable hshParamater, string strDSN)
        {
            throw new NotImplementedException();
        }
    }
}
