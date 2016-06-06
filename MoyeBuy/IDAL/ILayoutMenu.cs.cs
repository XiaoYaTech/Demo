using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface ILayoutMenu
    {
        string AddUpdateMenu(IList<Model.Menu> listMenu);
        IList<Model.Menu> GetMenuData(string strMenuTyp, bool IsAdmin);
        bool MappingSubMenu(string strMenuMappingIDs, string strMainMenuID, string strSubMenuIDs);
    }
}
