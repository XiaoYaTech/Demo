/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   2/6/2015 2:25:49 PM
 * FileName     :   ProjectProgress
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectProgress : BaseEntity<ProjectProgress>
    {
        public static void SetProgress(string projectId, string progress)
        {
            var pp = FirstOrDefault(e => e.ProjectId == projectId);
            if (pp == null)
            {
                pp = new ProjectProgress();
                pp.Id = Guid.NewGuid();
                pp.ProjectId = projectId;
                pp.Progress = progress;
                pp.Add();
            }
            else
            {
                pp.Progress = progress;
                pp.Update();
            }
        }

        public static string GetProgress(string projectId)
        {
            var pp = FirstOrDefault(e => e.ProjectId == projectId);
            return pp != null ? pp.Progress : null;
        }
    }
}
