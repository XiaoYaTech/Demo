using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class ArticleReply
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        public IList<Expose178.Com.Model.ReplyToArticle> GetListReplyToArticle(string strArticleID)
        {
            IList<Expose178.Com.Model.ReplyToArticle> listReplyToArticle = null;
            DataSet dsReplyToArticle = null;
            Hashtable hshParam = new Hashtable();
            dsReplyToArticle = dbOperator.ProcessData("usp_GetReplyToArticle", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsReplyToArticle))
            {
                listReplyToArticle = new List<Model.ReplyToArticle>();
                foreach (DataRow dr in dsReplyToArticle.Tables[0].Rows)
                {
                    Expose178.Com.Model.ReplyToArticle reply = new Model.ReplyToArticle();
                    reply.ArticleID = strArticleID;
                    reply.IsValidated = Gadget.GetDataRowBoolValue(dr, "IsValidated");
                    reply.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                    reply.ReplyBody = Gadget.GetDataRowStringValue(dr, "ReplyBody");
                    reply.ReplyID = Gadget.GetDataRowStringValue(dr, "ReplyID");
                    reply.UpdatedByUserID = Gadget.GetDataRowStringValue(dr, "UpdatedByUserID");
                }
            }
            return listReplyToArticle;
        }
        public bool AddUpdateReplyToArticle(Expose178.Com.Model.ReplyToArticle reply)
        { 
            DataSet dsReplyToArticle =null;
            Hashtable hshParam=new Hashtable();
            Gadget.Addparamater(ref hshParam, "ArticleID", reply.ArticleID);
            Gadget.Addparamater(ref hshParam, "ReplyID", reply.ReplyID);
            Gadget.Addparamater(ref hshParam, "ReplyBody", reply.ReplyBody);
            Gadget.Addparamater(ref hshParam, "IsValidated", reply.IsValidated == true ? "1" : "0");
            Gadget.Addparamater(ref hshParam, "UpdatedByUserID", reply.UpdatedByUserID);
            dsReplyToArticle = dbOperator.ProcessData("usp_AddUpdateReplyToArticleByReplyID", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsReplyToArticle))
                return true;
            else
                return false;
        }
    }
}
