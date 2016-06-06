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
    public class GiftCard
    {
        private static readonly MoyeBuy.Com.IDAL.IGiftCard dal = DataAcess.CreateGiftCard();

        public IList<Model.GiftCard> GetGifCard(string strPageSize, string strPageIndex, bool IsAsc, string strSortField)
        {
            return dal.GetGifCard(strPageSize, strPageIndex, IsAsc, strSortField);
        }
        public Model.GiftCard GetGifCard(string strGiftCardNo)
        {
            return dal.GetGifCard(strGiftCardNo);
        }

        public bool GenerateGiftCard(int CardNum, decimal CardAmount, string strCardPreNo,DateTime StartDate, DateTime ExpireDate)
        {
            return dal.GenerateGiftCard(CardNum, CardAmount, strCardPreNo,StartDate, ExpireDate);
        }

        public bool SetGiftCardToInvalidate(string strGiftCardNo)
        {
            return dal.SetGiftCardToInvalidate(strGiftCardNo);
        }
    }
}
