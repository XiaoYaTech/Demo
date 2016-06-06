using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface ISupplier
    {
        IList<Model.SupplierInfo> GetSupplier(string strSupplierID);
        bool AddUpdateSupplier(Model.SupplierInfo supplier);
        bool DelSupplierByID(string strSupplierID);
    }
}
