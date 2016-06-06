using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class Login
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        public Model.User LoginProcess(string strEmailID,string strPwd)
        {
            BLL.User bllUser = new User();
            Model.User mUser = bllUser.GetUserByEmailID(strEmailID);
            string strPwdSalt = mUser.PwdSalt;
            string strPwdHash = mUser.PwdHash;
            string strNewPwdHash = Encryption.SalGenerateHashCode(strPwd + strPwdSalt);
            if (strNewPwdHash == strPwdHash)
                return mUser;
            else
                return null;
        }
    }
}
