using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class Article
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        private DataSet GetArticleByID(string strArticleID)
        {
            DataSet dsArticle = null;
            Hashtable hshParam=new Hashtable();
            Gadget.Addparamater(ref hshParam, "ArticleID", strArticleID);
            dsArticle = dbOperator.ProcessData("usp_GetArticleByArticleID", hshParam, strDSN);
            return dsArticle;
        }
        private DataSet GetArticleAddtionalByArticleID(string strArticleID)
        {
            DataSet dsArticleAdd = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ArticleID", strArticleID);
            dsArticleAdd = dbOperator.ProcessData("usp_GetArticleAddtionalByArticleID", hshParam, strDSN);
            return dsArticleAdd;
        }
        private DataSet UpdateDataSetArticleAddtional(Expose178.Com.Model.ArticleAddtional addtional)
        {
            DataSet dsAddtional = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ArticleID", addtional.ArticleID);
            Gadget.Addparamater(ref hshParam, "AdditionalID", addtional.AdditionalID);
            Gadget.Addparamater(ref hshParam, "ReadNum", addtional.ReadNum.ToString());
            Gadget.Addparamater(ref hshParam, "ReplyNum", addtional.ReplyNum.ToString());
            Gadget.Addparamater(ref hshParam, "UpdatedByUserID", addtional.UpdatedByUserID);

            dsAddtional = dbOperator.ProcessData("usp_AddUpdateArticleAdditionalByAdditionalID", hshParam, strDSN);
            return dsAddtional;
        }
        
        public bool UpdateArticleAddtional(Expose178.Com.Model.ArticleAddtional addtional)
        {
            if (Gadget.DatatSetIsNotNullOrEmpty(UpdateDataSetArticleAddtional(addtional)))
                return true;
            else
                return false;
        }
        public Expose178.Com.Model.Article GetArticle(string strArticleID)
        {
            Expose178.Com.Model.Article art = new Model.Article();
            DataSet dsArticle = GetArticleByID(strArticleID);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsArticle))
            {
                art.ArticleID = strArticleID;
                art.ArticleTile = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "ArticleTile");
                art.ArticleDate = Gadget.GetDataRowDateTimeValue(dsArticle.Tables[0].Rows[0], "ArticleDate");
                art.ArticleBody = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "ArticleBody");
                art.BackgroundImgUrl = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "BackgroundImgUrl");
                art.IsDraft = Gadget.GetDataRowBoolValue(dsArticle.Tables[0].Rows[0], "IsDraft");
                art.IsValidated = Gadget.GetDataRowBoolValue(dsArticle.Tables[0].Rows[0], "IsValidated");
                art.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsArticle.Tables[0].Rows[0], "LastUpdatedDate");

                string strUID = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "UpdatedByUserID");
                string strstrAritcleTypeCode = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "AritcleTypeCode");
                string strReadRoleTypeCode = Gadget.GetDataRowStringValue(dsArticle.Tables[0].Rows[0], "ReadRoleTypeCode");
                ArticleReply reply = new ArticleReply();
                User user = new User();
                art.ListReply = reply.GetListReplyToArticle(strArticleID);
                art.ReadRoleType = GetReadRoleType(strReadRoleTypeCode);
                art.User = user.GetUserByUID(strUID);
                art.AritcleType = GetAritcleType(strstrAritcleTypeCode);
            }
            return art;
        }
        public Expose178.Com.Model.ArticleAddtional GetArticleAddtional(string strArticleID)
        {
            Expose178.Com.Model.ArticleAddtional addtion = new Model.ArticleAddtional();
            try
            {
                DataSet dsArticleAdd = GetArticleAddtionalByArticleID(strArticleID);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsArticleAdd))
                {
                    addtion.AdditionalID = Gadget.GetDataRowStringValue(dsArticleAdd.Tables[0].Rows[0], "AdditionalID");
                    addtion.ArticleID = strArticleID;
                    addtion.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsArticleAdd.Tables[0].Rows[0], "LastUpdatedDate");
                    addtion.ReadNum = Gadget.GetDataRowIntValue(dsArticleAdd.Tables[0].Rows[0], "ReadNum");
                    addtion.ReplyNum = Gadget.GetDataRowIntValue(dsArticleAdd.Tables[0].Rows[0], "ReplyNum");
                    addtion.UpdatedByUserID = Gadget.GetDataRowStringValue(dsArticleAdd.Tables[0].Rows[0], "UpdatedByUserID");
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error", ex.Message);
                hshParam.Add("UID", Expose178.Com.GadgetScripts.Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "ProxyArticle.GetArticleAddtional", UtilityFactory.LogType.LogToDB);
            }
            return addtion;
        }
        public bool AddUpdateArticle(Expose178.Com.Model.Article article)
        {
            DataSet dsArticle =null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ArticleID", article.ArticleID);
            Gadget.Addparamater(ref hshParam, "ArticleTile", article.ArticleTile);
            Gadget.Addparamater(ref hshParam, "ArticleBody", article.ArticleBody);
            Gadget.Addparamater(ref hshParam, "BackgroundImgUrl", article.BackgroundImgUrl);
            Gadget.Addparamater(ref hshParam, "IsDraft", article.IsDraft == true ? "1" : "0");
            Gadget.Addparamater(ref hshParam, "IsValidated", article.IsValidated == true ? "1" : "0");
            Gadget.Addparamater(ref hshParam, "UpdatedByUserID", article.UID);
            Gadget.Addparamater(ref hshParam, "ReadRoleTypeCode", article.ReadRoleTypeCode);
            Gadget.Addparamater(ref hshParam, "AritcleTypeCode", article.AritcleTypeCode);
            dsArticle = dbOperator.ProcessData("usp_AddUpdateArticleByArticleID", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsArticle))
                return true;
            else
                return false;
        }
        private Expose178.Com.Model.AritcleType GetAritcleType(string strAritcleTypeCode)
        {
            Expose178.Com.Model.AritcleType arType = new Model.AritcleType();
            DataSet dsAritcleType = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "AritcleTypeCode", strAritcleTypeCode);
            dsAritcleType = dbOperator.ProcessData("usp_GetAritcleType", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsAritcleType))
            {
                arType.AritcleTypeCode = strAritcleTypeCode;
                arType.AritcleTypeDesc = Gadget.GetDataRowStringValue(dsAritcleType.Tables[0].Rows[0], "AritcleTypeDesc");
                arType.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsAritcleType.Tables[0].Rows[0], "LastUpdatedDate");
                arType.UpdatedByUserID = Gadget.GetDataRowStringValue(dsAritcleType.Tables[0].Rows[0], "UpdatedByUserID");
            }
            return arType;
        }
        private Expose178.Com.Model.ReadRoleType GetReadRoleType(string strReadRoleTypeCode)
        {
            Expose178.Com.Model.ReadRoleType ReadRoleType = new Model.ReadRoleType();
            DataSet dsReadRoleType = null;
            Hashtable hshParam = new Hashtable();
            dsReadRoleType = dbOperator.ProcessData("usp_GetReadRoleType", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsReadRoleType))
            {
                ReadRoleType.ReadRoleTypeCode = strReadRoleTypeCode;
                ReadRoleType.ReadRoleTypeDesc = Gadget.GetDataRowStringValue(dsReadRoleType.Tables[0].Rows[0], "ReadRoleTypeDesc");
                ReadRoleType.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dsReadRoleType.Tables[0].Rows[0], "LastUpdatedDate");
                ReadRoleType.UpdatedByUserID = Gadget.GetDataRowStringValue(dsReadRoleType.Tables[0].Rows[0], "UpdatedByUserID");
            }
            return ReadRoleType;
        }
    }
}
