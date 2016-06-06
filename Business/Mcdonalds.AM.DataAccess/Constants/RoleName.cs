using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public class RoleName
    {
        public const string System_Admin = "System Admin";
        public const string PM = "PM";
        public const string Store_Planner = "Store Planner";
        public const string RE_Rep = "RE Rep";
        public const string Planning_Mgr = "Planning Mgr";
        public const string RE_Mgr = "RE Mgr";
        public const string Coordinator = "";
        public const string MCCL_Alliance_Mgr = "MCCL_Alliance Mgr";
        public const string Asset_Mgr = "Asset Mgr";
        public const string MCCL_Trainning_Mgr = "MCCL_Trainning Mgr";
        public const string MCCL_HR_Mgr = "MCCL_HR Mgr";
        public const string Planner = "Planner";
        public const string VPGM = "VPGM";
        public const string GM = "GM";
        public const string CDC_Director = "CDC Director";
        public const string CDC_Manager = "CDC Manager";
        public const string Asset_Rep = "Asset Rep";
        public const string MCCL_Asset_Director = "MCCL_Asset Director";
        public const string CDO = "CDO";
        public const string Market_DD = "Market DD";
        public const string MCCL_Planning_Leader = "MCCL Planning Leader";
        public const string Cons_Mgr = "Cons Mgr";
        public const string RE_VP = "RE VP";
        public const string Regional_DD = "Regional DD";
        public const string CEO = "CEO";
        public const string DO = "DO";
        public const string MCCL_DT_Leader = "MCCL DT Leader";
        public const string CFO = "CFO";
        public const string FOO = "FOO";
        public const string MCCL_Cons_Leader = "MCCL Cons Leader";
        public const string MCCL_Cons_Manager = "MCCL_Construction Mgr";
        public const string COO = "COO";
        public const string MCCL_Governance_Leader = "MCCL Governance Leader";
        public const string MCCL_Alliance_Leader = "MCCL Alliance Leader";
        public const string MCCL_Finance_Controller = "MCCL Finance Controller";
        public const string Finance_Consultant = "Finance Consultant";
        public const string Finance_Controller = "Finance Controller";
        public const string Legal_Counsel = "Legal Counsel";
        public const string Legal_GeneralCounsel = "Legal GeneralCounsel";
        public const string MCCL_Coordinator = "MCCL Coordinator";
        public const string OM = "OM";
        public const string MCCL_Planner = "MCCL_Planner";
        public const string MCCL_Planning_Mgr = "MCCL_Planning Mgr";
        public const string MCCL_DT_Manager = "MCCL DT Manager";
        public const string MCCL_Asset_Mgr = "MCCL_Asset Mgr";
        public const string Package_DD = "Package DD";
        public const string Finance_Team = "Finance Team";
        public const string MCCL_Fin_Dev = "MCCL Fin Dev";
        public const string Management_Team = "Management Team";
        public const string Fin_Dev = "Fin Dev";
        public const string MCCL_Cons_Director = "MCCL Cons Director";
        public const string DD = "DD";
    }

    /// <summary>
    /// 角色标识
    /// </summary>
    public enum RoleCode : int
    {
        System_Admin = 1,
        PM = 2,
        Store_Planner = 3,
        RE_Rep = 4,
        Planning_Mgr = 6,
        RE_Mgr = 7,
        Coordinator = 8,
        MCCL_Alliance_Mgr = 12,
        Market_Asset_Mgr = 15,
        MCCL_Trainning_Mgr = 16,
        MCCL_HR_Mgr = 17,
        Planner = 18,
        VPGM = 19,
        GM = 20,
        CDC_Director = 38,
        CDC_Manager = 39,
        Asset_Rep = 40,
        MCCL_Asset_Director = 41,
        CDO = 47,
        Market_DD = 48,
        MCCL_Planning_Leader = 49,
        Cons_Mgr = 50,
        RE_VP = 54,
        Regional_DD = 55,
        CEO = 56,
        DO = 58,
        MCCL_DT_Leader = 59,
        CFO = 60,
        FOO = 61,
        MCCL_Cons_Leader = 62,
        MCCL_Cons_Manager = 63,
        COO = 65,
        MCCL_Governance_Leader = 66,
        MCCL_Alliance_Leader = 67,
        MCCL_Finance_Controller = 68,
        Finance_Consultant = 69,
        Finance_Controller = 70,
        Legal_Counsel = 71,
        Legal_GeneralCounsel = 72,
        MCCL_Coordinator = 73,
        OM = 74,
        MCCL_Planner = 75,
        MCCL_Planning_Mgr = 76,
        MCCL_DT_Manager = 77,
        MCCL_Asset_Mgr = 78,
        Package_DD = 79,
        Finance_Team = 80,
        MCCL_Fin_Dev = 81,
        Management_Team = 82,
        Fin_Dev = 83,
        MCCL_Cons_Director = 84,
        IT = 86,
        MD = 87,
        Regional_Asset_Mgr = 88,
        Dev_Equipment = 89,
        IT_Equipment = 90,
        SSC = 91,
        MCCL_Training_Dir = 92,
        Finance_Manager = 93
    }
}
