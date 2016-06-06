using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class GBMemo : BaseEntity<GBMemo>
    {
        public StoreBasicInfo.StoreInfo Store { get; set; }
        public RebuildInfo Info { get; set; }

        public ReimageInfo rmgInfo { get; set; }
        public static GBMemo GetGBMemo(string projectId)
        {
            var memo = FirstOrDefault(e => e.ProjectId.Equals(projectId) && !e.IsHistory);
            if (memo == null)
                memo = new GBMemo();

            memo.ProjectId = projectId;
            string usCode = "";
            if (projectId.ToLower().IndexOf("rebuild") >=0)
            {
                var rbdInfo = new RebuildInfo();
                rbdInfo = rbdInfo.GetRebuildInfo(projectId);
                memo.Info = rbdInfo;
                usCode = rbdInfo.USCode;
            }
            if (projectId.ToLower().IndexOf("reimage") >=0)
            {
                var reimageInfo = ReimageInfo.GetReimageInfo(projectId);
                memo.rmgInfo = reimageInfo;
                usCode = reimageInfo.USCode;
            }
            memo.Store = StoreBasicInfo.GetStore(usCode);

            if (memo.Store.Hour24Count == 24)
                memo.Is24Hour = true;
            else
                memo.Is24Hour = false;
            return memo;
        }

        public static void SaveGBMemo(GBMemo memo)
        {
            memo.LastUpdateTime = DateTime.Now;
            memo.LastUpdateUserAccount = ClientCookie.UserCode;
            memo.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
            memo.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
            if (memo.Id == Guid.Empty)
            {
                memo.Id = Guid.NewGuid();
                memo.IsHistory = false;
                memo.CreateTime = DateTime.Now;
                memo.LastUpdateTime = DateTime.Now;
                Add(memo);
            }
            else
            {
                Update(memo);
            }
        }

        public static void Submit(GBMemo memo)
        {
            string strFlowCode = "";
            string strNodeCode = "";
            string strTypeCode = "";
            if (memo.ProjectId.ToLower().IndexOf("rebuild") != -1)
            {
                strFlowCode = FlowCode.Rebuild;
                strNodeCode = NodeCode.Finish;
                strTypeCode = FlowCode.Rebuild_GBMemo;
            }
            else if (memo.ProjectId.ToLower().IndexOf("reimage") != -1)
            {
                strFlowCode = FlowCode.Reimage;
                strNodeCode = NodeCode.Finish;
                strTypeCode = FlowCode.Reimage_GBMemo;
            }
            var task = TaskWork.GetTaskWork(memo.ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish, strFlowCode, strTypeCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/" + strFlowCode + "/Main#/GBMemo/Process/View?projectId=" + memo.ProjectId;
            task.Url = taskUrl;
            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);
                SaveGBMemo(memo);
                ProjectInfo.UpdateProjectNode(memo.ProjectId, strTypeCode, strNodeCode);
                scope.Complete();
            }
        }
    }
}
