using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using MoyeBuy.Com.IDAL;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.DALFactory;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.BLL
{
    public class Account
    {
        private static readonly IDAL.IAccount dal = DALFactory.DataAcess.CreateUser();
        public void LogOff()
        { 
            //
        }
        public string Register(Model.User user)
        {
            string strPwdSalt = Gadget.SalGenerateSalt(10);
            string strPwdHash = Gadget.SalGenerateHashCode(user.MoyeBuyComPwdHash + strPwdSalt);
            user.IsNeedChangePwd =false;
            user.MoyeBuyComPwdHash = strPwdHash;
            user.MoyeBuyComPwdSalt = strPwdSalt;
            return dal.Register(user);
        }

        public string LogOn(string strEmail, string strPwd)
        {
            string strPwdHash="";
            string strPwdSalt = "";
            IList<Model.User> listUser = dal.GetUser("", strEmail);
            if (listUser != null)
            {
                Model.User user = listUser[0];
                strPwdSalt = user.MoyeBuyComPwdSalt;
                strPwdHash = user.MoyeBuyComPwdHash;

                string strNewPwdHash = Gadget.SalGenerateHashCode(strPwd + strPwdSalt);

                if (strNewPwdHash == strPwdHash)
                    return WebConstant.LoginSuccess;
                else
                    return WebConstant.PwdIncorrect;
            }
            else
                return WebConstant.UserNotExist;
        }

        public bool ChangePassword(string strUID, string strNewPwd)
        {
            string strPwdHash = Gadget.SalGenerateSalt(10);
            string strPwdSalt = Gadget.SalGenerateHashCode(strNewPwd+strPwdHash);
            return dal.ChangePassword(strUID, strPwdHash, strPwdSalt);
        }

        public bool UpdateUser(Model.User user)
        {
            return dal.UpdateUser(user);
        }

        public IList<Model.User> GetUserByUID(string strUID)
        {
            IList<Model.User> listUser = dal.GetUser(strUID,"");
            return listUser;
        }

        public IList<Model.User> GetUserByEmail(string strEmail)
        {
            if (string.IsNullOrEmpty(strEmail))
                return null;
            IList<Model.User> listUser = dal.GetUser("", strEmail);
            return listUser;
        }

        public IList<Model.User> GetUser()
        {
            IList<Model.User> listUser = dal.GetUser("", "");
            return listUser;
        }

        public bool DelUser(string strUID)
        {
            return dal.DelUser(strUID);
        }
    }
}
