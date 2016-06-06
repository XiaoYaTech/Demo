using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Expose178.Com.GadgetScripts;

namespace Expose178.Com.BLL
{
    public class ArticleTitle
    {
        private static readonly Expose178.Com.IDAL.IDBBaseOperator dbOperator = Expose178.Com.DALFactory.DataAcess.CreateDBBaseOperator();
        private static readonly string strDSN = Expose178.Com.GadgetScripts.Gadget.GetConnectionString("Expose178Com");

        public string AritcleTypeCode { get; set; }
        public string UpdatedByUserID { get; set; }
        public bool IsAsc { get; set; }
        public bool IsReturnAll { get; set; }
        private int intPageSize = 300;
        public int PageSize { get { return this.intPageSize; } set { this.intPageSize = value; } }
        private int intPageIndex = 1;
        public int PageIndex { get { return this.intPageIndex; } set { this.intPageIndex = value; } }
        public string SortField { get; set; }

        private DataSet GetDataSetArticleTitle()
        {
            DataSet dsArtileTitle = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "AritcleTypeCode", AritcleTypeCode);
            Gadget.Addparamater(ref hshParam, "UpdatedByUserID", UpdatedByUserID);
            Gadget.Addparamater(ref hshParam, "IsAsc", IsAsc==true?"1":"0");
            Gadget.Addparamater(ref hshParam, "IsReturnAll", IsReturnAll==true?"1":"0");
            Gadget.Addparamater(ref hshParam, "PageSize", PageSize.ToString());
            Gadget.Addparamater(ref hshParam, "PageIndex", PageIndex.ToString());
            Gadget.Addparamater(ref hshParam, "SortField", SortField);
            dsArtileTitle = dbOperator.ProcessData("usp_GetArticleTitle", hshParam, strDSN);
            return dsArtileTitle;
        }

        public IList<Expose178.Com.Model.ArticleTitle> GetArticleTitle()
        {
            IList<Expose178.Com.Model.ArticleTitle> listArticleTile = null;
            DataSet dsArtileTitle = GetDataSetArticleTitle();
            if (Gadget.DatatSetIsNotNullOrEmpty(dsArtileTitle))
            {
                listArticleTile = new List<Expose178.Com.Model.ArticleTitle>();
                foreach (DataRow dr in dsArtileTitle.Tables[0].Rows)
                {
                    Expose178.Com.Model.ArticleTitle art = new Model.ArticleTitle();
                    art.ArticleID = Gadget.GetDataRowStringValue(dr, "ArticleID");
                    art.ArticleDate = Gadget.GetDataRowDateTimeValue(dr, "ArticleDate");
                    art.ArticleTile = Gadget.GetDataRowStringValue(dr, "ArticleTile");
                    art.ArticleSummary = Gadget.GetDataRowStringValue(dr, "ArticleSummary");
                    art.ReadNum = Gadget.GetDataRowIntValue(dr, "ReadNum");
                    art.ReplyNum = Gadget.GetDataRowIntValue(dr, "ReplyNum");
                    listArticleTile.Add(art);
                }
            }
            return listArticleTile;
        }
    }
}
