using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RPFieldSetting : BaseEntity<RPFieldSetting>
    {
        #region 属性
        public List<RPSelectionSetting> Selections { get; set; }

        public bool Checked { get; set; }

        #endregion

        public static List<RPFieldSetting> GetFieldsByTableID(int tableId)
        {
            return Search(e => e.TableID == tableId).ToList();
        }
    }
}
