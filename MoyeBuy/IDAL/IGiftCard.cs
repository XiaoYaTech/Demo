using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface IGiftCard
    {
        IList<Model.GiftCard> GetGifCard(string strPageSize, string strPageIndex, bool IsAsc, string strSortField);
        Model.GiftCard GetGifCard(string strGiftCardNo);
        bool GenerateGiftCard(int CardNum, decimal CardAmount, string strCardPreNo,DateTime StartDate, DateTime ExpireDate);
        bool SetGiftCardToInvalidate(string strGiftCardNo);
    }
}
