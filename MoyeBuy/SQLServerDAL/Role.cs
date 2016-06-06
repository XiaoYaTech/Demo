using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class Role : DALBase, IDAL.IRole
    {
        public IList<Model.Role> GetRole()
        {
            IList<Model.Role> listRole = null;
            DataSet ds = GetDataRole();
            if (Gadget.DatatSetIsNotNullOrEmpty(ds))
            {
                listRole = new List<Model.Role>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Model.Role role = new Model.Role();
                    role.RoleID = Gadget.GetDataRowStringValue(dr, "RoleID");
                    role.RoleName = Gadget.GetDataRowStringValue(dr, "RoleName");
                    role.RoleDesc = Gadget.GetDataRowStringValue(dr, "RoleDesc");
                    listRole.Add(role);
                }
            }
            return listRole;
        }

        public bool AddUpdateRole(Model.Role role)
        {
            return Gadget.DatatSetIsNotNullOrEmpty(AddUpdate(role));
        }
        private DataSet GetDataRole()
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            dsResult = dbOperator.ProcessData("usp_GetRole", hshParam, strServiceDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
        private DataSet AddUpdate(Model.Role role)
        {
            DataSet dsResult = null;
            try
            {
                Hashtable hshParam = new Hashtable();
                Gadget.Addparamater(ref hshParam, "RoleID", role.RoleID);
                Gadget.Addparamater(ref hshParam, "RoleName", role.RoleName);
                Gadget.Addparamater(ref hshParam, "RoleDesc", role.RoleDesc);
                dsResult = dbOperator.ProcessData("usp_AddUpdateRole", hshParam, strGiftDSN);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                    return dsResult;
                else
                    return null;
            }
            catch (Exception ex)
            { 
                
            }
            return dsResult;
        }
    }
}
