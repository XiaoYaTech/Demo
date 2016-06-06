using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class ReadRole
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        private DataSet GetDataSetReadRole(string strReadRoleTypeCode)
        {
            DataSet dsReadRole = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ReadRoleTypeCode", strReadRoleTypeCode);
            dsReadRole = dbOperator.ProcessData("usp_GetReadRoleType", hshParam, strDSN);
            return dsReadRole;
        }

        public IList<Expose178.Com.Model.ReadRoleType> GetListReadRole()
        {
            DataSet dsReadRole = GetDataSetReadRole("");
            IList<Expose178.Com.Model.ReadRoleType> listReadRole = null;
            if (Gadget.DatatSetIsNotNullOrEmpty(dsReadRole))
            {
                listReadRole = new List<Expose178.Com.Model.ReadRoleType>();
                foreach (DataRow dr in dsReadRole.Tables[0].Rows)
                {
                    Expose178.Com.Model.ReadRoleType read = new Model.ReadRoleType();
                    read.ReadRoleTypeCode = Gadget.GetDataRowStringValue(dr, "ReadRoleTypeCode");
                    read.ReadRoleTypeDesc = Gadget.GetDataRowStringValue(dr, "ReadRoleTypeDesc");
                    read.UpdatedByUserID = Gadget.GetDataRowStringValue(dr, "UpdatedByUserID");
                    read.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                    listReadRole.Add(read);
                }
            }
            return listReadRole;
        }

        public Expose178.Com.Model.ReadRoleType GetReadRole(string strReadRoleTypeCode)
        {
            DataSet dsReadRole = GetDataSetReadRole("");
            Expose178.Com.Model.ReadRoleType read = null;
            if (Gadget.DatatSetIsNotNullOrEmpty(dsReadRole))
            {
                read = new Model.ReadRoleType();
                read.ReadRoleTypeCode = Gadget.GetDataRowStringValue(dsReadRole.Tables[0].Rows[0], "ReadRoleTypeCode");
                read.ReadRoleTypeDesc = Gadget.GetDataRowStringValue(dsReadRole.Tables[0].Rows[0], "ReadRoleTypeDesc");
                read.UpdatedByUserID = Gadget.GetDataRowStringValue(dsReadRole.Tables[0].Rows[0], "UpdatedByUserID");
                read.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsReadRole.Tables[0].Rows[0], "LastUpdatedDate");
            }
            return read;
        }
    }
}
