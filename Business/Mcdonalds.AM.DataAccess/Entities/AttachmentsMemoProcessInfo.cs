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
    public partial class AttachmentsMemoProcessInfo : BaseEntity<AttachmentsMemoProcessInfo>
    {
        public static string GetUSCode(string projectId)
        {
            return Search(p => p.ProjectId == projectId).Select(p => p.USCode).FirstOrDefault();
        }
        public static AttachmentsMemoProcessInfo Get(string projectId, string memoCode)
        {
            return FirstOrDefault(p => p.ProjectId == projectId && p.MemoCode == memoCode);
        }
        public static void UpdateNotifyDate(string projectId, string memoCode, bool IsNotified=true)
        {
            var memo = FirstOrDefault(p => p.ProjectId == projectId && p.MemoCode == memoCode);
            memo.LastUpdateTime = DateTime.Now;
            if(IsNotified)
                memo.NotifyDate = DateTime.Now;
            else
                memo.NotifyDate = null;
            memo.IsNotified = IsNotified;
            Update(memo);
        }
        public static void CreateAttachmentsMemoProcessInfo(string projectId, string MemoCode, string flowCode, string usCode)
        {
            var memo = new AttachmentsMemoProcessInfo();
            memo.Id = Guid.NewGuid();
            memo.IsNotified = false;
            memo.LastUpdateTime = DateTime.Now;
            memo.MemoCode = MemoCode;
            memo.FlowCode = flowCode;
            memo.ProjectId = projectId;
            memo.USCode = usCode;
            memo.NotifyDate = null;
            memo.CreateTime = DateTime.Now;
            memo.CreateUserAccount = ClientCookie.UserCode;
            Add(memo);
        }
    }
}
