using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.IDAL
{
    public interface IAccount
    {
        void LogOff();
        string Register(Model.User user);
        string LogOn(string strEmail, string strPwdHash, string strPwdSal);
        bool ChangePassword(string strUID,string strNewPwdHash,string strNewPwdSal);
        bool UpdateUser(Model.User user);
        IList<Model.User> GetUser(string strUID, string strEmail);
        bool DelUser(string strUID);
        string GetUserSalt(string strEmail);
    }
}
