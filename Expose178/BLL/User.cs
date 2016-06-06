using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;


namespace Expose178.Com.BLL
{
    public class User
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        public Expose178.Com.Model.User GetUserByUID(string strUID)
        {
            Expose178.Com.Model.User user = new Model.User();
            DataSet dsUser = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "UserID", strUID);
            dsUser = dbOperator.ProcessData("usp_GetUserByUserIDEmail", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsUser))
            {
                user.UserID = strUID;
                user.UserName = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "UserName");
                user.UserEmail = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "UserEmail");
                user.PwdHash = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "PwdHash");
                user.PwdSalt = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "PwdSalt");
                user.IsEffective = Gadget.GetDataRowBoolValue(dsUser.Tables[0].Rows[0], "IsEffective");
                user.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsUser.Tables[0].Rows[0], "LastUpdatedDate");
            }
            return user;
        }
        public Expose178.Com.Model.User GetUserByEmailID(string strEmailID)
        {
            Expose178.Com.Model.User user = new Model.User();
            DataSet dsUser = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "UserID", strEmailID);
            dsUser = dbOperator.ProcessData("usp_GetUserByUserIDEmail", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsUser))
            {
                user.UserID = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "UserID"); ;
                user.UserName = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "UserName");
                user.UserEmail = strEmailID;
                user.PwdHash = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "PwdHash");
                user.PwdSalt = Gadget.GetDataRowStringValue(dsUser.Tables[0].Rows[0], "PwdSalt");
                user.IsEffective = Gadget.GetDataRowBoolValue(dsUser.Tables[0].Rows[0], "IsEffective");
                user.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsUser.Tables[0].Rows[0], "LastUpdatedDate");
            }
            return user;
        }
        public bool AddUpdateUser(Model.User user)
        {
            DataSet dsUser = null;
            Hashtable hshPamra = new Hashtable();
            Gadget.Addparamater(ref hshPamra, "UserID", user.UserID);
            Gadget.Addparamater(ref hshPamra, "UserEmail", user.UserEmail);
            Gadget.Addparamater(ref hshPamra, "UserName", user.UserName);
            Gadget.Addparamater(ref hshPamra, "PwdHash", user.PwdHash);
            Gadget.Addparamater(ref hshPamra, "PwdSalt", user.PwdSalt);
            Gadget.Addparamater(ref hshPamra, "IsEffective", user.IsEffective==true?"1":"0");
            Gadget.Addparamater(ref hshPamra, "IsTempUser", user.IsTempUser == true ? "1" : "0");
            Gadget.Addparamater(ref hshPamra, "IP", user.IP);
            Gadget.Addparamater(ref hshPamra, "Browser", user.Browser);
            dsUser = dbOperator.ProcessData("usp_AddUpdateUserInfoByUserID", hshPamra, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsUser))
                return true;
            else
                return false;
        }
    }
}
