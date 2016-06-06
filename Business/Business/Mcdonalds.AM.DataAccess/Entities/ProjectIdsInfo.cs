using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectIdsInfo : BaseEntity<ProjectIdsInfo>
    {
        private McdAMEntities db = new McdAMEntities();

        public DateTime CurrentDate
        {
            get; set;
        }

        public ProjectIdsInfo GetByProjectCode(string projectCode)
        {
            return db.ProjectIdsInfo.First(e => e.ProjectCode == projectCode);
        }
    }
}
