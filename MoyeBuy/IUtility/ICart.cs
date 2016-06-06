using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IUtility
{
    public interface ICart
    {
        decimal GetCartItemPrice(decimal decPrice, int intProdCount);
        int GetCartItemProdCount(int intProdCount);
        decimal GetGatherGrade(decimal decPrice);//积份
    }
}
