using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ApproveDialogUser : BaseEntity<ApproveDialogUser>
    {
        public static ApproveDialogUser GetApproveDialogUser(string strProjectId, string strFlowCode)
        {
            return FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && e.FlowCode.Equals(strFlowCode));
        }

        public static ApproveDialogUser GetApproveDialogUser(string strRefTableID)
        {
            return FirstOrDefault(e => e.RefTableID.Equals(strRefTableID));
        }

        /// <summary>
        /// 获取通知用户EID
        /// </summary>
        public static List<string> GetNotifyDialogUser(string strProjectId, string strFlowCode, bool isNecessary = false)
        {
            List<string> eidList = new List<string>();
            string eids = string.Empty;
            ApproveDialogUser dialogUser;
            if (!isNecessary)
            {
                dialogUser = FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && e.FlowCode.Equals(strFlowCode));
                if (dialogUser != null)
                {
                    eids = dialogUser.NoticeUsers;
                }
            }
            else
            {
                dialogUser = FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && e.FlowCode.Equals(strFlowCode));
                if (dialogUser != null)
                {
                    eids = dialogUser.NecessaryNoticeUsers;
                }
            }

            if (!string.IsNullOrEmpty(eids))
            {
                return eids.Split(';').ToList();
            }
            return eidList;
        }

        /// <summary>
        /// 获取指定角色的特定通知用户
        /// </summary>
        public static string GetNotifyDialogUserByRole(string strProjectId, string strFlowCode, RoleCode roleCode)
        {
            string eid = string.Empty;
            switch (roleCode)
            {
                case RoleCode.MCCL_Asset_Mgr:
                    var approveUser = FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && e.FlowCode.Equals(strFlowCode));
                    if (approveUser != null)
                    {
                        eid = approveUser.MCCLAssetMgrCode;
                    }
                    break;
                default:
                    break;

            }
            return eid;
        }

        public void Save()
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
                Add(this);
            }
            else
                Update(this);
        }
    }
}
