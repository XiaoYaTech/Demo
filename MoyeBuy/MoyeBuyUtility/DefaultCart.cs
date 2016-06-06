using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoyeBuy.Com.IUtility;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class DefaultCart:ICart
    {
        public decimal GetCartItemPrice(decimal decPrice, int intProdCount)
        {
            return decPrice * intProdCount;
        }

        public int GetCartItemProdCount(int intProdCount)
        {
            return intProdCount;
        }

        public decimal GetGatherGrade(decimal decPrice)
        {
            return 0;
        }
    }
}
