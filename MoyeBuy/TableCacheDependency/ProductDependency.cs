using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.TableCacheDependency
{
    public class ProductDependency:TableDependency
    {
        public ProductDependency() : base("Product", "CacheMoyeBuyCom") { }
    }
}
