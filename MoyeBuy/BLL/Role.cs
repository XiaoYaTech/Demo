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
    public class Role
    {
        private static readonly IDAL.IRole dal = DALFactory.DataAcess.CreateRole();
        public IList<Model.Role> GetRole()
        {
            return dal.GetRole();
        }

        public bool AddUpdateRole(Model.Role role)
        {
            if (role != null && !string.IsNullOrEmpty(role.RoleName))
                return dal.AddUpdateRole(role);
            else
                return false;
        }
    }
}
