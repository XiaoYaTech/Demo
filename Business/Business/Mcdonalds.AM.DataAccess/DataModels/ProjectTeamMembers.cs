using Mcdonalds.AM.DataAccess.DataTransferObjects;

namespace Mcdonalds.AM.DataAccess.DataModels
{
    public class ProjectTeamMembers
    {
        public ProjectTeamMember AssetRep { get; set; }

        public ProjectTeamMember AssetActor { get; set; }

        public ProjectTeamMember AssetMgr { get; set; }

        public ProjectTeamMember Finance { get; set; }

        public ProjectTeamMember PM { get; set; }

        public ProjectTeamMember CM { get; set; }

        public ProjectTeamMember Legal { get; set; }
    }
}
