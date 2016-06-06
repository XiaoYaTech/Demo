using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class Menu
    {
        public Nullable<int> MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string Disq { get; set; }
        public string Target { get; set; }
        public string MenuType { get; set; }
        public bool IsAdminMenu { get; set; }
        public string MenuClassName { get; set; }
        public string MenuControlID { get; set; }
        public string MenuTitle { get; set; }
        public IList<Menu> SubMenu { get; set; }
    }
}
