using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface IRole
    {
        IList<Model.Role> GetRole();
        bool AddUpdateRole(Model.Role role);
    }
}
