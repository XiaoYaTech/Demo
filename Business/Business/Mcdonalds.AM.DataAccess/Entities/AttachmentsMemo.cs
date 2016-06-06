using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class AttachmentsMemo : BaseEntity<AttachmentsMemo>
    {
        public static List<AttachmentsMemo> GetAttachmentsMemoList(string flowCode)
        {
            return Search(e => e.FlowCode.Equals(flowCode)).ToList();
        }
    }
}
