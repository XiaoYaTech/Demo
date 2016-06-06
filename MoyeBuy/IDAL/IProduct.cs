using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.IDAL
{
    public interface IProduct
    {
        IList<ProductInfo> GetProductByCategory(string strCategory);
        IList<ProductInfo> GetProduct(string strFilterString, string strPageIndex, string strPageSize, string strSortField, bool IsASC);
        IList<ProductInfo> GetProduct(string strProductIDs);
        bool AddProduct(IList<ProductInfo> listProduct);
        bool DelProduct(string strProductID);
    }
}
