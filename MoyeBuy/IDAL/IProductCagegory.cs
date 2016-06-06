using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface IProductCagegory
    {
        IList<Model.ProductCategory> GetProductCategory(string strCategoryID);
        bool AddUpdateCatgory(Model.ProductCategory pcatgory);
        bool DelProductCatgory(string strCategoryId);
    }
}
