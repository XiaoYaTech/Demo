using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class GiftCard : DALBase,IDAL.IGiftCard
    {
        public IList<Model.GiftCard> GetGifCard(string strPageSize, string strPageIndex, bool IsAsc, string strSortField)
        {
            IList<Model.GiftCard> giftcard = null;
            try
            {
                DataSet dsGift = GetDataSetGifCard("", strPageSize, strPageIndex, IsAsc, strSortField);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsGift))
                {
                    giftcard = new List<Model.GiftCard>();
                    foreach (DataRow dr in dsGift.Tables[0].Rows)
                    {
                        Model.GiftCard g = new Model.GiftCard();
                        g.GiftCardNoID = Gadget.GetDataRowStringValue(dr, "GiftCardNoID");
                        g.GiftCardNo = Gadget.GetDataRowStringValue(dr, "GiftCardNo");
                        g.GiftCardInvalidID = Gadget.GetDataRowStringValue(dr, "GiftCardInvalidID");
                        g.GiftCardAmount = Gadget.GetDataRowStringValue(dr, "GiftCardAmount");
                        g.GifCardPwd = Gadget.GetDataRowStringValue(dr, "GifCardPwd");
                        g.IsInvalid = Gadget.GetDataRowBoolValue(dr, "IsInvalid");
                        g.ExpireDate = Gadget.GetDataRowDateTimeValue(dr, "ExpireDate");
                        g.StartDate = Gadget.GetDataRowDateTimeValue(dr, "StartDate");
                        g.UpdateByUserID = Gadget.GetDataRowStringValue(dr, "UpdateByUserID");
                        g.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                        giftcard.Add(g);
                    }
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.GiftCard.GetGifCard()", UtilityFactory.LogType.LogToFile);
            }
            return giftcard;
        }

        public Model.GiftCard GetGifCard(string strGiftCardNo)
        {
            Model.GiftCard g = null;
            try
            {
                string strPageSize = "";
                string strPageIndex = "";
                bool IsAsc = false;
                string strSortField = "";
                DataSet dsGift = GetDataSetGifCard(strGiftCardNo, strPageSize, strPageIndex, IsAsc, strSortField);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsGift))
                {
                    foreach (DataRow dr in dsGift.Tables[0].Rows)
                    {
                        g = new Model.GiftCard();
                        g.GiftCardNoID = Gadget.GetDataRowStringValue(dr, "GiftCardNoID");
                        g.GiftCardNo = Gadget.GetDataRowStringValue(dr, "GiftCardNo");
                        g.GiftCardInvalidID = Gadget.GetDataRowStringValue(dr, "GiftCardInvalidID");
                        g.GiftCardAmount = Gadget.GetDataRowStringValue(dr, "GiftCardAmount");
                        g.GifCardPwd = Gadget.GetDataRowStringValue(dr, "GifCardPwd");
                        g.IsInvalid = Gadget.GetDataRowBoolValue(dr, "IsInvalid");
                        g.ExpireDate = Gadget.GetDataRowDateTimeValue(dr, "ExpireDate");
                        g.StartDate = Gadget.GetDataRowDateTimeValue(dr, "StartDate");
                        g.UpdateByUserID = Gadget.GetDataRowStringValue(dr, "UpdateByUserID");
                        g.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                    }
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.GiftCard.GetGifCard(string)", UtilityFactory.LogType.LogToFile);
            }
            return g;
        }

        public bool GenerateGiftCard(int CardNum, decimal CardAmount, string CardPreNo, DateTime StartDate, DateTime ExpireDate)
        {
            bool bReturn = false;
            if (CardNum <= 0 || CardAmount <= 0)
                return bReturn;
            if (StartDate > ExpireDate)
                return bReturn;
            Hashtable hshParam = new Hashtable();
            try
            {
                DataSet dsResult = null;
                Gadget.Addparamater(ref hshParam, "CardNum", CardNum);
                Gadget.Addparamater(ref hshParam, "CardAmount", CardAmount);
                Gadget.Addparamater(ref hshParam, "CardPreNo", CardPreNo);
                Gadget.Addparamater(ref hshParam, "StartDate", StartDate);
                Gadget.Addparamater(ref hshParam, "ExpireDate", ExpireDate);
                Gadget.Addparamater(ref hshParam, "UID", Gadget.GetUserID());
                dsResult = dbOperator.ProcessData("usp_GenerateGiftCardNo", hshParam, strGiftDSN);
                bReturn = Gadget.DatatSetIsNotNullOrEmpty(dsResult);
            }
            catch (Exception ex)
            {
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.GiftCard.GenerateGiftCard()", UtilityFactory.LogType.LogToFile);
            }
            return bReturn;
        }

        public bool SetGiftCardToInvalidate(string strGiftCardNo)
        {
            bool bReturn = false;
            if (String.IsNullOrEmpty(strGiftCardNo))
                return bReturn;
            Hashtable hshParam = new Hashtable();
            try
            {
                DataSet dsResult = null;
                Gadget.Addparamater(ref hshParam, "GiftCardNo", strGiftCardNo);
                Gadget.Addparamater(ref hshParam, "UID", Gadget.GetUserID());
                dsResult = dbOperator.ProcessData("usp_AddUsedCardNo", hshParam, strGiftDSN);
                bReturn = Gadget.DatatSetIsNotNullOrEmpty(dsResult);
            }
            catch (Exception ex)
            {
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.GiftCard.SetGiftCardToInvalidate()", UtilityFactory.LogType.LogToFile);
            }
            return bReturn;
        }

        private DataSet GetDataSetGifCard(string strGiftCardNo, string strPageSize, string strPageIndex, bool IsAsc, string strSortField)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "GiftCardNo", strGiftCardNo);
            Gadget.Addparamater(ref hshParam, "PageSize", strPageSize);
            Gadget.Addparamater(ref hshParam, "PageIndex", strPageIndex);
            Gadget.Addparamater(ref hshParam, "IsAsc", IsAsc ? "1" : "0");
            Gadget.Addparamater(ref hshParam, "SortField", strSortField);
            dsResult = dbOperator.ProcessData("usp_GetGiftCardByNo", hshParam, strGiftDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
    }
}
