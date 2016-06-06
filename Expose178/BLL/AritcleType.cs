using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class AritcleType
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        public IList<Expose178.Com.Model.AritcleType> GetListArticleType()
        {
            IList<Expose178.Com.Model.AritcleType> listAritcleType = null;
            DataSet dsArticleType = GetDataSetArticleType("");
            if (Gadget.DatatSetIsNotNullOrEmpty(dsArticleType))
            {
                listAritcleType = new List<Expose178.Com.Model.AritcleType>();
                foreach (DataRow dr in dsArticleType.Tables[0].Rows)
                {
                    Expose178.Com.Model.AritcleType mAritcleType = new Model.AritcleType();
                    mAritcleType.AritcleTypeCode = Gadget.GetDataRowStringValue(dr, "AritcleTypeCode");
                    mAritcleType.AritcleTypeDesc = Gadget.GetDataRowStringValue(dr, "AritcleTypeDesc");
                    mAritcleType.UpdatedByUserID = Gadget.GetDataRowStringValue(dr, "UpdatedByUserID");
                    mAritcleType.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                    listAritcleType.Add(mAritcleType);
                }
            }
            return listAritcleType;
        }

        public Expose178.Com.Model.AritcleType GetArticleType(string strArticleTypeCode)
        {
            DataSet dsArticleType = GetDataSetArticleType(strArticleTypeCode);
            Expose178.Com.Model.AritcleType mAritcleType = null;
            if (Gadget.DatatSetIsNotNullOrEmpty(dsArticleType))
            {
                mAritcleType = new Model.AritcleType();
                mAritcleType.AritcleTypeCode = Gadget.GetDataRowStringValue(dsArticleType.Tables[0].Rows[0], "AritcleTypeCode");
                mAritcleType.AritcleTypeDesc = Gadget.GetDataRowStringValue(dsArticleType.Tables[0].Rows[0], "AritcleTypeDesc");
                mAritcleType.UpdatedByUserID = Gadget.GetDataRowStringValue(dsArticleType.Tables[0].Rows[0], "UpdatedByUserID");
                mAritcleType.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsArticleType.Tables[0].Rows[0], "LastUpdatedDate");
            }
            return mAritcleType;
        }

        private DataSet GetDataSetArticleType(string strArticleTypeCode)
        {
            DataSet dsArticleType = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "AritcleTypeCode", strArticleTypeCode);
            dsArticleType = dbOperator.ProcessData("usp_GetAritcleType", hshParam, strDSN);
            return dsArticleType;
        }
    }
}
