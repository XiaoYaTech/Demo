using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.DALFactory;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class DALBase
    {
        protected static readonly MoyeBuy.Com.IDAL.IDBBaseOperator dbOperator = DataAcess.CreateDBBaseOperator();
        protected static readonly string strDSN = Gadget.GetConnectionString("MoyeBuyCom");
        protected static readonly string strGiftDSN = Gadget.GetConnectionString("GiftCardService");
        protected static readonly string strServiceDSN = Gadget.GetConnectionString("MoyeBuyComServices");
        protected static readonly string strOrderDSN = Gadget.GetConnectionString("MoyeBuyComOrder");
        protected static readonly string strLogDSN = Gadget.GetConnectionString("MoyeBuyComLog");
        public DALBase() { }
    }
}
