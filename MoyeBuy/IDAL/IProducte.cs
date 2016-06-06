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
        IList<ProductInfo> GetProductBySearch(string[] strKeywords);
        ProductInfo GetProductById(string strProductId);
        void AddProduct(ProductInfo product);
    }
}
