using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class Account : DALBase, IDAL.IAccount
    {

        public void LogOff()
        {
            throw new NotImplementedException();
        }

        public string Register(Model.User user)
        {
            string strUID = "";
            DataSet dsResult = this.AddUpdateUser(user);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
            {
                strUID = Gadget.GetDataRowStringValue(dsResult.Tables[0].Rows[0], "MoyeBuyComUserID");
            }
            return strUID;
        }

        public string LogOn(string strEmail, string strPwdHash, string strPwdSal)
        {
            DataSet dsUser = GetDataUser("", strEmail);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsUser))
                return WebConstant.UserNotExist;
            string strCurrPwdHash = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "MoyeBuyComPwdHash");
            string strCurrPwdSal = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "MoyeBuyComPwdSalt");
            if (strPwdHash == strCurrPwdHash && strPwdSal == strCurrPwdSal)
                return WebConstant.LoginSuccess;
            else
                return WebConstant.PwdIncorrect;
        }

        public bool ChangePassword(string strUID, string strNewPwdHash, string strNewPwdSal)
        {
            Model.User user = new Model.User();
            user.MoyeBuyComUserID = strUID;
            user.MoyeBuyComPwdHash = strNewPwdHash;
            user.MoyeBuyComPwdSalt = strNewPwdSal;
            return Gadget.DatatSetIsNotNullOrEmpty(this.AddUpdateUser(user));
        }

        public bool UpdateUser(Model.User user)
        {
            return Gadget.DatatSetIsNotNullOrEmpty(this.AddUpdateUser(user));
        }

        public IList<Model.User> GetUser(string strUID, string strEmail)
        {
            IList<Model.User> listUser = null;
            try
            {
                DataSet dsUser = GetDataUser(strUID, strEmail);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsUser))
                {
                    listUser = new List<Model.User>();
                    foreach (DataRow dr in dsUser.Tables[0].Rows)
                    {
                        Model.User user = new Model.User();
                        user.MoyeBuyComUserID = Gadget.GetDataRowStringValue(dr, "MoyeBuyComUserID");
                        user.MoyeBuyComUserName = Gadget.GetDataRowStringValue(dr, "MoyeBuyComUserName");
                        user.MoyeBuyComEmail = Gadget.GetDataRowStringValue(dr, "MoyeBuyComEmail");
                        user.UserPhoneNo = Gadget.GetDataRowStringValue(dr, "UserPhoneNo");
                        user.AddressID = Gadget.GetDataRowStringValue(dr, "AddressID");
                        user.IsEffective = Gadget.GetDataRowBoolValue(dr, "IsEffective");
                        user.IsNeedChangePwd = Gadget.GetDataRowBoolValue(dr, "IsNeedChangePwd");
                        user.MoyeBuyComPwdHash = Gadget.GetDataRowStringValue(dr, "MoyeBuyComPwdHash");
                        user.MoyeBuyComPwdSalt = Gadget.GetDataRowStringValue(dr, "MoyeBuyComPwdSalt");
                        user.RoleDesc = Gadget.GetDataRowStringValue(dr, "RoleDesc");
                        user.RoleID = Gadget.GetDataRowStringValue(dr, "RoleID");
                        user.RoleName = Gadget.GetDataRowStringValue(dr, "RoleName");
                        user.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                        user.Amount = Gadget.GetDataRowDecimalValue(dr, "Amount");
                        user.GatherGrade = Gadget.GetDataRowDecimalValue(dr, "GatherGrade");
                        listUser.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Account.GetUser()", UtilityFactory.LogType.LogToFile);
            }
            return listUser;
        }

        private DataSet GetDataUser(string strUID, string strEmail)
        {
            DataSet dsAds = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "UserID", strUID);
            Gadget.Addparamater(ref hshParam, "Email", strEmail);
            dsAds = dbOperator.ProcessData("usp_GetUserByUserIDEmail", hshParam, strServiceDSN);
            return dsAds;
        }

        private DataSet AddUpdateUser(Model.User user)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                Gadget.Addparamater(ref hshParam, "UserID", user.MoyeBuyComUserID);
                Gadget.Addparamater(ref hshParam, "UserPhoneNo", user.UserPhoneNo);
                Gadget.Addparamater(ref hshParam, "MoyeBuyComUserName", user.MoyeBuyComUserName);
                Gadget.Addparamater(ref hshParam, "MoyeBuyComEmail", user.MoyeBuyComEmail);
                Gadget.Addparamater(ref hshParam, "MoyeBuyComPwdSalt", user.MoyeBuyComPwdSalt);
                Gadget.Addparamater(ref hshParam, "MoyeBuyComPwdHash", user.MoyeBuyComPwdHash);
                Gadget.Addparamater(ref hshParam, "RoleID", user.RoleID);
                Gadget.Addparamater(ref hshParam, "AddressID", user.AddressID);
                Gadget.Addparamater(ref hshParam, "IsEffective", user.IsEffective);
                Gadget.Addparamater(ref hshParam, "IsNeedChangePwd", user.IsNeedChangePwd);
                Gadget.Addparamater(ref hshParam, "Amount", user.Amount);
                Gadget.Addparamater(ref hshParam, "GatherGrade", user.GatherGrade);
                dsResult = dbOperator.ProcessData("usp_AddUpdateUserInfoByUserID", hshParam, strServiceDSN);
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Account.AddUpdateUser()", UtilityFactory.LogType.LogToFile);
            }
            return dsResult;
        }

        public bool DelUser(string strUID)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                Gadget.Addparamater(ref hshParam, "UserID", strUID);
                dsResult = dbOperator.ProcessData("", hshParam, strServiceDSN);
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Account.DelUser()", UtilityFactory.LogType.LogToFile);
                dsResult = null;
            }
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        public string GetUserSalt(string strEmail)
        {
            string strSalt = "";
            DataSet ds = GetDataUser("", strEmail);
            if (Gadget.DatatSetIsNotNullOrEmpty(ds))
            {
                strSalt = Gadget.GetDataRowStringValue(ds.Tables[0].Rows[0], "MoyeBuyComPwdSalt");
            }
            return strSalt;
        }
    }
}
