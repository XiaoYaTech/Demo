using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RPTableSetting : BaseEntity<RPTableSetting>
    {
        #region 属性
        public List<RPFieldSetting> Fields { get; set; }

        public bool Checked { get; set; }
        #endregion

        public static List<RPTableSetting> GetTables()
        {
            return Search(p => true).OrderBy(p => p.OrderBy).ToList();

        }



    }
}
