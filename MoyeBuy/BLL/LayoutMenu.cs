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
    public class LayoutMenu
    {
        private static readonly MoyeBuy.Com.IDAL.ILayoutMenu dal = DataAcess.CreateLayoutMenu();

        public LayoutMenu() { }

        public IList<Menu> GetMenuData(string strMenuTyp, bool IsAdmin)
        {
            return dal.GetMenuData(strMenuTyp, IsAdmin);
        }

        public string AddUpdateMenu(IList<Menu> listMenu)
        {
            return dal.AddUpdateMenu(listMenu);
        }   

        public bool MappingSubMenu(string strMenuMappingIDs,string strMainMenuID,string strSubMenuIDs)
        {
            return dal.MappingSubMenu(strMenuMappingIDs, strMainMenuID, strSubMenuIDs);
        }
    }
}
