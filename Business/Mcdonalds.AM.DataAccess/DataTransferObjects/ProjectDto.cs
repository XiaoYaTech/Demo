using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.DataModels;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class ProjectDto
    {
        public string ProjectId { get; set; }

        public string FlowCode { get; set; }

        public ProjectTeamMembers ProjectTeamMembers { get; set; }

        public ApproveUsers ApproveUsers { get; set; }

        public string Comment { get; set; }

        public ProjectStatus PrevStatus { get; set; }

        public ProjectStatus CurrStatus { get; set; }
    }
}
